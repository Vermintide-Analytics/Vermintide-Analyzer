﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VA.LogReader;
using Vermintide_Analyzer.Statistics;

namespace Vermintide_Analyzer.Controls
{
    /// <summary>
    /// Interaction logic for GameCompareView.xaml
    /// </summary>
    public partial class GameCompareView : UserControl, IAnalyticsPage
    {
        public IEnumerable<GameHeader> Games1 => FilterDisplay1.Filter.Filter(GameRepository.Instance.GameHeaders);
        public IEnumerable<GameHeader> Games2 => FilterDisplay2.Filter.Filter(GameRepository.Instance.GameHeaders);

        public string GamesCount1 => $"{Games1.Count()} games";
        public string GamesCount2 => $"{Games2.Count()} games";

        public GameCompareView()
        {
            InitializeComponent();
            DataContext = this;

            FilterDisplay1.Filter.OnFilterChange += (propName) => RefreshDisplay();
            FilterDisplay2.Filter.OnFilterChange += (propName) => RefreshDisplay();
        }

        public void RefreshDisplay()
        {
            GamesCount1TextBlock.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
            GamesCount2TextBlock.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
        }

        public void OnNavigatedTo()
        {
            RefreshDisplay();
            FilterDisplay1.RefreshSavedFilters();
            FilterDisplay2.RefreshSavedFilters();
        }

        private void Compare_Button_Clicked(object sender, RoutedEventArgs e)
        {
            new GameComparisonWindow(
                new GameAverages(Games1.Select(gh =>
                {
                    var g = gh.ToGame();
                    return (g, new GameStats(g));
                }).Where(tuple => tuple.g.Duration > 0).ToList()),
                new GameAverages(Games2.Select(gh =>
                {
                    var g = gh.ToGame();
                    return (g, new GameStats(g));
                }).Where(tuple => tuple.g.Duration > 0).ToList()),
                FilterDisplay1.Filter,
                FilterDisplay2.Filter)
            {
                Owner = Window.GetWindow(this)
            }.Show();
        }

        private void FilterDisplay_FilterChanged()
        {
            RefreshDisplay();
        }

        private void FilterDisplay_SavedFiltersChanged()
        {
            FilterDisplay1.RefreshSavedFilters();
            FilterDisplay2.RefreshSavedFilters();
        }
    }
}
