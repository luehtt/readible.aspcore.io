using Readible.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;


namespace Readible.Hub
{
    public class BookCommentHub : Microsoft.AspNetCore.SignalR.Hub, IDataContextHub<BookComment>
    {
        protected IHubContext<BookCommentHub> context;

        public BookCommentHub(IHubContext<BookCommentHub> context)
        {
            this.context = context;
        }

        public Task Delete(int id)
        {
            return context.Clients.All.SendAsync("BookCommentDelete", id);
        }

        public Task Store(BookComment data)
        {
            return context.Clients.All.SendAsync("BookCommentStore", data);
        }

        public Task Update(BookComment data)
        {
            return context.Clients.All.SendAsync("BookCommentUpdate", data);
        }
    }
}
