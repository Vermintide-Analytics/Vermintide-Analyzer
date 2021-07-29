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
using ToastNotifications;
using Vermintide_Analyzer.Misc;

namespace Vermintide_Analyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }

        #region Toast
        public Notifier ToastNotifier { get; set; }
        #endregion

        public MainWindow()
        {
            Instance = this;
            ToastNotifier = Toast.MakeNotifier(this);

            InitializeComponent();
            DataContext = this;

            Navigation.ContentPane = ContentPane;
            Navigation.NavigateTo(NavPage.Dashboard);
        }
    }
}
