using KidsChoresApp.Models;
using KidsChoresApp.Services;
using System.Collections.ObjectModel;


namespace KidsChoresApp.Pages
{
    public partial class HomePage : ContentPage
    {
        private readonly UserService _userService;

        public ObservableCollection<User> Users { get; set; }
        public User CurrentUser { get; set; }
        public ObservableCollection<Child> Children { get; set; }


        public HomePage(UserService userService)
        {
            InitializeComponent();

            _userService = userService;

            Users = new ObservableCollection<User>();

            Children = new ObservableCollection<Child>();

            BindingContext = this;

            Loaded += OnPageLoaded;
        }


        private async void OnPageLoaded(object? sender, EventArgs e)
        {
            //await LoadData();
            await LoadUsers();

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            //await LoadData();
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
            if (picker?.SelectedItem is User selectedUser)
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

        //private async Task LoadData()
        //{
        //    CurrentUser = await _userService.GetUserWithDetailsAsync(1); // Assuming user with ID 1
        //    if (CurrentUser != null)
        //    {
        //        Children.Clear();
        //        foreach (var child in CurrentUser.Children)
        //        {
        //            Children.Add(child);
        //        }
        //    }
        //    OnPropertyChanged(nameof(CurrentUser));
        //}

        private async void OnAddChildClicked(object sender, EventArgs e)
        {

        }

        private async void OnAssignChoresClicked(object sender, EventArgs e)
        {

        }
    }
}
