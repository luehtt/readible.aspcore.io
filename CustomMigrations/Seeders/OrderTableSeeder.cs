using Bogus;
using Readible.Models;
using System;
using System.Collections.Generic;
using static Readible.Migrations.MigrationConst;

namespace Readible.Migrations.DataSeeders
{
    public class OrderTableSeeder
    {
        public List<OrderDetail> OrderDetails { get; set; }
        public List<Order> Orders { get; set; }

        private class OrderTotalModel
        {
            public int TotalItem { get; set; }
            public double TotalPrice { get; set; }

            public OrderTotalModel(IReadOnlyList<OrderDetail> data)
            {
                var count = data.Count;
                var totalItem = 0;
                var totalPrice = 0.0;

                for (var i = 0; i < count; i++)
                {
                    totalItem += data[i].Amount;
                    totalPrice += data[i].Price * data[i].Amount;
                }

                TotalItem = totalItem;
                TotalPrice = totalPrice;
            }
        }

        public OrderTableSeeder(int total, int multiple, IReadOnlyList<Customer> customers, IReadOnlyList<Manager> managers, IReadOnlyList<Book> books)
        {
            Randomizer.Seed = new Random(FAKER_GENERATOR);
            var faker = new Faker();

            Orders = SeedOrders(total, multiple, customers, managers, books, faker);
            OrderDetails = SeedOrderDetails(Orders);
            Orders = TruncateOrders(Orders);
        }

        private List<Order> SeedOrders(int total, int multiple,
            IReadOnlyList<Customer> customers,
            IReadOnlyList<Manager> managers,
            IReadOnlyList<Book> books, Faker faker)
        {
            var list = new List<Order>();
            var customerCount = customers.Count;
            var managerCount = managers.Count;

            for (var i = 0; i < total; i++)
            {
                // seed item
                var timestamp = faker.Date.Past(YEAR_OFFSET);
                var customer = customers[faker.Random.Int(0, customerCount - 1)];

                var item = new Order
                {
                    Id = i + 1,
                    CustomerId = customer.UserId,
                    Contact = customer.Fullname,
                    Address = customer.Address,
                    Phone = customer.Phone,
                    StatusId = RandomStatus(faker),
                    CreatedAt = timestamp,
                    UpdatedAt = timestamp
                };

                // seed manager
                var manager = managers[faker.Random.Int(0, managerCount - 1)];
                if (item.StatusId > 1) {
					item.ConfirmerId = manager.UserId;
					item.ConfirmedAt = timestamp;
				}
                if (item.StatusId > 2) {
					item.CompleterId = manager.UserId;
					item.CompletedAt = timestamp;
				}

                // seed detail
                var details = SeedOrderDetails(multiple, item.Id, books, faker);
                var totalModel = new OrderTotalModel(details);
                item.OrderDetails = details;
                item.TotalItem = totalModel.TotalItem;
                item.TotalPrice = totalModel.TotalPrice;

                list.Add(item);
            }
            return list;
        }

        private List<OrderDetail> SeedOrderDetails(int multiple, int orderId, IReadOnlyList<Book> books, Faker faker)
        {
            var list = new List<OrderDetail>();
            var bookCount = books.Count;
            var amount = faker.Random.Int(1, multiple);

            for (var i = 0; i < amount; i++)
            {
                var book = books[faker.Random.Int(0, bookCount - 1)];
                var detail = new OrderDetail
                {
                    OrderId = orderId,
                    BookIsbn = book.Isbn,
                    Price = book.Discount == 0 ? book.Price : book.Price - book.Price * book.Discount / 100,
                    Amount = faker.Random.Int(1, multiple)
                };
                list.Add(detail);
            }
            return list;
        }

        private List<OrderDetail> SeedOrderDetails(IReadOnlyList<Order> orders)
        {
            var list = new List<OrderDetail>();
            var orderCount = orders.Count;
            for (var i = 0; i < orderCount; i++) list.AddRange(orders[i].OrderDetails);
            return list;
        }

        private List<Order> TruncateOrders(List<Order> orders)
        {
            var orderCount = orders.Count;
            for (var i = 0; i < orderCount; i++) orders[i].OrderDetails = null;
            return orders;
        }

        private int RandomStatus(Faker faker)
        {
            var factor = faker.Random.Int(1, 10);
            switch (factor)
            {
                case 1: return 1;
                case 2: return 1;
                case 3: return 2;
                case 10: return 4;
                default: return 3;
            }
        }


    }
}
