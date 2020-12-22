using Bogus;
using Readible.Models;
using System;
using System.Collections.Generic;
using static Readible.Migrations.MigrationConst;

namespace Readible.Migrations.DataSeeders
{
    public class BookCommentTableSeeder
    {
        public List<BookComment> BookComments { get; set; }

        public BookCommentTableSeeder(int total, IReadOnlyList<Book> books, IReadOnlyList<Customer> customers)
        {
            Randomizer.Seed = new Random(FAKER_GENERATOR);
            var faker = new Faker();

            BookComments = SeedComments(total, books, customers, faker);
        }

        private List<BookComment> SeedComments(int amount, IReadOnlyList<Book> books, IReadOnlyList<Customer> customers, Faker faker)
        {
            var customerCount = customers.Count;
            var bookCount = books.Count;

            var list = new List<BookComment>();
            for (var i = 0; i < amount; i++)
            {
                var timestamp = faker.Date.Past(YEAR_OFFSET);
                var item = new BookComment
                {
                    Id = i + 1,
                    CustomerId = customers[faker.Random.Int(0, customerCount - 1)].UserId,
                    BookIsbn = books[faker.Random.Int(0, bookCount - 1)].Isbn,
                    Rating = faker.Random.Int(1, 5),
                    Comment = faker.Lorem.Sentence(32),
                    CreatedAt = timestamp,
                    UpdatedAt = timestamp
                };
                list.Add(item);
            }

            return list;
        }
    }
}
