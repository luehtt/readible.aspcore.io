using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Readible.Models;
using Readible.Services;
using Readible.Shared;
using static Readible.Shared.HttpStatus;
using static Readible.Shared.Const;
using static Readible.Shared.RouteConst;

namespace Readible.Controllers
{
    [Route(BOOK_CATEGORY)]
    [ApiController]
    public class BookCategoryController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] BookCategoryService service)
        {
            try
            {
                var list = await service.Fetch();
                return Ok(list);
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
        public async Task<IActionResult> Get([FromServices] BookCategoryService service, int id)
        {
            try
            {
                var item = await service.GetDetail(id);
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

        [HttpPost]
        [Authorize(Roles = USER_ROLE_ADMIN_MANAGER)]
        public async Task<IActionResult> Post([FromServices] BookCategoryService service, [FromBody] BookCategory input)
        {
            try
            {
                var item = await service.Store(input);
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
        
        [HttpPut(ID)]
        [Authorize(Roles = USER_ROLE_ADMIN_MANAGER)]
        public async Task<IActionResult> Put(int id, [FromServices] BookCategoryService service, [FromBody] BookCategory input)
        {
            try
            {
                var item = await service.Update(id, input);
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

        [HttpDelete(ID)]
        [Authorize(Roles = USER_ROLE_ADMIN_MANAGER)]
        public async Task<IActionResult> Delete([FromServices] BookCategoryService service, int id)
        {
            try
            {
                var item = await service.Delete(id);
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