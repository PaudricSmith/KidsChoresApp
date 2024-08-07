

using KidsChoresApp.Services;

namespace KidsChoresApp.Pages
{
    [QueryProperty(nameof(UserId), "userId")]
    public partial class SettingsPage : ContentPage
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private int _userId;

        public int UserId
        {
            get => _userId;
            set
            {
                _userId = value;
            }
        }

        public SettingsPage(AuthService authService, UserService userService)
        {
            InitializeComponent();

            _authService = authService;
            _userService = userService;

            BindingContext = this;
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            CurrencyPicker.SelectedItem = await _userService.GetUserPreferredCurrency(UserId);
        }

        private async void OnCurrencySelected(object sender, EventArgs e)
        {
            if (CurrencyPicker.SelectedItem is string selectedCurrency)
            {
                await _userService.SetUserPreferredCurrency(UserId, selectedCurrency);
            }
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
