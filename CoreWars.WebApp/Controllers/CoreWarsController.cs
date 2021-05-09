using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace CoreWars.WebApp.Controllers
{
    public abstract class CoreWarsController : Controller
    {
        protected Guid UserId
        {
            get
            {
                var userIdClaim = HttpContext.User.Claims
                    .Single(claim => claim.Type == "user-id");
                
                return Guid.Parse(userIdClaim.Value);
            }
        }

    }
}