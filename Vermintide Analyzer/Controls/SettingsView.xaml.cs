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
        public bool ConfirmDeleteGames { get; set; } = Settings.Current.ConfirmDeleteGames;
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
            Settings.Current.ConfirmDeleteGames = ConfirmDeleteGames;

            Settings.Save();
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }
    }
}
