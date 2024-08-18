using KidsChoresApp.Models;
using SQLite;
using System.Security.Cryptography;
using System.Text;


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

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _database.Table<User>().Where(u => u.Email == email).FirstOrDefaultAsync();
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


        public async Task<bool> RegisterAsync(string email, string password)
        {
            var user = await _database.Table<User>().Where(u => u.Email == email).FirstOrDefaultAsync();
            if (user != null) return false; // User already exists

            var newUser = new User
            {
                Email = email,
                PasswordHash = HashPassword(password),
                IsSetupCompleted = false,
                PreferredCurrency = "EUR" // Default to Euro currency
            };

            // Create new User
            await _database.InsertAsync(newUser);
            return true;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var user = await _database.Table<User>().Where(u => u.Email == email).FirstOrDefaultAsync();
            if (user == null) return false;

            return VerifyPassword(password, user.PasswordHash);
        }

        private string HashPassword(string password)
        {
            var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));

            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var hashedPassword = HashPassword(password);

            return hashedPassword == storedHash;
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
