using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Readible.Services;
using Readible.Shared;
using static Readible.Shared.HttpStatus;
using static Readible.Shared.Const;
using static Readible.Shared.RouteConst;

namespace Readible.Controllers
{
    [Route(MANAGER)]
    [Authorize(Roles = USER_ROLE_ADMIN)]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] ManagerService service)
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
        public async Task<IActionResult> Get([FromServices] ManagerService service,
            [FromServices] UserService userService, int id)
        {
            try
            {
                var manager = await service.GetDetail(id);
                var user = await userService.GetDetail(id);

                var json = new {manager, user};
                return Ok(json);
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
                return StatusCode(SERVER_ERROR_CODE, err.Message);
            }
        }
    }
}