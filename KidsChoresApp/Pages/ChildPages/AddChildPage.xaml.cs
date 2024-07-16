using KidsChoresApp.Services;
using KidsChoresApp.Models;


namespace KidsChoresApp.Pages.ChildPages
{
    [QueryProperty(nameof(UserId), "userId")]
    public partial class AddChildPage : ContentPage
    {
        private readonly ChildService _childService;
        
        public int UserId { get; set; }

        
        public AddChildPage(ChildService childService)
        {
            InitializeComponent();
            _childService = childService;
        }


        private async void OnAddChildClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
                string.IsNullOrWhiteSpace(PasscodeEntry.Text) ||
                string.IsNullOrWhiteSpace(WeeklyAllowanceEntry.Text) ||
                PasscodeEntry.Text != VerifyPasscodeEntry.Text)
            {
                await DisplayAlert("Error", "Please fill all fields correctly.", "OK");
                return;
            }

            var child = new Child
            {
                UserId = UserId,
                Name = NameEntry.Text,
                Image = ImageEntry.Text,
                Passcode = PasscodeEntry.Text,
                Money = 20,
                WeeklyEarnings = decimal.TryParse(WeeklyAllowanceEntry.Text, out var weeklyAllowance) ? weeklyAllowance : 0,
                LifetimeEarnings = 20
            };

            await _childService.SaveChildAsync(child);
            await DisplayAlert("Success", "Child added successfully.", "OK");

            await Shell.Current.GoToAsync("..");
        }
    }
}
