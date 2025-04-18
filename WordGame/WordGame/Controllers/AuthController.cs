using WordGame.Models.ViewModels;
using WordGame.Services;
using Microsoft.AspNetCore.Mvc;
namespace WordGame.Controllers
{

    

    namespace WordGame.Controllers
    {
        public class AuthController : Controller
        {
            private readonly AuthService _authService;

            public AuthController(AuthService authService)
            {
                _authService = authService;
            }

            // Kullanıcı kayıt sayfasını gösterir
            public IActionResult Register()
            {
                return View();
            }

            // Kullanıcı kayıt işlemi
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
                    ModelState.AddModelError("", "Bu e-posta adresi zaten kullanımda.");
                    return View(model);
                }

                return RedirectToAction("Login");
            }

            // Kullanıcı giriş sayfasını gösterir
            public IActionResult Login()
            {
                return View();
            }

            // Kullanıcı giriş işlemi
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
                    ModelState.AddModelError("", "Geçersiz e-posta veya şifre.");
                    return View(model);
                }

                // Session'a kullanıcı bilgilerini kaydediyoruz
                HttpContext.Session.SetString("UserName", user.UserName);
                HttpContext.Session.SetInt32("UserId", user.UserId);

                return RedirectToAction("Index", "Home");
            }

            // Kullanıcı çıkış işlemi
            public IActionResult Logout()
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }
        }
    }

}
