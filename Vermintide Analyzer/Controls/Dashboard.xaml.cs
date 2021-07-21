using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
                foreach(var careerDash in this.FindVisualChildren<CareerDashboard>())
                {
                    careerDash.UpdateDisplayMode(mShowQuitGames);
                }
            }
        }

        public IEnumerable<DIFFICULTY> DifficultyFilter => DifficultyStrings.Select(str => str.FromDisplay<DIFFICULTY>());
        public List<string> DifficultyStrings { get; set; } = new List<string>(Util.FilterOptions(typeof(DIFFICULTY)));

        public List<string> DifficultyFilterValues => Util.FilterOptions(typeof(DIFFICULTY)).ToList();


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
            foreach (var careerDash in this.FindVisualChildren<CareerDashboard>())
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
