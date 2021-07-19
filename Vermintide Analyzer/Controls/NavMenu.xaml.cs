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
