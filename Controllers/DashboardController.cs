using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Readible.Services;
using Readible.Shared;
using static Readible.Shared.Const;
using static Readible.Shared.RouteConst;
using static Readible.Shared.HttpStatus;
using System;

namespace Readible.Controllers
{
    [Route(DASHBOARD)]
    [Authorize(Roles = USER_ROLE_ADMIN_MANAGER)]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        [HttpGet("topten")]
        public async Task<IActionResult> GetTop([FromServices] BookService bookService,
            [FromServices] CustomerService customerService)
        {
            try
            {
                var topSold = bookService.SummarizeSold(DEFAULT_TOP_SIZE);
                var topRating = bookService.SummarizeRating(DEFAULT_TOP_SIZE);
                var topPurchased = customerService.SummarizePurchased(DEFAULT_TOP_SIZE);
                var topPaid = customerService.SummarizePaid(DEFAULT_TOP_SIZE);
                await Task.WhenAll(topSold, topPaid, topPurchased, topRating);

                var data = new { sold = await topSold, rating = await topRating, purchased = await topPurchased, paid = await topPaid };
                return Ok(data);
            }
            catch (HttpResponseException err)
            {
                return err.ToResponse();
            }
            catch (Exception err)
            {
                return StatusCode(SERVER_ERROR_CODE, err.Message);
            }

        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetCount([FromServices] BookService bookService,
            [FromServices] CustomerService customerService,
            [FromServices] OrderService orderService,
            [FromServices] BookCommentService commentService)
        {
            try
            {
                var countBook = bookService.Count();
                var countCustomer = customerService.Count();
                var countComment = commentService.Count();
                var countOrder = orderService.Count();
                await Task.WhenAll(countBook, countComment, countCustomer, countOrder);

                var data = new { book = await countBook, customer = await countCustomer, comment = await countComment, order = await countOrder };
                return Ok(data);
            }
            catch (HttpResponseException err)
            {
                return err.ToResponse();
            }
            catch (Exception err)
            {
                return StatusCode(SERVER_ERROR_CODE, err.Message);
            }
        }
    }
}