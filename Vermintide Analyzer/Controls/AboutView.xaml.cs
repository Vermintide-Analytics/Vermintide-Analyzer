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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Mail;
using System.Net;
using VA.LogReader;
using System.Reflection;

namespace Vermintide_Analyzer.Controls
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : UserControl, IAnalyticsPage
    {
        public string EmailAddress => "VermintideAnalytics@gmail.com";
        public string DiscordID => "Prismism#5276";
        public string AnalyzerVersion =>
            $"{AssemblyVersion.Major}.{AssemblyVersion.Minor}.{AssemblyVersion.Build}";
        private Version AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version;
        public string SchemaVersion => Schema.SCHEMA_VERSION;

        public AboutView()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void OnNavigatedTo() { }

        private void Copy_Email_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(EmailAddress);
            EmailCopiedText.Visibility = Visibility.Visible;
            Task.Delay(3000).ContinueWithSafe((_) => { EmailCopiedText.Visibility = Visibility.Hidden; });
        }
        private void Copy_DiscordID_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(DiscordID);
            DiscordIDCopiedText.Visibility = Visibility.Visible;
            Task.Delay(3000).ContinueWithSafe((_) => { DiscordIDCopiedText.Visibility = Visibility.Hidden; });
        }

    }
}
