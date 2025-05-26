using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

        //Returns Word select screen 
        [HttpGet]
        public IActionResult Select()
        {
            return View();
        }


        //Returns Quiz Screen with words 
        public IActionResult Start()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var quizWords = GetTodayWords(userId.Value);

            return View(quizWords);
        }

        //Returns Word to Start screen randomly
        public IActionResult StartRandom(int count)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var todayQuizWords = new List<Word>();

            var progressList = _context.QuizProgresses
                .Where(p => p.UserId == userId && !p.IsCompleted)
                .ToList();

            var now = DateTime.Now;
            foreach (var progress in progressList)
            {
                var expectedNext = GetNextRepeatDate(progress.LastAnsweredDate, progress.CorrectCount);
                if (expectedNext <= now)
                {
                    var word = _context.Words
                        .Include(w => w.WordSamples) 
                        .FirstOrDefault(w => w.WordId == progress.WordId);

                    if (word != null)
                        todayQuizWords.Add(word);
                }
            }

            int left = count - todayQuizWords.Count;
            if (left > 0)
            {
                var alreadyInProgressIds = progressList.Select(p => p.WordId).ToList();

                var newWords = _context.Words
                    .Include(w => w.WordSamples) 
                    .Where(w => !alreadyInProgressIds.Contains(w.WordId))
                    .OrderBy(x => Guid.NewGuid())
                    .Take(left)
                    .ToList();

                foreach (var word in newWords)
                {
                    todayQuizWords.Add(word);

                    var newProgress = new QuizProgress
                    {
                        UserId = userId.Value,
                        WordId = word.WordId,
                        CorrectCount = 0,
                        LastAnsweredDate = DateTime.Now.AddDays(-1),
                        IsCompleted = false
                    };

                    _context.QuizProgresses.Add(newProgress);
                }

                _context.SaveChanges();
            }

            if (!todayQuizWords.Any())
            {
                TempData["Error"] = "Quiz için yeterli kelime bulunamadı. Lütfen önce kelime ekleyin.";
                return RedirectToAction("Start");
            }

            return View("Start", todayQuizWords);
        }


        //Returns Todays words 
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
                    var word = _context.Words
                        .Include(w => w.WordSamples) // ✅ Buraya dikkat
                        .FirstOrDefault(w => w.WordId == progress.WordId);

                    if (word != null)
                        wordList.Add(word);
                }
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            int dailyLimit = user?.DailyWordLimit ?? 10;

            int remain = dailyLimit - wordList.Count;
            if (remain > 0)
            {
                var knownWordsIds = progressList.Select(p => p.WordId).ToList();

                var newWords = _context.Words
                    .Include(w => w.WordSamples) 
                    .Where(w => !knownWordsIds.Contains(w.WordId))
                    .Take(remain)
                    .ToList();

                wordList.AddRange(newWords);
            }

            return wordList;
        }



        //Determine which word when it's take
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
        
        //Quiz answer controle with entered by user
        [HttpPost]
        public async Task<IActionResult> Submit(QuizSubmitViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            int correctCount = 0;

            var answerResults = new List<(string Eng, string Tur, bool IsCorrect)>();

            foreach (var answer in model.Answers)
            {
                var word = await _context.Words
                    .Include(w => w.WordSamples)
                    .FirstOrDefaultAsync(w => w.WordId == answer.WordId);

                var progress = await _context.QuizProgresses
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.WordId == answer.WordId);

                
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
                 answerResults.Add((word.EngWordName, word.TurWordName, isCorrect));
               
            }

            await _context.SaveChangesAsync();

            ViewBag.Total = model.Answers.Count;
            ViewBag.Correct = correctCount;
            TempData["AnswerResults"] = JsonConvert.SerializeObject(answerResults);
            return View("Result");
        }


        


    }

}
