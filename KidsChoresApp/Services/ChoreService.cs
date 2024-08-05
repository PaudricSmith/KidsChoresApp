using SQLite;
using KidsChoresApp.Models;


namespace KidsChoresApp.Services
{
    public class ChoreService
    {
        private readonly SQLiteAsyncConnection _database;


        public ChoreService(SQLiteAsyncConnection database)
        {
            _database = database;
            _database.CreateTableAsync<Chore>().Wait();
        }


        public async Task<Chore> GetChoreAsync(int id)
        {
            return await _database.Table<Chore>().Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Chore>> GetChoresByChildIdAsync(int childId)
        {
            return await _database.Table<Chore>().Where(c => c.ChildId == childId).ToListAsync();
        }

        public async Task<int> SaveChoreAsync(Chore chore)
        {
            if (chore.Id != 0)
            {
                return await _database.UpdateAsync(chore);
            }
            else
            {
                return await _database.InsertAsync(chore);
            }
        }

        public async Task<int> DeleteChoreAsync(Chore chore)
        {
            return await _database.DeleteAsync(chore);
        }
    }
}
