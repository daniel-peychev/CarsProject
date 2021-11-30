namespace CarsInfoApp.Controllers
{
    using CarsInfoApp.Models;
    using CarsInfoApp.Utils;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;

    public class BaseController : Controller
    {
        private readonly JwtUtils _jwtUtils = new JwtUtils();
        internal CarsDbContext _context = new CarsDbContext();

        internal Auth IsAuthorized()
        {
            var jwt = Request.Cookies["token"];

            var userId = this._jwtUtils.ValidateJwtToken(jwt);
            var user = this._context.Users.FirstOrDefault(x => x.Id == userId);

            if (user.IsAdmin)
            {
                return Auth.Authorized;
            }

            return Auth.Unauthorized;
        }
    }
}
