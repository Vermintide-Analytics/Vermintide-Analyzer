using System.Windows.Controls;

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
