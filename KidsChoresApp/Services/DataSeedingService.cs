using KidsChoresApp.Models;
using SQLite;
using System.Security.Cryptography;
using System.Text;


namespace KidsChoresApp.Services
{
    public class DataSeedingService
    {
        private readonly SQLiteAsyncConnection _database;


        public DataSeedingService(SQLiteAsyncConnection database)
        {
            _database = database;
        }


        public async Task WipeDatabaseAsync()
        {
            // Drop all tables
            await _database.DropTableAsync<User>();
            await _database.DropTableAsync<Parent>();
            await _database.DropTableAsync<Child>();
            await _database.DropTableAsync<Chore>();
        }

        public async Task SeedDatabaseAsync()
        {
            await _database.CreateTableAsync<User>();
            await _database.CreateTableAsync<Parent>();
            await _database.CreateTableAsync<Child>();
            await _database.CreateTableAsync<Chore>();

            // Check if the database already contains any data
            var usersCount = await _database.Table<User>().CountAsync();
            if (usersCount == 0)
            {
                var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes("Password1!"));

                // Add default users, parents, children, and chores
                var user = new User
                {
                    Email = "testemail1@email.com",
                    PasswordHash = Convert.ToBase64String(hashedBytes),
                    PreferredCurrency = "USD",
                    IsSetupCompleted = true
                };

                await _database.InsertAsync(user);

                var parent = new Parent
                {
                    UserId = user.Id,
                    Passcode = "1111"
                };

                await _database.InsertAsync(parent);

                var child1a = new Child
                {
                    UserId = user.Id,
                    Name = "John Doe",
                    Money = 10.5m,
                    WeeklyEarnings = 5.0m,
                    LifetimeEarnings = 50.0m,
                    Image = "batboy"
                };

                var child2a = new Child
                {
                    UserId = user.Id,
                    Name = "Jane Doe",
                    Money = 15.75m,
                    WeeklyEarnings = 7.0m,
                    LifetimeEarnings = 70.0m,
                    Image = "batgirl"
                };

                await _database.InsertAsync(child1a);
                await _database.InsertAsync(child2a);

                var chore1a = new Chore
                {
                    ChildId = child1a.Id,
                    Name = "Clean Room",
                    Description = "Clean your room thoroughly.",
                    Image = "cleanroom.png",
                    Deadline = DateTime.Now.AddDays(2),
                    Worth = 5.0m,
                    Priority = 1,
                    IsComplete = false
                };

                var chore2a = new Chore
                {
                    ChildId = child2a.Id,
                    Name = "Wash Dishes",
                    Description = "Wash all the dishes after dinner.",
                    Image = "washdishes.png",
                    Deadline = DateTime.Now.AddDays(1),
                    Worth = 3.0m,
                    Priority = 2,
                    IsComplete = false
                };

                await _database.InsertAsync(chore1a);
                await _database.InsertAsync(chore2a);


                hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes("Password2!"));


                var user2 = new User
                {
                    Email = "testemail2@email.com",
                    PasswordHash = Convert.ToBase64String(hashedBytes),
                    PreferredCurrency = "USD",
                    IsSetupCompleted = true
                };

                await _database.InsertAsync(user2);

                var parent2 = new Parent
                {
                    UserId = user2.Id,
                    Passcode = "2222"
                };

                await _database.InsertAsync(parent2);

                var child1b = new Child
                {
                    UserId = user2.Id,
                    Name = "John Doe",
                    Money = 10.5m,
                    WeeklyEarnings = 5.0m,
                    LifetimeEarnings = 50.0m,
                    Image = "superboy"
                };

                var child2b = new Child
                {
                    UserId = user2.Id,
                    Name = "Jane Doe",
                    Money = 15.75m,
                    WeeklyEarnings = 7.0m,
                    LifetimeEarnings = 70.0m,
                    Image = "supergirl"
                };

                await _database.InsertAsync(child1b);
                await _database.InsertAsync(child2b);

                var chore1b = new Chore
                {
                    ChildId = child1b.Id,
                    Name = "Clean Room",
                    Description = "Clean your room thoroughly.",
                    Image = "cleanroom.png",
                    Deadline = DateTime.Now.AddDays(2),
                    Worth = 5.0m,
                    Priority = 1,
                    IsComplete = false
                };

                var chore2b = new Chore
                {
                    ChildId = child2b.Id,
                    Name = "Wash Dishes",
                    Description = "Wash all the dishes after dinner.",
                    Image = "washdishes.png",
                    Deadline = DateTime.Now.AddDays(1),
                    Worth = 3.0m,
                    Priority = 2,
                    IsComplete = false
                };

                await _database.InsertAsync(chore1b);
                await _database.InsertAsync(chore2b);
            }
        }
    }
}
