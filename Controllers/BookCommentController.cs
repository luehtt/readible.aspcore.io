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
    [Route(BOOK_COMMENT)]
    [ApiController]
    public class BookCommentController : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = USER_ROLE_ADMIN_MANAGER)]
        public async Task<IActionResult> Fetch([FromServices] BookCommentService service)
        {
            try
            {
                var item = await service.Fetch();
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
        [Authorize(Roles = USER_ROLE_CUSTOMER)]
        public async Task<IActionResult> Post(BookComment input,
			[FromServices] BookCommentService service,
            [FromServices] UserService userService)
        {
            try
            {
                var customer = await userService.GetCustomer(User);
                input.CustomerId = customer.UserId;
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
        [Authorize(Roles = USER_ROLE_CUSTOMER)]
        public async Task<IActionResult> Put(BookComment input, int id,
			[FromServices] BookCommentService service)
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
        [Authorize]
        public async Task<IActionResult> Delete(int id,
            [FromServices] BookCommentService service,
            [FromServices] UserService userService)
        {
            try
            {
                var user = await userService.GetUserPrincipal(User);
                var userRole = await userService.GetUserRole(user);

                switch (userRole.Name)
                {
                    case USER_ROLE_ADMIN: case USER_ROLE_MANAGER:
                        return Ok(await service.Delete(id));
                    case USER_ROLE_CUSTOMER:
                        return Ok(await service.Delete(id, user.Id));
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
    }
}