using KidsChoresApp.Pages;
using KidsChoresApp.Pages.ChildPages;
using KidsChoresApp.Pages.ChorePages;
using KidsChoresApp.Pages.FeedbackPages;
using KidsChoresApp.Services;
using MauiIcons.Fluent;
using Microsoft.Extensions.Logging;
using SQLite;


namespace KidsChoresApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()

        {
            var builder = MauiApp.CreateBuilder();


#if DEBUG
            builder.Logging.AddDebug();
#endif


            builder
               .UseMauiApp<App>()
               .ConfigureFonts(fonts =>
               {
                   fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                   fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
               })
               .UseFluentMauiIcons();


            SQLitePCL.Batteries_V2.Init();


            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "kidschoresapp.db3");

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////// Uncomment to reset the preferences and database /////////////////////////////////////////////////////////////////
            //Preferences.Default.Clear();
            //if (File.Exists(dbPath)) File.Delete(dbPath);
            //dbPath ??= Path.Combine(FileSystem.AppDataDirectory, "kidschoresapp.db3");
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            
            builder.Services.AddSingleton(s => new SQLiteAsyncConnection(dbPath));
            
            // Services
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<DataSeedingService>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<ParentService>();
            builder.Services.AddSingleton<ChildService>();
            builder.Services.AddSingleton<ChoreService>();

            // Pages
            builder.Services.AddTransient<LoadingPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<SetupPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<FeedbackPage>();
            builder.Services.AddTransient<ParentalLockPage>();
            builder.Services.AddTransient<AddChildPage>();
            builder.Services.AddTransient<ChildPage>();
            builder.Services.AddTransient<AddChoresPage>();
            builder.Services.AddTransient<ChoresPage>();
            builder.Services.AddSingleton<HomePage>();


            return builder.Build();

            //var app = builder.Build();
            // Seed database
            //SeedDatabase(app);
            //return app;
        }


        //private static async void SeedDatabase(MauiApp app)
        //{
        //    using var scope = app.Services.CreateScope();
        //    var seeder = scope.ServiceProvider.GetRequiredService<DataSeedingService>();
        //    await seeder.SeedDatabaseAsync();
        //}
    }
}
