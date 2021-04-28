using Readible.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using static Readible.Shared.Const;
using Microsoft.EntityFrameworkCore;

namespace Readible.Hub
{
    public class OrderHub : Microsoft.AspNetCore.SignalR.Hub, IDataContextHub<Order>
    {
        private IHubContext<OrderHub> context;
        private DataContext dataContext;

        public OrderHub(IHubContext<OrderHub> context, DataContext dataContext)
        {
            this.context = context;
            this.dataContext = dataContext;
        }

        private async Task<List<string>> GetListManagerUpConnectId()
        {
            var users = await dataContext.Users.Where(x => x.Active == true && (x.UserRole.Name == USER_ROLE_MANAGER || x.UserRole.Name == USER_ROLE_ADMIN)).ToListAsync();
            var res = new List<string>();
            foreach (var user in users)
            {
                res.Add(user.ConnectId);
            }
            return res;
        }

        [Authorize(Roles = USER_ROLE_ADMIN_MANAGER)]
        public async Task Delete(int id)
        {
            var connectId = await GetListManagerUpConnectId();
            _ = context.Clients.Users(connectId).SendAsync("OrderDelete", id);
        }

        [Authorize(Roles = USER_ROLE_ADMIN_MANAGER)]
        public async Task Store(Order data)
        {
            var connectId = await GetListManagerUpConnectId();
            _ = context.Clients.Users(connectId).SendAsync("OrderStore", data);
        }

        [Authorize(Roles = USER_ROLE_ADMIN_MANAGER)]
        public async Task Update(Order data)
        {
            var connectId = await GetListManagerUpConnectId();
            _ = context.Clients.Users(connectId).SendAsync("OrderUpdate", data);
        }
    }
}
