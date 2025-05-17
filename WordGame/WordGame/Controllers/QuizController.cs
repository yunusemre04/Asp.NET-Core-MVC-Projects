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

        [HttpGet]
        public IActionResult Select()
        {
            return View();
        }

        public IActionResult Start()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var quizWords = GetTodayWords(userId.Value);

            return View(quizWords);
        }

        public IActionResult StartRandom(int count)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var todayQuizWords = new List<Word>();

            // QuizProgress üzerinden zamanı gelen kelimeleri topla
            var progressList = _context.QuizProgresses
                .Where(p => p.UserId == userId && !p.IsCompleted)
                .ToList();

            var now = DateTime.Now;
            foreach (var progress in progressList)
            {
                var expectedNext = GetNextRepeatDate(progress.LastAnsweredDate, progress.CorrectCount);
                if (expectedNext <= now)
                {
                    var word = _context.Words.FirstOrDefault(w => w.WordId == progress.WordId);
                    if (word != null)
                        todayQuizWords.Add(word);
                }
            }

            // Eksik varsa rastgele kelime seç (daha önce QuizProgress'e hiç eklenmemiş olanlardan)
            int eksik = count - todayQuizWords.Count;
            if (eksik > 0)
            {
                var alreadyInProgressIds = progressList.Select(p => p.WordId).ToList();

                var newWords = _context.Words
                    .Where(w => !alreadyInProgressIds.Contains(w.WordId))
                    .OrderBy(x => Guid.NewGuid())
                    .Take(eksik)
                    .ToList();

                // Yeni kelimeleri hem listeye ekle, hem QuizProgress'e yaz
                foreach (var word in newWords)
                {
                    todayQuizWords.Add(word);

                    var newProgress = new QuizProgress
                    {
                        UserId = userId.Value,
                        WordId = word.WordId,
                        CorrectCount = 0,
                        LastAnsweredDate = DateTime.Now.AddDays(-1), // Hemen gösterilsin
                        IsCompleted = false
                    };

                    _context.QuizProgresses.Add(newProgress);
                }

                _context.SaveChanges(); // Yeni kayıtları DB'ye yaz

                return View("Start", todayQuizWords);
            }



            if (!todayQuizWords.Any())
            {
                TempData["Error"] = "Quiz için yeterli kelime bulunamadı. Lütfen önce kelime ekleyin.";
                return RedirectToAction("Index", "Home");
            }

            return View("Start", todayQuizWords);
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

            int remain = dailyLimit - wordList.Count;
            if (remain > 0)
            {
                var knownWordsIds = progressList.Select(p => p.WordId).ToList();
                
                var newWords = _context.Words
                    .Where(w => !knownWordsIds.Contains(w.WordId))
                    .Take(remain)
                    .ToList();
              
                wordList.AddRange(newWords);
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

            int correctCount = 0;

            foreach (var answer in model.Answers)
            {
                var word = await _context.Words
                    .Include(w => w.WordSamples)
                    .FirstOrDefaultAsync(w => w.WordId == answer.WordId);

                var progress = await _context.QuizProgresses
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.WordId == answer.WordId);

                // Küçük büyük harf farkı yok!
                bool isCorrect = string.Equals(word?.TurWordName?.Trim(), answer.UserAnswer?.Trim(), StringComparison.OrdinalIgnoreCase);

                if (isCorrect)
                {
                    correctCount++;

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
                            progress.IsCompleted = true;

                        _context.QuizProgresses.Update(progress);
                    }
                }
                else
                {
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

            ViewBag.Total = model.Answers.Count;
            ViewBag.Correct = correctCount;

            return View("Result");
        }


        [HttpPost]
        public IActionResult StartManualQuiz(List<int> selectedWordIds)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || selectedWordIds == null || !selectedWordIds.Any())
                return RedirectToAction("ManualSelect");

            var todayWords = new List<Word>();

            foreach (var wordId in selectedWordIds)
            {
                // Eğer daha önce eklenmemişse QuizProgress'e kaydet
                var existing = _context.QuizProgresses
                    .FirstOrDefault(p => p.UserId == userId && p.WordId == wordId);

                if (existing == null)
                {
                    var progress = new QuizProgress
                    {
                        UserId = userId.Value,
                        WordId = wordId,
                        CorrectCount = 0,
                        LastAnsweredDate = DateTime.Now.AddDays(-1), // hemen gösterilsin
                        IsCompleted = false
                    };
                    _context.QuizProgresses.Add(progress);
                }

                // View'a göndermek için kelimeyi listeye ekle
                var word = _context.Words.FirstOrDefault(w => w.WordId == wordId);
                if (word != null)
                    todayWords.Add(word);
            }

            _context.SaveChanges();

            // Start.cshtml view'ına gönder
            return View("Start", todayWords);
        }


    }

}
