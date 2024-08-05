

namespace KidsChoresApp.Views
{
    public partial class CustomActionSheet : ContentView
    {
        public event EventHandler? CapturePhotoTapped;
        public event EventHandler? ChooseFromLibraryTapped;
        public event EventHandler? SelectFromAvatarsTapped;
        public event EventHandler? CancelTapped;


        public CustomActionSheet()
        {
            InitializeComponent();
        }


        private void OnCapturePhotoTapped(object sender, EventArgs e) => CapturePhotoTapped?.Invoke(this, e);
        private void OnChooseFromLibraryTapped(object sender, EventArgs e) => ChooseFromLibraryTapped?.Invoke(this, e);
        private void OnSelectFromAvatarsTapped(object sender, EventArgs e) => SelectFromAvatarsTapped?.Invoke(this, e);
        private void OnCancelTapped(object sender, EventArgs e) => CancelTapped?.Invoke(this, e);
    }
}
