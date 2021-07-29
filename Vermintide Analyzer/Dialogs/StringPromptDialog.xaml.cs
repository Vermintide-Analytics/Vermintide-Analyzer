using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Vermintide_Analyzer.Dialogs
{
    /// <summary>
    /// Interaction logic for StringPromptDialog.xaml
    /// </summary>
    public partial class StringPromptDialog : Window
    {
        public StringPromptDialog(string promptText, string prefillValue, string titleText = "")
        {
            InitializeComponent();
            PromptText = promptText;
            Title = titleText;
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
