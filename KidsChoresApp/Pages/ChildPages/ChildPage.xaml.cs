using KidsChoresApp.Models;
using KidsChoresApp.Pages.ChorePages;
using KidsChoresApp.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace KidsChoresApp.Pages.ChildPages
{
    [QueryProperty(nameof(ChildId), "childId")]
    public partial class ChildPage : ContentPage
    {
        private readonly ChildService _childService;
        private readonly ChoreService _choreService;

        private Child? _child;
        private int _childId;

        public ObservableCollection<string> Avatars { get; set; } = new ObservableCollection<string>();

        public Child? Child
        {
            get => _child;
            set
            {
                _child = value;
                OnPropertyChanged();
            }
        }
        public int ChildId
        {
            get => _childId;
            set
            {
                _childId = value;
                OnPropertyChanged();
            }
        }

        public ICommand? SelectAvatarCommand { private set; get; }


        public ChildPage(ChildService childService, ChoreService choreService)
        {
            InitializeComponent();

            _childService = childService;
            _choreService = choreService;

            SelectAvatarCommand = new Command<string>(async (avatar) => await OnAvatarSelected(avatar));

            BindingContext = this;
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await LoadData();
        }

        private async Task LoadData()
        {
            if (ChildId != 0)
            {
                Child = await _childService.GetChildByIdAsync(ChildId);
            }
        }

        private void LoadAvatars()
        {
            // Add the known avatars to the Avatars collection
            var avatarFiles = new[]
            {
                "batboy.png", "batgirl.png", "flashboy.png", "flashgirl.png", "hulkboy.png",
                "hulkgirl.png", "superboy.png", "supergirl.png", "spiderboy.png", "spidergirl.png",
            };

            foreach (var avatar in avatarFiles)
            {
                Avatars.Add($"Resources/Images/Avatars/{avatar}");
            }
        }

        private async void OnViewChildDetailsTapped(object sender, EventArgs e)
        {
            if (Child != null)
            {
                OnPropertyChanged(nameof(Child));

                await DisplayAlert("Child Details",
                    $"User Id: {Child.UserId}\n" +
                    $"Id: {Child.Id}\n" +
                    $"Name: {Child.Name}\n" +
                    $"Weekly Allowance: {Child.WeeklyAllowance}\n" +
                    $"Lifetime Earnings: {Child.LifetimeEarnings}\n" +
                    $"Weekly Earnings: {Child.WeeklyEarnings}\n" +
                    $"Last Week Reset: {Child.LastWeekReset}\n" +
                    $"Passcode: {Child.Passcode}\n",
                    "OK");
            }
        }

        private async void OnViewChoresTapped(object sender, EventArgs e)
        {
            if (Child != null)
                await Shell.Current.GoToAsync($"{nameof(ChoresPage)}?childId={ChildId}");
        }

        private async Task OnAvatarSelected(string selectedAvatar)
        {
            if (selectedAvatar != null && Child != null)
            {
                Child.Image = selectedAvatar;

                AvatarSelectionOverlay.IsVisible = false;

                await _childService.SaveChildAsync(Child);
            }
        }

        private void OnChangeAvatarTapped(object sender, EventArgs e)
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

            if (Avatars.Count == 0)
                LoadAvatars();
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

                    // Use a Guid for the image name
                    var imagePath = await ImageHelper.SaveImageAsync(stream, Guid.NewGuid().ToString());

                    // Delete the old image if it exists
                    if (!string.IsNullOrEmpty(Child.Image) && File.Exists(Child.Image))
                    {
                        ImageHelper.DeleteImage(Child.Image);
                    }

                    if (imagePath != null)
                    {
                        Child.Image = imagePath;
                        await _childService.SaveChildAsync(Child);
                        ChildImage.Source = ImageSource.FromFile(imagePath);
                    }
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
                    var imagePath = await ImageHelper.SaveImageAsync(stream, Guid.NewGuid().ToString());

                    // Delete the old image if it exists
                    if (!string.IsNullOrEmpty(Child.Image) && File.Exists(Child.Image))
                    {
                        ImageHelper.DeleteImage(Child.Image);
                    }

                    if (imagePath != null)
                    {
                        Child.Image = imagePath;
                        await _childService.SaveChildAsync(Child);
                        ChildImage.Source = ImageSource.FromFile(imagePath);
                    }
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

        private void OnNameFrameTapped(object sender, EventArgs e)
        {
            NameLabel.IsVisible = false;
            NameEntry.IsVisible = true;
            NameEntry.Focus();
        }

        private async void OnNameEntryCompleted(object sender, EventArgs e)
        {
            if (Child != null)
                await _childService.SaveChildAsync(Child);

            NameLabel.IsVisible = true;
            NameEntry.IsVisible = false;
        }
    }
}
