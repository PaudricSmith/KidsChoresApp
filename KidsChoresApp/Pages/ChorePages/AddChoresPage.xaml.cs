using KidsChoresApp.Models;
using KidsChoresApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace KidsChoresApp.Pages.ChorePages
{
    [QueryProperty(nameof(UserId), "userId")]
    public partial class AddChoresPage : ContentPage, INotifyPropertyChanged
    {
        private readonly ChoreService _choreService;
        private readonly ChildService _childService;
        
        private ObservableCollection<Child> _children = [];
        private ObservableCollection<string> _choreImages = [];
        private string _selectedImage = "";
        private int _userId;

        public ObservableCollection<Child> Children
        {
            get => _children;
            set
            {
                _children = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> ChoreImages
        {
            get => _choreImages;
            set
            {
                _choreImages = value;
                OnPropertyChanged();
            }
        }

        public int UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                LoadChildren(value);
            }
        }

        public ICommand SelectImageCommand { get; }


        public AddChoresPage(ChoreService choreService, ChildService childService)
        {
            InitializeComponent();

            _choreService = choreService;
            _childService = childService;

            SelectImageCommand = new Command<string>(OnChoreImageSelected);

            LoadChoreImages();

            BindingContext = this;
        }


        private void LoadChoreImages()
        {
            var imageFiles = new[]
            {
                "brushteeth.png", "cleanbathroom.png", "cleankitchen.png", "cleanlivingroom.png", "cleanwindows.png", 
                "cookfood.png", "eatallfood.png", "feedpets.png", "makebed.png", "mopfloor.png", 
                "mowlawn.png", "sweepfloor.png", "takeouttrash.png", "tidybedroom.png", "vacuumfloor.png", 
                "washcar.png", "washdishes.png", "washlaundry.png",
            };

            foreach (var image in imageFiles)
            {
                ChoreImages.Add($"Resources/Images/Chores/{image}");
            }
        }

        private void OnChoreImageSelected(string selectedImage)
        {
            if (selectedImage != null)
            {
                ImageSelectionOverlay.IsVisible = false;
                ChoreImage.Source = selectedImage;
                _selectedImage = selectedImage;
            }
        }

        private void OnAddChoreImageClicked(object sender, EventArgs e)
        {
            ImageSelectionOverlay.IsVisible = true;
        }

        private void OnCloseImageSelectionClicked(object sender, EventArgs e)
        {
            ImageSelectionOverlay.IsVisible = false;
        }

        private async void LoadChildren(int userId)
        {
            var children = await _childService.GetChildrenByUserIdAsync(userId);
            Children.Clear();
            foreach (var child in children)
            {
                Children.Add(child);
            }
        }

        private async void OnSaveChoreClicked(object sender, EventArgs e)
        {
            if (IsInputInvalid(out decimal worth))
            {
                await DisplayAlert("Error", "Please fill all fields correctly with valid values.", "OK");
                return;
            }

            var selectedChild = (Child)ChildPicker.SelectedItem;

            if (await IsExceedingAllowance(selectedChild, worth))
            {
                await DisplayAlert("Error", $"You have reached this child's weekly allowance limit of {selectedChild.WeeklyAllowance}. " +
                    "Either reduce this chore's worth or increase the child's weekly allowance budget.", "OK");
                return;
            }

            await AddChoresForSelectedDays(selectedChild, worth);

            await DisplayAlert("Success", "Chore added successfully.", "OK");

            await Shell.Current.GoToAsync("..");
        }

        private bool IsInputInvalid(out decimal worth)
        {
            worth = 0;
            return string.IsNullOrWhiteSpace(ChoreNameEntry.Text) ||
                   ChildPicker.SelectedItem == null ||
                   !decimal.TryParse(ChoreWorthEntry.Text, out worth) || worth < 0;
        }

        private async Task<bool> IsExceedingAllowance(Child selectedChild, decimal newChoreWorth)
        {
            var chores = await _choreService.GetChoresByChildIdAsync(selectedChild.Id);
            var totalPotentialWeeklyEarnings = chores?.Sum(c => c.Worth) ?? 0m;
            totalPotentialWeeklyEarnings += newChoreWorth;

            return totalPotentialWeeklyEarnings > selectedChild.WeeklyAllowance;
        }

        private async Task AddChoresForSelectedDays(Child selectedChild, decimal worth)
        {
            foreach (var dayOfWeek in GetSelectedDaysOfWeek())
            {
                var chore = new Chore
                {
                    Name = ChoreNameEntry.Text,
                    Description = ChoreDescriptionEditor.Text,
                    Worth = worth,
                    Deadline = DateTime.Now,
                    ChildId = selectedChild.Id,
                    AssignedTo = selectedChild.Name,
                    Image = _selectedImage,
                    DayOfWeek = dayOfWeek,
                    IsComplete = false,
                    IsDetailsVisible = false,
                    Priority = 0
                };

                await _choreService.SaveChoreAsync(chore);
            }
        }

        private List<DayOfWeek> GetSelectedDaysOfWeek()
        {
            var selectedDays = new List<DayOfWeek>();

            if (MondayCheckBox.IsChecked) 
                selectedDays.Add(DayOfWeek.Monday);
            if (TuesdayCheckBox.IsChecked)
                selectedDays.Add(DayOfWeek.Tuesday);
            if (WednesdayCheckBox.IsChecked)
                selectedDays.Add(DayOfWeek.Wednesday);
            if (ThursdayCheckBox.IsChecked) 
                selectedDays.Add(DayOfWeek.Thursday);
            if (FridayCheckBox.IsChecked) 
                selectedDays.Add(DayOfWeek.Friday);
            if (SaturdayCheckBox.IsChecked) 
                selectedDays.Add(DayOfWeek.Saturday);
            if (SundayCheckBox.IsChecked) 
                selectedDays.Add(DayOfWeek.Sunday);

            return selectedDays;
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
 