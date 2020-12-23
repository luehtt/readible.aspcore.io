using Readible.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Readible.services.hub;

namespace Readible.Services.Hub
{
    public class BookCommentHub : Microsoft.AspNetCore.SignalR.Hub, IHubContextService<BookComment>
    {
        protected IHubContext<BookCommentHub> context;

        public BookCommentHub(IHubContext<BookCommentHub> context)
        {
            this.context = context;
        }
        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task Store(BookComment data)
        {
            throw new NotImplementedException();
        }

        public Task Update(BookComment data)
        {
            throw new NotImplementedException();
        }
    }
}
