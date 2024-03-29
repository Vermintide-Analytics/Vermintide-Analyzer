﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using VA.LogReader;
using Vermintide_Analyzer.Dialogs;
using Vermintide_Analyzer.Misc;

namespace Vermintide_Analyzer.Controls
{
    /// <summary>
    /// Interaction logic for FilterDisplay.xaml
    /// </summary>
    public partial class FilterDisplay : UserControl
    {
        #region Events
        public delegate void SavedFiltersChangedEvent();
        public event SavedFiltersChangedEvent SavedFiltersChanged;

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

        public IEnumerable<string> SavedFilters => GameRepository.Instance.GameFilters.Keys.OrderBy(str => str);

        public List<string> RoundResultStrings { get; set; } = new List<string>();
        public List<string> CareerStrings { get; set; } = new List<string>();
        public List<string> DifficultyStrings { get; set; } = new List<string>();
        public List<string> MissionStrings { get; set; } = new List<string>();
        public List<string> OnslaughtStrings { get; set; } = new List<string>();
        public string DaysString { get; set; } = "";
        public string MinutesString { get; set; } = "";

        public List<string> GameVersionValues => GameRepository.Instance.GameVersions.ToList();
        public List<string> RoundResultValues => GameFilter.FilterOptions(typeof(ROUND_RESULT)).ToList();
        public List<string> CareerFilterValues => GameFilter.FilterOptions(typeof(CAREER)).ToList();
        public List<string> DifficultyFilterValues => GameFilter.FilterOptions(typeof(DIFFICULTY)).ToList();
        public List<string> MissionFilterValues => GameFilter.FilterOptions(typeof(MISSION)).ToList();
        public List<string> OnslaughtFilterValues => GameFilter.FilterOptions(typeof(ONSLAUGHT_TYPE)).ToList();

        public string[] YesNoEither { get; } = new string[3]
        {
            "Yes",
            "No",
            "Either"
        };

        public string[] OlderYounger { get; } = new string[2]
        {
            "Older",
            "Younger"
        };

        public string[] LongerShorter { get; } = new string[2]
        {
            "Longer",
            "Shorter"
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

        public void ResetFilter()
        {
            DeathwishDropdown.SelectedItem = "Either";
            EmpoweredDropdown.SelectedItem = "Either";

            GameVersionDropdown.ResetSelection();
            ResultDropdown.ResetSelection();
            DaysTextBox.Text = "";
            MinutesTextBox.Text = "";
            CareerDropdown.ResetSelection();
            DifficultyDropdown.ResetSelection();
            MissionDropdown.ResetSelection();
            OnslaughtDropdown.ResetSelection();
        }

        private void RefreshBindingLists()
        {
            GameVersionDropdown.SyncSelection(Filter.GameVersion.ToList());
            ResultDropdown.SyncSelection(Filter.Result.Select(c => c.ForDisplay()).ToList());
            CareerDropdown.SyncSelection(Filter.Career.Select(c => c.ForDisplay()).ToList());
            DifficultyDropdown.SyncSelection(Filter.Difficulty.Select(d => d.ForDisplay()).ToList());
            MissionDropdown.SyncSelection(Filter.Mission.Select(m => m.ForDisplay()).ToList());
            OnslaughtDropdown.SyncSelection(Filter.Onslaught.Select(o => o.ForDisplay()).ToList());
        }

        public void RefreshDisplay()
        {
            RefreshBindingLists();

            GameVersionDropdown.GetBindingExpression(MultiSelectComboBox.SelectedProperty).UpdateTarget();
            ResultDropdown.GetBindingExpression(MultiSelectComboBox.SelectedProperty).UpdateTarget();
            OlderYoungerComboBox.GetBindingExpression(ComboBox.SelectedItemProperty).UpdateTarget();
            LongerShorterComboBox.GetBindingExpression(ComboBox.SelectedItemProperty).UpdateTarget();
            DaysString = Filter.Days.HasValue ? Filter.Days.ToString() : "";
            MinutesString = Filter.Minutes.HasValue ? Filter.Minutes.ToString() : "";
            DaysTextBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            MinutesTextBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            CareerDropdown.GetBindingExpression(MultiSelectComboBox.SelectedProperty).UpdateTarget();
            DifficultyDropdown.GetBindingExpression(MultiSelectComboBox.SelectedProperty).UpdateTarget();
            MissionDropdown.GetBindingExpression(MultiSelectComboBox.SelectedProperty).UpdateTarget();
            DeathwishDropdown.GetBindingExpression(ComboBox.SelectedItemProperty).UpdateTarget();
            OnslaughtDropdown.GetBindingExpression(MultiSelectComboBox.SelectedProperty).UpdateTarget();
            EmpoweredDropdown.GetBindingExpression(ComboBox.SelectedItemProperty).UpdateTarget();
        }

        public void RaiseFilterChanged() => FilterChanged?.Invoke();

        private void MultiSelectComboBox_SelectionChanged(MultiSelectComboBox source, List<string> newSelection)
        {
            if(source == ResultDropdown)
            {
                Filter.Result.Clear();
                Filter.Result.AddRange(RoundResultStrings.Select(str => str.FromDisplay<ROUND_RESULT>()));
            }
            else if (source == CareerDropdown)
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

        private void WithinDaysTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var parseSuccess = uint.TryParse(DaysTextBox.Text, out uint withinDays);
            Filter.Days = parseSuccess ? (uint?)withinDays : null;

            RaiseFilterChanged();
        }

        private void MinutesLongTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var parseSuccess = uint.TryParse(MinutesTextBox.Text, out uint minutesLong);
            Filter.Minutes = parseSuccess ? (uint?)minutesLong : null;

            RaiseFilterChanged();
        }

        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            ResetFilter();
        }

        public void RaiseSavedFiltersChanged() => SavedFiltersChanged?.Invoke();

        public void RefreshSavedFilters()
        {
            SelectedFilterName.GetBindingExpression(ComboBox.ItemsSourceProperty).UpdateTarget();
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            var namePromptDlg = new StringPromptDialog(Window.GetWindow(this), "Filter Name:", "");
            var result = namePromptDlg.ShowDialog();
            if(result.HasValue && result.Value)
            {
                if(string.IsNullOrWhiteSpace(namePromptDlg.ResponseText))
                {
                    MainWindow.Instance.ShowError("You must enter a name");
                }
                else
                {
                    if(GameRepository.Instance.GameFilters.ContainsKey(namePromptDlg.ResponseText))
                    {
                        if(Util.ConfirmWithDialog("There is already a filter with this name. Overwrite it?"))
                        {
                            GameRepository.Instance.GameFilters[namePromptDlg.ResponseText] = Filter.ToString();
                        }
                    }
                    else
                    {
                        GameRepository.Instance.GameFilters.Add(namePromptDlg.ResponseText, Filter.ToString());
                        GameRepository.Instance.WriteGameFiltersToDisk();
                        RefreshSavedFilters();
                        RaiseSavedFiltersChanged();
                    }
                }
            }
        }

        private void Load_Button_Click(object sender, RoutedEventArgs e)
        {
            var name = (string)SelectedFilterName.SelectedItem;
            
            if(name != null)
            {
                if(GameRepository.Instance.GameFilters.ContainsKey(name))
                {
                    Filter.UpdateFromString(GameRepository.Instance.GameFilters[name]);
                    RefreshDisplay();
                    RaiseFilterChanged();
                }
                else
                {
                    MainWindow.Instance.ShowError($"Could not load saved filter \"{name}\"");
                }
            }
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            var name = (string)SelectedFilterName.SelectedItem;
            
            if(name != null)
            {
                if(GameRepository.Instance.GameFilters.Remove(name))
                {
                    MainWindow.Instance.ShowSuccess($"Saved filter \"{name}\" deleted");
                    GameRepository.Instance.WriteGameFiltersToDisk();
                    RefreshSavedFilters();
                    RaiseSavedFiltersChanged();
                }
                else
                {
                    MainWindow.Instance.ShowError($"Could not remove saved filter \"{name}\"");
                }
            }
        }

        private void Export_Button_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Filter.ToString());
            MainWindow.Instance.ShowInformation("Filter copied to clipboard");
        }

        private void Import_Button_Click(object sender, RoutedEventArgs e)
        {
            if(Clipboard.ContainsText() && GameFilter.FilterRegex.IsMatch(Clipboard.GetText().Trim()))
            {
                Filter.UpdateFromString(Clipboard.GetText());
                MainWindow.Instance.ShowInformation("Filter imported from clipboard");
                RefreshDisplay();
                RaiseFilterChanged();
            }
            else
            {
                var filterInputDlg = new StringPromptDialog(Window.GetWindow(this), "Filter Text:", "");
                var result = filterInputDlg.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    if(GameFilter.FilterRegex.IsMatch(filterInputDlg.ResponseText.Trim()))
                    {
                        Filter.UpdateFromString(filterInputDlg.ResponseText);
                        MainWindow.Instance.ShowSuccess("Filter imported");
                        RefreshDisplay();
                        RaiseFilterChanged();
                    }
                    else
                    {
                        MainWindow.Instance.ShowError("Failed to import filter, invalid format");
                    }
                }
            }
        }
    }
}
