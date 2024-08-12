using KidsChoresApp.Pages.ChildPages;
using KidsChoresApp.Pages.ChorePages;


namespace KidsChoresApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ChildPage), typeof(ChildPage));
            Routing.RegisterRoute(nameof(ChoresPage), typeof(ChoresPage));
        }
    }
}
