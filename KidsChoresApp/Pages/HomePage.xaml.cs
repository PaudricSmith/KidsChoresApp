using KidsChoresApp.Models;
using KidsChoresApp.Pages.ChildPages;
using KidsChoresApp.Pages.ChorePages;
using KidsChoresApp.Pages.FeedbackPages;
using KidsChoresApp.Services;
using MauiIcons.Core;
using MauiIcons.Fluent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace KidsChoresApp.Pages
{
    public partial class HomePage : ContentPage, INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private readonly ParentService _parentService;
        private readonly ChildService _childService;
        private readonly ChoreService _choreService;

        private ImageSource _padlockImage;
        private User? _currentUser;
        private int _userId;
        private bool _isNavigating;

        public Parent? CurrentParent { get; set; }
        public ObservableCollection<Child> Children { get; set; } = new ObservableCollection<Child>();

        public int UserId
        {
            get => _userId;
            set
            {
                _userId = value;
            }
        }
        public User? CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }
        public bool IsPadlockUnlocked
        {
            get => CurrentParent?.IsPadlockUnlocked ?? false;
            set
            {
                if (CurrentParent != null)
                {
                    CurrentParent.IsPadlockUnlocked = value;
                    OnPropertyChanged();
                    UpdatePadlockImage();
                }
            }
        }
        public ImageSource PadlockImage
        {
            get => _padlockImage;
            set
            {
                _padlockImage = value;
                OnPropertyChanged();
            }
        }


        public HomePage(AuthService authService, UserService userService, ParentService parentService, ChildService childService, ChoreService choreService)
        {
            InitializeComponent();

            _authService = authService;
            _userService = userService;
            _parentService = parentService;
            _childService = childService;
            _choreService = choreService;

            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadData();
        }

        private async Task LoadData()
        {
            UserId = _authService.GetUserId() ?? 0;
            CurrentUser = await _userService.GetUserByIdAsync(UserId);
            CurrentParent = await _parentService.GetParentByUserIdAsync(UserId);

            UpdatePadlockImage();

            var children = await _childService.GetChildrenByUserIdAsync(UserId);
            if (children == null) return;

            Children.Clear();
            foreach (var child in children)
            {
                Children.Add(child);
            }
        }

        private void UpdatePadlockImage()
        {
            // Set the padlock image based on the current lock state
            PadlockImage = (ImageSource)(IsPadlockUnlocked
                ? new MauiIcon().Icon(FluentIcons.LockOpen24).IconBackgroundColor(Colors.Red)
                : new MauiIcon().Icon(FluentIcons.LockClosed24).IconBackgroundColor(Colors.Red));
        }

        private async void OnPadlockToggleTapped(object sender, EventArgs e)
        {
            if (CurrentParent == null) return;

            if (IsPadlockUnlocked)
            {
                IsPadlockUnlocked = false;
            }
            else
            {
                string result = await DisplayPromptAsync("Enter your Parental Passcode", "", maxLength: 4, keyboard: Keyboard.Numeric);
                if (result == CurrentParent.Passcode)
                {
                    IsPadlockUnlocked = true;
                }
                else
                {
                    await DisplayAlert("Error", "Incorrect passcode", "OK");
                }
            }

            await _parentService.SaveParentAsync(CurrentParent);
        }

        private async void OnAddChildButtonTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(AddChildPage)}?userId={UserId}");
        }

        private async void OnAddChoresButtonTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(AddChoresPage)}?userId={UserId}");
        }

        private async void OnSettingsButtonTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(SettingsPage)}");
        }

        private async void OnFeedbackButtonTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"///{nameof(FeedbackPage)}");
        }

        private async void OnChildFrameTapped(object sender, EventArgs e)
        {
            if (_isNavigating) return;
            _isNavigating = true;

            if (sender is Frame frame && frame.BindingContext is Child child)
            {
                var originalColor = frame.BackgroundColor;
                frame.BackgroundColor = Colors.AliceBlue;

                await Task.Delay(100);

                frame.BackgroundColor = originalColor;

                await Shell.Current.GoToAsync($"{nameof(ChildPage)}?childId={child.Id}");

                _isNavigating = false;
            }
        }

        public new event PropertyChangedEventHandler? PropertyChanged;
        protected new void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
