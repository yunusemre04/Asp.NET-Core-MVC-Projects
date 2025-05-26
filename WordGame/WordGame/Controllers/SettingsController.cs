using Microsoft.AspNetCore.Mvc;
using WordGame.Data;
using WordGame.Models.ViewModels;

namespace WordGame.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SettingsController(ApplicationDbContext context)
        {
            _context = context;
        }
        //Returns Settings Screen 
        [HttpGet]
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return NotFound();

            var model = new UserSettingsViewModel
            {
                DailyWordLimit = user.DailyWordLimit
            };

            return View(model);
        }


        //Function save daily word limit
        [HttpPost]
        public async Task<IActionResult> Index(UserSettingsViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return NotFound();

            user.DailyWordLimit = model.DailyWordLimit;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            ViewBag.Message = "Ayarlar başarıyla güncellendi.";
            return View(model);
        }
    }
}

