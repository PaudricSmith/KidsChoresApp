using KidsChoresApp.Models;
using KidsChoresApp.Pages.ChildPages;
using KidsChoresApp.Pages.ChorePages;
using KidsChoresApp.Pages.FeedbackPages;
using KidsChoresApp.Services;
using System.Collections.ObjectModel;


namespace KidsChoresApp.Pages
{
    public partial class HomePage : ContentPage
    {
        private readonly UserService _userService;
        private readonly ParentService _parentService;
        private readonly ChildService _childService;
        private readonly ChoreService _choreService;

        private bool _isNavigating;

        public User? CurrentUser { get; set; }
        public Parent? CurrentParent { get; set; }
        public ObservableCollection<Child> Children { get; set; } = [];


        public HomePage(UserService userService, ParentService parentService, ChildService childService, ChoreService choreService)
        {
            InitializeComponent();

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
            CurrentUser = await _userService.GetUserAsync(1);

            Children.Clear();
            var children = await _childService.GetChildrenByUserIdAsync(CurrentUser.Id);
            foreach (var child in children)
            {
                Children.Add(child);
            }

            CurrentParent = await _parentService.GetParentByUserIdAsync(CurrentUser.Id);
        }

        private async void OnAddChildButtonTapped(object sender, EventArgs e)
        {
            if (CurrentUser == null) return;

            await Shell.Current.GoToAsync($"{nameof(AddChildPage)}?userId={CurrentUser.Id}");
        }

        private async void OnAddChoresButtonTapped(object sender, EventArgs e)
        {
            if (CurrentUser == null) return;

            await Shell.Current.GoToAsync($"{nameof(AddChoresPage)}?userId={CurrentUser.Id}");
        }

        private async void OnSettingsButtonTapped(object sender, EventArgs e)
        {
            if (CurrentUser == null) return;

            await Shell.Current.GoToAsync($"{nameof(SettingsPage)}");
        }

        private async void OnFeedbackButtonTapped(object sender, EventArgs e)
        {
            if (CurrentUser == null) return;

            await Shell.Current.GoToAsync($"{nameof(FeedbackPage)}");
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
    }
}
