using KidsChoresApp.Models;
using KidsChoresApp.Pages.ChildPages;
using KidsChoresApp.Pages.ChorePages;
using KidsChoresApp.Pages.FeedbackPages;
using KidsChoresApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace KidsChoresApp.Pages
{
    [QueryProperty(nameof(UserId), "userId")]
    public partial class HomePage : ContentPage, INotifyPropertyChanged
    {
        private readonly UserService _userService;
        private readonly ParentService _parentService;
        private readonly ChildService _childService;
        private readonly ChoreService _choreService;

        private User? _currentUser;
        private int _userId;
        private bool _isNavigating;
        private bool _isUserIdRetrieved;

        public Parent? CurrentParent { get; set; }
        public ObservableCollection<Child> Children { get; set; } = new ObservableCollection<Child>();

        public int UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                _isUserIdRetrieved = true;
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


        public HomePage(UserService userService, ParentService parentService, ChildService childService, ChoreService choreService)
        {
            InitializeComponent();

            _userService = userService;
            _parentService = parentService;
            _childService = childService;
            _choreService = choreService;

            BindingContext = this;

            Loaded += HomePage_Loaded;
        }


        private async void HomePage_Loaded(object? sender, EventArgs e)
        {
            if (_isUserIdRetrieved)
            {
                _isUserIdRetrieved = false;
                await LoadData();
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_isUserIdRetrieved)
            {
                _isUserIdRetrieved = false;
                await LoadData();
            }
        }

        private async Task LoadData()
        {
            CurrentUser = await _userService.GetUserByIdAsync(UserId);

            Children.Clear();
            var children = await _childService.GetChildrenByUserIdAsync(UserId);
            foreach (var child in children)
            {
                Children.Add(child);
            }

            CurrentParent = await _parentService.GetParentByUserIdAsync(UserId);
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
            await Shell.Current.GoToAsync($"{nameof(SettingsPage)}?userId={UserId}");
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
