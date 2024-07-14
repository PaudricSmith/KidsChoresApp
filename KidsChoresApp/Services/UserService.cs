using KidsChoresApp.Data;
using KidsChoresApp.Models;
using Microsoft.EntityFrameworkCore;


namespace KidsChoresApp.Services
{
    public class UserService
    {
        private readonly KidsChoresDbContext _dbContext;


        public UserService(KidsChoresDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<User> GetUserWithDetailsAsync(int userId)
        {
            return await _dbContext.Users
                .Include(u => u.Parent)
                .Include(u => u.Children)
                .ThenInclude(c => c.Chores)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _dbContext.Users
                .Include(u => u.Parent)
                .Include(u => u.Children)
                .ThenInclude(c => c.Chores)
                .ToListAsync();
        }

        public async Task AddUserAsync(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
