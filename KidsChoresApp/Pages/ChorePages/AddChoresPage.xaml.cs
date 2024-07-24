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
        private int _userId;
        private ObservableCollection<Child> _children;
        private ObservableCollection<string> _choreImages;
        private string _selectedImage = "";

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

            _children = [];
            _choreImages = new ObservableCollection<string>();

            SelectImageCommand = new Command<string>(OnChoreImageSelected);

            LoadChoreImages();

            BindingContext = this;
        }


        private void LoadChoreImages()
        {
            var imageFiles = new[]
            {
                "brushteeth.png",
                "cleanbathroom.png",
                "cleankitchen.png",
                "cleanlivingroom.png",
                "cleanwindows.png",
                "cookfood.png",
                "eatallfood.png",
                "feedpets.png",
                "makebed.png",
                "mopfloor.png",
                "mowlawn.png",
                "sweepfloor.png",
                "takeouttrash.png",
                "tidybedroom.png",
                "vacuumfloor.png",
                "washcar.png",
                "washdishes.png",
                "washlaundry.png",
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
            if (string.IsNullOrWhiteSpace(ChoreNameEntry.Text) || ChildPicker.SelectedItem == null || string.IsNullOrWhiteSpace(ChoreWorthEntry.Text))
            {
                await DisplayAlert("Error", "Please fill all fields correctly.", "OK");
                return;
            }

            if (!decimal.TryParse(ChoreWorthEntry.Text, out var worth))
            {
                await DisplayAlert("Error", "Invalid worth value.", "OK");
                return;
            }

            var selectedChild = (Child)ChildPicker.SelectedItem;

            var chore = new Chore
            {
                Name = ChoreNameEntry.Text,
                Description = ChoreDescriptionEditor.Text,
                Worth = worth,
                Deadline = DeadlinePicker.Date,
                ChildId = selectedChild.Id,
                AssignedTo = selectedChild.Name,
                Image = _selectedImage
            };

            await _choreService.SaveChoreAsync(chore);

            await DisplayAlert("Success", "Chore added successfully.", "OK");

            await Shell.Current.GoToAsync("..");
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
 