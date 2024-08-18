using KidsChoresApp.Services;

namespace KidsChoresApp.Pages
{
    public partial class LoadingPage : ContentPage
    {
        private readonly AuthService _authService;

        public LoadingPage(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        protected async override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (await _authService.IsAuthenticatedAsync())
            {
                int? userId = _authService.GetUserId();
                if (userId.HasValue)
                {
                    await Shell.Current.GoToAsync($"///{nameof(HomePage)}?userId={userId.Value}");
                }
                else
                {
                    await Shell.Current.GoToAsync($"///{nameof(LoginPage)}");
                }
            }
            else
            {
                await Shell.Current.GoToAsync($"///{nameof(LoginPage)}");
            }
        }
    }
}