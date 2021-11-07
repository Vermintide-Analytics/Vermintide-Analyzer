using System.Linq;
using System.Windows.Controls;

namespace Vermintide_Analyzer.Controls
{
    /// <summary>
    /// Interaction logic for NavMenu.xaml
    /// </summary>
    public partial class NavMenu : UserControl
    {
        public bool ShowInvalidGamesButton => GameRepository.Instance.InvalidGames.Any();

        public NavMenu()
        {
            InitializeComponent();
            DataContext = this;
        }

        public NavPage? CurrentPage => Navigation.CurrentPage;
    }
}
