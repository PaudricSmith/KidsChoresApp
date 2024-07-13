using KidsChoresApp.Models;
using System.Collections.ObjectModel;


namespace KidsChoresApp.Pages
{
    public partial class HomePage : ContentPage
    {
        public ObservableCollection<Child> Children { get; set; }


        public HomePage()
        {
            InitializeComponent();

            Children = new ObservableCollection<Child>();

            BindingContext = this;

            Loaded += OnPageLoaded;
        }


        private async void OnPageLoaded(object? sender, EventArgs e)
        {
            await LoadData();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadData();
        }

        private async Task LoadData()
        {
            // Simulate loading data
            await Task.Delay(1000);

            var children = new ObservableCollection<Child>
            {
                new()
                {
                    Id = 1,
                    Name = "John Doe",
                    Money = 10.5m,
                    WeeklyEarnings = 5.0m,
                    LifetimeEarnings = 50.0m,
                    Image = "child1.png" // Ensure you have this image in your resources
                },
                new()
                {
                    Id = 2,
                    Name = "Jane Doe",
                    Money = 15.75m,
                    WeeklyEarnings = 7.0m,
                    LifetimeEarnings = 70.0m,
                    Image = "child2.png" // Ensure you have this image in your resources
                },
                new()
                {
                    Id = 2,
                    Name = "Tom Doe",
                    Money = 15.75m,
                    WeeklyEarnings = 7.0m,
                    LifetimeEarnings = 70.0m,
                    Image = "child3.png" // Ensure you have this image in your resources
                },
                new()
                {
                    Id = 2,
                    Name = "Tanya Doe",
                    Money = 15.75m,
                    WeeklyEarnings = 7.0m,
                    LifetimeEarnings = 70.0m,
                    Image = "child4.png" // Ensure you have this image in your resources
                },
                new()
                {
                    Id = 2,
                    Name = "Mick Doe",
                    Money = 15.75m,
                    WeeklyEarnings = 7.0m,
                    LifetimeEarnings = 70.0m,
                    Image = "child5.png" // Ensure you have this image in your resources
                }
            };

            Children.Clear();
            foreach (var child in children)
            {
                Children.Add(child);
            }
        }

        private async void OnAddChildClicked(object sender, EventArgs e)
        {

        }

        private async void OnAssignChoresClicked(object sender, EventArgs e)
        {

        }
    }
}
