using KidsChoresApp.Models;
using KidsChoresApp.Pages.ChildPages;
using KidsChoresApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace KidsChoresApp.Pages
{
    public partial class HomePage : ContentPage, INotifyPropertyChanged
    {
        private readonly UserService _userService;
        private readonly ChildService _childService;
        private readonly ParentService _parentService;

        private User _currentUser;
        private Parent _currentParent;
        private ObservableCollection<Child> _children;

        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }
        public Parent CurrentParent
        {
            get => _currentParent;
            set
            {
                _currentParent = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Child> Children
        {
            get => _children;
            set
            {
                _children = value;
                OnPropertyChanged();
            }
        }

        public HomePage(UserService userService, ChildService childService, ParentService parentService)
        {
            InitializeComponent();

            _userService = userService;
            _childService = childService;
            _parentService = parentService;

            Children = new ObservableCollection<Child>();

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

        private async void OnViewChildDetailsClicked(object sender, EventArgs e)
        {
            var selectedChild = ChildPicker.SelectedItem as Child;
            if (selectedChild != null)
            {
                var child = await _childService.GetChildAsync(selectedChild.Id);
                if (child != null)
                {
                    await DisplayAlert("Child Details", 
                        $"Name: {child.Name}\n" +
                        $"Money: {child.Money}\n" +
                        $"Weekly Earnings: {child.WeeklyEarnings}\n" +
                        $"Lifetime Earnings: {child.LifetimeEarnings}", 
                        "OK");
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

            await Shell.Current.GoToAsync($"{nameof(AddChildPage)}?userId={CurrentUser.Id}");
        }



        public new event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
