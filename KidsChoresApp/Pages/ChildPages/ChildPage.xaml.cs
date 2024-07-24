using KidsChoresApp.Models;
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

        public ObservableCollection<string> Avatars { get; set; } = [];
        public ObservableCollection<Chore> Chores { get; set; } = [];

        public ICommand SelectAvatarCommand { get; }
        public ICommand ChoreCheckedChangedCommand { get; }

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
                LoadChildData(value);
            }
        }


        public ChildPage(ChildService childService, ChoreService choreService)
        {
            InitializeComponent();

            _childService = childService;
            _choreService = choreService;

            SelectAvatarCommand = new Command<string>(OnAvatarSelected);
            ChoreCheckedChangedCommand = new Command<Chore>(OnChoreCheckedChanged);

            LoadAvatars();

            BindingContext = this;
        }


        private async void LoadChildData(int childId)
        {
            Child = await _childService.GetChildAsync(childId);

            if (Child != null)
            {
                LoadChores(childId);
            }
        }

        private async void LoadChores(int childId)
        {
            var chores = await _choreService.GetChoresByChildIdAsync(childId);

            Chores.Clear();
            foreach (var chore in chores)
            {
                Chores.Add(chore);
            }
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

        private async void OnAvatarSelected(string selectedAvatar)
        {
            if (selectedAvatar != null && Child != null)
            {
                Child.Image = selectedAvatar;
                
                AvatarSelectionOverlay.IsVisible = false;
                
                await _childService.SaveChildAsync(Child);
            }
        }

        private void OnChangeAvatarClicked(object sender, EventArgs e)
        {
            CustomActionSheet.IsVisible = true;
        }

        private async void OnCapturePhotoClicked(object sender, EventArgs e)
        {
            CustomActionSheet.IsVisible = false;
            await CapturePhotoAsync();
        }

        private async void OnChooseFromLibraryClicked(object sender, EventArgs e)
        {
            CustomActionSheet.IsVisible = false;
            await PickPhotoAsync();
        }

        private void OnSelectFromAvatarsClicked(object sender, EventArgs e)
        {
            CustomActionSheet.IsVisible = false;
            AvatarSelectionOverlay.IsVisible = true;
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            CustomActionSheet.IsVisible = false;
        }

        private async void OnChoreCheckedChanged(Chore chore)
        {
            if (chore == null || Child == null) return;

            chore.IsComplete = !chore.IsComplete;

            if (chore.IsComplete)
            {
                Child.LifetimeEarnings += chore.Worth;
                Child.WeeklyEarnings += chore.Worth;
            }
            else
            {
                Child.LifetimeEarnings -= chore.Worth;
                Child.WeeklyEarnings -= chore.Worth;
            }

            await _choreService.SaveChoreAsync(chore);
            await _childService.SaveChildAsync(Child);
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


        private void OnCloseAvatarSelectionClicked(object sender, EventArgs e)
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
            {
                await _childService.SaveChildAsync(Child);
            }

            NameLabel.IsVisible = true;
            NameEntry.IsVisible = false;
        }
    }
}
