﻿using System;
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
using ToastNotifications.Messages;
using VA.LogReader;
using Vermintide_Analyzer.Dialogs;
using Vermintide_Analyzer.Models;

namespace Vermintide_Analyzer.Controls
{
    /// <summary>
    /// Interaction logic for GameListView.xaml
    /// </summary>
    public partial class GameListView : UserControl, IAnalyticsPage
    {        
        public IEnumerable<GameHeaderItem> Games => FilterDisplay.Filter.Filter(GameRepository.Instance.GameHeaders).Select(gh => new GameHeaderItem(gh));

        public string GamesCount => $"{Games.Count()} Games";

        public GameListView()
        {
            InitializeComponent();
            DataContext = this;

            FilterDisplay.Filter.OnFilterChange += (propName) => RefreshDisplay();
        }

        public void RefreshDisplay()
        {
            GamesList.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            GamesCountTextBlock.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
        }

        private void Game_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;

            var ghi = ((ContentPresenter)((FrameworkElement)sender).TemplatedParent).Content as GameHeaderItem;

            new GameViewWindow(Game.FromFile(ghi.GameHeader.FilePath)) {
                Owner = Window.GetWindow(this)
            }.Show();
        }

        public void OnNavigatedTo()
        {
            RefreshDisplay();
            FilterDisplay.RefreshSavedFilters();
        }

        private void FilterDisplay_FilterChanged()
        {
            RefreshDisplay();
        }

        private void Delete_These_Games_Click(object sender, RoutedEventArgs e)
        {
            if (!Util.ConfirmWithDialog()) return;

            int count = Games.Count();
            GameRepository.Instance.DeleteGames(Games.Select(ghi => ghi.GameHeader));
            RefreshDisplay();
            MainWindow.Instance.ToastNotifier.ShowInformation($"{count} game{(count == 1 ? "" : "s")} deleted");
        }

        private void Delete_Selected_Game_Click(object sender, RoutedEventArgs e)
        {
            if (!Util.ConfirmWithDialog()) return;

            if (GamesList.SelectedItem is GameHeader gh)
            {
                GameRepository.Instance.DeleteGame(gh);
                RefreshDisplay();
                MainWindow.Instance.ToastNotifier.ShowInformation($"Game deleted");
            }
        }

        private void Make_Note_For_Selected_Game_Click(object sender, RoutedEventArgs e)
        {
            if (GamesList.SelectedItem is GameHeaderItem ghi)
            {
                var dialog = new StringPromptDialog(Window.GetWindow(this), "Notes:", ghi.HasCustomNotes ? ghi.CustomNotes : "");
                if (dialog.ShowDialog() == true)
                {
                    if(string.IsNullOrWhiteSpace(dialog.ResponseText))
                    {
                        GameRepository.Instance.GameNotes.Remove(ghi.GameHeader.FilePath);
                    }
                    else if(GameRepository.Instance.GameNotes.ContainsKey(ghi.GameHeader.FilePath))
                    {
                        GameRepository.Instance.GameNotes[ghi.GameHeader.FilePath] = dialog.ResponseText;
                    }
                    else
                    {
                        GameRepository.Instance.GameNotes.Add(ghi.GameHeader.FilePath, dialog.ResponseText);
                    }
                    RefreshDisplay();
                    GameRepository.Instance.WriteGameNotesToDisk();
                }
                e.Handled = true;
            }
        }
    }
}
