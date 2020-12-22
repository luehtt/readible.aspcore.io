using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Readible.Models;
using Readible.Shared;
using static Readible.Shared.HttpStatus;

namespace Readible.Services
{
    public class UserService : DataContextService, IDataContextService<User>
    {
        private readonly DataContext context;

        public UserService(DataContext context)
        {
            this.context = context;
        }

        public Task<User> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<User> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> Fetch()
        {
            return await context.Users.AsNoTracking().ToListAsync();
        }

        public async Task<int> Count()
        {
            return await context.Users.AsNoTracking().CountAsync();
        }

        private User SelectUser(User user)
        {
            return new User
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                Active = user.Active,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<User> Get(int id)
        {
            var item = await context.Users.AsNoTracking().Where(e => e.Id == id).Select(x => SelectUser(x)).FirstOrDefaultAsync();
            CatchNotFound(item);
            return item;
        }

        public async Task<User> Get(string id)
        {
            var item = await context.Users.AsNoTracking().Where(e => e.Email == id).Select(x => SelectUser(x)).FirstOrDefaultAsync();
            CatchNotFound(item);
            return item;
        }
        
        public async Task<UserRole> GetUserRole(int id)
        {
            var item = await context.UserRoles.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            CatchNotFound(item);
            return item;
        }
        
        public async Task<UserRole> GetUserRole(User user)
        {
            var item = await context.UserRoles.AsNoTracking().FirstOrDefaultAsync(e => e.Id == user.UserRoleId);
            CatchNotFound(item);
            return item;
        }

        public async Task<User> Store(User data)
        {
            var timestamp = DateTime.UtcNow;
            data.CreatedAt = timestamp;
            data.UpdatedAt = timestamp;
            data.Active = true;

            await context.Users.AddAsync(data);
            await context.SaveChangesAsync();
            return data;
        }

        public async Task<Customer> Store(User data, Customer customer)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var timestamp = DateTime.UtcNow;
                    data.CreatedAt = timestamp;
                    data.UpdatedAt = timestamp;
                    data.Active = true;
                    await context.Users.AddAsync(data);
                    await context.SaveChangesAsync();

                    customer.UserId = data.Id;
                    customer.CreatedAt = timestamp;
                    customer.UpdatedAt = timestamp;
                    await context.Customers.AddAsync(customer);
                    await context.SaveChangesAsync();

                    transaction.Commit();
                    return customer;
                }
                catch (Exception err)
                {
                    transaction.Rollback();
                    throw new HttpResponseException(SERVER_ERROR_CODE, err.Message);
                }
            }
        }

        public async Task<Manager> Store(User data, Manager manager)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var timestamp = DateTime.UtcNow;
                    data.CreatedAt = timestamp;
                    data.UpdatedAt = timestamp;
                    data.Active = true;
                    await context.Users.AddAsync(data);
                    await context.SaveChangesAsync();

                    manager.UserId = data.Id;
                    manager.CreatedAt = timestamp;
                    manager.UpdatedAt = timestamp;
                    await context.Managers.AddAsync(manager);
                    await context.SaveChangesAsync();

                    transaction.Commit();
                    return manager;
                }
                catch (Exception err)
                {
                    transaction.Rollback();
                    throw new HttpResponseException(SERVER_ERROR_CODE, err.Message);
                }
            }
        }

        public Task<User> Update(int id, User data)
        {
            throw new NotImplementedException();
        }

        public async Task<User> UpdateActive(int id)
        {
            var item = await context.Users.FirstOrDefaultAsync(e => e.Id == id);
            CatchNotFound(item);

            item.Active = !item.Active;
            item.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            var res = await context.Users.Select(x => new User { Id = x.Id, Active = x.Active, CreatedAt = x.CreatedAt, UpdatedAt = x.UpdatedAt }).FirstOrDefaultAsync(e => e.Id == id);
            return res;
        }

        public async Task<User> Update(int id, string password)
        {
            var item = await context.Users.FirstOrDefaultAsync(e => e.Id == id);
            item.Password = password;
            item.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return item;
        }

        public Task<User> Update(string id, User data)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserPrincipal(ClaimsPrincipal loginPrincipal)
        {
            var email = loginPrincipal.FindFirst(ClaimTypes.Email).Value;
            var user = await context.Users.Where(x => x.Email == email).Include(x => x.UserRole).FirstOrDefaultAsync();
            CatchNotFound(user, FORBIDDEN_CODE);

            return user;
        }

        public async Task<Customer> GetCustomer(ClaimsPrincipal loginPrincipal)
        {
            var email = loginPrincipal.FindFirst(ClaimTypes.Email).Value;
            var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
            CatchNotFound(user, FORBIDDEN_CODE);

            var customer = await context.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == user.Id);
            CatchNotFound(customer, FORBIDDEN_CODE);
            return customer;
        }

        public async Task<Manager> GetManager(ClaimsPrincipal loginPrincipal)
        {
            var email = loginPrincipal.FindFirst(ClaimTypes.Email).Value;
            var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
            CatchNotFound(user, FORBIDDEN_CODE);

            var manager = await context.Managers.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == user.Id);
            CatchNotFound(manager, FORBIDDEN_CODE);
            return manager;
        }
    }
}