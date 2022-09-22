using Microsoft.AspNet.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TaruChat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using TaruChat.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using TaruChat.Models.ChatViewModels;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;

namespace TaruChat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ChatContext _context;
        private readonly ILogger<HomeController> _logger;

        private IHostingEnvironment hostingEnv;

        PasswordHasher ph = new PasswordHasher();

        public HomeController(ILogger<HomeController> logger, ChatContext context, IHostingEnvironment env)
        {
            _logger = logger;
            _context = context;
            this.hostingEnv = env;

        }

        public IActionResult Index()
        {
            if (User.IsInRole("Admin")){
                return RedirectToAction("Index", "Classes");
            }
            else
            {
                return RedirectToAction("Index", "Taruchat");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secured()
        {
            var user = _context.Users
                .FirstOrDefaultAsync(m => m.ID == ClaimTypes.NameIdentifier);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewData["ID"] = userId;    

            ViewData["Name"] = User.Identity.Name;
            return View();
        }

        public IActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Taruchat");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username,string password,bool rememberMe ,string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.ID == username);

            if(user != null)
            {
                if (VerifyPassword(user.Password, password))
                {
                    SignIn(user.ID, user.Name, user.Role.ToString(), rememberMe);

                    if (returnUrl != null)
                    {
                        return Redirect(returnUrl);
                    }
                    HttpContext.Session.SetString("PhotoURL", user.ProfilePic);
                    HttpContext.Session.SetString("Name", user.Name);

                    return RedirectToAction("Index", "Home");
                }
            }
            
            TempData["Error"] = "Error. Invalid Username OR Password !";
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.Remove("PhotoURL");
            HttpContext.Session.Remove("Name");
            return RedirectToAction("Login", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Detail()
        {
            var user = await _context.Users.FindAsync(User.Identity.Name);

            var viewModel = new UserDetailModel 
            { 
                Name = user.Name,
                Email = user.Email,
                Gender = user.Gender,
                ProfilePic = user.ProfilePic,
            };

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Detail(UserDetailModel viewModel) {

            var user = await _context.Users.FindAsync(User.Identity.Name);

            if (viewModel.Photo != null)
            {
                string err = ValidatePhoto(viewModel.Photo);
                if (err != null)
                {
                    ModelState.AddModelError("Photo", err);
                }
            }

            if (ModelState.IsValid)
            {
                if (viewModel.ProfilePic != null)
                {
                    //DeletePhoto(user.ProfilePic);
                    user.ProfilePic = viewModel.ProfilePic;
                    HttpContext.Session.SetString("PhotoURL", user.ProfilePic);

                }
                user.Name = viewModel.Name;
                HttpContext.Session.SetString("Name", viewModel.Name);
                user.Email = viewModel.Email;
                user.Gender = viewModel.Gender;
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Taruchat");
            }


            return View();

        }

        [Authorize]
        public IActionResult Reset()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Reset(ChangePwVM viewModel)
        {
            var user = await _context.Users.FindAsync(User.Identity.Name);

            if (user == null || VerifyPassword(user.Password, viewModel.Old) == false)
            {
                ModelState.AddModelError("Old", "Current Password not matched.");
            }

            if (ModelState.IsValid)
            {
                string hash = HashPassword(viewModel.New);

                user.Password = hash;

                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Taruchat");
            }

            return View(viewModel);
        }


        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.ID == id);
        }

        //Save Photo
        private string SavePhoto(IFormFile photo)
        {
            string name = Guid.NewGuid().ToString("n") + ".png";

            string FilePath = Path.Combine(hostingEnv.WebRootPath, "ProfilePic");
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }

            var fileName = Path.GetFileName(name);
            var filePath = Path.Combine(FilePath, fileName);

            using (FileStream fs = System.IO.File.Create(filePath))

            {
                photo.CopyTo(fs);
            }

            return name;
        }

        public IActionResult Denied()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }




        //Authentication Function
        private async void SignIn(string username, string name, string role, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, name),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                IsPersistent = rememberMe,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(claimsPrincipal, authProperties);
        }

        private bool VerifyPassword(string hash, string password)
        {
            return ph.VerifyHashedPassword(hash, password)
                   == PasswordVerificationResult.Success;
        }
        private string HashPassword(string password)
        {
            return ph.HashPassword(password);
        }

        // --------------------------------------------------------------------
        // Photo helper functions
        // --------------------------------------------------------------------

        private string ValidatePhoto(IFormFile f)
        {
            var reType = new Regex(@"^image\/(jpeg|png)$", RegexOptions.IgnoreCase);
            var reName = new Regex(@"^.+\.(jpg|jpeg|png)$", RegexOptions.IgnoreCase);

            if (f == null)
            {
                return "No photo.";
            }
            else if (!reType.IsMatch(f.ContentType) || !reName.IsMatch(f.FileName))
            {
                return "Only JPG or PNG photo is allowed.";
            }
            else if (f.Length > 1 * 1024 * 1024)
            {
                return "Photo size cannot more than 1MB.";
            }

            return null;
        }

        private string SavePhoto(string base64image)
        {
            var t = base64image.Substring(23);
            byte[] bytes = Convert.FromBase64String(t);


            string name = Guid.NewGuid().ToString("n") + ".jpg";
            string FilePath = Path.Combine(hostingEnv.WebRootPath, "ProfilePic");

            if (!Directory.Exists(FilePath))
                Directory.CreateDirectory(FilePath);

            var fileName = name;
            var filePath = Path.Combine(FilePath, fileName);

            using (MemoryStream ms = new MemoryStream(bytes))
            {
      //          upload.File.CopyTo(fs);
            }


            return name;
        }

        private void DeletePhoto(string name)
        {
            string FilePath = Path.Combine(hostingEnv.WebRootPath, "ProfilePic");
            name = System.IO.Path.GetFileName(name);
            var filePath = Path.Combine(FilePath, name);
            System.IO.File.Delete(filePath);
        }




    }
}
