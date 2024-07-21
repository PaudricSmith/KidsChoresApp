using KidsChoresApp.Services;
using KidsChoresApp.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace KidsChoresApp.Pages.ChildPages
{
    [QueryProperty(nameof(UserId), "userId")]
    public partial class AddChildPage : ContentPage
    {
        private readonly ChildService _childService;
        
        public int UserId { get; set; }

        public ObservableCollection<string> Avatars { get; set; } = new ObservableCollection<string>();

        public ICommand SelectAvatarCommand { get; }


        public AddChildPage(ChildService childService)
        {
            InitializeComponent();
            
            _childService = childService;

            SelectAvatarCommand = new Command<string>(OnAvatarSelected);

            LoadAvatars();

            BindingContext = this;
        }

        private void LoadAvatars()
        {
            // Add the known avatars to the Avatars collection
            var avatarFiles = new[]
            {
                "batboy.png",
                "batgirl.png",
                "flashboy.png",
                "flashgirl.png",
                "hulkboy.png",
                "hulkgirl.png",
                "superboy.png",
                "supergirl.png",
                "spiderboy.png",
                "spidergirl.png",
            };

            foreach (var avatar in avatarFiles)
            {
                Avatars.Add($"Resources/Images/Avatars/{avatar}");
            }
        }

        private void OnAvatarSelected(string selectedAvatar)
        {
            if (selectedAvatar != null)
            {
                SelectedAvatarImage.Source = selectedAvatar;
                AvatarSelectionOverlay.IsVisible = false;
            }
        }

        private async void OnAddAvatarClicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Choose an Avatar", "Cancel", null, "Choose from library", "Take a photo", "Select from avatars");
            switch (action)
            {
                case "Choose from library":
                    // Implement photo library selection
                    break;
                case "Take a photo":
                    // Implement photo capture
                    break;
                case "Select from avatars":
                    AvatarSelectionOverlay.IsVisible = true;
                    break;
            }
        }

        private void OnCloseAvatarSelectionClicked(object sender, EventArgs e)
        {
            AvatarSelectionOverlay.IsVisible = false;
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
                Image = SelectedAvatarImage.Source.ToString(),
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
