﻿using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Vermintide_Analyzer
{
    /// <summary>
    /// Interaction logic for StartupWindow.xaml
    /// </summary>
    public partial class StartupWindow : Window, INotifyPropertyChanged
    {
        private const int STARTUP_DELAY = 1000; // ms

        private string mCurrentStepText = "";
        public string CurrentStepText
        {
            get => mCurrentStepText;
            set
            {
                mCurrentStepText = value;
                NotifyPropertyChanged();
            }
        }

        private string mStartupLogText = "";
        public string StartupLogText
        {
            get => mStartupLogText;
            set
            {
                mStartupLogText = value;
                NotifyPropertyChanged();
            }
        }

        public StartupWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private bool HasSetup = false;

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        private void Startup()
        {
            BeginStep("Checking directories...");
            if(!GameRepository.Instance.CheckDirectories())
            {
                BeginStep("Creating directories...");
                GameRepository.Instance.CreateDirectories();
            }

            BeginStep("Loading user settings...");
            Settings.Load();

            BeginStep("Gathering latest data...");
            GameRepository.Instance.ReadAndMoveNewGameLogs();

            BeginStep("Reading games...");
            var existingGameHeaders = GameRepository.Instance.ReadExistingGameHeaders();
            var previouslyInvalidGameHeaders = GameRepository.Instance.ReadPreviouslyInvalidGameHeaders();
            LogDetail($"Found {existingGameHeaders.Count() + previouslyInvalidGameHeaders.Count()} existing games.");

            BeginStep("Reading new data...");
            var newGameHeaders = GameRepository.Instance.ReadAndMoveNewGameHeaders();
            LogDetail($"Found {newGameHeaders.Count()} new games.");

            LogDetail($"Found {GameRepository.Instance.InvalidGames.Count} invalid games.");

            BeginStep("Finishing up...");
            GameRepository.Instance.GameHeaders.Sort((gh1, gh2) => gh2.GameStart.CompareTo(gh1.GameStart));
            GameRepository.Instance.ReadGameNotesFromDisk();
            GameRepository.Instance.ReadGameFiltersFromDisk();

            // Delete and recreate the temp dir to clear it out
            try
            {
                Directory.Delete(GameRepository.TempDir, true);
            }
            catch { }
            Directory.CreateDirectory(GameRepository.TempDir);
            GameRepository.Instance.RemoveTemporaryGameNotes();

            Thread.Sleep(STARTUP_DELAY);

            Util.SafeInvoke(() =>
            {
                new MainWindow().Show();
                Close();
            });
        }

        private void BeginStep(string step)
        {
            CurrentStepText = step;
            LogDetail(step);
        }
        
        private void LogDetail(string detail)
        {
            StartupLogText += Environment.NewLine + detail;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            // It became visible
            if (!HasSetup)
            {
                new Task(Startup).Start();
                HasSetup = true;
            }
        }
    }
}
