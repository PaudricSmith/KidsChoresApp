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
        private readonly UserService _userService;
        private readonly ChoreService _choreService;
        private readonly ChildService _childService;

        private ObservableCollection<Child> _children = new ObservableCollection<Child>();
        private ObservableCollection<string> _choreImages = new ObservableCollection<string>();
        private string? _selectedImage = string.Empty;
        private string? _preferredCurrency;
        private int _userId;
        private bool _assignAllChildren;
        private bool _selectAllDays;

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
            }
        }
        public string? PreferredCurrency
        {
            get => _preferredCurrency;
            set
            {
                _preferredCurrency = value;
                OnPropertyChanged();
            }
        }
        public bool AssignAllChildren
        {
            get => _assignAllChildren;
            set
            {
                if (_assignAllChildren != value)
                {
                    _assignAllChildren = value;
                    OnPropertyChanged();
                    UpdateChildrenSelection(_assignAllChildren);
                }
            }
        }
        public bool SelectAllDays
        {
            get => _selectAllDays;
            set
            {
                if (_selectAllDays != value)
                {
                    _selectAllDays = value;
                    OnPropertyChanged();
                    UpdateDaysSelection(_selectAllDays);
                }
            }
        }


        public ICommand SelectImageCommand { get; }


        public AddChoresPage(UserService userService, ChoreService choreService, ChildService childService)
        {
            InitializeComponent();

            _userService = userService;
            _choreService = choreService;
            _childService = childService;

            SelectImageCommand = new Command<string>(OnChoreImageSelected);

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

            var basePath = "Resources/Images/Chores/";
            var imagePaths = imageFiles.Select(image => $"{basePath}{image}").ToList();


            foreach (var imagePath in imagePaths)
            {
                _choreImages.Add(imagePath);
            }

        }

        private void OnChoreImageSelected(string selectedImage)
        {
            if (!string.IsNullOrEmpty(selectedImage))
            {
                ImageSelectionOverlay.IsVisible = false;
                ChoreImage.Source = selectedImage;
                _selectedImage = selectedImage;
            }
        }

        private void OnAddChoreImageClicked(object sender, EventArgs e)
        {
            if (ChoreImages.Count == 0)
                LoadChoreImages();

            ImageSelectionOverlay.IsVisible = !ImageSelectionOverlay.IsVisible;
        }

        private async void OnEnterChoreDetailsClicked(object sender, EventArgs e)
        {
            PreferredCurrency = await _userService.GetUserPreferredCurrency(UserId);

            ChoreDetailsSection.IsVisible = !ChoreDetailsSection.IsVisible;
        }

        private async void OnAssignChoreClicked(object sender, EventArgs e)
        {
            if (Children.Count == 0)
                await LoadChildren(UserId);

            ChildrenAssignmentSection.IsVisible = !ChildrenAssignmentSection.IsVisible;
        }

        private async Task LoadChildren(int userId)
        {
            var children = await _childService.GetChildrenByUserIdAsync(userId);

            _children.Clear();
            foreach (var child in children)
            {
                _children.Add(child);
            }
        }

        private void OnSelectDaysClicked(object sender, EventArgs e)
        {
            DaySelectionSection.IsVisible = !DaySelectionSection.IsVisible;
        }

        private async void OnSaveChoreClicked(object sender, EventArgs e)
        {
            if (IsInputInvalid(out decimal worth))
            {
                await DisplayAlert("Error", "Please fill all fields correctly with valid values.", "OK");
                return;
            }

            var selectedDays = GetSelectedDaysOfWeek();
            if (selectedDays.Count == 0)
            {
                await DisplayAlert("Error", "Please select at least one day for the chore.", "OK");
                return;
            }

            var selectedChildren = Children.Where(c => c.IsSelected).ToList();
            if (selectedChildren.Count == 0)
            {
                await DisplayAlert("Error", "Please select at least one child for the chore.", "OK");
                return;
            }

            var exceedingChildren = new List<string>();

            foreach (var selectedChild in selectedChildren)
            {
                if (await IsExceedingAllowance(selectedChild, worth, selectedDays.Count))
                {
                    var chores = await _choreService.GetChoresByChildIdAsync(selectedChild.Id);
                    var totalPotentialWeeklyEarnings = chores?.Sum(c => c.Worth) ?? 0m;
                    totalPotentialWeeklyEarnings += worth * selectedDays.Count;

                    exceedingChildren.Add($"You have reached {selectedChild.Name}'s weekly allowance limit of {selectedChild.WeeklyAllowance} as the chore worth total would be {totalPotentialWeeklyEarnings}.");
                }
            }

            if (exceedingChildren.Count > 0)
            {
                var errorMessage = string.Join("\n\n", exceedingChildren);
                await DisplayAlert("Error", errorMessage, "OK");
                return;
            }

            foreach (var selectedChild in selectedChildren)
            {
                await AddChoresForSelectedDays(selectedChild, worth, selectedDays);
            }

            // Reset the IsSelected property of each child
            foreach (var child in Children)
            {
                child.IsSelected = false;
            }

            // Prompt the user for next action
            var action = await DisplayActionSheet("", null, null, "Add Another Chore", "Go to Homepage");

            if (action == "Add Another Chore")
            {
                ResetForm();
            }
            else if (action == "Go to Homepage")
            {
                await Shell.Current.GoToAsync("..");
            }
        }

        private void ResetForm()
        {
            // Clear form fields and reset checkboxes, images, etc.
            ChoreNameEntry.Text = string.Empty;
            ChoreDescriptionEditor.Text = string.Empty;
            ChoreWorthEntry.Text = string.Empty;
            _selectedImage = string.Empty;
            ChoreImage.Source = null;

            AssignAllChildren = false;
            SelectAllDays = false;
            UpdateDaysSelection(false);

            // Hide sections
            ChoreDetailsSection.IsVisible = false;
            ChildrenAssignmentSection.IsVisible = false;
            DaySelectionSection.IsVisible = false;
        }

        private bool IsInputInvalid(out decimal worth)
        {
            worth = 0;
            return string.IsNullOrWhiteSpace(ChoreNameEntry.Text) ||
                   !decimal.TryParse(ChoreWorthEntry.Text, out worth) || worth < 0;
        }

        private async Task<bool> IsExceedingAllowance(Child selectedChild, decimal newChoreWorth, int selectedDaysCount)
        {
            var chores = await _choreService.GetChoresByChildIdAsync(selectedChild.Id);
            var totalPotentialWeeklyEarnings = chores?.Sum(c => c.Worth) ?? 0m;

            // Multiply newChoreWorth by the number of selected days
            totalPotentialWeeklyEarnings += newChoreWorth * selectedDaysCount;

            return totalPotentialWeeklyEarnings > selectedChild.WeeklyAllowance;
        }

        private async Task AddChoresForSelectedDays(Child selectedChild, decimal worth, List<DayOfWeek> selectedDays)
        {
            var tasks = selectedDays.Select(dayOfWeek => new Chore
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
            }).Select(_choreService.SaveChoreAsync);

            await Task.WhenAll(tasks);
        }

        private void UpdateDaysSelection(bool isSelected)
        {
            MondayCheckBox.IsChecked = isSelected;
            TuesdayCheckBox.IsChecked = isSelected;
            WednesdayCheckBox.IsChecked = isSelected;
            ThursdayCheckBox.IsChecked = isSelected;
            FridayCheckBox.IsChecked = isSelected;
            SaturdayCheckBox.IsChecked = isSelected;
            SundayCheckBox.IsChecked = isSelected;
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

        private void UpdateChildrenSelection(bool isSelected)
        {
            foreach (var child in Children)
            {
                child.IsSelected = isSelected;
            }
        }


        public new event PropertyChangedEventHandler? PropertyChanged;
        protected new void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
