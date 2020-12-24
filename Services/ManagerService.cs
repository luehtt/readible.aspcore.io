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
    public class ManagerService : DataContextService, IDataContextService<Manager>
    {
        private readonly DataContext context;

        public ManagerService(DataContext context)
        {
            this.context = context;
        }

        public Task<Manager> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Manager> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Manager>> Fetch()
        {
            return await context.Managers.AsNoTracking().OrderBy(e => e.Fullname).ToListAsync();
        }

        public async Task<int> Count()
        {
            return await context.Managers.AsNoTracking().CountAsync();
        }

        public async Task<Manager> GetDetail(int id)
        {
            var item = await context.Managers.AsNoTracking().Where(e => e.UserId == id)
                .Include(e => e.ConfirmedOrders).ThenInclude(f => f.Status)
                .Include(e => e.CompletedOrders).ThenInclude(f => f.Status)
                .FirstOrDefaultAsync();				
            CatchNotFound(item);
            return item;
        }

        public Task<Manager> Get(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Manager> Store(Manager data)
        {
            throw new NotImplementedException();
        }

        public async Task<Manager> Update(int id, Manager data)
        {
			var user = await context.Users.AsNoTracking().Where(e => e.Id == id).Include(e => e.UserRole).FirstOrDefaultAsync();
			CatchNotFound(user);
            CatchCondition(user.UserRole.Name == "ADMIN", HttpStatus.FORBIDDEN_CODE);
			
            CatchCondition(id != data.UserId);
            var item = await context.Managers.FirstOrDefaultAsync(e => e.UserId == id);
            CatchNotFound(item);
            CatchTimestampMismatched(data.UpdatedAt, item.UpdatedAt);

            item.Fullname = data.Fullname;
            item.Male = data.Male;
            item.Birth = data.Birth;
            item.Phone = data.Phone;
            item.Address = data.Address;
            item.Image = UpdateImage(item.Image, data.Image, USER_MAX_WIDTH, USER_MAX_HEIGHT);
            item.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return item;
        }

        public Task<Manager> Update(string id, Manager data)
        {
            throw new NotImplementedException();
        }
    }
}