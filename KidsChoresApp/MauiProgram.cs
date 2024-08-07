﻿using KidsChoresApp.Pages;
using KidsChoresApp.Pages.ChildPages;
using KidsChoresApp.Pages.ChorePages;
using KidsChoresApp.Pages.FeedbackPages;
using KidsChoresApp.Services;
using Microsoft.Extensions.Logging;
using SQLite;


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


            // Sqlite-net-pcl
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "kidschoresapp.db3");
            //builder.Services.AddSingleton(s => new SQLiteAsyncConnection(dbPath));

            //if (File.Exists(dbPath)) File.Delete(dbPath);
            //dbPath ??= Path.Combine(FileSystem.AppDataDirectory, "kidschoresapp.db3");

            builder.Services.AddSingleton(s => new SQLiteAsyncConnection(dbPath));
            
            // Services
            builder.Services.AddTransient<AuthService>();
            builder.Services.AddSingleton<DataSeedingService>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<ParentService>();
            builder.Services.AddSingleton<ChildService>();
            builder.Services.AddSingleton<ChoreService>();

            // Pages
            builder.Services.AddTransient<LoadingPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<FeedbackPage>();
            builder.Services.AddTransient<AddChildPage>();
            builder.Services.AddTransient<ChildPage>();
            builder.Services.AddTransient<AddChoresPage>();
            builder.Services.AddSingleton<HomePage>();

            var app = builder.Build();

            // Seed database
            SeedDatabase(app);

            return app;
        }


        private static async void SeedDatabase(MauiApp app)
        {
            using var scope = app.Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<DataSeedingService>();
            await seeder.SeedDatabaseAsync();
        }
    }
}
