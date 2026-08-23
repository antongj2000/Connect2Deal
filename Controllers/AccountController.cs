using Connect2Deal.Models;
using Connect2Deal.Services;
using Connect2Deal.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Connect2Deal.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserService _userService;
        private readonly IWebHostEnvironment _environment;

        public AccountController(UserService userService, IWebHostEnvironment environment)
        {
            _userService = userService;
            _environment = environment;
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
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }


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

            if (user == null)
            {
                ModelState.AddModelError("Username", "This username doesn't exist or password is wrong");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (user.IsBlocked)
            {
                ModelState.AddModelError("Username", "This account has been blocked.");
                return View(model);
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim("CoockieUserId", user.Id.ToString()),
        new Claim("ProfileImage", user.ProfileImage ?? "")
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }

        #endregion

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


        #region Settings

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var user = await _userService.getUserById(userId);

            if (user == null)
            {
                return NotFound();
            }

            var model = new UpdateProfileDetails
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Description = user.Description
            };

            ViewData["PreferredLanguage"] = user.PreferredLanguage;
            return View(model);
        }



        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateLanguage(string preferredLanguage)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            bool ok = await _userService.UpdateLanguage(userId, preferredLanguage);

            if (ok)
            {
                TempData["SettingsSuccess"] = "Language updated.";
            }
            else
            {
                TempData["SettingsError"] = "Could not update language.";
            }

            return RedirectToAction("Settings");
        }



        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfileImage(IFormFile image)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var newPath = await _userService.SaveProfilePicture(userId, image, _environment.WebRootPath);

            if (newPath == null)
            {
                TempData["SettingsError"] = "Upload failed. Use a JPG, PNG or WEBP under 5 MB.";
                return RedirectToAction("Settings");
            }

            var claims = new List<Claim>
             {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
             new Claim(ClaimTypes.Name, User.Identity!.Name ?? ""),
        new Claim(ClaimTypes.Role, User.FindFirstValue(ClaimTypes.Role) ?? "User"),
        new Claim("CoockieUserId", userId.ToString()),
                 new Claim("ProfileImage", newPath)
              };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            TempData["SettingsSuccess"] = "Profile picture updated.";
            return RedirectToAction("Settings");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDetails model)
        {
            if (!ModelState.IsValid)
            {
                TempData["SettingsError"] = "Please check the profile fields and try again.";
                return RedirectToAction("Settings");
            }

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var success = await _userService.UpdateProfile(
                userId,
                model.FirstName,
                model.LastName,
                model.PhoneNumber,
                model.Description
            );

            if (!success)
            {
                TempData["SettingsError"] = "Profile update failed.";
                return RedirectToAction("Settings");
            }

            TempData["SettingsSuccess"] = "Profile updated.";
            return RedirectToAction("Settings");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePassword model)
        {
            if (!ModelState.IsValid)
            {
                TempData["SettingsError"] = "Check the password fields — the new passwords must match and be at least 6 characters.";
                return RedirectToAction("Settings");
            }

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var success = await _userService.ChangePassword(userId, model.CurrentPassword, model.NewPassword);

            if (!success)
            {
                TempData["SettingsError"] = "Current password is incorrect.";
                return RedirectToAction("Settings");
            }

            TempData["SettingsSuccess"] = "Password changed.";
            return RedirectToAction("Settings");
        }

        #endregion


    }
}