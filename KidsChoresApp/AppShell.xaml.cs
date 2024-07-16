using KidsChoresApp.Pages.ChildPages;


namespace KidsChoresApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(AddChildPage), typeof(AddChildPage));
        }
    }
}
