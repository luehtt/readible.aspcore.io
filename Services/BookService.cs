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
    public class BookService : DataContextService, IDataContextService<Book>
    {
        private readonly DataContext context;

        public BookService(DataContext context)
        {
            this.context = context;
        }

        public Task<Book> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Book> Delete(string id)
        {
            var item = await context.Books.FindAsync(id);
            CatchNotFound(item);
            var order = await context.OrderDetails.FirstOrDefaultAsync(x => x.BookIsbn == id);
            CatchConflict(order);
            var comment = await context.BookComments.FirstOrDefaultAsync(x => x.BookIsbn == id);
            CatchConflict(comment);

            context.Books.Remove(item);
            await context.SaveChangesAsync();

            return item;
        }

        public async Task<int> Count()
        {
            return await context.Books.AsNoTracking().CountAsync();
        }

        public async Task<Book> Get(string id)
        {
            var item = await context.Books.AsNoTracking().Where(e => e.Isbn == id)
                .Include(e => e.Category).Include(e => e.BookComments).ThenInclude(f => f.Customer)
                .FirstOrDefaultAsync();
            CatchNotFound(item);
            return item;
        }

        public Task<Book> GetDetail(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Book> Store(Book data)
        {
            var item = await context.Books.FindAsync(data.Isbn);
            CatchConflict(item);

            data.Image = UpdateImage(data.Image, BOOK_MAX_WIDTH, BOOK_MAX_HEIGHT);
            data.CreatedAt = DateTime.UtcNow;
            data.UpdatedAt = DateTime.UtcNow;

            context.Books.Add(data);
            await context.SaveChangesAsync();

            var res = await context.Books.Where(x => x.Isbn == data.Isbn).Include(e => e.Category).FirstOrDefaultAsync();
			CatchNotFound(res);
            return res;
        }

        public Task<Book> Update(int id, Book data)
        {
            throw new NotImplementedException();
        }

        public async Task<Book> Update(string id, Book data)
        {
            CatchCondition(id != data.Isbn);
        
            var item = await context.Books.FirstOrDefaultAsync(e => e.Isbn == id);
            CatchNotFound(item);
            CatchTimestampMismatched(item.UpdatedAt, data.UpdatedAt);

            item.Title = data.Title;
            item.Author = data.Author;
            item.Active = data.Active;
            item.CategoryId = data.CategoryId;
            item.Publisher = data.Publisher;
            item.Published = data.Published;
            item.Discount = data.Discount;
            item.Page = data.Page;
            item.Price = data.Price;
            item.Info = data.Info;
            item.Language = data.Language;
            item.Image = UpdateImage(item.Image, data.Image, BOOK_MAX_WIDTH, BOOK_MAX_HEIGHT);
            item.CreatedAt = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
			var res = await context.Books.AsNoTracking().Where(e => e.Isbn == id).Include(e => e.Category).FirstOrDefaultAsync();
            CatchNotFound(res);
			return res;
        }

        public async Task<IEnumerable<Book>> Fetch()
        {
            return await context.Books.AsNoTracking().Include(e => e.Category).OrderBy(e => e.Isbn).ToListAsync();
        }

        public async Task<Book> Get(string id, bool increment)
        {
            // if it NOT increment then use NoTracking()
            if (increment != true)
            {
                var item = await context.Books.AsNoTracking().Where(e => e.Isbn == id)
                    .Include(e => e.Category).Include(e => e.BookComments).ThenInclude(f => f.Customer).FirstOrDefaultAsync();
                CatchNotFound(item);
                return item;
            }
            // else increase Book.viewed
            else
            {
                var item = await context.Books.Where(e => e.Isbn == id).Where(e => e.Active).Include(e => e.Category)
                    .Include(e => e.BookComments).ThenInclude(f => f.Customer).FirstOrDefaultAsync();
                CatchNotFound(item);
                item.Viewed = item.Viewed + 1;
                await context.SaveChangesAsync();
                return item;
            }
        }
        
        public async Task<List<Book>> GetSimilar(string id)
        {
            var item = await context.Books.FirstOrDefaultAsync(x => x.Isbn == id);
            CatchNotFound(item);
            var res = await context.Books.AsNoTracking().Where(x => x.CategoryId == item.CategoryId && x.Published.Value < item.Published.Value)
                .OrderByDescending(e => e.Published).Take(SIMILAR_BOOK)
                .Include(e => e.Category).Include(e => e.BookComments).ThenInclude(f => f.Customer).ToListAsync();
            return res;
        }

        private bool DataInclude(Book data, string search)
        {
            return data.Isbn.ToLower().Contains(search) ||
                data.Title.ToLower().Contains(search) ||
                data.Author.ToLower().Contains(search) ||
                data.Publisher.ToLower().Contains(search) ||
                data.Info.ToLower().Contains(search);
        }

        public async Task<IEnumerable<Book>> FetchShop()
        {
            return await context.Books.AsNoTracking().Where(x => x.Active)
                .Include(e => e.Category).Include(e => e.BookComments)
                .OrderByDescending(e => e.Published).ToListAsync();
        }

        public async Task<IEnumerable<Book>> FetchShop(int page, int pageSize)
        {
            return await context.Books.AsNoTracking().Where(x => x.Active).OrderByDescending(e => e.Published)
                .Skip((page - 1) * pageSize).Take(pageSize).Include(e => e.Category).Include(e => e.BookComments)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> FetchShop(string search, int page, int pageSize)
        {
            search = search.ToLower();
            return await context.Books.AsNoTracking().Where(x => x.Active).OrderByDescending(e => e.Published)
                .Where(x => DataInclude(x, search)).Skip((page - 1) * pageSize).Take(pageSize).Include(e => e.Category)
                .Include(e => e.BookComments).ToListAsync();
        }

        private async Task<BookCategory> GetBookCategory(string categoryName)
        {
            var item = await context.BookCategories.AsNoTracking().FirstOrDefaultAsync(x => x.Name.ToLower() == categoryName);
            CatchNotFound(item);
            return item;
        }

        public async Task<IEnumerable<Book>> FetchShop(int page, int pageSize, string categoryName)
        {
            var category = await GetBookCategory(categoryName.ToLower());
            return await context.Books.AsNoTracking().Where(x => x.Active).OrderByDescending(e => e.Published)
                .Where(x => x.CategoryId == category.Id).Skip((page - 1) * pageSize).Take(pageSize)
                .Include(e => e.Category)
                .Include(e => e.BookComments).ToListAsync();
        }

        public async Task<IEnumerable<Book>> FetchShop(string search, int page, int pageSize, string filter)
        {
            var category = await GetBookCategory(filter.ToLower());
            return await context.Books.AsNoTracking().Where(x => x.Active).OrderByDescending(e => e.Published)
                .Where(x => x.CategoryId == category.Id).Where(x => DataInclude(x, search))
                .Skip((page - 1) * pageSize).Take(pageSize).Include(e => e.Category)
                .Include(e => e.BookComments).ToListAsync();
        }

        public async Task<IEnumerable<Book>> FetchNoImage()
        {
            return await context.Books.AsNoTracking().Include(e => e.Category).Select(e => new Book {
                Isbn = e.Isbn,
                Title = e.Title,
                Author = e.Author,
                Publisher = e.Publisher,
                Published = e.Published,
                Price = e.Price,
                Discount = e.Discount,
                CategoryId = e.CategoryId,
                Category = e.Category
            }).OrderBy(e => e.Isbn).ToListAsync();
        }

        public async Task<int> CountFetch()
        {
            return await context.Books.AsNoTracking().CountAsync(x => x.Active);
        }

        public async Task<int> CountSearch(string search)
        {
            search = search.ToLower();
            return await context.Books.AsNoTracking().Where(x => x.Active).Where(x => x.Active).CountAsync(x => DataInclude(x, search));
        }

        public async Task<int> CountFilter(string filter)
        {
            var category = await GetBookCategory(filter.ToLower());
            return await context.Books.AsNoTracking().Where(x => x.Active).CountAsync(x => x.CategoryId == category.Id);
        }

        public async Task<int> CountFetch(string search, string filter)
        {
            var category = await GetBookCategory(filter.ToLower());
            return await context.Books.AsNoTracking().Where(x => x.Active).Where(x => x.CategoryId == category.Id).CountAsync(x => DataInclude(x, search));
        }

        private async Task<Book> GetBookSummarize(string bookIsbn)
        {
            return await context.Books.AsNoTracking().Where(x => x.Isbn == bookIsbn).Include(e => e.Category)
                .Select(e => new Book
                {
                    Isbn = e.Isbn,
                    Category = e.Category,
                    CategoryId = e.CategoryId,
                    Author = e.Author,
                    Title = e.Title
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<object>> SummarizeSold(int limit)
        {
            var data = await context.OrderDetails.AsNoTracking().GroupBy(e => e.BookIsbn)
                .Select(e => new {BookIsbn = e.Key, TotalSold = e.Sum(f => f.Amount), TotalPrice = e.Sum(f => f.Amount * f.Price)})
                .OrderByDescending(x => x.TotalSold).Take(limit).ToListAsync();

            var list = new List<object>();
            foreach (var d in data)
            {
                var item = await GetBookSummarize(d.BookIsbn);
                if (item == null) continue;

                list.Add(new
                {
                    item.Isbn,
                    item.Category,
                    item.CategoryId,
                    item.Author,
                    item.Title,
                    d.TotalSold,
                    d.TotalPrice
                });
            }

            return list;
        }

        public async Task<IEnumerable<object>> SummarizeRating(int limit)
        {
            var data = await context.BookComments.AsNoTracking().GroupBy(e => e.BookIsbn)
                .Select(e => new {BookIsbn = e.Key, Count = e.Count(), Rating = e.Average(f => f.Rating)})
                .OrderByDescending(x => x.Rating).Take(limit).ToListAsync();

            var list = new List<object>();
            foreach (var d in data)
            {
                var item = await GetBookSummarize(d.BookIsbn);
                if (item == null) continue;

                list.Add(new
                {
                    item.Isbn,
                    item.Category,
                    item.CategoryId,
                    item.Author,
                    item.Title,
                    d.Count,
                    d.Rating
                });
            }

            return list;
        }
    }
}