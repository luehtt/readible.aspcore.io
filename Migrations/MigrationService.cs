using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Readible.Migrations.DataSeeders;
using Readible.Models;
using Readible.Shared;
using static Readible.Migrations.MigrationConst;
using static Readible.Shared.HttpStatus;

namespace Readible.Migrations
{
    public class MigrationService
    {
        private readonly DataContext context;
        private readonly List<DbTable> dbModel;

        public MigrationService(DataContext context)
        {
            this.context = context;
            dbModel = new List<DbTable>
            {
                new BookCommentTable(),
				new OrderDetailTable(),
                new OrderTable(),
                new OrderStatusTable(),
                new CustomerTable(),
                new ManagerTable(),
                new UserTable(),
                new UserRoleTable(),
                new BookTable(),
                new BookCategoryTable()
            };            
        }

        public List<User> SeedUsers(int totalAdmin, int totalManager, int totalCustomer)
        {
            // get data from dbModel
            var userTable = dbModel.FirstOrDefault(x => x.TableName == TABLE_USER);
            var managerTable = dbModel.FirstOrDefault(x => x.TableName == TABLE_MANAGER);
            var customerTable = dbModel.FirstOrDefault(x => x.TableName == TABLE_CUSTOMER);
            if (userTable == null || managerTable == null || customerTable == null)
                throw new HttpResponseException(BAD_REQUEST_CODE);

            // clear existed data
            context.Database.ExecuteSqlCommand(userTable.MakeDeleteDataCmd());
            context.Database.ExecuteSqlCommand(managerTable.MakeDeleteDataCmd());
            context.Database.ExecuteSqlCommand(customerTable.MakeDeleteDataCmd());

            // begin to insert
            var seeder = new UserTableSeeder(totalAdmin, totalManager, totalCustomer);
            context.Users.AddRange(seeder.Users);
            context.Managers.AddRange(seeder.Managers);
            context.Customers.AddRange(seeder.Customers);
            context.SaveChanges();

            // perform reset sequence
            context.Database.ExecuteSqlCommand(userTable.MakeSetSequenceCmd(totalAdmin + totalManager + totalCustomer));
            return context.Users.AsNoTracking().ToList();
        }

        public List<Book> SeedBooks(int totalCategory, int totalBook)
        {
            // get data from dbModel
            var bookTable = dbModel.FirstOrDefault(x => x.TableName == TABLE_BOOK);
            var categoryTable = dbModel.FirstOrDefault(x => x.TableName == TABLE_BOOK_CATEGORY);
            
            if (bookTable == null || categoryTable == null)
                throw new HttpResponseException(SERVER_ERROR_CODE);

            // clear existed data
            context.Database.ExecuteSqlCommand(bookTable.MakeDeleteDataCmd());
            context.Database.ExecuteSqlCommand(categoryTable.MakeDeleteDataCmd());

            // begin to insert
            var seeder = new BookTableSeeder(totalCategory, totalBook);
            context.BookCategories.AddRange(seeder.BookCategories);
            context.Books.AddRange(seeder.Books);
            context.SaveChanges();

            // perform reset sequence
            context.Database.ExecuteSqlCommand(categoryTable.MakeSetSequenceCmd(totalCategory));
            return context.Books.AsNoTracking().ToList();
        }

        public List<BookComment> SeedBookComments(int totalComment)
        {
            // get data from dbModel
            var bookTable = dbModel.FirstOrDefault(x => x.TableName == TABLE_BOOK);
            var customerTable = dbModel.FirstOrDefault(x => x.TableName == TABLE_CUSTOMER);
            var bookCommentTable = dbModel.FirstOrDefault(x => x.TableName == TABLE_BOOK_COMMENT);
            
            if (bookTable == null || customerTable == null || bookCommentTable == null)
                throw new HttpResponseException(SERVER_ERROR_CODE);

            // clear existed data
            context.Database.ExecuteSqlCommand(bookCommentTable.MakeDeleteDataCmd());

            // begin to insert
            var limit = totalComment / 5;
            var books = context.Books.FromSql(bookTable.MakeGetRandomCmd(limit)).Select(x => new Book {Isbn = x.Isbn}).ToList();
            var customers = context.Customers.FromSql(customerTable.MakeGetRandomCmd(limit)).Select(x => new Customer {UserId = x.UserId}).ToList();

            // perform reset sequence
            var seeder = new BookCommentTableSeeder(totalComment, books, customers);
            context.Database.ExecuteSqlCommand(bookCommentTable.MakeSetSequenceCmd(totalComment));
            context.BookComments.AddRange(seeder.BookComments);
            context.SaveChanges();
            return context.BookComments.AsNoTracking().ToList();
        }

        public List<Order> SeedOrders(int total, int multiplier)
        {
            // get data from dbModel
            var bookTable = dbModel.FirstOrDefault(x => x.TableName == TABLE_BOOK);
            var customerTable = dbModel.FirstOrDefault(x => x.TableName == TABLE_CUSTOMER);
            var managerTable = dbModel.FirstOrDefault(x => x.TableName == TABLE_MANAGER);
            var orderTable = dbModel.FirstOrDefault(x => x.TableName == TABLE_ORDER);
            var orderDetailTable = dbModel.FirstOrDefault(x => x.TableName == TABLE_ORDER_DETAIL);
            
            if (bookTable == null || customerTable == null || managerTable == null || orderTable == null || orderDetailTable == null)
                throw new HttpResponseException(SERVER_ERROR_CODE);

            // clear existed data
            context.Database.ExecuteSqlCommand(orderTable.MakeDeleteDataCmd());
            context.Database.ExecuteSqlCommand(orderDetailTable.MakeDeleteDataCmd());

            // begin to insert
            var limit = total / 5;
            var books = context.Books.FromSql(bookTable.MakeGetRandomCmd(limit)).Select(x => new Book {Isbn = x.Isbn, Price = x.Price}).ToList();
            var customers = context.Customers.FromSql(customerTable.MakeGetRandomCmd(limit)).Select(x => new Customer {UserId = x.UserId, Fullname = x.Fullname, Address = x.Address, Phone = x.Phone}).ToList();
            var managers = context.Managers.AsNoTracking().Select(x => new Manager {UserId = x.UserId}).ToList();

            // perform reset sequence
            var seeder = new OrderTableSeeder(total, multiplier, customers, managers, books);
            context.Orders.AddRange(seeder.Orders);
            context.OrderDetails.AddRange(seeder.OrderDetails);
            context.Database.ExecuteSqlCommand(orderTable.MakeSetSequenceCmd(total));
            context.Database.ExecuteSqlCommand(orderDetailTable.MakeSetSequenceCmd(seeder.OrderDetails.Count));
            context.SaveChanges();
            return context.Orders.AsNoTracking().ToList();
        }

        public List<DbTable> Migration()
        {
            var amount = dbModel.Count;
            for (var i = 0; i < amount; i++) RunDefineTable(dbModel[i]);

            var seeder = new MigrationTableSeeder();
            context.UserRoles.AddRange(seeder.UserRoles);
            context.OrderStatuses.AddRange(seeder.OrderStatuses);

            context.SaveChanges();
            return dbModel;
        }

        private void RunDefineTable(DbTable dbTable)
        {
            context.Database.ExecuteSqlCommand(dbTable.MakeDropCmd());
            context.Database.ExecuteSqlCommand(dbTable.MakeCreateCmd());
            if (!string.IsNullOrEmpty(dbTable.IndexColumn)) context.Database.ExecuteSqlCommand(dbTable.MakeIndexCmd());
        }
    }
}