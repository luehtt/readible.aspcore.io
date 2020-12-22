namespace Readible.Migrations
{
    public class MigrationConst
    {
        public const string TABLE_USER_ROLE = "user_roles";
        public const string TABLE_USER = "users";
        public const string TABLE_MANAGER = "managers";
        public const string TABLE_CUSTOMER = "customers";
        public const string TABLE_ORDER_STATUS = "order_statuses";
        public const string TABLE_ORDER = "orders";
        public const string TABLE_ORDER_DETAIL = "order_details";
        public const string TABLE_BOOK_CATEGORY = "book_categories";
        public const string TABLE_BOOK = "books";
        public const string TABLE_BOOK_COMMENT = "book_comments";

        public const string COLUMN_ID = "id";
        public const string COLUMN_ISBN = "isbn";
        public const string COLUMN_USER_ID = "user_id";
        public const string COLUMN_USERNAME = "username";

        public const int FAKER_GENERATOR = 1;

        public const string DEFAULT_EMAIL_SUFFIX = "@demo.com";
        public const string DEFAULT_PASSWORD = "password";
        public const string DEFAULT_LANGUAGE = "English";

        public const int ADOLESCENCE = 12;
        public const int YOUNG_ADULT = 18;
        public const int MIDDLE_ADULT = 45;
        public const int OLD_ADULT = 65;
        public const int HIGHEST_AGE = 100;
        public const int YEAR_OFFSET = 3;

        public const int SEEDER_ADMIN = 5;
        public const int SEEDER_MANAGER = 25;
        public const int SEEDER_CUSTOMER = 500;
        public const int SEEDER_BOOK = 10000;
        public const int SEEDER_BOOK_CATEGORY = 10;
        public const int SEEDER_BOOK_COMMENT = 10000;
        public const int SEEDER_ORDER = 10000;
        public const int SEEDER_MULTIPLIER = 5;
        public const int SEEDER_AUTHOR = 500;
        public const int SEEDER_PUBLISHER = 100;

        public const string QUERY_SEED_ADMIN = "admin";
        public const string QUERY_SEED_MANAGER = "manager";
        public const string QUERY_SEED_CUSTOMER = "customer";
        public const string QUERY_SEED_CATEGORY = "category";
        public const string QUERY_SEED_BOOK = "book";
        public const string QUERY_SEED_ORDER = "order";
        public const string QUERY_SEED_MULTIPLIER = "multiplier";
        public const string QUERY_BOOK_COMMENT = "comment";
    }
}