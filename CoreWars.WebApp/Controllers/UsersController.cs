using CoreWars.Data;
using CoreWars.Data.Entities;
using CoreWars.WebApp.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreWars.WebApp.Controllers
{
    [ApiController, Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly IDataContext _dataContext;

        public UsersController(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }
        
        [AllowAnonymous]    
        [HttpPost]    
        public IActionResult Create([FromBody]Login login)
        {
            var user = new User()
            {
                EmailAddress = login.Username,
                Password = login.Password
            };

            _dataContext.Users.Add(user);
            _dataContext.Commit();
            
            return Ok();
        } 
    }
}