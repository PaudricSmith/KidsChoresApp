

namespace KidsChoresApp.Views
{
    public partial class CustomActionSheet : ContentView
    {
        public event EventHandler CapturePhotoClicked;
        public event EventHandler ChooseFromLibraryClicked;
        public event EventHandler SelectFromAvatarsClicked;
        public event EventHandler CancelClicked;


        public CustomActionSheet()
        {
            InitializeComponent();
        }


        private void OnCapturePhotoClicked(object sender, EventArgs e) => CapturePhotoClicked?.Invoke(this, e);
        private void OnChooseFromLibraryClicked(object sender, EventArgs e) => ChooseFromLibraryClicked?.Invoke(this, e);
        private void OnSelectFromAvatarsClicked(object sender, EventArgs e) => SelectFromAvatarsClicked?.Invoke(this, e);
        private void OnCancelClicked(object sender, EventArgs e) => CancelClicked?.Invoke(this, e);
    }
}
