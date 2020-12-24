using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Readible.Hub;
using Readible.Models;
using Readible.Shared;

namespace Readible.Services
{
    public class BookCommentService : DataContextService, IDataContextService<BookComment>
    {
        private readonly DataContext context;
        private readonly BookCommentHub bookCommentHub;

        public BookCommentService(DataContext context, BookCommentHub bookCommentHub)
        {
            this.context = context;
            this.bookCommentHub = bookCommentHub;
        }

        public async Task<BookComment> Delete(int id)
        {
            var item = await context.BookComments.FindAsync(id);
            CatchNotFound(item);

            context.BookComments.Remove(item);
            await context.SaveChangesAsync();

            _ = bookCommentHub.Delete(id);
            return item;
        }
        
        public async Task<BookComment> Delete(int id, int customerId)
        {
            var item = await context.BookComments.FindAsync(id);
            CatchNotFound(item);
            CatchCondition(item.CustomerId != customerId, HttpStatus.UNAUTHORIZED_CODE);

            context.BookComments.Remove(item);
            await context.SaveChangesAsync();

            _ = bookCommentHub.Delete(id);
            return item;
        }

        public Task<BookComment> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<BookComment>> Fetch()
        {
            return await context.BookComments.AsNoTracking()
                .Include(e => e.Customer).Include(e => e.Book)
                .Select(e => new BookComment
                {
                    Id = e.Id,
                    CustomerId = e.CustomerId,
                    BookIsbn = e.BookIsbn,
                    Rating = e.Rating,
                    Book = new Book { Isbn = e.Book.Isbn, Title = e.Book.Title},
                    Customer = new Customer { UserId = e.Customer.UserId, Fullname = e.Customer.Fullname},
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt
                }).ToListAsync();
        }

        public async Task<int> Count()
        {
            return await context.BookComments.AsNoTracking().CountAsync();
        }

        public async Task<BookComment> GetDetail(int id)
        {
            var item = await context.BookComments.AsNoTracking().Include(e => e.Customer).FirstOrDefaultAsync(e => e.Id == id);
            CatchNotFound(item);
            return item;
        }

        public Task<BookComment> Get(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<BookComment> Store(BookComment data)
        {
            CatchCondition(data.Id != 0);
            data.CreatedAt = DateTime.UtcNow;
            data.UpdatedAt = DateTime.UtcNow;
            
			context.BookComments.Add(data);
            await context.SaveChangesAsync();

            var res = await GetDetail(data.Id);
            _ = bookCommentHub.Store(res);
            return res;
        }

        public async Task<BookComment> Update(int id, BookComment data)
        {
            CatchCondition(id != data.Id);
            var item = await context.BookComments.FirstOrDefaultAsync(e => e.Id == id);
            CatchNotFound(item);
            CatchTimestampMismatched(data.UpdatedAt, item.UpdatedAt);

            item.Rating = data.Rating;
			item.Comment = data.Comment;
            item.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            var res = await GetDetail(data.Id);
            _ = bookCommentHub.Update(res);
            return res;
        }

        public Task<BookComment> Update(string id, BookComment data)
        {
            throw new NotImplementedException();
        }
    }
}