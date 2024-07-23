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

        public async Task<User> GetUserAsync(int id)
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
    }
}
