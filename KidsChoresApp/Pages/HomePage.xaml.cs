using KidsChoresApp.Models;
using KidsChoresApp.Services;
using System.Collections.ObjectModel;


namespace KidsChoresApp.Pages
{
    public partial class HomePage : ContentPage
    {
        private readonly UserService _userService;
        private readonly ChildService _childService;

        public ObservableCollection<User> Users { get; set; }
        public ObservableCollection<Child> Children { get; set; }
        public User? CurrentUser { get; set; }
        public Child? SelectedChild { get; set; }


        public HomePage(UserService userService, ChildService childService)
        {
            InitializeComponent();

            _userService = userService;
            _childService = childService;

            Users = new ObservableCollection<User>();
            Children = new ObservableCollection<Child>();

            BindingContext = this;
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadUsers();
        }

        private async Task LoadUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(user);
            }
        }

        private async void OnUserSelected(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            if (picker.SelectedItem is User selectedUser)
            {
                CurrentUser = await _userService.GetUserWithDetailsAsync(selectedUser.Id);
                Children.Clear();
                if (CurrentUser != null)
                {
                    foreach (var child in CurrentUser.Children)
                    {
                        Children.Add(child);
                    }
                }
                OnPropertyChanged(nameof(CurrentUser));
            }
        }

        private async void OnViewChildDetailsClicked(object sender, EventArgs e)
        {
            var selectedChild = ChildPicker.SelectedItem as Child;
            if (selectedChild != null && CurrentUser != null)
            {
                SelectedChild = await _childService.GetChildAsync(CurrentUser.Id, selectedChild.Id);
                if (SelectedChild != null)
                {
                    await DisplayAlert("Child Details", $"Name: {SelectedChild.Name}\nMoney: {SelectedChild.Money}\nWeekly Earnings: {SelectedChild.WeeklyEarnings}\nLifetime Earnings: {SelectedChild.LifetimeEarnings}", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "Child does not belong to the current user.", "OK");
                }
            }
        }

        private async void OnAddChildClicked(object sender, EventArgs e)
        {
            if (CurrentUser == null) return;

            // Create a new child (for simplicity, hardcoding values here)
            var newChild = new Child
            {
                Name = "Hanna Doe",
                Image = "hulkgirl",
                Money = 100,
                WeeklyEarnings = 100,
                LifetimeEarnings = 100,
            };

            await _childService.AddChildAsync(CurrentUser.Id, newChild);

            // Update the UI
            Children.Add(newChild);
            OnPropertyChanged(nameof(Children));
        }

        private async void OnAssignChoresClicked(object sender, EventArgs e)
        {
            // Implement assign chores logic
        }
    }
}