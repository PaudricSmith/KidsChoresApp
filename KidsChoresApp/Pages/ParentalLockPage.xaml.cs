using KidsChoresApp.Models;
using KidsChoresApp.Services;


namespace KidsChoresApp.Pages
{
    public partial class ParentalLockPage : ContentPage
    {
        private readonly ParentService _parentService;
        private readonly AuthService _authService;

        private Parent? _currentParent;
        private int _userId;

        public Parent? CurrentParent
        {
            get => _currentParent;
            set
            {
                if (_currentParent != value)
                {
                    _currentParent = value;
                    OnPropertyChanged();
                }
            }
        }


        public ParentalLockPage(AuthService authService, ParentService parentService)
        {
            InitializeComponent();

            _authService = authService;
            _parentService = parentService;

            BindingContext = this;
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _userId = _authService.GetUserId() ?? 0;
            CurrentParent = await _parentService.GetParentByUserIdAsync(_userId);

            ResetUI();

            if (CurrentParent?.IsParentLockEnabled == true)
            {
                LockButton.IsVisible = false;

                await AttemptUnlock();
            }
            else
            {
                ShowLockButton();
            }
        }

        private async void OnLockButtonTapped(object sender, EventArgs e)
        {
            if (CurrentParent != null)
            {
                await SetParentLockStatusAsync(true);
                await Shell.Current.GoToAsync($"///{nameof(HomePage)}");
            }
        }

        private async Task AttemptUnlock()
        {
            string? passcode = await DisplayPromptAsync("Enter your Parental Passcode", "", maxLength: 4, keyboard: Keyboard.Numeric);
            if (passcode == null || passcode != CurrentParent?.Passcode)
            {
                await DisplayAlert("Error", "Incorrect passcode", "OK");
                await Shell.Current.GoToAsync($"///{nameof(HomePage)}");

                return;
            }

            await SetParentLockStatusAsync(false);
            await Shell.Current.GoToAsync($"///{nameof(HomePage)}");
        }

        private async Task SetParentLockStatusAsync(bool isLocked)
        {
            if (CurrentParent == null) return;

            CurrentParent.IsParentLockEnabled = isLocked;
            await _parentService.SaveParentAsync(CurrentParent);

            ShowActivityIndicator(true, isLocked ? "Locking..." : "Unlocking...");

            await Task.Delay(1000);
        }

        private void ResetUI()
        {
            LockButton.IsVisible = false;
            ShowActivityIndicator(false);
            ActivityIndicatorLabel.Text = string.Empty;
        }

        private void ShowLockButton()
        {
            LockButton.IsVisible = true;
            LockButton.Text = "Lock";
        }

        private void ShowActivityIndicator(bool isVisible, string message = "")
        {
            ActivityIndicator.IsRunning = isVisible;
            ActivityIndicator.IsVisible = isVisible;
            ActivityIndicatorLabel.Text = message;
        }
    }
}
