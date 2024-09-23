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
            // Remove pages when navigating to ParentalLockPage so they don't blink on screen
            // when navigating to the HomePage from the ParentLockPage
            if (e.Target.Location.OriginalString == $"//{nameof(ParentalLockPage)}")
            {
                if (e.Current.Location.OriginalString == $"//{nameof(HomePage)}")
                {
                    return;
                }

                // Remove specific pages based on the current route
                if (e.Current.Location.OriginalString == $"//{nameof(HomePage)}/{nameof(ChildPage)}")
                {
                    RemovePageIfExists<ChildPage>();
                }
                else if (e.Current.Location.OriginalString == $"//{nameof(HomePage)}/{nameof(ChildPage)}/{nameof(ChoresPage)}")
                {
                    RemovePageIfExists<ChoresPage>();
                    RemovePageIfExists<ChildPage>();
                }
                else if (e.Current.Location.OriginalString == $"//{nameof(HomePage)}/{nameof(AddChildPage)}")
                {
                    RemovePageIfExists<AddChildPage>();
                }
                else if (e.Current.Location.OriginalString == $"//{nameof(HomePage)}/{nameof(AddChoresPage)}")
                {
                    RemovePageIfExists<AddChoresPage>();
                }
            }
            else if (e.Current.Location.OriginalString == $"//{nameof(ParentalLockPage)}" &&
                e.Target.Location.OriginalString == $"///{nameof(HomePage)}")
            {
                _currentParent = await _parentService.GetParentByUserIdAsync(_userId);

                UpdateParentLockIcon();
            }
            else if (e.Current.Location.OriginalString == $"//{nameof(LoadingPage)}")
            {
                _userId = _authService.GetUserId() ?? 0;
                _currentParent = await _parentService.GetParentByUserIdAsync(_userId);

                UpdateParentLockIcon();
            }
        }

        private void RemovePageIfExists<TPage>() where TPage : Page
        {
            var page = Shell.Current.Navigation.NavigationStack.FirstOrDefault(p => p is TPage);
            if (page != null)
            {
                Shell.Current.Navigation.RemovePage(page);
            }
        }
    }
}
