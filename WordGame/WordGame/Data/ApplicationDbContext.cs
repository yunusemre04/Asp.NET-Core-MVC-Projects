namespace WordGame.Data
{
    using Microsoft.EntityFrameworkCore;
    using WordGame.Models.Entities;

    public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

        public DbSet<User> Users { get; set; }
        public DbSet<Word> Words { get; set; }
        public DbSet<WordSample> WordSamples { get; set; }
        public DbSet<QuizProgress> QuizProgresses { get; set; }

    }

}
