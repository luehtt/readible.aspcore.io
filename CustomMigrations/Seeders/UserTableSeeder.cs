using Bogus;
using Bogus.DataSets;
using Readible.Auth;
using Readible.Models;
using System;
using System.Collections.Generic;
using static Readible.Migrations.MigrationConst;

namespace Readible.Migrations.DataSeeders
{
    public class UserTableSeeder
    {
        public List<User> Users { get; }
        public List<Manager> Managers { get; }
        public List<Customer> Customers { get; }

        private class UserTableSeederModel
        {
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public bool Male { get; set; }
            public DateTime Timestamp { get; set; }

            public string Fullname => Firstname + " " + Lastname;
            public string Email => Username + DEFAULT_EMAIL_SUFFIX;
            public string Username => (Firstname + Lastname).ToLower();

            public UserTableSeederModel(string firstname, string lastname, bool male, DateTime timestamp)
            {
                Firstname = firstname;
                Lastname = lastname;
                Male = male;
                Timestamp = timestamp;
            }
        }

        public UserTableSeeder(int totalAdmin, int totalManager, int totalCustomer)
        {
            Randomizer.Seed = new Random(FAKER_GENERATOR);
            var faker = new Faker();

            var total = totalAdmin + totalManager + totalCustomer;
            var userModels = SeedUserModels(total, faker);
            Users = SeedUsers(totalAdmin, totalManager, totalCustomer, userModels);
            Customers = SeedCustomers(totalAdmin, totalManager, totalCustomer, userModels, faker);
            Managers = SeedManagers(totalAdmin, totalManager, userModels, faker);
        }

        private List<UserTableSeederModel> SeedUserModels(int total, Faker faker)
        {
            var list = new List<UserTableSeederModel>();
            for (var i = 0; i < total; i++)
            {
                var timestamp = faker.Date.Past(YEAR_OFFSET);
                if (i % 2 == 0) list.Add(new UserTableSeederModel(faker.Name.FirstName(Name.Gender.Male), faker.Name.LastName(), true, timestamp));
                else list.Add(new UserTableSeederModel(faker.Name.FirstName(Name.Gender.Female), faker.Name.LastName(), false, timestamp));
            }
            return list;
        }

        private List<User> SeedUsers(int totalAdmin, int totalManager, int totalCustomer, IReadOnlyList<UserTableSeederModel> userModels)
        {
            var list = new List<User>();
            var total = totalAdmin + totalManager + totalCustomer;

            var hasher = new BCryptPasswordHasher<User>();
            var password = hasher.HashPassword(null, DEFAULT_PASSWORD);

            for (var i = 0; i < total; i++)
            {
                var item = new User
                {
                    Id = i + 1,
                    Username = userModels[i].Username,
                    Email = userModels[i].Email,
                    Password = password,
                    Active = true,
                    UserRoleId = i < totalAdmin ? 1 : i < totalAdmin + totalManager ? 2 : 3,
                    CreatedAt = userModels[i].Timestamp,
                    UpdatedAt = userModels[i].Timestamp,
                    ConnectId = Guid.NewGuid().ToString()
                };
                list.Add(item);
            }
            return list;
        }

        private List<Manager> SeedManagers(int totalAdmin, int totalManager, IReadOnlyList<UserTableSeederModel> userModels, Faker faker)
        {
            var list = new List<Manager>();
            var year = DateTime.Now.Year;

            for (var i = 0; i < totalAdmin + totalManager; i++)
            {
                var item = new Manager
                {
                    UserId = i + 1,
                    Fullname = userModels[i].Fullname,
                    Birth = year - RandomAge(faker),
                    Male = userModels[i].Male,
                    Address = faker.Address.FullAddress(),
                    Phone = faker.Phone.PhoneNumberFormat(),
                    CreatedAt = userModels[i].Timestamp,
                    UpdatedAt = userModels[i].Timestamp
                };
                list.Add(item);
            }
            return list;
        }

        private List<Customer> SeedCustomers(int totalAdmin, int totalManager, int totalCustomer, IReadOnlyList<UserTableSeederModel> userModels, Faker faker)
        {
            var list = new List<Customer>();
            var total = totalAdmin + totalManager + totalCustomer;
            var year = DateTime.Now.Year;

            for (var i = totalAdmin + totalManager; i < total; i++)
            {
                var item = new Customer
                {
                    UserId = i + 1,
                    Fullname = userModels[i].Fullname,
                    Birth = year - RandomAge(faker),
                    Male = userModels[i].Male,
                    Address = faker.Address.FullAddress(),
                    Phone = faker.Phone.PhoneNumberFormat(),
                    CreatedAt = userModels[i].Timestamp,
                    UpdatedAt = userModels[i].Timestamp
                };
                list.Add(item);
            }
            return list;
        }

        private static int RandomAge(Faker faker)
        {
            var factor = faker.Random.Int(0, 20);
            if (factor < 3) return faker.Random.Int(ADOLESCENCE, YOUNG_ADULT);
            if (factor < 6) return faker.Random.Int(OLD_ADULT, HIGHEST_AGE);
            if (factor < 12) return faker.Random.Int(MIDDLE_ADULT, OLD_ADULT);
            return faker.Random.Int(YOUNG_ADULT, MIDDLE_ADULT);
        }
    }
}
