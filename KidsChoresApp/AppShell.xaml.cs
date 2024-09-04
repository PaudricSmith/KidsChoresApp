using KidsChoresApp.Models;
using KidsChoresApp.Pages;
using KidsChoresApp.Pages.ChildPages;
using KidsChoresApp.Pages.ChorePages;
using KidsChoresApp.Services;
using MauiIcons.Core;
using MauiIcons.Fluent;


namespace KidsChoresApp
{
    public partial class AppShell : Shell
    {
        private readonly AuthService _authService;
        private readonly ParentService _parentService;

        private ImageSource? _parentLockIcon;
        private Parent? _currentParent;
        private int _userId;


        public ImageSource? ParentLockIcon
        {
            get => _parentLockIcon;
            private set
            {
                _parentLockIcon = value;
                OnPropertyChanged();
            }
        }

        
        public AppShell(AuthService authService, ParentService parentService)
        {
            InitializeComponent();

            _authService = authService;
            _parentService = parentService;

            RegisterRoutes();

            BindingContext = this;

            Navigating += OnShellNavigating;
        }


        private void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(ChildPage), typeof(ChildPage));
            Routing.RegisterRoute(nameof(AddChildPage), typeof(AddChildPage));
            Routing.RegisterRoute(nameof(ChoresPage), typeof(ChoresPage));
            Routing.RegisterRoute(nameof(AddChoresPage), typeof(AddChoresPage));
        }

        private void UpdateParentLockIcon()
        {
            if (_currentParent == null) return;

            ParentLockIcon = _currentParent.IsParentLockEnabled
                ? (ImageSource)new MauiIcon().Icon(FluentIcons.LockShield24)
                : (ImageSource)new MauiIcon().Icon(FluentIcons.LockOpen24);
        }

        private async void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
        {
            if (e.Target.Location.OriginalString == "//ParentalLockPage")
            {
                await HandleParentalLockPageNavigation();
                UpdateParentLockIcon();
            }
            else if (e.Current.Location.OriginalString == "//LoadingPage")
            {
                await LoadParentData();
                UpdateParentLockIcon();
            }
        }

        private async Task HandleParentalLockPageNavigation()
        {
            if (_currentParent == null)
                return;

            if (!_currentParent.IsParentLockEnabled)
            {
                _currentParent.IsParentLockEnabled = true;
            }
            else
            {
                string? passcode = await DisplayPromptAsync("Enter your Parental Passcode", "", maxLength: 4, keyboard: Keyboard.Numeric);

                if (passcode == null)
                {
                    await Shell.Current.GoToAsync($"///{nameof(HomePage)}");
                    return;
                }

                if (passcode == _currentParent.Passcode)
                {
                    _currentParent.IsParentLockEnabled = false;
                }
                else
                {
                    await DisplayAlert("Error", "Incorrect passcode", "OK");
                    await Shell.Current.GoToAsync($"///{nameof(HomePage)}");
                    return;
                }

                _currentParent.IsParentLockEnabled = false;
            }

            await _parentService.SaveParentAsync(_currentParent);
            await Shell.Current.GoToAsync($"///{nameof(HomePage)}");
        }

        private async Task LoadParentData()
        {
            _userId = _authService.GetUserId() ?? 0;
            _currentParent = await _parentService.GetParentByUserIdAsync(_userId);
        }
    }
}
