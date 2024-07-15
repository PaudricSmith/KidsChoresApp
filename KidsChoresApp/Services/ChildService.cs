using KidsChoresApp.Models;
using SQLite;


namespace KidsChoresApp.Services
{
    public class ChildService
    {
        private readonly SQLiteAsyncConnection _database;


        public ChildService(SQLiteAsyncConnection database)
        {
            _database = database;
            _database.CreateTableAsync<Child>();
        }


        public async Task<List<Child>> GetChildrenByUserIdAsync(int userId)
        {
            return await _database.Table<Child>().Where(c => c.UserId == userId).ToListAsync();
        }

        public async Task<Child> GetChildAsync(int id)
        {
            return await _database.Table<Child>().Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveChildAsync(Child child)
        {
            if (child.Id != 0)
            {
                return await _database.UpdateAsync(child);
            }
            else
            {
                return await _database.InsertAsync(child);
            }
        }

        public async Task<int> DeleteChildAsync(Child child)
        {
            return await _database.DeleteAsync(child);
        }
    }
}
