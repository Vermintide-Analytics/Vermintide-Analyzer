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
    public partial class InvalidGamesView : UserControl, IAnalyticsPage
    {
        public IEnumerable<InvalidGame> Games => GameRepository.Instance.InvalidGames.Where(g => true);

        public InvalidGamesView()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void RefreshDisplay()
        {
            GamesList.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
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

        private void Delete_Selected_Game_Click(object sender, RoutedEventArgs e)
        {
            if (!Util.ConfirmWithDialog()) return;

            if (GamesList.SelectedItem is InvalidGame g)
            {
                GameRepository.Instance.DeleteInvalidGame(g);
                RefreshDisplay();
            }
        }

        private void Delete_All_Click(object sender, RoutedEventArgs e)
        {
            if (!Util.ConfirmWithDialog()) return;

            GameRepository.Instance.DeleteInvalidGames();
            RefreshDisplay();
        }
    }
}
