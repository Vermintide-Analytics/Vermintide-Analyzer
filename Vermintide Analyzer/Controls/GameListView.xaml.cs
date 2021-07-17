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
using VA.LogReader;

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

        private void RefreshDisplay()
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
            if (!ConfirmWithDialog()) return;

            GameRepository.Instance.DeleteGames(Games);
            RefreshDisplay();
        }

        private void Delete_Selected_Game_Click(object sender, RoutedEventArgs e)
        {
            if (!ConfirmWithDialog()) return;

            if (GamesList.SelectedItem is GameHeader gh)
            {
                GameRepository.Instance.DeleteGame(gh);
                RefreshDisplay();
            }
        }

        private bool ConfirmWithDialog() =>
            MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
    }
}
