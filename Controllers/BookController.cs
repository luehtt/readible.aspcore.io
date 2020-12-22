using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Readible.Models;
using Readible.Services;
using Readible.Shared;
using static Readible.Shared.Const;
using static Readible.Shared.RouteConst;
using static Readible.Shared.HttpStatus;

namespace Readible.Controllers
{
    [Route(BOOK)]
    [Authorize(Roles = USER_ROLE_ADMIN_MANAGER)]
    [ApiController]
    public class BookController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] BookService service)
        {
            try
            {
                var list = await service.FetchNoImage();
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
        public async Task<IActionResult> Get([FromServices] BookService service, string id)
        {
            try
            {
                var item = await service.Get(id);
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
        public async Task<IActionResult> Put([FromServices] BookService service, string id, [FromBody] Book input)
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

        [HttpPost]
        public async Task<IActionResult> Post([FromServices] BookService service, [FromBody] Book input)
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

        [HttpDelete(ID)]
        public async Task<IActionResult> Delete([FromServices] BookService service, string id)
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