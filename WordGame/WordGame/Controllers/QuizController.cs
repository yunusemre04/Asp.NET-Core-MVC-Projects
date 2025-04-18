using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WordGame.Data;
using WordGame.Models.Entities;
using WordGame.Models.ViewModels;

namespace WordGame.Controllers
{
    public class QuizController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuizController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Start()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var quizWords = GetTodayWords(userId.Value);

            return View(quizWords);
        }

        private List<Word> GetTodayWords(int userId)
        {
            var wordList = new List<Word>();
            var progressList = _context.QuizProgresses
                .Where(q => q.UserId == userId && !q.IsCompleted)
                .ToList();

            var now = DateTime.Now;

            foreach (var progress in progressList)
            {
                var expectedNextDate = GetNextRepeatDate(progress.LastAnsweredDate, progress.CorrectCount);
                if (expectedNextDate <= now)
                {
                    var word = _context.Words.FirstOrDefault(w => w.WordId == progress.WordId);
                    if (word != null)
                        wordList.Add(word);
                }
            }

            // Eğer kelime sayısı yetersizse, yeni kelimelerden ekle
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            int dailyLimit = user?.DailyWordLimit ?? 10;

            int eksik = dailyLimit - wordList.Count;
            if (eksik > 0)
            {
                var bilinenKelimeIdler = progressList.Select(p => p.WordId).ToList();
                var yeniKelimeler = _context.Words
                    .Where(w => !bilinenKelimeIdler.Contains(w.WordId))
                    .Take(eksik)
                    .ToList();

                wordList.AddRange(yeniKelimeler);
            }

            return wordList;
        }

        private DateTime GetNextRepeatDate(DateTime lastDate, int correctCount)
        {
            return correctCount switch
            {
                0 => lastDate.AddDays(1),
                1 => lastDate.AddDays(7),
                2 => lastDate.AddMonths(1),
                3 => lastDate.AddMonths(3),
                4 => lastDate.AddMonths(6),
                5 => lastDate.AddYears(1),
                _ => lastDate
            };
        }
        [HttpPost]
        public async Task<IActionResult> Submit(QuizSubmitViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            foreach (var answer in model.Answers)
            {
                var progress = await _context.QuizProgresses
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.WordId == answer.WordId);

                if (answer.IsCorrect)
                {
                    if (progress == null)
                    {
                        progress = new QuizProgress
                        {
                            UserId = userId.Value,
                            WordId = answer.WordId,
                            CorrectCount = 1,
                            LastAnsweredDate = DateTime.Now,
                            IsCompleted = false
                        };
                        _context.QuizProgresses.Add(progress);
                    }
                    else
                    {
                        progress.CorrectCount++;
                        progress.LastAnsweredDate = DateTime.Now;

                        if (progress.CorrectCount >= 6)
                        {
                            progress.IsCompleted = true;
                        }

                        _context.QuizProgresses.Update(progress);
                    }
                }
                else
                {
                    // yanlışsa sıfırla
                    if (progress != null)
                    {
                        progress.CorrectCount = 0;
                        progress.LastAnsweredDate = DateTime.Now;
                        progress.IsCompleted = false;
                        _context.QuizProgresses.Update(progress);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Result");
        }
        public IActionResult Result()
        {
            return View();
        }

    }

}
