using KidsChoresApp.Models;
using KidsChoresApp.Pages.ChildPages;
using KidsChoresApp.Pages.ChorePages;
using KidsChoresApp.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace KidsChoresApp.Pages
{
    public partial class HomePage : ContentPage
    {
        private readonly UserService _userService;
        private readonly ChildService _childService;
        private readonly ParentService _parentService;

        public User CurrentUser { get; set; }
        public Parent CurrentParent { get; set; }
        public ObservableCollection<Child> Children { get; set; } = [];

        public ICommand OnChildTappedCommand { get; }


        public HomePage(UserService userService, ChildService childService, ParentService parentService)
        {
            InitializeComponent();

            _userService = userService;
            _childService = childService;
            _parentService = parentService;

            OnChildTappedCommand = new Command<Child>(async (child) => await OnChildTapped(child));

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


        private async Task OnChildTapped(Child child)
        {
            if (child != null)
            {
                await Shell.Current.GoToAsync($"{nameof(ChildPage)}?childId={child.Id}");
            }
        }

        private async void OnViewChildDetailsClicked(object sender, EventArgs e)
        {
            if (ChildPicker.SelectedItem is Child selectedChild)
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

        private async void OnAddChildClicked(object sender, EventArgs e)
        {
            if (CurrentUser == null) return;

            await Shell.Current.GoToAsync($"{nameof(AddChildPage)}?userId={CurrentUser.Id}");
        }

        private async void OnAddChoresClicked(object sender, EventArgs e)
        {
            if (CurrentUser == null) return;

            await Shell.Current.GoToAsync($"{nameof(AddChoresPage)}?userId={CurrentUser.Id}");
        }

        private async void OnDeleteChildClicked(object sender, EventArgs e)
        {
            if (ChildPicker.SelectedItem is Child selectedChild)
            {
                var confirm = await DisplayAlert("Confirm", $"Are you sure you want to delete {selectedChild.Name}?", "Yes", "No");
                if (confirm)
                {
                    await _childService.DeleteChildAsync(selectedChild);

                    Children.Remove(selectedChild);

                    await DisplayAlert("Success", "Child deleted successfully.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "Please select a child to delete.", "OK");
            }
        }
    }
}
