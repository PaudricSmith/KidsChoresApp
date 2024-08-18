using KidsChoresApp.Pages;
using KidsChoresApp.Pages.ChildPages;
using KidsChoresApp.Pages.ChorePages;


namespace KidsChoresApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));

            Routing.RegisterRoute(nameof(ChildPage), typeof(ChildPage));
            Routing.RegisterRoute(nameof(AddChildPage), typeof(AddChildPage));

            Routing.RegisterRoute(nameof(ChoresPage), typeof(ChoresPage));
            Routing.RegisterRoute(nameof(AddChoresPage), typeof(AddChoresPage));
        }

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);
            Console.WriteLine($"Navigating from {args.Source} to {args.Target}");
        }

        protected override void OnNavigated(ShellNavigatedEventArgs args)
        {
            base.OnNavigated(args);
            Console.WriteLine($"Navigated to {args.Current.Location}");
        }
    }
}
