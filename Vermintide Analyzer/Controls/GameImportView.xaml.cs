using Microsoft.Win32;
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
using ToastNotifications.Messages;
using VA.LogReader;
using Vermintide_Analyzer.Models;
using Vermintide_Analyzer.Statistics;

namespace Vermintide_Analyzer.Controls
{
    /// <summary>
    /// Interaction logic for GameCompareView.xaml
    /// </summary>
    public partial class GameImportView : UserControl, IAnalyticsPage
    {
        public List<ImportedGameItem> GamesToImport { get; set; } = new List<ImportedGameItem>();

        public bool RaiseWarning { get; set; } = false;
        public string Warning { get; set; } = "";

        public GameImportView()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void RefreshDisplay()
        {
            Validate();
            GamesToImportListView.Items.Refresh();
            WarningIndicator.GetBindingExpression(TextBlock.VisibilityProperty).UpdateTarget();
            WarningIndicator.GetBindingExpression(TextBlock.ToolTipProperty).UpdateTarget();
        }

        public void OnNavigatedTo()
        {
            RefreshDisplay();
        }

        private void Validate()
        {
            RaiseWarning = false;
            Warning = "";

            if(!GamesToImport.Any())
            {
                return;
            }

            bool missionsDiffer = false;
            bool duplicatePlayerName = false;

            MISSION firstMission = GamesToImport.First().Mission;
            Dictionary<string, int> nameCount = new Dictionary<string, int>();

            foreach (var game in GamesToImport)
            {
                if(game.Mission != firstMission)
                {
                    missionsDiffer = true;
                }

                if(game.PlayerName != ImportedGameItem.UnknownPlayerName)
                {
                    if(nameCount.ContainsKey(game.PlayerName))
                    {
                        nameCount[game.PlayerName] = nameCount[game.PlayerName] + 1;
                    }
                    else
                    {
                        nameCount.Add(game.PlayerName, 1);
                    }
                }
            }

            foreach(var kvp in nameCount)
            {
                if(kvp.Value > 1)
                {
                    duplicatePlayerName = true;
                }
            }

            List<string> warnings = new List<string>();

            if(missionsDiffer)
            {
                warnings.Add("Data is not all from the same game");
            }
            if(duplicatePlayerName)
            {
                warnings.Add("Multiple data from a single player");
            }

            if(warnings.Any())
            {
                RaiseWarning = true;
                Warning = string.Join("; ", warnings);
            }
        }

        private void Browse_Button_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog()
            {
                Filter = $"Vermintide Data files|*.{ImportedGameItem.EXPORT_EXTENSION}",
                Multiselect = true,
            };

            if (dlg.ShowDialog() == true)
            {
                if (!dlg.FileNames.Any())
                {
                    MainWindow.Instance.ToastNotifier.ShowError($"No .{ImportedGameItem.EXPORT_EXTENSION} files selected");
                    return;
                }

                Import_Files(dlg.FileNames);
            }

            RefreshDisplay();
        }

        private void File_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                Import_Files(files);
            }

            RefreshDisplay();
        }

        private void Import_Files(IEnumerable<string> fileNames)
        {
            var successfullyImported = 0;
            foreach(var fileName in fileNames)
            {
                if(!fileName.EndsWith($".{ImportedGameItem.EXPORT_EXTENSION}"))
                {
                    // error
                    MainWindow.Instance.ToastNotifier.ShowError($"Failed to import {fileName} (Incorrect file type)");
                    continue;
                }

                var imported = ImportedGameItem.ImportFromZip(fileName, out string failReason);

                if(imported is null)
                {
                    // error
                    MainWindow.Instance.ToastNotifier.ShowError($"Failed to import {fileName} ({(failReason is null ? "Unknown reason" : failReason)})");
                    continue;
                }
                else
                {
                    GamesToImport.Add(imported);
                    successfullyImported++;
                }
            }

            if(successfullyImported == 0)
            {
                MainWindow.Instance.ToastNotifier.ShowWarning("Nothing imported");
            }
            else
            {
                MainWindow.Instance.ToastNotifier.ShowSuccess($"{successfullyImported} game{(successfullyImported == 1 ? "" : "s")} imported");
            }
        }
        private void View_Button_Click(object sender, RoutedEventArgs e)
        {
            bool fail = false;
            foreach(var game in GamesToImport)
            {
                bool success = game.LoadGame(out string failReason);
                if(!success)
                {
                    fail = true;
                    MainWindow.Instance.ToastNotifier.ShowError($"Could not load an imported game ({failReason})");
                }
            }

            if(!fail)
            {
                new ImportedGamesWindow(new List<ImportedGameItem>(GamesToImport))
                {
                    Owner = Window.GetWindow(this)
                }.Show();
            }

        }

        private void Clear_Button_Click(object sender, RoutedEventArgs e)
        {
            foreach(var game in GamesToImport)
            {
                game.Cleanup();
            }
            GamesToImport.Clear();
            RefreshDisplay();
        }

        private void View_Selected_Game_Click(object sender, RoutedEventArgs e)
        {
            if (GamesToImportListView.SelectedItem is ImportedGameItem igi)
            {
                if(igi.LoadGame(out string failReason))
                {
                    new GameViewWindow(igi.LoadedGame, igi.PlayerName)
                    {
                        Owner = Window.GetWindow(this)
                    }.Show();
                }
                else
                {
                    MainWindow.Instance.ToastNotifier.ShowError($"Could not load game ({failReason})");
                }
            }
        }

        private void Remove_Selected_Game_Click(object sender, RoutedEventArgs e)
        {
            if (GamesToImportListView.SelectedItem is ImportedGameItem igi)
            {
                igi.Cleanup();
                GamesToImport.Remove(igi);
                RefreshDisplay();
            }
        }
    }
}
