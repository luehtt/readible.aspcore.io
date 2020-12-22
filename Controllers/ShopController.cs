using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Readible.Models;
using Readible.Services;
using Readible.Shared;
using static Readible.Shared.HttpStatus;
using static Readible.Shared.Const;
using static Readible.Shared.RouteConst;

namespace Readible.Controllers
{
    [Route(SHOP)]
    [ApiController]
    public class ShopController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] BookService service,
            [FromQuery(Name = QUERY_PAGE)] string inputPage,
            [FromQuery(Name = QUERY_PAGE_SIZE)] string inputPageSize,
            [FromQuery(Name = QUERY_CATEGORY)] string category,
            [FromQuery(Name = QUERY_SEARCH)] string search
        )
        {
            var page = inputPage == null ? 1 : Common.IntParse(inputPage, 1);
            var pageSize = inputPageSize == null ? DEFAULT_SHOP_SIZE : Common.IntParse(inputPageSize, DEFAULT_SHOP_SIZE);

            try
            {
                // tried to use group them into one task and failed
                // since object BookPagination need real value
                IEnumerable<Book> list;
                int total;

                if (category != null && search == null)
                {
                    list = await service.FetchShop(page, pageSize, category);
                    total = await service.CountFilter(category);
                }
                else if (category == null && search != null)
                {
                    list = await service.FetchShop(search, page, pageSize);
                    total = await service.CountSearch(search);
                }
                else if (category != null)
                {
                    list = await service.FetchShop(search, page, pageSize, category);
                    total = await service.CountFetch(search, category);
                }
                else
                {
                    list = await service.FetchShop(page, pageSize);
                    total = await service.CountFetch();
                }

                return Ok(new BookPagination(total, pageSize, page, list));
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

        [HttpGet(ID)]
        public async Task<IActionResult> Get([FromServices] BookService service, string id)
        {
            try
            {
                var item = await service.Get(id, true);
                return Ok(item);
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
        
        [HttpGet("similar/{id}")]
        public async Task<IActionResult> GetSimilar([FromServices] BookService service, string id)
        {
            try
            {
                var item = await service.GetSimilar(id);
                return Ok(item);
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