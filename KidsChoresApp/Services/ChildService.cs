using KidsChoresApp.Data;
using KidsChoresApp.Models;
using Microsoft.EntityFrameworkCore;


namespace KidsChoresApp.Services
{
    public class ChildService
    {
        private readonly KidsChoresDbContext _dbContext;


        public ChildService(KidsChoresDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<Child> GetChildAsync(int childId)
        {
            return await _dbContext.Children
                .Include(c => c.Chores)
                .FirstOrDefaultAsync(c => c.Id == childId);
        }

        public async Task<Child> GetChildForUserAsync(int userId, int childId)
        {
            return await _dbContext.Children
                .Include(c => c.Chores)
                .FirstOrDefaultAsync(c => c.Id == childId && c.UserId == userId);
        }

        public async Task AddChildAsync(Child child)
        {
            _dbContext.Children.Add(child);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateChildAsync(Child child)
        {
            _dbContext.Children.Update(child);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteChildAsync(int childId)
        {
            var child = await _dbContext.Children.FindAsync(childId);
            if (child != null)
            {
                _dbContext.Children.Remove(child);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
