using KidsChoresApp.Models;
using KidsChoresApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace KidsChoresApp.Pages.ChildPages
{
    [QueryProperty(nameof(ChildId), "childId")]
    public partial class ChildPage : ContentPage
    {
        private readonly ChildService _childService;
        private readonly ChoreService _choreService;

        private Child? _child;
        private DateTime _selectedDate;
        private Frame _previouslySelectedFrame;
        private string _choresForText;
        private int _childId;

        public ObservableCollection<string> Avatars { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<Chore> Chores { get; set; } = new ObservableCollection<Chore>();
        public ObservableCollection<Chore> FilteredChores { get; set; } = new ObservableCollection<Chore>();
        public ObservableCollection<DateTime> WeekDates { get; set; } = new ObservableCollection<DateTime>();


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
            }
        }
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
            }
        }
        public string ChoresForText
        {
            get => _choresForText;
            set
            {
                _choresForText = value;
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

            Loaded += ChildPage_Loaded;
        }


        private async void ChildPage_Loaded(object? sender, EventArgs e)
        {
            await LoadChildData();
            GenerateWeekDates();
        }

        private async Task LoadChildData()
        {
            Child = await _childService.GetChildAsync(ChildId);
        }

        private async Task LoadChores()
        {
            if (Child != null)
            {
                var chores = await _choreService.GetChoresByChildIdAsync(Child.Id);

                Chores.Clear();
                foreach (var chore in chores)
                {
                    Chores.Add(chore);
                }
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

        private void GenerateWeekDates()
        {
            var today = DateTime.Today;

            // Adjust the day value to treat Monday as 0 and Sunday as 6
            int dayValue = today.DayOfWeek == 0 ? 6 : (int)today.DayOfWeek - 1;

            // Calculate the start of the week
            var startOfWeek = today.AddDays(-dayValue);

            WeekDates.Clear();
            for (int i = 0; i < 7; i++)
            {
                WeekDates.Add(startOfWeek.AddDays(i));
            }

            SelectedDate = today;
        }

        private void FilterChoresByDate()
        {
            FilteredChores.Clear();

            var dayOfWeek = SelectedDate.DayOfWeek;
            foreach (var chore in Chores)
            {
                if (chore.DayOfWeek == dayOfWeek)
                {
                    FilteredChores.Add(chore);
                }
            }
        }

        private void UpdateChoresForText()
        {
            var today = DateTime.Today;
            if (SelectedDate.Date == today)
            {
                ChoresForText = "Chores for Today";
            }
            else if (SelectedDate.Date == today.AddDays(1))
            {
                ChoresForText = "Chores for Tomorrow";
            }
            else if (SelectedDate.Date == today.AddDays(-1))
            {
                ChoresForText = "Chores for Yesterday";
            }
            else
            {
                ChoresForText = $"Chores for {SelectedDate:dddd}";
            }
        }

        private async void OnViewChoresTapped(object sender, EventArgs e)
        {
            if (Chores.Count == 0)
                await LoadChores();

            MoneySummary.IsVisible = !MoneySummary.IsVisible;
            WeekDaysCollectionView.IsVisible = !WeekDaysCollectionView.IsVisible;
            ChoresForTextLabel.IsVisible = !ChoresForTextLabel.IsVisible;
            ChoresCollectionView.IsVisible = !ChoresCollectionView.IsVisible;
        }

        private async void OnDaySelectionTapped(object sender, TappedEventArgs e)
        {
            if (sender is Frame frame && frame.BindingContext is DateTime date)
            {
                if (Chores.Count == 0)
                    await LoadChores();

                if (_previouslySelectedFrame != null)
                    _previouslySelectedFrame.BackgroundColor = Colors.CornflowerBlue;

                SelectedDate = date;
                frame.BackgroundColor = Colors.DarkSlateBlue;
                _previouslySelectedFrame = frame;

                // Scroll to the selected day
                var itemIndex = WeekDates.IndexOf(date);
                WeekDaysCollectionView.ScrollTo(itemIndex, position: ScrollToPosition.Center, animate: true);

                FilterChoresByDate();
                UpdateChoresForText();
            }
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

        private async void OnChoreCompleteButtonTapped(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Chore chore)
            {
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
        }

        private void OnExpandButtonTapped(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Chore chore)
            {
                chore.IsDetailsVisible = !chore.IsDetailsVisible;
            }
        }

        private async void OnDeleteChoreButtonTapped(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Chore chore)
            {
                bool isConfirmed = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this chore?", "Yes", "No");
                if (isConfirmed)
                {
                    Chores.Remove(chore);
                    FilteredChores.Remove(chore);
                    await _choreService.DeleteChoreAsync(chore);

                    if (chore.IsComplete)
                    {
                        Child.LifetimeEarnings -= chore.Worth;
                        Child.WeeklyEarnings -= chore.Worth;
                    }
                   
                    await _childService.SaveChildAsync(Child);
                }
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
            {
                await _childService.SaveChildAsync(Child);
            }

            NameLabel.IsVisible = true;
            NameEntry.IsVisible = false;
        }
    }
}
