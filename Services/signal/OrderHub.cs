using Readible.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Readible.services.hub;

namespace Readible.Services.Hub
{
    public class OrderHub : Microsoft.AspNetCore.SignalR.Hub, IHubContextService<Order>
    {
        protected IHubContext<OrderHub> context;

        public OrderHub(IHubContext<OrderHub> context)
        {
            this.context = context;
        }

        public Task Delete(int id)
        {
            return context.Clients.All.SendAsync("OrderDelete", id);
        }

        public Task Store(Order data)
        {
            return context.Clients.All.SendAsync("OrderStore", data);
        }

        public Task Update(Order data)
        {
            return context.Clients.All.SendAsync("OrderUpdate", data);
        }
    }
}
