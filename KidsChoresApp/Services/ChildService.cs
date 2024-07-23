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
            _database.CreateTableAsync<Child>().Wait();
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
            if (!string.IsNullOrEmpty(child.Image) && File.Exists(child.Image))
            {
                File.Delete(child.Image);
            }

            return await _database.DeleteAsync(child);
        }

        public async Task<bool> ChildExistsAsync(string name)
        {
            var child = await _database.Table<Child>().Where(c => c.Name == name).FirstOrDefaultAsync();
            return child != null;
        }
    }
}
