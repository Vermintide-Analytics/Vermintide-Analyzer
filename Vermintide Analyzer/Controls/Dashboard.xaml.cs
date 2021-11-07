using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using VA.LogReader;

namespace Vermintide_Analyzer.Controls
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl, INotifyPropertyChanged, IAnalyticsPage
    {
        private bool mShowQuitGames = true;

        public bool ShowQuitGames
        {
            get => mShowQuitGames;
            set
            {
                mShowQuitGames = value;
                NotifyPropertyChanged();
                foreach(var careerDash in this.FindLogicalChildren<CareerDashboard>())
                {
                    careerDash.UpdateDisplayMode(mShowQuitGames);
                }
            }
        }

        public IEnumerable<DIFFICULTY> DifficultyFilter => DifficultyStrings.Select(str => str.FromDisplay<DIFFICULTY>());
        public List<string> DifficultyStrings { get; set; } = new List<string>(GameFilter.FilterOptions(typeof(DIFFICULTY)));

        public List<string> DifficultyFilterValues => GameFilter.FilterOptions(typeof(DIFFICULTY)).ToList();


        public Dashboard()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private void UpdateDisplay()
        {
            foreach (var careerDash in this.FindLogicalChildren<CareerDashboard>())
            {
                careerDash.UpdateDisplay();
            }
        }

        public void OnNavigatedTo()
        {
            UpdateDisplay();
        }

        private void Difficulty_SelectionChanged(MultiSelectComboBox source, List<string> newSelection)
        {
            UpdateDisplay();
        }
    }
}
