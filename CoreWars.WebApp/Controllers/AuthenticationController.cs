using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using CoreWars.Common;
using CoreWars.Data;
using CoreWars.Data.Entities;
using CoreWars.WebApp.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CoreWars.WebApp.Controllers
{
    [ApiController, Route("[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IDataContext _dataContext;

        public AuthenticationController(IConfiguration config, IDataContext dataContext)
        {
            _config = config;
            _dataContext = dataContext;
        }

        [AllowAnonymous]    
        [HttpPost]    
        public IActionResult Login([FromBody]Login login)    
        {    
            var user = _dataContext.Users.SingleOrDefault(x => x.EmailAddress == login.Username);
            if (user == null) return Unauthorized();
            var tokenString = GenerateWebToken(user);    
            return Ok(new { token = tokenString });
        }    
        
        private string GenerateWebToken(IUser userInfo)    
        {    
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));    
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);    
            
            var claims = new[] {    
                new Claim(JwtRegisteredClaimNames.Email, userInfo.EmailAddress)
                , new Claim("user-id", userInfo.Id.ToString())
            };    
            
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"]
                , _config["Jwt:Issuer"]
                , claims
                , expires: DateTime.Now.AddMinutes(120)
                , signingCredentials: credentials);    
    
            return new JwtSecurityTokenHandler().WriteToken(token);    
        }    
    
     
    }
}