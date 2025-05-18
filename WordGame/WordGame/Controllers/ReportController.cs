using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            var progressList = _context.QuizProgresses
                .Where(q => q.UserId == userId)
                .Include(q => q.Word) // WordName erişimi için gerekli
                .ToList();

            int total = progressList.Count;
            int completed = progressList.Count(q => q.IsCompleted);
            int wrongAttempts = progressList.Count(q => q.CorrectCount == 0);
            double successRate = total == 0 ? 0 : Math.Round(100.0 * completed / total, 2);

            var zorlanilan = progressList
                .Where(q => q.CorrectCount == 0)
                .GroupBy(q => q.WordId)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.First().Word?.EngWordName ?? "Bilinmeyen")
                .ToList();

            var wordProgresses = progressList
                .Where(p => p.Word != null)
                .Select(p => new WordProgressViewModel
                {
                    WordName = p.Word.EngWordName,
                    CorrectCount = p.CorrectCount
                })
                .OrderBy(w => w.WordName)
                .ToList();

            var model = new UserStatsViewModel
            {
                TotalWorkedWords = total,
                TotalCompletedWords = completed,
                TotalWrongAttempts = wrongAttempts,
                SuccessRate = successRate,
                HardestWords = zorlanilan,
                WordProgresses = wordProgresses
            };

            return View(model);
        }

    }

}
