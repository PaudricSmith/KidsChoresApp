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

            BindingContext = this;

            LoadAvatars();
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

        private void OnAddAvatarTapped(object sender, EventArgs e)
        {
            CustomActionSheet.IsVisible = true;
        }

        private async void OnCapturePhotoTapped(object sender, EventArgs e)
        {
            CustomActionSheet.IsVisible = false;
            await CapturePhotoAsync();
        }

        private async void OnChooseFromLibraryTapped(object sender, EventArgs e)
        {
            CustomActionSheet.IsVisible = false;
            await PickPhotoAsync();
        }

        private void OnSelectFromAvatarsTapped(object sender, EventArgs e)
        {
            CustomActionSheet.IsVisible = false;
            AvatarSelectionOverlay.IsVisible = true;
        }

        private void OnCancelTapped(object sender, EventArgs e)
        {
            CustomActionSheet.IsVisible = false;
        }


        private async Task CapturePhotoAsync()
        {
            try
            {
                var result = await MediaPicker.CapturePhotoAsync();
                if (result != null)
                {
                    var stream = await result.OpenReadAsync();

                    var tempImagePath = await ImageHelper.SaveImageAsync(stream, Guid.NewGuid().ToString());

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

                    var tempImagePath = await ImageHelper.SaveImageAsync(stream, Guid.NewGuid().ToString());

                    _selectedImage = tempImagePath;
                    ChildImage.Source = ImageSource.FromFile(tempImagePath);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private void OnCloseAvatarSelectionTapped(object sender, EventArgs e)
        {
            AvatarSelectionOverlay.IsVisible = false;
        }

        private async void OnAddChildTapped(object sender, EventArgs e)
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
                Money = 0m,
                WeeklyAllowance = decimal.TryParse(WeeklyAllowanceEntry.Text, out var weeklyAllowance) ? weeklyAllowance : 0m,
                WeeklyEarnings = 0m,
                LifetimeEarnings = 0m
            };

            await _childService.SaveChildAsync(child);
            await DisplayAlert("Success", "Child added successfully.", "OK");

            await Shell.Current.GoToAsync($"///{nameof(HomePage)}");
        }
    }
}
