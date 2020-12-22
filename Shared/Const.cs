namespace Readible.Shared
{
    public class Const
    {
        public const int DEFAULT_TOP_SIZE = 10;
        public const int DEFAULT_SHOP_SIZE = 30;
        public const double COMPARE_TOLERANCE = 0.1;

        public const int SIMILAR_BOOK = 4;
        public const int ADOLESCENCE = 12;
        public const int YOUNG_ADULT = 18;
        public const int MIDDLE_ADULT = 45;
        public const int OLD_ADULT = 65;
        public const int HIGHEST_AGE = 100;
        public static readonly string[] ENUM_AGE_NAME = { "adolescent", "young adult", "middle adult", "old adult" };
        public static readonly string[] ENUM_GENDER_NAME = { "male", "female" };

        public const int BOOK_MAX_WIDTH = 500;
        public const int BOOK_MAX_HEIGHT = 500;
        public const int USER_MAX_WIDTH = 250;
        public const int USER_MAX_HEIGHT = 250;
        public const int DEFAULT_JPEG_QUALITY = 75;

        public const string ORDER_STATUS_PENDING = "PENDING";
        public const string ORDER_STATUS_DELIVERING = "DELIVERING";
        public const string ORDER_STATUS_SUCCESS = "SUCCESS";
        public const string ORDER_STATUS_FAILED = "FAILED";
        public const string ORDER_STATUS_ETC = "FAILED";

        public const string USER_ROLE_ADMIN = "ADMIN";
        public const string USER_ROLE_MANAGER = "MANAGER";
        public const string USER_ROLE_CUSTOMER = "CUSTOMER";
        public const string USER_ROLE_MANAGER_CUSTOMER = "MANAGER, CUSTOMER";
        public const string USER_ROLE_ADMIN_MANAGER = "ADMIN, MANAGER";

        public const string QUERY_STATUS = "status";
        public const string QUERY_FROM_DATE = "fromDate";
        public const string QUERY_TO_DATE = "toDate";
        public const string QUERY_REFERENCE = "reference";
        public const string QUERY_DATE = "day";
        public const string QUERY_MONTH = "month";
        public const string QUERY_YEAR = "year";
        public const string QUERY_AGE = "age";
        public const string QUERY_GENDER = "gender";
        public const string QUERY_PAGE = "page";
        public const string QUERY_PAGE_SIZE = "pageSize";
        public const string QUERY_CATEGORY = "category";
        public const string QUERY_SEARCH = "search";

    }

    public class RouteConst {
        public const string ACCOUNT = "api/accounts";
        public const string BOOK = "api/books";
        public const string BOOK_CATEGORY = "api/book-categories";
        public const string BOOK_COMMENT = "api/book-comments";
        public const string CUSTOMER = "api/customers";
        public const string DASHBOARD = "api/dashboard";
        public const string MANAGER = "api/managers";
        public const string ORDER = "api/orders";
        public const string SHOP = "api/shop";
        public const string STATISTIC = "api/statistic";
        public const string ID = "{id}";
    }

    public class HttpStatus
    {
        public const int BAD_REQUEST_CODE = 400;
        public const int UNAUTHORIZED_CODE = 401;
        public const int FORBIDDEN_CODE = 403;
        public const int NOT_FOUND_CODE = 404;
        public const int CONFLICT_CODE = 409;
        public const int UNPROCESSABLE_ENTITY = 422;
        public const int SERVER_ERROR_CODE = 500;
        public const int NOT_IMPLEMENTED_CODE = 501;

        public const string BAD_REQUEST = "The server cannot or will not process the request due to an apparent client error.";
        public const string FORBIDDEN = "The request was valid, but the server is refusing action due to lack of permissions.";
        public const string NOT_FOUND = "The requested resource could not be found but may be available in the future.";
        public const string CONFLICT = "The request was valid, but could not be processed because of conflict in the current state of the resource.";
        public const string SERVER_ERROR = "The server has met an unknown error and cannot complete the requests.";
        public const string NOT_IMPLEMENTED = "The server either does not recognize the request method.";
        public const string TIMESTAMP_MISMATCHED = "The timestamp of the item that has been updating does not match the one in the database. Please reload the page to continue.";

        public static string Message(int code)
        {
            switch (code)
            {
                case BAD_REQUEST_CODE: return BAD_REQUEST;
                case FORBIDDEN_CODE: return FORBIDDEN;
                case NOT_FOUND_CODE: return NOT_FOUND;
                case CONFLICT_CODE: return CONFLICT;
                case SERVER_ERROR_CODE: return SERVER_ERROR;
                case NOT_IMPLEMENTED_CODE: return NOT_IMPLEMENTED;
                default: return SERVER_ERROR;
            }
        }
    }
}
