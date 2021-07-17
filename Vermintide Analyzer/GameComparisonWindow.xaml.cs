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
using System.Windows.Shapes;
using VA.LogReader;
using Vermintide_Analyzer.Statistics;

namespace Vermintide_Analyzer
{
    /// <summary>
    /// Interaction logic for GameComparisonWindow.xaml
    /// </summary>
    public partial class GameComparisonWindow : Window
    {
        public GameAverages Averages1 { get; set; }
        public GameAverages Averages2 { get; set; }

        public GameFilter Filter1 { get; set; }
        public GameFilter Filter2 { get; set; }

        public string Filter1Description => Filter1.ToString();
        public string Filter2Description => Filter2.ToString();

        #region Highlighting
        public int BetterWinLossRatioColumn { get; set; } = 1;
        public int BetterTimeAlivePercentColumn { get; set; } = 1;
        public int BetterTimeDownedPercentColumn { get; set; } = 1;
        public int BetterTimeDeadPercentColumn { get; set; } = 1;
        public int BetterDamageDealtColumn { get; set; } = 1;
        public int BetterMonsterDamageDealtColumn { get; set; } = 1;
        public int BetterAllyDamageDealtColumn { get; set; } = 1;
        public int BetterOverkillDamageDealtColumn { get; set; } = 1;
        public int BetterEnemiesKilledColumn { get; set; } = 1;
        public int BetterElitesKilledColumn { get; set; } = 1;
        public int BetterSpecialsKilledColumn { get; set; } = 1;
        //public int BetterStaggerDealtColumn { get; set; } = 1;
        public int BetterDamageTakenColumn { get; set; } = 1;
        public int BetterTimesDownedColumn { get; set; } = 1;
        public int BetterTimesDiedColumn { get; set; } = 1;
        #endregion

        public GameComparisonWindow(GameAverages avgs1, GameAverages avgs2, GameFilter filter1, GameFilter filter2)
        {
            Averages1 = avgs1;
            Averages2 = avgs2;

            Filter1 = filter1;
            Filter2 = filter2;

            BetterWinLossRatioColumn = GetHighlightColumn(nameof(GameAverages.WinLossRatio));
            BetterTimeAlivePercentColumn = GetHighlightColumn(nameof(GameAverages.TimeAlivePercent));
            BetterTimeDownedPercentColumn = GetHighlightColumn(nameof(GameAverages.TimeDownedPercent), true);
            BetterTimeDeadPercentColumn = GetHighlightColumn(nameof(GameAverages.TimeDeadPercent), true);
            BetterDamageDealtColumn = GetHighlightColumn(nameof(GameAverages.DamageDealtPerMin));
            BetterMonsterDamageDealtColumn = GetHighlightColumn(nameof(GameAverages.MonsterDamageDealtPerMin));
            BetterAllyDamageDealtColumn = GetHighlightColumn(nameof(GameAverages.AllyDamageDealtPerMin), true);
            BetterOverkillDamageDealtColumn = GetHighlightColumn(nameof(GameAverages.OverKillDamagePerMin), true);
            BetterEnemiesKilledColumn = GetHighlightColumn(nameof(GameAverages.EnemiesKilledPerMin));
            BetterElitesKilledColumn = GetHighlightColumn(nameof(GameAverages.ElitesKilledPerMin));
            BetterSpecialsKilledColumn = GetHighlightColumn(nameof(GameAverages.SpecialsKilledPerMin));
            //BetterStaggerDealtColumn = GetHighlightColumn(nameof(GameAverages.StaggerDealtPerMin));
            BetterDamageTakenColumn = GetHighlightColumn(nameof(GameAverages.DamageDealtPerMin), true);
            BetterTimesDownedColumn = GetHighlightColumn(nameof(GameAverages.TimesDowned), true);
            BetterTimesDiedColumn = GetHighlightColumn(nameof(GameAverages.TimesDied), true);

            InitializeComponent();
            DataContext = this;
        }

        private int GetHighlightColumn(string propName, bool lowerIsBetter = false)
        {
            var comparison = Averages1.Compare(Averages2, propName);
            if (comparison < 0) return lowerIsBetter ? 0 : 2;
            if (comparison > 0) return lowerIsBetter ? 2 : 0;
            return 1;
        }
    }
}
