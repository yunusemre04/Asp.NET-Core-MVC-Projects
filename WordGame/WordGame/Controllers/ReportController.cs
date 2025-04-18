using Microsoft.AspNetCore.Mvc;
using WordGame.Data;
using WordGame.Models.ViewModels;

namespace WordGame.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var progress = _context.QuizProgresses
                .Where(q => q.UserId == userId)
                .ToList();

            int total = progress.Count;
            int completed = progress.Count(q => q.IsCompleted);
            int wrongAttempts = progress.Count(q => q.CorrectCount == 0);
            double successRate = total == 0 ? 0 : Math.Round(100.0 * completed / total, 2);

            // En çok sıfırlanan (yanlış yapılan) kelimeleri bulalım
            var zorlanilan = progress
                .Where(q => q.CorrectCount == 0)
                .GroupBy(q => q.WordId)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => _context.Words.FirstOrDefault(w => w.WordId == g.Key)?.EngWordName ?? "Bilinmeyen")
                .ToList();

            var model = new UserStatsViewModel
            {
                TotalWorkedWords = total,
                TotalCompletedWords = completed,
                TotalWrongAttempts = wrongAttempts,
                SuccessRate = successRate,
                HardestWords = zorlanilan
            };

            return View(model);
        }
    }

}
