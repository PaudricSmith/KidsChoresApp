using KidsChoresApp.Models;
using KidsChoresApp.Pages.ChildPages;
using KidsChoresApp.Pages.ChorePages;
using KidsChoresApp.Services;
using System.Collections.ObjectModel;


namespace KidsChoresApp.Pages
{
    public partial class HomePage : ContentPage
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private readonly ParentService _parentService;
        private readonly ChildService _childService;
        private readonly ChoreService _choreService;

        private Parent? _currentParent;
        private ImageSource _padlockImage;
        private User? _currentUser;
        private int _userId;
        private bool _isNavigating;

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

            var children = await _childService.GetChildrenByUserIdAsync(UserId);
            if (children == null) return;

            Children.Clear();
            foreach (var child in children)
            {
                Children.Add(child);
            }
        }

        private async void OnAddChildButtonTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(AddChildPage)}?userId={UserId}");
        }

        private async void OnAddChoresButtonTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(AddChoresPage)}?userId={UserId}");
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

                if (CurrentParent?.IsParentLockEnabled == true)
                {
                    string enteredPasscode = await DisplayPromptAsync("Enter Passcode", $"Enter the passcode for {child.Name}", maxLength: 4, keyboard: Keyboard.Numeric);

                    if (enteredPasscode != null && enteredPasscode == child.Passcode)
                    {
                        // Passcode is correct, navigate to the child page
                        await Shell.Current.GoToAsync($"{nameof(ChildPage)}?childId={child.Id}");
                    }
                    else if (String.IsNullOrEmpty(enteredPasscode))
                    {
                        // Cancel tapped or nothing was entered for the passcode
                        // So display no message and show HomePage again
                    }
                    else
                    {
                        // Passcode is incorrect
                        await DisplayAlert("Incorrect Passcode", "The passcode you entered is incorrect. Please try again.", "OK");
                    }
                }
                else
                {
                    // Parental lock is not enabled, navigate to the child page directly
                    await Shell.Current.GoToAsync($"{nameof(ChildPage)}?childId={child.Id}");
                }

                _isNavigating = false;
            }
        }
    }
}
