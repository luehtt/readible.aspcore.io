using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Readible.Models;
using Readible.Shared;
using static Readible.Shared.Const;

namespace Readible.Services
{
    public class CustomerService : DataContextService, IDataContextService<Customer>
    {
        private readonly DataContext context;

        public CustomerService(DataContext context)
        {
            this.context = context;
        }

        public Task<Customer> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Customer> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Customer>> Fetch()
        {
            return await context.Customers.AsNoTracking().OrderBy(e => e.Fullname).ToListAsync();
        }

        public async Task<int> Count()
        {
            return await context.Customers.AsNoTracking().CountAsync();
        }

        public async Task<Customer> GetDetail(int id)
        {
            var item = await context.Customers.AsNoTracking().Where(e => e.UserId == id)
                .Include(e => e.BookComments).Include(e => e.Orders).ThenInclude(f => f.Status).FirstOrDefaultAsync();
            CatchNotFound(item);
            return item;
        }

        public Task<Customer> Get(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Customer> Store(Customer data)
        {
            throw new NotImplementedException();
        }

        public async Task<Customer> Update(int id, Customer data)
        {
            CatchCondition(id != data.UserId);

            var item = await context.Customers.FirstOrDefaultAsync(e => e.UserId == id);
            CatchNotFound(item);
            CatchTimestampMismatched(data.UpdatedAt, item.UpdatedAt);

            item.Fullname = data.Fullname;
            item.Male = data.Male;
            item.Birth = data.Birth;
            item.Phone = data.Phone;
            item.Address = data.Address;
            item.Image = UpdateImage(item.Image, data.Image, USER_MAX_WIDTH, USER_MAX_HEIGHT);
            item.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return item;
        }

        public Task<Customer> Update(string id, Customer data)
        {
            throw new NotImplementedException();
        }

        private async Task<Customer> GetCustomerSummarize(int id)
        {
            return await context.Customers.AsNoTracking()
                .Select(e => new Customer {
                    UserId = e.UserId,
                    Fullname = e.Fullname,
                    Male = e.Male,
                    Birth = e.Birth,
                    Address = e.Address,
                    Phone = e.Phone
                }).FirstOrDefaultAsync(x => x.UserId == id);
        }

        public async Task<IEnumerable<object>> SummarizePurchased(int limit)
        {
            var data = await context.Orders.AsNoTracking().GroupBy(e => e.CustomerId)
                .Select(e => new {CustomerId = e.Key, Purchased = e.Sum(f => f.TotalItem)})
                .OrderByDescending(x => x.Purchased).Take(limit).ToListAsync();

            var list = new List<object>();
            foreach (var d in data)
            {
                var item = await GetCustomerSummarize(d.CustomerId);
                if (item == null) continue;

                list.Add(new {
                    item.UserId,
                    item.Fullname,
                    item.Male,
                    item.Birth,
                    item.Address,
                    item.Phone,
                    TotalPurchased = d.Purchased
                });
            }

            return list;
        }

        public async Task<IEnumerable<object>> SummarizePaid(int limit)
        {
            var data = await context.Orders.AsNoTracking().GroupBy(e => e.CustomerId)
                .Select(e => new {CustomerId = e.Key, Paid = e.Sum(f => f.TotalPrice)})
                .OrderByDescending(x => x.Paid).Take(limit).ToListAsync();

            var list = new List<object>();
            foreach (var d in data)
            {
                var item = await GetCustomerSummarize(d.CustomerId);
                if (item == null) continue;

                list.Add(new {
                    item.UserId,
                    item.Fullname,
                    item.Male,
                    item.Birth,
                    item.Address,
                    item.Phone,
                    TotalPaid = d.Paid
                });
            }

            return list;
        }

        public async Task<OrderStatistic[]> SummarizeAge(DateTime fromDate, DateTime toDate)
        {
            var data = await GetOrdersForSummarize(fromDate, toDate);
            var list = SummarizeAgeExtend(data);
            var properties = ENUM_AGE_NAME;

            return MapOrderStatistic(list, properties);
        }

        private async Task<IEnumerable<Order>> GetOrdersForSummarize(DateTime fromDate, DateTime toDate)
        {
            return await context.Orders.AsNoTracking().Where(x => x.UpdatedAt > fromDate && x.UpdatedAt < toDate).Include(e => e.Customer).ToListAsync();
        }

        private List<Order>[] SummarizeAgeExtend(IEnumerable<Order> data)
        {
            var list = new List<Order>[4];
            for (var i = 0; i < list.Length; i++) list[i] = new List<Order>();

            foreach (var d in data)
            {
                var age = d.CreatedAt?.Year - d.Customer.Birth;
                if (age < YOUNG_ADULT) list[0].Add(d);
                else if (age < MIDDLE_ADULT) list[1].Add(d);
                else if (age < OLD_ADULT) list[2].Add(d);
                else list[3].Add(d);
            }

            return list;
        }

        private OrderStatistic[] MapOrderStatistic(IReadOnlyList<IEnumerable<Order>> data, IReadOnlyList<string> properties)
        {
            var length = properties.Count;
            var list = new OrderStatistic[length];

            for (var i = 0; i < length; i++)
            {
                list[i] = new OrderStatistic(properties[i], data[i].ToList().Count, data[i].Sum(x => x.TotalItem), data[i].Sum(x => x.TotalItem));
            }

            return list;
        }

        private List<Order>[] SummarizeGenderExtend(IEnumerable<Order> data)
        {
            var list = new List<Order>[4];
            for (var i = 0; i < list.Length; i++) list[i] = new List<Order>();

            foreach (var d in data)
            {
                if (d.Customer.Male) list[0].Add(d);
                else if (d.Customer.Male == false) list[1].Add(d);
            }

            return list;
        }

        public async Task <OrderStatistic[]> SummarizeGender(DateTime fromDate, DateTime toDate)
        {
            var data = await GetOrdersForSummarize(fromDate, toDate);
            var list = SummarizeGenderExtend(data);
            var properties = ENUM_GENDER_NAME;

            return MapOrderStatistic(list, properties);
        }
    }
}