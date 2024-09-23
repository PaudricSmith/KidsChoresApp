using KidsChoresApp.Models;
using KidsChoresApp.Services;


namespace KidsChoresApp.Pages
{
    public partial class SettingsPage : ContentPage
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private readonly ParentService _parentService;
        private Parent? _parent;
        private int _userId;

        public int UserId
        {
            get => _userId;
            set
            {
                _userId = value;
            }
        }

        public SettingsPage(AuthService authService, UserService userService, ParentService parentService)
        {
            InitializeComponent();

            _authService = authService;
            _userService = userService;
            _parentService = parentService;

            BindingContext = this;
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            UserId = _authService.GetUserId() ?? 0;

            if (CurrencyPicker != null)
            {
                CurrencyPicker.SelectedItem = await _userService.GetUserPreferredCurrency(UserId);
            }
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
            
            await Shell.Current.GoToAsync($"///{nameof(LoginPage)}");
        }

        private async void OnChangePasscodeClicked(object sender, EventArgs e)
        {
            _parent = await _parentService.GetParentByUserIdAsync(UserId);

            // Ask for old passcode
            string oldPasscode = await DisplayPromptAsync("Change Parental Passcode", "Enter your old Parental Passcode", maxLength: 4, keyboard: Keyboard.Numeric);
            if (oldPasscode == null) return;

            if (oldPasscode == _parent?.Passcode)
            {
                // Ask for new passcode
                string newPasscode = await DisplayPromptAsync("Change Parental Passcode", "Enter your new Parental Passcode", maxLength: 4, keyboard: Keyboard.Numeric);
                if (newPasscode == null) return;

                if (!string.IsNullOrEmpty(newPasscode))
                {
                    // Confirm new passcode
                    string confirmPasscode = await DisplayPromptAsync("Change Parental Passcode", "Confirm your new Parental Passcode", maxLength: 4, keyboard: Keyboard.Numeric);
                    if (confirmPasscode == null) return;

                    if (newPasscode == confirmPasscode)
                    {
                        _parent.Passcode = newPasscode;
                        await _parentService.SaveParentAsync(_parent);
                        await DisplayAlert("Success", "Parental Passcode changed successfully.", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", "New passcodes do not match.", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "New passcode cannot be empty.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "Incorrect old passcode.", "OK");
            }
        }
    }
}
