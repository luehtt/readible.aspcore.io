using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Readible.Models;

namespace Readible.Auth
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration config;
        private readonly DataContext context;
        private BCryptPasswordHasher<User> passwordHasher;

        public LoginController(DataContext context, IConfiguration config)
        {
            this.config = config;
            this.context = context;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Post([FromBody] LoginModel login)
        {
            var user = Authenticate(login);
            if (user == null) return Unauthorized();

            var tokenString = Generate(user);
            return Ok(new {token = tokenString});
        }

        private UserToken Authenticate(LoginModel login)
        {
            // get account from database to check if email exists
            var user = context.Users.FirstOrDefault(x => x.Email == login.Email);
            if (user == null || user.Active != true) return null;

            // verify password
            passwordHasher = new BCryptPasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, login.Password, user.Password);
            if (result == PasswordVerificationResult.Failed) return null;

            // get user role from database
            var userRole = context.UserRoles.Find(user.UserRoleId);
            var userToken = new UserToken
            {
				UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                ConnectId = user.ConnectId,
                UserRole = userRole.Name
            };

            return userToken;
        }

        private string Generate(UserToken user)
        {
            var expires = DateTime.UtcNow.AddMinutes(int.Parse(config["Jwt:ExpiredAfter"]));

            var claims = new[]
            {
                new Claim(ClaimTypes.Role, user.UserRole),
                new Claim(ClaimTypes.Expiration, expires.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.NameIdentifier, user.ConnectId),
				new Claim(JwtRegisteredClaimNames.NameId, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(config["Jwt:Issuer"], config["Jwt:Audience"], claims,
                expires: expires, signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}