

namespace KidsChoresApp.Views
{
    public partial class KeypadView : ContentView
    {
        public event EventHandler<string> KeypadNumClicked;
        public event EventHandler BackspaceClicked;


        public KeypadView()
        {
            InitializeComponent();
        }


        private void OnKeypadNumClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                KeypadNumClicked?.Invoke(this, button.Text);
            }
        }

        private void OnBackspaceClicked(object sender, EventArgs e)
        {
            BackspaceClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
