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
        private string _selectedImage = "";
        private int _userId;

        public ObservableCollection<string> Avatars { get; set; } = [];

        public int UserId
        {
            get => _userId;
            set
            {
                _userId = value;
            }
        }

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
                AvatarSelectionOverlay.IsVisible = false;

                ChildImage.Source = selectedAvatar;
                _selectedImage = selectedAvatar;
            }
        }

        private async void OnAddAvatarClicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Choose an Avatar", "Cancel", null, "Take a photo", "Choose from library", "Select from avatars");
            switch (action)
            {
                case "Take a photo":
                    await CapturePhotoAsync();
                    break;
                case "Choose from library":
                    await PickPhotoAsync();
                    break;
                case "Select from avatars":
                    AvatarSelectionOverlay.IsVisible = true;
                    break;
            }
        }

        private async Task CapturePhotoAsync()
        {
            try
            {
                var result = await MediaPicker.CapturePhotoAsync();
                if (result != null)
                {
                    var stream = await result.OpenReadAsync();

                    // Use a Guid for the image name
                    var tempImagePath = await ImageHelper.SaveImageAsync(stream, Guid.NewGuid().ToString());

                    // Set the selected image
                    _selectedImage = tempImagePath;
                    ChildImage.Source = ImageSource.FromFile(tempImagePath);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async Task PickPhotoAsync()
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync();
                if (result != null)
                {
                    var stream = await result.OpenReadAsync();

                    // Use a Guid for the image name
                    var tempImagePath = await ImageHelper.SaveImageAsync(stream, Guid.NewGuid().ToString());

                    // Set the selected image
                    _selectedImage = tempImagePath;
                    ChildImage.Source = ImageSource.FromFile(tempImagePath);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
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

            var childInDatabase = await _childService.ChildExistsAsync(NameEntry.Text);
            if (await _childService.ChildExistsAsync(NameEntry.Text)) 
            {
                await DisplayAlert("Error", "Child already exists!", "OK");
                return;
            }

            var child = new Child
            {
                UserId = UserId,
                Name = NameEntry.Text,
                Image = _selectedImage,
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
