using System;
using System.Collections.Generic;
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
    [Route(ORDER)]
    [ApiController]
    public class OrderController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get([FromServices] OrderService service,
            [FromServices] UserService userService,
            [FromQuery(Name = QUERY_STATUS)] string status)
        {
            try
            {
                var user = await userService.GetUserPrincipal(User);
                IEnumerable<Order> list;

                switch (user.UserRole.Name)
                {
                    case USER_ROLE_ADMIN:
                    case USER_ROLE_MANAGER:
                        list = status == null ? await service.Fetch() : await service.Fetch(status);
                        return Ok(list);
                    case USER_ROLE_CUSTOMER:
                        list = status == null ? await service.Fetch(user.Id) : await service.Fetch(user.Id, status);
                        return Ok(list);
                    default:
                        return StatusCode(FORBIDDEN_CODE, FORBIDDEN);
                }
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
        [Authorize]
        public async Task<IActionResult> Get([FromServices] OrderService service,
            [FromServices] UserService userService, int id)
        {
            try
            {
                var user = await userService.GetUserPrincipal(User);
                Order item;

                switch (user.UserRole.Name)
                {
                    case USER_ROLE_ADMIN:
                    case USER_ROLE_MANAGER:
                        item = await service.Get(id);
                        return Ok(item);
                    case USER_ROLE_CUSTOMER:
                        item = await service.Get(id, user.Id);
                        return Ok(item);
                    default:
                        return StatusCode(FORBIDDEN_CODE, FORBIDDEN);
                }
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
        [Authorize(Roles = USER_ROLE_CUSTOMER)]
        public async Task<IActionResult> Post(
            [FromServices] OrderService service,
            [FromServices] UserService userService,
            [FromBody] Order input)
        {
            try
            {
                var user = await userService.GetUserPrincipal(User);
                input.CustomerId = user.Id;
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
        public async Task<IActionResult> Put([FromServices] OrderService service, int id,
            [FromBody] Order input,
            [FromServices] UserService userService,
            [FromQuery(Name = QUERY_STATUS)] string status)
        {
            try
            {
                var user = userService.GetUserPrincipal(User);
                var item = status == null ? await service.Update(id, input) : await service.Update(id, input, status, user.Id);
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
        [Authorize]
        public async Task<IActionResult> Delete([FromServices] OrderService service,
            [FromServices] UserService userService, int id)
        {
            try
            {
                var user = await userService.GetUserPrincipal(User);
                Order item;

                switch (user.UserRole.Name)
                {
                    case USER_ROLE_ADMIN:
                    case USER_ROLE_MANAGER:
                        item = await service.Delete(id);
                        return Ok(item);
                    case USER_ROLE_CUSTOMER:
                        item = await service.Delete(id, user.Id);
                        return Ok(item);
                    default:
                        return StatusCode(FORBIDDEN_CODE, FORBIDDEN);
                }
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