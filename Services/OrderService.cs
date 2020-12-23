using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Readible.Models;
using Readible.Shared;
using static Readible.Shared.HttpStatus;
using static Readible.Shared.Const;
using Readible.Services.Hub;

namespace Readible.Services
{
    public class OrderService : DataContextService, IDataContextService<Order>
    {
        private readonly DataContext context;
        private readonly OrderHub hub;

        public OrderService(DataContext context, OrderHub hub)
        {
            this.context = context;
            this.hub = hub;
        }

        public async Task<Order> Delete(int id)
        {
            var item = await context.Orders.FindAsync(id);
            CatchNotFound(item);

            // ef core sucks for not having rows delete query effectively
            // unless someone want to write raw sql
            context.OrderDetails.RemoveRange(context.OrderDetails.Where(e => e.OrderId == item.Id));
            context.Orders.Remove(item);

            await context.SaveChangesAsync();
            hub.Delete(id);
            return item;
        }

        public Task<Order> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Count()
        {
            return await context.Orders.AsNoTracking().CountAsync();
        }

        public Task<Order> Get(string id)
        {
            throw new NotImplementedException();
        }

        private async Task<OrderStatus> GetOrderStatus(string statusName)
        {
            var item = await context.OrderStatuses.AsNoTracking().FirstOrDefaultAsync(x => x.Name == statusName);
            CatchNotFound(item);
            return item;
        }

        private async Task<Book> GetBook(string isbn)
        {
            var item = await context.Books.AsNoTracking().FirstOrDefaultAsync(x => x.Isbn == isbn);
            CatchNotFound(item);
            return item;
        }
        
        public async Task<Order> Store(Order data)
        {
            CatchCondition(data.Id != 0);
            var status = await GetOrderStatus(ORDER_STATUS_PENDING);

            foreach (var d in data.OrderDetails)
            {
                var book = await GetBook(d.BookIsbn);
                d.Price = Common.CalcDiscountPercent(book.Price, book.Discount);
            }

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var item = new Order
                    {
                        CustomerId = data.CustomerId,
                        Contact = data.Contact,
                        StatusId = status.Id,
                        TotalItem = data.OrderDetails.Sum(x => x.Amount),
                        TotalPrice = data.OrderDetails.Sum(x => x.Price * x.Amount),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Address = data.Address,
                        Phone = data.Phone,
                        Note = data.Note
                    };

                    await context.Orders.AddAsync(item);
                    await context.SaveChangesAsync();

                    foreach (var d in data.OrderDetails)
                    {
                        d.OrderId = item.Id;
                        await context.OrderDetails.AddAsync(d);
                    }

                    await context.SaveChangesAsync();
                    transaction.Commit();

                    item.Status = status;
                    hub.Store(item);
                    return item;
                }
                catch (Exception err)
                {
                    transaction.Rollback();
                    throw new HttpResponseException(SERVER_ERROR_CODE, err.Message);
                }
            }
        }

        public async Task<Order> Update(int id, Order data)
        {
            CatchCondition(id != data.Id);

            var item = await context.Orders.Include(e => e.Status).FirstOrDefaultAsync(e => e.Id == id);
            CatchNotFound(item);
            CatchTimestampMismatched(data.UpdatedAt, item.UpdatedAt);

            item.Contact = data.Contact;
            item.Address = data.Address;
            item.Phone = data.Phone;
            item.TotalItem = data.TotalItem;
            item.TotalPrice = data.TotalPrice;
            item.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            hub.Update(item);
            return item;
        }
        
        public async Task<Order> Update(int id, Order data, string statusName, int managerId)
        {
            CatchCondition(id != data.Id);

            var item = await context.Orders.Where(e => e.Id == id).Include(x => x.Status).FirstOrDefaultAsync();
            CatchNotFound(item);
            CatchTimestampMismatched(data.UpdatedAt, item.UpdatedAt);

            var status = await GetOrderStatus(statusName.ToUpper());
            var originStatus = item.Status.Name;
			var timestamp = DateTime.UtcNow;
            item.StatusId = status.Id;
            item.UpdatedAt = timestamp;

            switch (status.Name)
            {
                case "PENDING":
                    item.ConfirmerId = null;
                    item.CompleterId = null;
					item.ConfirmedAt = null;
					item.CompletedAt = null;
                    break;
                case "DELIVERING":
                    item.ConfirmerId = managerId;
					item.ConfirmedAt = timestamp;
                    item.CompleterId = null;
					item.CompletedAt = null;
                    break;
                case "SUCCESS":
                case "FAILED":
                    item.CompleterId = managerId;
					item.CompletedAt = timestamp;
                    break;
            }

            await context.SaveChangesAsync();
            item.Status = status;
            hub.Update(item);

            var res = await context.Orders.AsNoTracking().Where(x => x.Id == id)
                .Include(x => x.Status)
                .Include(x => x.ConfirmedManager)
                .Include(x => x.CompletedManager)
                .FirstOrDefaultAsync();
            return res;
        }

        public Task<Order> Update(string id, Order data)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Order>> Fetch()
        {
            return await context.Orders.AsNoTracking().Include(x => x.Status).Include(x => x.Customer).ToListAsync();
        }

        public async Task<Order> Get(int id)
        {
            var item = await context.Orders.AsNoTracking().Where(x => x.Id == id)
                .Include(x => x.Status)
                .Include(x => x.Customer)
                .Include(x => x.OrderDetails).ThenInclude(y => y.Book)
                .Include(x => x.ConfirmedManager)
                .Include(x => x.CompletedManager)
                .FirstOrDefaultAsync();
            CatchNotFound(item);
            return item;
        }

        public async Task<int> Count(string statusName)
        {
            var status = await GetOrderStatus(statusName.ToUpper());
            return await context.Orders.CountAsync(x => x.StatusId == status.Id);
        }

        public async Task<Order> Delete(int id, int customerId)
        {
            var item = await context.Orders.Where(x => x.CustomerId == customerId).Include(x => x.Status).FirstOrDefaultAsync(x => x.Id == id);
            var originStatus = item.Status.Name;
            CatchNotFound(item);

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.OrderDetails.RemoveRange(context.OrderDetails.Where(e => e.OrderId == item.Id));
                    context.Orders.Remove(item);
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    hub.Delete(id);
                    return item;
                }
                catch (Exception err)
                {
                    transaction.Rollback();
                    throw new HttpResponseException(SERVER_ERROR_CODE, err.Message);
                }
            } 
        }

        public async Task<IEnumerable<Order>> Fetch(int customerId)
        {
            return await context.Orders.AsNoTracking().OrderBy(x => x.Id).Include(x => x.Status).Include(x => x.Customer).ToListAsync();
        }

        public async Task<IEnumerable<Order>> Fetch(int customerId, string filter)
        {
            var status = GetOrderStatus(filter.ToUpper());
            return await context.Orders.AsNoTracking().Where(x => x.StatusId == status.Id).Where(x => x.CustomerId == customerId).OrderBy(x => x.Id).Include(x => x.Status).ToListAsync();
        }

        public async Task<IEnumerable<Order>> Fetch(string statusName)
        {
            if (statusName == "live") {
                var statusPending = await GetOrderStatus("PENDING");
                var statusDelivering = await GetOrderStatus("DELIVERING");
                return await context.Orders.AsNoTracking().Where(x => x.StatusId == statusPending.Id || x.StatusId == statusDelivering.Id).OrderBy(x => x.Id).Include(x => x.Status).ToListAsync();
            } else {
                var status = await GetOrderStatus(statusName.ToUpper());
                return await context.Orders.AsNoTracking().Where(x => x.StatusId == status.Id).OrderBy(x => x.Id).Include(x => x.Status).ToListAsync();
            }
        }

        public async Task<Order> Get(int id, int customerId)
        {
            var item = await context.Orders.AsNoTracking().Where(x => x.Id == id).Where(x => x.CustomerId == customerId)
                .Include(x => x.Status).Include(x => x.Customer).Include(x => x.OrderDetails).ThenInclude(y => y.Book).FirstOrDefaultAsync();
            CatchNotFound(item);
            return item;
        }

        public async Task<Order> GetLatestOrder()
        {
            return await context.Orders.AsNoTracking().OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync();
        }
        
        public async Task<Order> GetOldestOrder()
        {
            return await context.Orders.AsNoTracking().OrderBy(x => x.CreatedAt).FirstOrDefaultAsync();
        }

        private async Task<IEnumerable<OrderStatistic>> SummarizeByDate(DateTime fromDate, DateTime toDate)
        {
            return await context.Orders.AsNoTracking()
                        .Where(x => x.UpdatedAt > fromDate && x.UpdatedAt < toDate)
                        .GroupBy(x => x.UpdatedAt.Value.Date)
                        .OrderBy(x => x.Key)
                        .Select(x => new OrderStatistic(x.Key.ToShortDateString(), x.Count(), x.Sum(e => e.TotalItem), x.Sum(e => e.TotalPrice)))
                        .ToListAsync();
        }

        private async Task<IEnumerable<OrderStatistic>> SummarizeByMonth(DateTime fromDate, DateTime toDate)
        {
            return await context.Orders.AsNoTracking()
                        .Where(x => x.UpdatedAt > fromDate && x.UpdatedAt < toDate)
                        .GroupBy(x => x.UpdatedAt.Value.Year.ToString() + "-" + (x.UpdatedAt.Value.Month < 10 ? "0" + x.UpdatedAt.Value.Month.ToString() : x.UpdatedAt.Value.Month.ToString()))
                        .OrderBy(x => x.Key)
                        .Select(x => new OrderStatistic(x.Key.ToString(), x.Count(), x.Sum(e => e.TotalItem), x.Sum(e => e.TotalPrice)))
                        .ToListAsync();

        }

        private async Task<IEnumerable<OrderStatistic>> SummarizeByYear(DateTime fromDate, DateTime toDate)
        {
            return await context.Orders.AsNoTracking()
                         .Where(x => x.UpdatedAt > fromDate && x.UpdatedAt < toDate)
                         .GroupBy(x => x.UpdatedAt.Value.Year)
                         .OrderBy(x => x.Key)
                         .Select(x => new OrderStatistic(x.Key.ToString(), x.Count(), x.Sum(e => e.TotalItem), x.Sum(e => e.TotalPrice)))
                         .ToListAsync();
        }

        public async Task<IEnumerable<OrderStatistic>> SummarizeOrder(DateTime fromDate, DateTime toDate, string groupBy)
        {
            switch (groupBy)
            {
                case QUERY_DATE: return await SummarizeByDate(fromDate, toDate);
                case QUERY_MONTH: return await SummarizeByMonth(fromDate, toDate);
                case QUERY_YEAR: return await SummarizeByYear(fromDate, toDate);
                default: throw new HttpResponseException(BAD_REQUEST_CODE);
            }
        }
    }
}