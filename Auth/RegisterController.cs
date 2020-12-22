using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Readible.Models;
using Readible.Services;
using Readible.Shared;

namespace Readible.Auth
{
    [Route("api")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly DataContext context;

        public RegisterController(DataContext context)
        {
            this.context = context;
        }

        private async Task<RegisterModel> ValidateModel(RegisterModel model)
        {
            var email = context.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
            var username = context.Users.FirstOrDefaultAsync(x => x.Username == model.Username);

            await Task.WhenAll(email, username);
            model.EmailConflict = await email != null;
            model.UsernameConflict = await username != null;
            return model;
        }

        private async Task<UserRole> GetUserRole(string name)
        {
            var item = await context.UserRoles.FirstOrDefaultAsync(x => x.Name == name);
            if (item == null) throw new HttpResponseException(HttpStatus.SERVER_ERROR_CODE);
            return item;
        }

        private User InitUser(RegisterModel model, int userRoleId)
        {
            var passwordHasher = new BCryptPasswordHasher<User>();
            return new User
            {
                Username = model.Username,
                Email = model.Email,
                UserRoleId = userRoleId,
                Password = passwordHasher.HashPassword(null, model.Password)
            };
        }

        private Customer InitCustomer(RegisterModel model)
        {
            return new Customer
            {
                Fullname = model.Fullname,
                Birth = model.Birth,
                Male = model.Male,
                Address = model.Address,
                Phone = model.Phone
            };
        }

        private Manager InitManager(RegisterModel model)
        {
            return new Manager
            {
                Fullname = model.Fullname,
                Birth = model.Birth,
                Male = model.Male,
                Address = model.Address,
                Phone = model.Phone
            };
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateCustomer([FromBody] RegisterModel register, [FromServices] UserService userService)
        {
            try
            {
                var model = await ValidateModel(register);
                if (model.UsernameConflict || model.EmailConflict) return Ok(model);

                var userRole = await GetUserRole("CUSTOMER");
                var user = InitUser(model, userRole.Id);
                var customer = InitCustomer(model);

                var res = await userService.Store(user, customer);
                if (res == null) return BadRequest(HttpStatus.SERVER_ERROR);
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

        [HttpPost]
        [Route("register-manager")]
        [Authorize(Roles = Const.USER_ROLE_ADMIN)]
        public async Task<IActionResult> CreateManager([FromBody] RegisterModel register, [FromServices] UserService userService)
        {
            try
            {
                var model = await ValidateModel(register);
                if (model.UsernameConflict || model.EmailConflict) return Ok(model);

                var userRole = await GetUserRole("MANAGER");
                var user = InitUser(model, userRole.Id);
                var manager = InitManager(model);

                var res = await userService.Store(user, manager);
                if (res == null) return BadRequest(HttpStatus.SERVER_ERROR);
                
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

        [HttpPost]
        [Route("update-password")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] PasswordModel model, [FromServices] UserService userService)
        {
            var user = await userService.GetUserPrincipal(User);
            var passwordHasher = new BCryptPasswordHasher<User>();
            var compare = passwordHasher.VerifyHashedPassword(null, model.CurrentPassword, user.Password);

            if (compare != PasswordVerificationResult.Success) return Unauthorized();
            
            var hashedPassword = passwordHasher.HashPassword(null, model.UpdatePassword);
            user = await userService.Update(user.Id, hashedPassword);
            if (user != null) return Ok(model);
            return BadRequest(HttpStatus.SERVER_ERROR);

        }

    }
}