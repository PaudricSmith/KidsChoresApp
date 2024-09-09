using KidsChoresApp.Models;
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
            if (e.Target.Location.OriginalString == "///HomePage")
            {
                _userId = _authService.GetUserId() ?? 0;
                _currentParent = await _parentService.GetParentByUserIdAsync(_userId);

                UpdateParentLockIcon();
            }
        }
    }
}
