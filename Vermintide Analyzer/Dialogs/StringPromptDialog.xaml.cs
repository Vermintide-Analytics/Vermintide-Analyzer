using System.Windows;
using System.Windows.Input;

namespace Vermintide_Analyzer.Dialogs
{
    /// <summary>
    /// Interaction logic for StringPromptDialog.xaml
    /// </summary>
    public partial class StringPromptDialog : Window
    {
        public StringPromptDialog(Window owner, string promptText, string prefillValue)
        {
            Owner = owner;
            InitializeComponent();
            PromptText = promptText;
            ResponseText = prefillValue;
            ResponseTextBox.CaretIndex = int.MaxValue;
            ResponseTextBox.Focus();
        }

        public string PromptText
        {
            get => PromptLabel.Text;
            set { PromptLabel.Text = value; }
        }

        public string ResponseText
        {
            get => ResponseTextBox.Text;
            set { ResponseTextBox.Text = value; }
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ResponseText = "";
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if(e.Key == Key.Enter)
            {
                DialogResult = true;
            }
            else if(e.Key == Key.Escape)
            {
                DialogResult = false;
            }
        }
    }
}
