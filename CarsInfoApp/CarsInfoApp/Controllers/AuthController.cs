namespace CarsInfoApp.Controllers
{
    using CarsInfoApp.Models;
    using CarsInfoApp.Utils;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;

    public class AuthController : Controller
    {
        // Using dependency injection for IJwtUtils service (an object receives other objects that it depends on)
        // To inject JwtUtils service, we have to register it in Startup.cs as Scoped/Singleton/Transient

        private CarsDbContext _context = new CarsDbContext();
        private readonly IJwtUtils _jwtUtils;

        public AuthController(IJwtUtils jwtUtils)
        {
            this._jwtUtils = jwtUtils;
        }

        // This method returns the main view for the Register page (Auth/Register.cshtml)
        public IActionResult Register()
        {
            return View();
        }

        // This method is processing the request that is comming from the Register form (HttpPost is used while the method expects params)
        [HttpPost]
        public ActionResult Register(string username, string password)
        {
            if (username != null || password != null)
            {
                // Creates new user model from the request params if they are not null
                var user = new User()
                {
                    Id = Guid.NewGuid(),
                    Username = username,
                    Password = password
                };

                // Adding the user model to the database using the DbContext
                this._context.Users.Add(user);
                this._context.SaveChanges();

                // On successfull registration the client is redirected to the Login view
                return RedirectToAction("Login", "Auth");
            }

            // If registration is not successfull, the client is redirected to the same page
            return RedirectToAction("Register", "Auth");
        }


        // This method returns the main view for the Register page (Auth/Login.cshtml)
        public IActionResult Login()
        {
            return View();
        }


        // This method is processing the request that is comming from the Login form
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username != null || password != null)
            {
                // Check if there is a registered user with the provided credentials from the Login form and if there is such user assign it to new variable
                var user = this._context.Users
                    .FirstOrDefault(x => x.Username == username && x.Password == password);

                // Check if the user is not null and if so, generate a jwt token and append it in the cookie
                if (user != null)
                {
                    if (user.IsAdmin)
                    {
                        HttpContext.Response.Cookies.Append("admin", "power_user");
                    }
                    var token = this._jwtUtils.GenerateJwtToken(user);
                    HttpContext.Response.Cookies.Append("token", token);

                    return RedirectToAction("Index", "Home");
                }
            }

            return BadRequest("Invalid user.");
        }


        /// <summary>
        /// This method is used when the client click log out button and it redirects him to the login view
        /// The Layout.cshtml contains the js logic which is responsible for the cookies to be deleted after log out.
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            return RedirectToAction("Login", "Auth");
        }
    }
}
