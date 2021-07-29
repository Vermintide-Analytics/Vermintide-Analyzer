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
using Vermintide_Analyzer.Dialogs;

namespace Vermintide_Analyzer.Controls
{
    /// <summary>
    /// Interaction logic for GameListView.xaml
    /// </summary>
    public partial class GameListView : UserControl, IAnalyticsPage
    {        
        public IEnumerable<GameHeader> Games => FilterDisplay.Filter.Filter(GameRepository.Instance.GameHeaders);

        public string GamesCount => $"{Games.Count()} Games";

        public GameListView()
        {
            InitializeComponent();
            DataContext = this;

            FilterDisplay.Filter.OnFilterChange += (propName) => RefreshDisplay();
        }

        public void RefreshDisplay()
        {
            GamesList.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            GamesCountTextBlock.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
        }

        private void Game_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;

            var gh = ((ContentPresenter)((FrameworkElement)sender).TemplatedParent).Content as GameHeader;

            new GameViewWindow(Game.FromFile(gh.FilePath)) {
                Owner = Window.GetWindow(this)
            }.Show();
        }

        public void OnNavigatedTo()
        {
            RefreshDisplay();
        }

        private void FilterDisplay_FilterChanged()
        {
            RefreshDisplay();
        }

        private void Delete_These_Games_Click(object sender, RoutedEventArgs e)
        {
            if (!Util.ConfirmWithDialog()) return;

            int count = Games.Count();
            GameRepository.Instance.DeleteGames(Games);
            RefreshDisplay();
            MainWindow.Instance.ToastNotifier.ShowInformation($"{count} game{(count == 1 ? "" : "s")} deleted");
        }

        private void Delete_Selected_Game_Click(object sender, RoutedEventArgs e)
        {
            if (!Util.ConfirmWithDialog()) return;

            if (GamesList.SelectedItem is GameHeader gh)
            {
                GameRepository.Instance.DeleteGame(gh);
                RefreshDisplay();
                MainWindow.Instance.ToastNotifier.ShowInformation($"Game deleted");
            }
        }

        private void Make_Note_For_Selected_Game_Click(object sender, RoutedEventArgs e)
        {
            if (GamesList.SelectedItem is GameHeader gh)
            {
                var dialog = new StringPromptDialog("Notes:", gh.HasCustomNotes ? gh.CustomNotes : "", "Notes for this game");
                if (dialog.ShowDialog() == true)
                {
                    if(string.IsNullOrWhiteSpace(dialog.ResponseText))
                    {
                        GameRepository.Instance.GameNotes.Remove(gh.FilePath);
                    }
                    else if(GameRepository.Instance.GameNotes.ContainsKey(gh.FilePath))
                    {
                        GameRepository.Instance.GameNotes[gh.FilePath] = dialog.ResponseText;
                    }
                    else
                    {
                        GameRepository.Instance.GameNotes.Add(gh.FilePath, dialog.ResponseText);
                    }
                    RefreshDisplay();
                    GameRepository.Instance.WriteGameNotesToDisk();
                }
                e.Handled = true;
            }
        }
    }
}
