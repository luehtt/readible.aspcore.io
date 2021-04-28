using Readible.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using static Readible.Shared.Const;

namespace Readible.Hub
{
    public class CustomerOrderHub : Microsoft.AspNetCore.SignalR.Hub, IDataContextUserHub<Order>
    {
        protected IHubContext<CustomerOrderHub> context;

        public CustomerOrderHub(IHubContext<CustomerOrderHub> context)
        {
            this.context = context;
        }

        [Authorize(Roles = USER_ROLE_CUSTOMER)]
        public Task Delete(string connectId, int id)
        {
            return context.Clients.User(connectId).SendAsync("CustomerOrderDelete", id);
        }

        [Authorize(Roles = USER_ROLE_CUSTOMER)]
        public Task Store(string connectId, Order data)
        {
            return context.Clients.User(connectId).SendAsync("CustomerOrderStore", data);
        }

        [Authorize(Roles = USER_ROLE_CUSTOMER)]
        public Task Update(string connectId, Order data)
        {
            return context.Clients.User(connectId).SendAsync("CustomerOrderUpdate", data);
        }
    }
}
