namespace WordGame.Services
{
    using Microsoft.EntityFrameworkCore;
    using WordGame.Data;
    using WordGame.Models;
    using WordGame.Models.Entities;

    //This part controles register and login process 
    public class AuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterUser(string username, string email, string password)
        {
            if (_context.Users.Any(u => u.Email == email))
            {
                return false;
            }

            var user = new User
            {
                UserName = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User?> LoginUser(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }
    }

}
