

using KidsChoresApp.Services;

namespace KidsChoresApp.Pages
{
    public partial class SettingsPage : ContentPage
    {
        private readonly AuthService _authService;


        public SettingsPage(AuthService authService)
        {
            InitializeComponent();

            _authService = authService;

            BindingContext = this;
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void OnCurrencySelected(object sender, EventArgs e)
        {

        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            _authService.Logout();

            Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

        private async void OnChangePasscodeClicked(object sender, EventArgs e)
        {
            
        }
    }
}
