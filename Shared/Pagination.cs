namespace Readible.Shared
{
    public class Pagination
    {
        public Pagination(int totalItems, int itemsPerPage, int pageIndex)
        {
            TotalItems = totalItems;
            ItemsPerPage = itemsPerPage;
            PageIndex = pageIndex;
            StartIndex = (pageIndex - 1) * itemsPerPage;
            TotalPages = totalItems / itemsPerPage + (totalItems % itemsPerPage > 0 ? 1 : 0);
            CurrentItemCount = pageIndex * itemsPerPage <= totalItems
                ? itemsPerPage
                : totalItems - (pageIndex - 1) * itemsPerPage;
        }

        public int CurrentItemCount { get; }
        public int ItemsPerPage { get; }
        public int PageIndex { get; }
        public int StartIndex { get; }
        public int TotalItems { get; }
        public int TotalPages { get; }
    }
}