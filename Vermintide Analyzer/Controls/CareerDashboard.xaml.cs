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
    /// Interaction logic for CareerDashboard.xaml
    /// </summary>
    public partial class CareerDashboard : UserControl
    {
        public CareerDashboard()
        {
            InitializeComponent();
        }

        #region DP Props
        public CAREER Career
        {
            get { return (CAREER)GetValue(CareerProperty); }
            set { SetValue(CareerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Career.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CareerProperty =
            DependencyProperty.Register("Career", typeof(CAREER), typeof(CareerDashboard), new PropertyMetadata(CAREER.Mercenary,
                (o, e) => ((CareerDashboard)o).InitDisplay()));

        public IEnumerable<DIFFICULTY> DifficultyFilter
        {
            get { return (IEnumerable<DIFFICULTY>)GetValue(DifficultyFilterProperty); }
            set { SetValue(DifficultyFilterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DifficultyFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DifficultyFilterProperty =
            DependencyProperty.Register("DifficultyFilter", typeof(IEnumerable<DIFFICULTY>), typeof(CareerDashboard), new PropertyMetadata(new List<DIFFICULTY>(),
                (o, e) => ((CareerDashboard)o).UpdateDisplay()));
        #endregion


        public bool IncludeIncompleteGames { get; set; } = true;

        public string CareerName => Career.ForDisplay().Contains("UNKNOWN") ? "-" : Career.ForDisplay();
        public string PortraitImagePath => $"/Images/Career Portraits/{CareerName}.png";

        public int NumGames => GameRepository.Instance.GameHeaders.Count(gh => DifficultyFilter.Contains(gh.Difficulty) && gh.Career == Career);
        public int NumWins => GameRepository.Instance.GameHeaders.Count(gh => DifficultyFilter.Contains(gh.Difficulty) && gh.Career == Career && gh.Result.IsWin());
        public int NumLosses => GameRepository.Instance.GameHeaders.Count(gh => DifficultyFilter.Contains(gh.Difficulty) && gh.Career == Career && gh.Result.IsLoss());

        public int CountedGames => IncludeIncompleteGames ? NumGames : NumWins + NumLosses;

        public float PercentWins => NumGames == 0 ? 0 : (float)NumWins / NumGames;
        public float PercentLosses => NumGames == 0 ? 0 : (float)NumLosses / NumGames;
        public float PercentQuits => Math.Max(0, 1 - PercentWins - PercentLosses);

        public GridLength BarWinWidth => PercentWins > 0 ? new GridLength(PercentWins, GridUnitType.Star) : new GridLength(0);
        public GridLength BarLossWidth => PercentLosses > 0 ? new GridLength(PercentLosses, GridUnitType.Star) : new GridLength(0);
        public GridLength BarQuitWidth => IncludeIncompleteGames && PercentQuits > 0 ? new GridLength(PercentQuits, GridUnitType.Star) : new GridLength(0);

        public double BarHeight => CountedGames == 0 ? 0 :
            1 + 1.2f*Math.Log(CountedGames, 1.5f);

        public void UpdateDisplayMode(bool includeIncompleteGames)
        {
            IncludeIncompleteGames = includeIncompleteGames;

            UpdateColWidth(IncompleteGamesColumn);
            UpdateText(CountedGamesText);
            UpdateHeight(PerformanceBar);
        }

        public void InitDisplay()
        {
            UpdateText(CareerNameText);
            CareerIcon.GetBindingExpression(Image.SourceProperty).UpdateTarget();
        }

        public void UpdateDisplay()
        {
            UpdateText(WonGamesText);
            UpdateText(LostGamesText);
            UpdateText(CountedGamesText);

            UpdateHeight(PerformanceBar);
            UpdateColWidth(WonGamesColumn);
            UpdateColWidth(LostGamesColumn);
            UpdateColWidth(IncompleteGamesColumn);
        }

        private void UpdateText(FrameworkElement elem) => elem.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
        private void UpdateWidth(FrameworkElement elem) => elem.GetBindingExpression(WidthProperty).UpdateTarget();
        private void UpdateHeight(FrameworkElement elem) => elem.GetBindingExpression(HeightProperty).UpdateTarget();
        private void UpdateColWidth(FrameworkContentElement elem) => elem.GetBindingExpression(ColumnDefinition.WidthProperty).UpdateTarget();
        private void UpdateRowHeight(FrameworkContentElement elem) => elem.GetBindingExpression(RowDefinition.HeightProperty).UpdateTarget();
    }
}
