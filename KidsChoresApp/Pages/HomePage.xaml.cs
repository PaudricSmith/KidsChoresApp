using KidsChoresApp.Models;
using KidsChoresApp.Services;
using System.Collections.ObjectModel;


namespace KidsChoresApp.Pages
{
    public partial class HomePage : ContentPage
    {
        private readonly UserService _userService;
        private readonly ChildService _childService;
        private readonly ParentService _parentService;

        public ObservableCollection<User> Users { get; set; }
        public User CurrentUser { get; set; }
        public Parent CurrentParent { get; set; }
        public ObservableCollection<Child> Children { get; set; }


        public HomePage(UserService userService, ChildService childService, ParentService parentService)
        {
            InitializeComponent();

            _userService = userService;
            _childService = childService;
            _parentService = parentService;

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
            var users = await _userService.GetUsersAsync();
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
                CurrentUser = selectedUser;
                Children.Clear();
                var children = await _childService.GetChildrenByUserIdAsync(CurrentUser.Id);
                foreach (var child in children)
                {
                    Children.Add(child);
                }

                CurrentParent = await _parentService.GetParentByUserIdAsync(CurrentUser.Id);

                OnPropertyChanged(nameof(CurrentUser));
                OnPropertyChanged(nameof(CurrentParent));
            }
        }

        private async void OnViewChildDetailsClicked(object sender, EventArgs e)
        {
            var selectedChild = ChildPicker.SelectedItem as Child;
            if (selectedChild != null)
            {
                var child = await _childService.GetChildAsync(selectedChild.Id);
                if (child != null)
                {
                    await DisplayAlert("Child Details", $"Name: {child.Name}\nMoney: {child.Money}\nWeekly Earnings: {child.WeeklyEarnings}\nLifetime Earnings: {child.LifetimeEarnings}", "OK");
                }
            }
        }

        private async void OnDeleteChildClicked(object sender, EventArgs e)
        {
            var selectedChild = ChildPicker.SelectedItem as Child;
            if (selectedChild != null)
            {
                var confirm = await DisplayAlert("Confirm", $"Are you sure you want to delete {selectedChild.Name}?", "Yes", "No");
                if (confirm)
                {
                    await _childService.DeleteChildAsync(selectedChild);

                    // Update the UI
                    Children.Remove(selectedChild);
                    OnPropertyChanged(nameof(Children));

                    await DisplayAlert("Success", "Child deleted successfully.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "Please select a child to delete.", "OK");
            }
        }

        private async void OnAddChildClicked(object sender, EventArgs e)
        {
            if (CurrentUser == null) return;

            // Create a new child (for simplicity, hardcoding values here)
            var newChild = new Child
            {
                Name = "New Child",
                Image = "default_child_image.png",
                Money = 0,
                WeeklyEarnings = 0,
                LifetimeEarnings = 0,
                UserId = CurrentUser.Id
            };

            await _childService.SaveChildAsync(newChild);

            // Update the UI
            Children.Add(newChild);
            OnPropertyChanged(nameof(Children));
        }
    }
}
