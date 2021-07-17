using LiveCharts;
using System.Linq;
using System.Windows;
using VA.LogReader;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using Vermintide_Analyzer.Controls;
using LiveCharts.Geared;
using System.Windows.Media;

namespace Vermintide_Analyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Navigation.ContentPane = ContentPane;
            Navigation.NavigateTo(NavPage.Dashboard);
        }
    }
}
