using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Readible.Services;
using Readible.Shared;
using static Readible.Shared.Const;
using static Readible.Shared.RouteConst;

namespace Readible.Controllers
{
    [Route(CUSTOMER)]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = USER_ROLE_ADMIN_MANAGER)]
        public async Task<IActionResult> Get([FromServices] CustomerService service)
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
                return StatusCode(HttpStatus.SERVER_ERROR_CODE, err.Message);
            }
        }

        [HttpGet(ID)]
        [Authorize(Roles = USER_ROLE_ADMIN_MANAGER)]
        public async Task<IActionResult> Get([FromServices] CustomerService service,
            [FromServices] UserService userService, int id)
        {
            try
            {
                var customer = await service.Get(id);
                var user = await userService.Get(id);

                var res = new {customer, user};
                return Ok(res);
            }
            catch (HttpResponseException err)
            {
                return err.ToResponse();
            }
            catch (Exception err)
            {
                return StatusCode(HttpStatus.SERVER_ERROR_CODE, err.Message);
            }
        }

        [HttpPut(ID)]
        [Authorize(Roles = USER_ROLE_ADMIN)]
        public async Task<IActionResult> Put(int id, [FromServices] UserService userService)
        {
            try
            {
                var item = await userService.UpdateActive(id);
                return Ok(item);
            }
            catch (HttpResponseException err)
            {
                return err.ToResponse();
            }
            catch (Exception err)
            {
                return StatusCode(HttpStatus.SERVER_ERROR_CODE, err.Message);
            }
        }

        // this method called on user fetch his/her info for init order
        // the method return essential for filling order info
        [HttpPost]
        [Authorize(Roles = USER_ROLE_CUSTOMER)]
        public async Task<IActionResult> GetCustomer([FromServices] UserService userService)
        {
            try
            {
                var item = await userService.GetCustomer(User);
                return Ok(item);
            }
            catch (HttpResponseException err)
            {
                return err.ToResponse();
            }
            catch (Exception err)
            {
                return StatusCode(HttpStatus.SERVER_ERROR_CODE, err.Message);
            }
        }
    }
}