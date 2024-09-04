using KidsChoresApp.Services;


namespace KidsChoresApp
{
    public partial class App : Application
    {
        private readonly AuthService _authService;
        private readonly ParentService _parentService;

        public App(AuthService authService, ParentService parentService)
        {
            InitializeComponent();

            _authService = authService;
            _parentService = parentService;

            MainPage = new AppShell(authService, parentService);
        }
    }
}
