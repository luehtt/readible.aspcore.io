using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Readible.Models;
using Readible.Shared;

namespace Readible.Services
{
    public class BookCategoryService : DataContextService, IDataContextService<BookCategory>
    {
        private readonly DataContext context;

        public BookCategoryService(DataContext context)
        {
            this.context = context;
        }

        public async Task<BookCategory> Delete(int id)
        {
            var item = await context.BookCategories.FindAsync(id);
            CatchNotFound(item);
            var book = await context.Books.AsNoTracking().FirstOrDefaultAsync(x => x.CategoryId == item.Id);
            CatchConflict(book);

            context.BookCategories.Remove(item);
            await context.SaveChangesAsync();

            return item;
        }

        public Task<BookCategory> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<BookCategory>> Fetch()
        {
            return await context.BookCategories.AsNoTracking().OrderBy(e => e.Name).ToListAsync();
        }

        public async Task<int> Count()
        {
            return await context.BookCategories.AsNoTracking().CountAsync();
        }

        public async Task<BookCategory> GetDetail(int id)
        {
            var item = await context.BookCategories.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            CatchNotFound(item);
            return item;
        }

        public async Task<BookCategory> Get(string id)
        {
            var item = await context.BookCategories.AsNoTracking().FirstOrDefaultAsync(e => e.Name == id);
            CatchNotFound(item);
            return item;
        }

        public async Task<BookCategory> Store(BookCategory data)
        {
            CatchCondition(data.Id != 0);
            var item = await context.BookCategories.FirstOrDefaultAsync(e => e.Name == data.Name);
            CatchConflict(item);

            data.CreatedAt = DateTime.UtcNow;
            data.UpdatedAt = DateTime.UtcNow;

            context.BookCategories.Add(data);
            await context.SaveChangesAsync();
            return data;
        }

        public async Task<BookCategory> Update(int id, BookCategory data)
        {
            CatchCondition(id != data.Id);
            var item = await context.BookCategories.FirstOrDefaultAsync(e => e.Id == id);
            CatchNotFound(item);
            CatchTimestampMismatched(data.UpdatedAt, item.UpdatedAt);
            
            var unique = await context.BookCategories.FirstOrDefaultAsync(e => e.Name == data.Name);
            CatchCondition(unique != null && unique.Id != item.Id, HttpStatus.CONFLICT_CODE);

            item.Name = data.Name;
            data.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return item;
        }

        public Task<BookCategory> Update(string id, BookCategory data)
        {
            throw new NotImplementedException();
        }
    }
}