using System.Windows;
using System.Windows.Controls;
using ToastNotifications.Messages;

namespace Vermintide_Analyzer.Controls
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        #region Binding
        public string PlayerName { get; set; } = Settings.Current.PlayerName;
        public bool WatermarkScreenshots { get; set; } = Settings.Current.WatermarkScreenshots;
        public bool ShowHealthWhenDowned { get; set; } = Settings.Current.ShowHealthWhenDowned;
        public bool ConfirmDeleteGames { get; set; } = Settings.Current.ConfirmDeleteGames;
        public bool IncludeCustomNoteInExport { get; set; } = Settings.Current.IncludeCustomNoteInExport;
        public bool AutoDeleteEmptyGames { get; set; } = Settings.Current.AutoDeleteEmptyGames;
        public bool AutoDeleteShortGames
        {
            get => mAutoDeleteShortGames;
            set
            {
                mAutoDeleteShortGames = value;
                AutoDeleteShortThresholdTextBox.GetBindingExpression(TextBox.IsEnabledProperty).UpdateTarget();
            }
        }
        private bool mAutoDeleteShortGames = Settings.Current.AutoDeleteShortGames;
        public uint AutoDeleteShortThreshold { get; set; } = Settings.Current.AutoDeleteShortThreshold;
        #endregion

        public SettingsView()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void SaveSettings()
        {
            Settings.Current.PlayerName = PlayerName;
            Settings.Current.WatermarkScreenshots = WatermarkScreenshots;
            Settings.Current.ShowHealthWhenDowned = ShowHealthWhenDowned;
            Settings.Current.ConfirmDeleteGames = ConfirmDeleteGames;
            Settings.Current.IncludeCustomNoteInExport = IncludeCustomNoteInExport;
            Settings.Current.AutoDeleteEmptyGames = AutoDeleteEmptyGames;
            Settings.Current.AutoDeleteShortGames = AutoDeleteShortGames;
            Settings.Current.AutoDeleteShortThreshold = AutoDeleteShortThreshold;

            bool success = Settings.Save();
            if (success)
            {
                MainWindow.Instance.ToastNotifier.ShowSuccess("Settings saved");
            }
            else
            {
                MainWindow.Instance.ToastNotifier.ShowError("Error saving settings");
            }
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }
    }
}
