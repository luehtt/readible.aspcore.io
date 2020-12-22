using System.Collections.Generic;
using Readible.Models;

namespace Readible.Shared
{
    public interface IHttpResponsePagination<out T>
    {
        Pagination Pagination { get; }
        IEnumerable<T> Data { get; }
    }

    public class BookPagination : IHttpResponsePagination<Book>
    {
        public Pagination Pagination { get; }
        public IEnumerable<Book> Data { get; }

        public BookPagination(int totalItems, int itemsPerPage, int pageIndex, IEnumerable<Book> data)
        {
            Pagination = new Pagination(totalItems, itemsPerPage, pageIndex);
            Data = data;
        }

    }
}