using WordGame.Models.ViewModels;
using WordGame.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WordGame.Data;
namespace WordGame.Controllers
{

    namespace WordGame.Controllers
    {
        public class AuthController : Controller
        {
            private readonly AuthService _authService;
            private readonly ApplicationDbContext _context;

            public AuthController(AuthService authService,ApplicationDbContext context)
            {
                _authService = authService;
                _context = context;
            }

            // Returns User Register Page
            public IActionResult Register()
            {
                return View();
            }

            // User Register Process to Database
            [HttpPost]
            public async Task<IActionResult> Register(RegisterViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                bool result = await _authService.RegisterUser(model.UserName, model.Email, model.Password);

                if (!result)
                {
                    ModelState.AddModelError("Email", "Bu e-posta adresi zaten kullanımda.");
                    return View(model);
                }

                return RedirectToAction("Login");
            }

            // Returns Log in Page
            public IActionResult Login()
            {
                return View();
            }

            // Users Log in Controle and Process
            [HttpPost]
            public async Task<IActionResult> Login(LoginViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _authService.LoginUser(model.Email, model.Password);

                if (user == null)
                {
                    TempData["LoginError"] = "Kullanıcı Adı veya Şifre Hatalı";
                    ModelState.AddModelError("", "Geçersiz e-posta veya şifre.");
                    return View(model);
                }

                // Saving user information to Session
                HttpContext.Session.SetString("UserName", user.UserName);
                HttpContext.Session.SetInt32("UserId", user.UserId);

                return RedirectToAction("Index", "Home");
            }

            // User Logout Process
            public IActionResult Logout()
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            // Returns Forgot Password Page
            [HttpGet]
            public IActionResult ForgotPassword()
            {
                return View();
            }

            //The function returns new Password using Email information
            [HttpPost]
            public IActionResult ForgotPassword(ForgotPasswordViewModel model)
            {
                if (!ModelState.IsValid)
                    return View(model);

                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Email", "Bu e-posta adresine kayıtlı kullanıcı bulunamadı.");
                    return View(model);
                }
                
                var newPassword = Guid.NewGuid().ToString().Substring(0, 8); 
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                _context.Users.Update(user);
                _context.SaveChanges();

                ViewBag.Message = $"Yeni şifreniz: <strong>{newPassword}</strong><br/> Lütfen giriş yaptıktan sonra değiştirin.";

                return View("ForgotPasswordResult");
            }
            //Returns Password Screen
            [HttpGet]
            public IActionResult ChangePassword()
            {
                if (HttpContext.Session.GetInt32("UserId") == null)
                    return RedirectToAction("Login");

                return View();
            }

            //Changing Password Process and Save to Database
            [HttpPost]
            public IActionResult ChangePassword(ChangePasswordViewModel model)
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                    return RedirectToAction("Login");

                if (!ModelState.IsValid)
                    return View(model);

                var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                    return RedirectToAction("Login");

                if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash))
                {
                    ModelState.AddModelError("CurrentPassword", "Mevcut şifre yanlış");
                    return View(model);
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                _context.Users.Update(user);
                _context.SaveChanges();

                ViewBag.Message = "Şifreniz başarıyla değiştirildi.";
                return View("ChangePasswordSuccess");
            }


        }

    }

}
