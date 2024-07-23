using SQLite;
using KidsChoresApp.Models;


namespace KidsChoresApp.Services
{
    public class ParentService
    {
        private readonly SQLiteAsyncConnection _database;


        public ParentService(SQLiteAsyncConnection database)
        {
            _database = database;
            _database.CreateTableAsync<Parent>().Wait();
        }


        public async Task<Parent> GetParentByUserIdAsync(int userId)
        {
            return await _database.Table<Parent>().Where(p => p.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<int> SaveParentAsync(Parent parent)
        {
            if (parent.Id != 0)
            {
                return await _database.UpdateAsync(parent);
            }
            else
            {
                return await _database.InsertAsync(parent);
            }
        }

        public async Task<int> DeleteParentAsync(Parent parent)
        {
            return await _database.DeleteAsync(parent);
        }
    }
}
