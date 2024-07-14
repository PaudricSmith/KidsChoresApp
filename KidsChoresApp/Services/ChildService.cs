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


        public async Task<Child> GetChildAsync(int userId, int childId)
        {
            return await _dbContext.Children
                .Include(c => c.Chores)
                .Where(c => c.Id == childId && c.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<List<Child>> GetChildrenForUserAsync(int userId)
        {
            return await _dbContext.Children
                .Include(c => c.Chores)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task AddChildAsync(int userId, Child child)
        {
            child.UserId = userId;
            _dbContext.Children.Add(child);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateChildAsync(int userId, Child child)
        {
            var existingChild = await GetChildAsync(userId, child.Id);
            if (existingChild != null)
            {
                existingChild.Name = child.Name;
                existingChild.Image = child.Image;
                existingChild.Money = child.Money;
                existingChild.WeeklyEarnings = child.WeeklyEarnings;
                existingChild.LifetimeEarnings = child.LifetimeEarnings;

                _dbContext.Children.Update(existingChild);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteChildAsync(int userId, int childId)
        {
            var child = await GetChildAsync(userId, childId);
            if (child != null)
            {
                _dbContext.Children.Remove(child);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
