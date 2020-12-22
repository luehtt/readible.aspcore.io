using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Readible.Models;
using Readible.Services;
using Readible.Shared;
using static Readible.Shared.Const;
using static Readible.Shared.HttpStatus;
using static Readible.Shared.RouteConst;

namespace Readible.Controllers
{
    [Route(ACCOUNT)]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get([FromServices] UserService service,
            [FromServices] CustomerService customerService, 
            [FromServices] ManagerService managerService)
        {
            try
            {
                var user = await service.GetUserPrincipal(User);
                var userRole = await service.GetUserRole(user);

                switch (userRole.Name)
                {
                    case USER_ROLE_ADMIN:
                    case USER_ROLE_MANAGER:
                        user = await service.Get(user.Id);
                        var manager = await managerService.Get(user.Id);
                        return Ok(new { user, data = manager });
                    case USER_ROLE_CUSTOMER:
                        user = await service.Get(user.Id);
                        var customer = await customerService.Get(user.Id);
                        return Ok( new {user, data = customer} );
                    default:
                        return Unauthorized(FORBIDDEN);
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
        
        [HttpPut("customer")]
        [Authorize(Roles = USER_ROLE_CUSTOMER)]
        public async Task<IActionResult> PutCustomer([FromBody] Customer input,
            [FromServices] UserService service,
            [FromServices] CustomerService customerService)
        {
            try
            {
                var user = await service.GetUserPrincipal(User);
                input.UserId = user.Id;
                var item = await customerService.Update(user.Id, input);
                return Ok( item );  
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
        
        [HttpPut("manager")]
        [Authorize(Roles = USER_ROLE_ADMIN_MANAGER)]
        public async Task<IActionResult> PutManager([FromBody] Manager input,
            [FromServices] UserService service,
            [FromServices] ManagerService managerService)
        {
            try
            {
                var user = await service.GetUserPrincipal(User);
                input.UserId = user.Id;
                var item = await managerService.Update(user.Id, input);
                return Ok( item );  
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