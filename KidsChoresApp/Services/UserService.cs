using KidsChoresApp.Models;
using SQLite;


namespace KidsChoresApp.Services
{
    public class UserService
    {
        private readonly SQLiteAsyncConnection _database;


        public UserService(SQLiteAsyncConnection database)
        {
            _database = database;
            _database.CreateTableAsync<User>().Wait();
        }


        public async Task<List<User>> GetUsersAsync()
        {
            return await _database.Table<User>().ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _database.Table<User>().Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveUserAsync(User user)
        {
            if (user.Id != 0)
            {
                return await _database.UpdateAsync(user);
            }
            else
            {
                return await _database.InsertAsync(user);
            }
        }

        public async Task<int> DeleteUserAsync(User user)
        {
            return await _database.DeleteAsync(user);
        }


        public async Task<string> GetUserPreferredCurrency(int userId)
        {
            var user = await GetUserByIdAsync(userId);
            return user?.PreferredCurrency ?? "EUR"; // Return default currency if user not found
        }

        public async Task SetUserPreferredCurrency(int userId, string currency)
        {
            var user = await GetUserByIdAsync(userId);
            if (user != null)
            {
                user.PreferredCurrency = currency;
                await SaveUserAsync(user);
            }
        }
    }
}
