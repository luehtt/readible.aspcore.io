using Bogus;
using Readible.Models;
using System;
using System.Collections.Generic;
using static Readible.Migrations.MigrationConst;

namespace Readible.Migrations.DataSeeders
{
    public class BookTableSeeder
    {
        public List<BookCategory> BookCategories { get; set; }
        public List<Book> Books { get; set; }

        public BookTableSeeder(int totalCategory, int totalBook)
        {
            Randomizer.Seed = new Random(FAKER_GENERATOR);
            var faker = new Faker();

            BookCategories = SeedCategories(totalCategory, faker);
            Books = SeedBooks(totalBook, totalCategory, SEEDER_AUTHOR, SEEDER_PUBLISHER, faker);
        }

        private List<BookCategory> SeedCategories(int total, Faker faker)
        {
            var list = new List<BookCategory>();
            for (var i = 0; i < total; i++)
            {
                var timestamp = faker.Date.Past(YEAR_OFFSET);
                var item = new BookCategory
                {
                    Id = i + 1,
                    Name = faker.Commerce.ProductMaterial() + " " + faker.Commerce.ProductMaterial(),
                    CreatedAt = timestamp,
                    UpdatedAt = timestamp
                };
                list.Add(item);
            }
            return list;
        }

        private List<string> SeedBookAuthors(int total, Faker faker)
        {
            var list = new List<string>();
            for (var i = 0; i < total; i++)
            {
                list.Add(faker.Name.FullName());
            }
            return list;
        }

        private List<string> SeedBookPublishers(int total, Faker faker)
        {
            var list = new List<string>();
            for (var i = 0; i < total; i++)
            {
                list.Add(faker.Company.CompanyName());
            }
            return list;
        }

        private List<Book> SeedBooks(int total, int categoryAmount, int authorAmount, int publisherAmount, Faker faker)
        {
            var authors = SeedBookAuthors(authorAmount, faker);
            var publishers = SeedBookPublishers(publisherAmount, faker);
            var authorCount = authors.Count;
            var publisherCount = publishers.Count;

            var list = new List<Book>();
            for (var i = 0; i < total; i++)
            {
                var timestamp = faker.Date.Past();
                var item = new Book
                {
                    Isbn = faker.Commerce.Ean13(),
                    Title = faker.Commerce.ProductName(),
                    Author = authors[faker.Random.Int(0, authorCount - 1)],
                    Publisher = publishers[faker.Random.Int(0, publisherCount - 1)],
                    Published = faker.Date.Past(10),
                    Language = DEFAULT_LANGUAGE,
                    CategoryId = i % categoryAmount + 1,
                    Info = faker.Lorem.Sentence(128),
                    Page = faker.Random.Int(100, 1000),
                    Price = 20 + faker.Random.Int(0, 80) + faker.Random.Int(0, 100) / 100.0,
                    Discount = faker.Random.Bool() ? faker.Random.Int(0, 50) : 0,
                    Active = true,
                    Viewed = faker.Random.Int(0, 100) * 10,
                    CreatedAt = timestamp,
                    UpdatedAt = timestamp
                };
                list.Add(item);
            }
            return list;
        }
    }
}
