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
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        public GameListView ListView { get; set; } = new GameListView();

        public GameView()
        {
            InitializeComponent();
            DataContext = this;
            ReturnToList();
        }

        public void ReturnToList()
        {
            ContentGrid.Children.Clear();
            ContentGrid.Children.Add(ListView);
        }
    }
}
