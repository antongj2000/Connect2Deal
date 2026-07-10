using Connect2Deal.Models;
using Connect2Deal.Services;
using Connect2Deal.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Connect2Deal.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserService _userService;

        public AccountController(UserService userService)
        {
            _userService = userService;
        }


        #region Registration of a new user

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(Registration model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await _userService.UsernameTaken(model.Username))
            {
                ModelState.AddModelError("Username", "This username is already taken");
            }

            if (await _userService.EmailTaken(model.Email))
            {
                ModelState.AddModelError("Email", "An account with this email already exists");
            }

            //we will check this again now, did this validation pass
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            await _userService.RegisterUser(model.FirstName, model.LastName, model.Username, model.Email, model.Password);

            return RedirectToAction("Login", "Account");

        }


        #endregion


        #region Login user

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);     
            }

            var user = await _userService.LoginCheck(model.Username, model.Password);

            if (user==null)
            {
                ModelState.AddModelError("Username", "This username doesn't exist or password is wrong");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.Name, user.username),
            new Claim("CoockieUserId", user.id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

            
            await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);


            return RedirectToAction("Privacy", "Home");
        }


        #endregion




    }
}