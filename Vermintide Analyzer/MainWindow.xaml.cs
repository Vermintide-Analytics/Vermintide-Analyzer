using System.Windows;
using ToastNotifications;
using Vermintide_Analyzer.Misc;
using ToastNotifications.Messages;

namespace Vermintide_Analyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }

        #region Toast
        public Notifier ToastNotifier { get; set; }
        #endregion

        public MainWindow()
        {
            Instance = this;
            ToastNotifier = Toast.MakeNotifier(this);

            InitializeComponent();
            DataContext = this;

            Navigation.ContentPane = ContentPane;
            Navigation.NavigateTo(NavPage.Dashboard);
        }

        public void ShowError(string msg) => ToastNotifier?.ShowError(msg);
        public void ShowInformation(string msg) => ToastNotifier?.ShowInformation(msg);
        public void ShowSuccess(string msg) => ToastNotifier?.ShowSuccess(msg);
    }
}
