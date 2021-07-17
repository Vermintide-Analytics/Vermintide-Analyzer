using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Vermintide_Analyzer.Misc;

namespace Vermintide_Analyzer.Controls
{
    /// <summary>
    /// Interaction logic for FilterDisplay.xaml
    /// </summary>
    public partial class FilterDisplay : UserControl
    {
        #region Events
        public delegate void FilterChangedEvent();
        public event FilterChangedEvent FilterChanged;
        #endregion

        #region DP Props
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(FilterDisplay), new PropertyMetadata("Filter",
                (o, e) => { ((FilterDisplay)o).TitleText.GetBindingExpression(TextBlock.TextProperty).UpdateTarget(); }));

        public GameFilter Filter
        {
            get { return (GameFilter)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Filter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(GameFilter), typeof(FilterDisplay), new PropertyMetadata(null));
        #endregion

        public List<string> CareerStrings { get; set; } = new List<string>();
        public List<string> DifficultyStrings { get; set; } = new List<string>();
        public List<string> MissionStrings { get; set; } = new List<string>();
        public List<string> OnslaughtStrings { get; set; } = new List<string>();

        public List<string> GameVersionValues => GameRepository.Instance.GameVersions.ToList();
        public List<string> CareerFilterValues => Util.FilterOptions(typeof(CAREER)).ToList();
        public List<string> DifficultyFilterValues => Util.FilterOptions(typeof(DIFFICULTY)).ToList();
        public List<string> MissionFilterValues => Util.FilterOptions(typeof(MISSION)).ToList();
        public List<string> OnslaughtFilterValues => Util.FilterOptions(typeof(ONSLAUGHT_TYPE)).ToList();

        public string[] YesNoEither { get; } = new string[3]
        {
            "Yes",
            "No",
            "Either"
        };

        public FilterDisplay()
        {
            Filter = new GameFilter();

            InitializeComponent();
            DataContext = this;

            ICollectionView careerGroupView = CollectionViewSource.GetDefaultView(CareerFilterValues);
            careerGroupView.GroupDescriptions.Add(new CareerGrouper());
            CareerDropdown.ItemSource = careerGroupView;

            ICollectionView missionGroupView = CollectionViewSource.GetDefaultView(MissionFilterValues);
            missionGroupView.GroupDescriptions.Add(new MissionGrouper());
            MissionDropdown.ItemSource = missionGroupView;
        }

        private void ResetFilter()
        {
            DeathwishDropdown.SelectedItem = "Either";
            EmpoweredDropdown.SelectedItem = "Either";

            CareerDropdown.ResetSelection();
            DifficultyDropdown.ResetSelection();
            MissionDropdown.ResetSelection();
            OnslaughtDropdown.ResetSelection();
        }

        private void RaiseFilterChanged() => FilterChanged?.Invoke();

        private void MultiSelectComboBox_SelectionChanged(MultiSelectComboBox source, List<string> newSelection)
        {
            if(source == CareerDropdown)
            {
                Filter.Career.Clear();
                Filter.Career.AddRange(CareerStrings.Select(str => str.FromDisplay<CAREER>()));
            }
            else if (source == DifficultyDropdown)
            {
                Filter.Difficulty.Clear();
                Filter.Difficulty.AddRange(DifficultyStrings.Select(str => str.FromDisplay<DIFFICULTY>()));
            }
            else if (source == OnslaughtDropdown)
            {
                Filter.Onslaught.Clear();
                Filter.Onslaught.AddRange(OnslaughtStrings.Select(str => str.FromDisplay<ONSLAUGHT_TYPE>()));
            }
            else if (source == MissionDropdown)
            {
                Filter.Mission.Clear();
                Filter.Mission.AddRange(MissionStrings.Select(str => str.FromDisplay<MISSION>()));
            }

            RaiseFilterChanged();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RaiseFilterChanged();
        }

        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            ResetFilter();
        }
    }
}
