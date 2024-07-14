using KidsChoresApp.Data;
using KidsChoresApp.Models;
using KidsChoresApp.Pages;
using KidsChoresApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace KidsChoresApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif


            // Sqlite DB
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "kidschoresapp.db");

            builder.Services.AddDbContext<KidsChoresDbContext>(options =>
            {
                options.UseSqlite($"Filename={Path.Combine(FileSystem.AppDataDirectory, "kidschoresapp.db")}");
            });


            builder.Services.AddSingleton<UserService>();

            builder.Services.AddScoped<HomePage>();


            var mauiApp = builder.Build();

            // Ensure database is created and seed data
            using (var scope = mauiApp.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<KidsChoresDbContext>();
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
                SeedDatabaseAsync(dbContext).Wait();
            }

            return mauiApp;
        }

        private static async Task SeedDatabaseAsync(KidsChoresDbContext context)
        {
            // Check if the database already contains any data
            if (!context.Users.Any())
            {
                var hashedBytes1 = SHA256.HashData(Encoding.UTF8.GetBytes("Password1!"));
                var hashedBytes2 = SHA256.HashData(Encoding.UTF8.GetBytes("Password2!"));

                // Add first user, parent, children, and chores
                var user1 = new User
                {
                    Email = "testemail1@email.com",
                    PasswordHash = Convert.ToBase64String(hashedBytes1),
                    PreferredCurrency = "USD",
                    IsSetupCompleted = true
                };

                var parent1 = new Parent
                {
                    User = user1,
                    Passcode = "1111"
                };

                var child1a = new Child
                {
                    User = user1,
                    Name = "John Doe",
                    Money = 10.5m,
                    WeeklyEarnings = 5.0m,
                    LifetimeEarnings = 50.0m,
                    Image = "batboy"
                };

                var child1b = new Child
                {
                    User = user1,
                    Name = "Jane Doe",
                    Money = 15.75m,
                    WeeklyEarnings = 7.0m,
                    LifetimeEarnings = 70.0m,
                    Image = "batgirl"
                };

                var chore1a = new Chore
                {
                    Child = child1a,
                    Name = "Clean Room",
                    Description = "Clean your room thoroughly.",
                    Image = "cleanroom.png",
                    AssignedTo = "John Doe",
                    Deadline = DateTime.Now.AddDays(2),
                    Worth = 5.0m,
                    Priority = 1,
                    IsComplete = false
                };

                var chore1b = new Chore
                {
                    Child = child1b,
                    Name = "Wash Dishes",
                    Description = "Wash all the dishes after dinner.",
                    Image = "washdishes.png",
                    AssignedTo = "Jane Doe",
                    Deadline = DateTime.Now.AddDays(1),
                    Worth = 3.0m,
                    Priority = 2,
                    IsComplete = false
                };

                context.Users.Add(user1);
                context.Parents.Add(parent1);
                context.Children.AddRange(child1a, child1b);
                context.Chores.AddRange(chore1a, chore1b);

                // Add second user, parent, children, and chores
                var user2 = new User
                {
                    Email = "testemail2@email.com",
                    PasswordHash = Convert.ToBase64String(hashedBytes2),
                    PreferredCurrency = "EUR",
                    IsSetupCompleted = true
                };

                var parent2 = new Parent
                {
                    User = user2,
                    Passcode = "2222"
                };

                var child2a = new Child
                {
                    User = user2,
                    Name = "Alice Doe",
                    Money = 20.0m,
                    WeeklyEarnings = 10.0m,
                    LifetimeEarnings = 100.0m,
                    Image = "supergirl"
                };

                var child2b = new Child
                {
                    User = user2,
                    Name = "Bob Doe",
                    Money = 25.0m,
                    WeeklyEarnings = 12.0m,
                    LifetimeEarnings = 120.0m,
                    Image = "superboy"
                };

                var chore2a = new Chore
                {
                    Child = child2a,
                    Name = "Mow Lawn",
                    Description = "Mow the lawn on the weekend.",
                    Image = "mowlawn.png",
                    AssignedTo = "Alice Doe",
                    Deadline = DateTime.Now.AddDays(3),
                    Worth = 7.0m,
                    Priority = 1,
                    IsComplete = false
                };

                var chore2b = new Chore
                {
                    Child = child2b,
                    Name = "Take Out Trash",
                    Description = "Take out the trash every evening.",
                    Image = "takeouttrash.png",
                    AssignedTo = "Bob Doe",
                    Deadline = DateTime.Now.AddDays(1),
                    Worth = 2.0m,
                    Priority = 2,
                    IsComplete = false
                };

                context.Users.Add(user2);
                context.Parents.Add(parent2);
                context.Children.AddRange(child2a, child2b);
                context.Chores.AddRange(chore2a, chore2b);

                await context.SaveChangesAsync();
            }
        }

    }
}
