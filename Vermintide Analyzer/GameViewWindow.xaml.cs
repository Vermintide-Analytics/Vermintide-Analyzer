﻿using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Geared;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VA.LogReader;
using Vermintide_Analyzer.Controls;
using Vermintide_Analyzer.Dialogs;
using ToastNotifications;
using ToastNotifications.Messages;
using Vermintide_Analyzer.Misc;
using Vermintide_Analyzer.Models;
using System.Windows.Media.Animation;
using Vermintide_Analyzer.Statistics;

namespace Vermintide_Analyzer
{
    /// <summary>
    /// Interaction logic for OldMainWindow.xaml
    /// </summary>
    public partial class GameViewWindow : Window
    {
        public GameItem GameModel { get; set; }
        public Game Game => GameModel?.Game;

        #region Chart Properties
        public bool ShowHealthChart =>
            PermanentHealthPoints.Any() ||
            TemporaryHealthPoints.Any();
        public bool ShowDamageDealtCharts => AllDamagePoints.Any();
        public bool ShowDamageTakenChart => AllDamageTakenPoints.Any();
        public bool ShowOverkillDamageChart =>
            Weapon1OverkillDamagePoints.Any() ||
            Weapon2OverkillDamagePoints.Any() ||
            CareerOverkillDamagePoints.Any() ||
            OtherOverkillDamagePoints.Any();

        public ChartValues<ScatterPoint> PermanentHealthPoints { get; set; } = new ChartValues<ScatterPoint>();
        public ChartValues<ScatterPoint> TemporaryHealthPoints { get; set; } = new ChartValues<ScatterPoint>();

        private List<(Damage_Dealt, ScatterPoint)> AllDamagePoints { get; set; } = new List<(Damage_Dealt, ScatterPoint)>();
        private List<(Damage_Taken, ScatterPoint)> AllDamageTakenPoints { get; set; } = new List<(Damage_Taken, ScatterPoint)>();
        private List<(Enemy_Staggered, ScatterPoint)> AllStaggerPoints { get; set; } = new List<(Enemy_Staggered, ScatterPoint)>();

        public GearedValues<ScatterPoint> PlainDamagePoints { get; set; } = new GearedValues<ScatterPoint>();
        public GearedValues<ScatterPoint> MonsterDamagePoints { get; set; } = new GearedValues<ScatterPoint>();
        public GearedValues<ScatterPoint> AllyDamagePoints { get; set; } = new GearedValues<ScatterPoint>();

        public GearedValues<ScatterPoint> Weapon1DamagePoints { get; set; } = new GearedValues<ScatterPoint>();
        public GearedValues<ScatterPoint> Weapon2DamagePoints { get; set; } = new GearedValues<ScatterPoint>();
        public GearedValues<ScatterPoint> CareerDamagePoints { get; set; } = new GearedValues<ScatterPoint>();
        public GearedValues<ScatterPoint> OtherDamagePoints { get; set; } = new GearedValues<ScatterPoint>();

        public GearedValues<ScatterPoint> Weapon1OverkillDamagePoints { get; set; } = new GearedValues<ScatterPoint>();
        public GearedValues<ScatterPoint> Weapon2OverkillDamagePoints { get; set; } = new GearedValues<ScatterPoint>();
        public GearedValues<ScatterPoint> CareerOverkillDamagePoints { get; set; } = new GearedValues<ScatterPoint>();
        public GearedValues<ScatterPoint> OtherOverkillDamagePoints { get; set; } = new GearedValues<ScatterPoint>();

        public GearedValues<ScatterPoint> PushStaggerPoints { get; set; } = new GearedValues<ScatterPoint>();
        public GearedValues<ScatterPoint> OtherStaggerPoints { get; set; } = new GearedValues<ScatterPoint>();

        public GearedValues<ScatterPoint> DamageTakenPoints { get; set; } = new GearedValues<ScatterPoint>();
        public GearedValues<ScatterPoint> SpecialDamageTakenPoints { get; set; } = new GearedValues<ScatterPoint>();
        public GearedValues<ScatterPoint> MonsterDamageTakenPoints { get; set; } = new GearedValues<ScatterPoint>();
        public GearedValues<ScatterPoint> FriendlyFireTakenPoints { get; set; } = new GearedValues<ScatterPoint>();

        public Func<double, string> TimeFormatter { get; set; } = (time) => (time / 60).ToString();
        #endregion

        #region Stats
        public GameStats Stats => GameModel?.Stats;

        public string AveragePing => Stats.AvgPing.ToString(); 

        public string DamageDealt => DispDouble(Stats.TotalDamage);
        public string DamageDealtPerMin => DispDouble(Stats.DamagePerMin);
        public string MonsterDamageDealt => DispDouble(Stats.TotalMonsterDamage);
        public string MonsterDamageDealtPerMin => DispDouble(Stats.MonsterDamagePerMin);
        public string AllyDamageDealt => DispDouble(Stats.TotalAllyDamage);
        public string AllyDamageDealtPerMin => DispDouble(Stats.AllyDamagePerMin);

        public string StaggersDealt => Stats.NumStaggers.ToString();
        public string StaggersDealtPerMin => DispDouble(Stats.NumStaggersPerMin);
        public string StaggerLengthDealt => DispDouble(Stats.TotalStaggerLength);
        public string StaggerLengthDealtPerMin => DispDouble(Stats.StaggerLengthPerMin);

        public string EnemiesKilled => Stats.TotalEnemiesKilled.ToString();
        public string EnemiesKilledPerMin => DispDouble(Stats.EnemiesKilledPerMin);
        public string ElitesKilled => Stats.TotalElitesKilled.ToString();
        public string ElitesKilledPerMin => DispDouble(Stats.ElitesKilledPerMin);
        public string SpecialsKilled => Stats.TotalSpecialsKilled.ToString();
        public string SpecialsKilledPerMin => DispDouble(Stats.SpecialsKilledPerMin);

        public string OverkillDamage => DispDouble(Stats.TotalOverkillDamage);
        public string OverkillDamagePerMin => DispDouble(Stats.OverkillDamagePerMin);

        public string Headshots => Stats.Headshots.ToString();
        public string HeadshotsPerMin => DispDouble(Stats.HeadshotsPerMin);

        public string DamageTaken => DispDouble(Stats.TotalDamageTaken);
        public string DamageTakenPerMin => DispDouble(Stats.DamageTakenPerMin);
        public string FriendlyFireTaken => DispDouble(Stats.FriendlyFireTaken);
        public string FriendlyFireTakenPerMin => DispDouble(Stats.FriendlyFireTakenPerMin);

        public string UncappedTempHPGained => DispDouble(Stats.TotalUncappedTempHPGained);
        public string UncappedTempHPGainedPerMin => DispDouble(Stats.UncappedTempHPGainedPerMin);
        public string CappedTempHPGained => DispDouble(Stats.TotalCappedTempHPGained);
        public string CappedTempHPGainedPerMin => DispDouble(Stats.CappedTempHPGainedPerMin);

        public string TimesDowned => Stats.TimesDowned.ToString();
        public string TimesDied => Stats.TimesDied.ToString();
        public string TimeDownedPercent => $"{DispDouble(Stats.TimeDownedPercent)}%";
        public string TimeDeadPercent => $"{DispDouble(Stats.TimeDeadPercent)}%";
        public string TimeAlivePercent => $"{DispDouble(Stats.TimeAlivePercent)}%";

        public string HealingItemsApplied => Stats.HealingItemsApplied.ToString();
        public string HealingClearedWounds => Stats.HealingClearedWounds.ToString();
        #endregion

        #region Misc Display Props
        public bool IsChaosWastes => Game.Campaign == CAMPAIGN.ChaosWastes;
        public int ChartTitleHeight => 40;
        public int ChartHeight => 390;
        public int ChartSeparatorHeight => 1;
        public int TotalChartHeight => ChartTitleHeight + ChartHeight + ChartSeparatorHeight;
        public int VisibleChartIndex { get; set; } = 0;
        public int NumCharts => 5;

        public string ResultDisp
        {
            get
            {
                switch (Game.Result)
                {
                    case ROUND_RESULT.Quit:
                        return "Unfinished";
                    case ROUND_RESULT.Loss:
                        return "Defeat";
                    case ROUND_RESULT.Win:
                        return "Victory";
                }
                return "Unkown";
            }
        }
        
        public string DurationDisp
        {
            get
            {
                var span = TimeSpan.FromSeconds(Game.Duration);

                return $"{span.Minutes} minutes, {span.Seconds} seconds";
            }
        }

        public string HostText => Game.IsHost ? "Yes" : "No";

        public string ScreenshotWatermarkText => PlayerNameOverride ?? Settings.Current.PlayerName ?? "";
        public string PlayerNameOverride { get; set; }
        public string PlayerNameOverrideDisplay => PlayerNameOverride ?? "";
        public bool HasPlayerNameOverride => !string.IsNullOrEmpty(PlayerNameOverride);

        public List<TalentTabItem> TalentTabItems { get; set; } = new List<TalentTabItem>();

        public List<WeaponTabItem> Weapon1TabItems { get; set; } = new List<WeaponTabItem>();
        public List<WeaponTabItem> Weapon2TabItems { get; set; } = new List<WeaponTabItem>();

        public bool AnyTalentData => TalentTabItems.Any();
        public bool AnyWeapon1Data => Weapon1TabItems.Any();
        public bool AnyWeapon2Data => Weapon2TabItems.Any();

        public bool ShowTalentTabs => TalentTabItems.Count > 1;
        public bool ShowWeapon1Tabs => Weapon1TabItems.Count > 1;
        public bool ShowWeapon2Tabs => Weapon2TabItems.Count > 1;

        public float RangeFilterStart { get; set; } = float.MinValue;
        public float RangeFilterEnd { get; set; } = float.MaxValue;
        #endregion

        #region Toast
        public Notifier ToastNotifier { get; set; }
        #endregion

        public GameViewWindow(Game g, string playerNameOverride = null)
        {
            PlayerNameOverride = playerNameOverride;

            ToastNotifier = Toast.MakeNotifier(this);

            GameModel = new GameItem(g);

            #region Init tab item lists
            TalentTabItems.AddRange(Game.TalentTrees.Select(tTree => new TalentTabItem(tTree, Game.Career)));
            if(TalentTabItems.Any())
            {
                TalentTabItems.First().IsFirst = true;
            }

            Weapon1TabItems.AddRange(Game.Weapon1Datas.Select(weap => new WeaponTabItem(weap)));
            if (Weapon1TabItems.Any())
            {
                Weapon1TabItems.First().IsFirst = true;
            }
            Weapon2TabItems.AddRange(Game.Weapon2Datas.Select(weap => new WeaponTabItem(weap)));
            if (Weapon2TabItems.Any())
            {
                Weapon2TabItems.First().IsFirst = true;
            }
            #endregion

            InitializeComponent();
            DataContext = this;

            ConstructSeries();
            ConstructSections();

            PrettifyCharts();

            HideForScreenshot.Add(ScreenshotInstructionsContainer);
        }

        #region Private
        private void ConstructSeries()
        {
            var allDamageEvents = Game.Events.Where(e => e is Damage_Dealt).Cast<Damage_Dealt>();
            AllDamagePoints.AddRange(allDamageEvents.Select(e => (e, new ScatterPoint(e.Time, e.Damage))));
            var allDamageTakenEvents = Game.Events.Where(e => e is Damage_Taken).Cast<Damage_Taken>();
            AllDamageTakenPoints.AddRange(allDamageTakenEvents.Select(e => (e, new ScatterPoint(e.Time, e.Damage))));
            var allStaggerEvents = Game.Events.Where(e => e is Enemy_Staggered).Cast<Enemy_Staggered>();
            AllStaggerPoints.AddRange(allStaggerEvents.Select(e => (e, new ScatterPoint(e.Time, e.StaggerDuration))));

            PlainDamagePoints.AddRange(AllDamagePoints.Where(p => p.Item1.Target == DAMAGE_TARGET.Enemy).Select(p => p.Item2));
            MonsterDamagePoints.AddRange(AllDamagePoints.Where(p => p.Item1.Target == DAMAGE_TARGET.Monster).Select(p => p.Item2));
            AllyDamagePoints.AddRange(AllDamagePoints.Where(p => p.Item1.Target == DAMAGE_TARGET.Ally).Select(p => p.Item2));

            Weapon1DamagePoints.AddRange(AllDamagePoints.Where(p => p.Item1.Source == DAMAGE_SOURCE.Weapon1).Select(p => p.Item2));
            Weapon2DamagePoints.AddRange(AllDamagePoints.Where(p => p.Item1.Source == DAMAGE_SOURCE.Weapon2).Select(p => p.Item2));
            CareerDamagePoints.AddRange(AllDamagePoints.Where(p => p.Item1.Source == DAMAGE_SOURCE.Career).Select(p => p.Item2));
            OtherDamagePoints.AddRange(AllDamagePoints.Where(p => p.Item1.Source == DAMAGE_SOURCE.Other).Select(p => p.Item2));

            var allKillEvents = Game.Events.Where(e => e is Enemy_Killed).Cast<Enemy_Killed>();
            var overkillEvents = allKillEvents.Where(e => e.OverkillDamage > 0);
            Weapon1OverkillDamagePoints.AddRange(overkillEvents.Where(e => e.Source == DAMAGE_SOURCE.Weapon1).Select(e => new ScatterPoint(e.Time, e.OverkillDamage)));
            Weapon2OverkillDamagePoints.AddRange(overkillEvents.Where(e => e.Source == DAMAGE_SOURCE.Weapon2).Select(e => new ScatterPoint(e.Time, e.OverkillDamage)));
            CareerOverkillDamagePoints.AddRange(overkillEvents.Where(e => e.Source == DAMAGE_SOURCE.Career).Select(e => new ScatterPoint(e.Time, e.OverkillDamage)));
            OtherOverkillDamagePoints.AddRange(overkillEvents.Where(e => e.Source == DAMAGE_SOURCE.Other).Select(e => new ScatterPoint(e.Time, e.OverkillDamage)));

            PushStaggerPoints.AddRange(AllStaggerPoints.Where(p => p.Item1.Source == STAGGER_SOURCE.Push).Select(p => p.Item2));
            OtherStaggerPoints.AddRange(AllStaggerPoints.Where(p => p.Item1.Source == STAGGER_SOURCE.Other).Select(p => p.Item2));

            DamageTakenPoints.AddRange(AllDamageTakenPoints.Where(p => p.Item1.Source == DAMAGE_TAKEN_SOURCE.Enemy).Select(p => p.Item2));
            SpecialDamageTakenPoints.AddRange(AllDamageTakenPoints.Where(p => p.Item1.Source == DAMAGE_TAKEN_SOURCE.Special).Select(p => p.Item2));
            MonsterDamageTakenPoints.AddRange(AllDamageTakenPoints.Where(p => p.Item1.Source == DAMAGE_TAKEN_SOURCE.Monster).Select(p => p.Item2));
            FriendlyFireTakenPoints.AddRange(AllDamageTakenPoints.Where(p => p.Item1.Source.IsFriendlyFire()).Select(p => p.Item2));

            var allHealthEvents = Game.Events.Where(e => e is Current_Health).Cast<Current_Health>().ToList();
            var dupeEvts = new List<Current_Health>();
            for(int i = 1; i < allHealthEvents.Count; i++)
            {
                if(allHealthEvents[i].Time == allHealthEvents[i-1].Time)
                {
                    dupeEvts.Add(allHealthEvents[i]);
                }
            }
            foreach(var evt in dupeEvts)
            {
                allHealthEvents.Remove(evt);
            }

            PermanentHealthPoints.AddRange(allHealthEvents.Select(e =>
            {
                return Settings.Current.ShowHealthWhenDowned || Stats.GetPlayerStateAtTime(e.Time) == PLAYER_STATE.Alive ?
                    new ScatterPoint(e.Time, e.PermanentHealth) :
                    new ScatterPoint(e.Time, 0);
            }));
            TemporaryHealthPoints.AddRange(allHealthEvents.Select(e =>
            {
                return Settings.Current.ShowHealthWhenDowned || Stats.GetPlayerStateAtTime(e.Time) == PLAYER_STATE.Alive ?
                    new ScatterPoint(e.Time, e.TemporaryHealth) :
                    new ScatterPoint(e.Time, 0);
            }));
        }

        private void ConstructSections()
        {
            //StaggerDealtChart.AxisX[0].Sections = new SectionsCollection();
            DamageTakenChart.AxisX[0].Sections = new SectionsCollection();
            CurrentHealthChart.AxisX[0].Sections = new SectionsCollection();

            //SetAxisSections(StaggerDealtChart.AxisX[0].Sections);
            SetAxisSections(DamageTakenChart.AxisX[0].Sections);
            SetAxisSections(CurrentHealthChart.AxisX[0].Sections);

            InitSelectionSection(CurrentHealthChart);
            InitSelectionSection(DamageTakenChart);
            InitSelectionSection(DamageDealtBySourceChart);
            InitSelectionSection(DamageDealtByTargetChart);
            InitSelectionSection(OverkillDamageChart);

            void InitSelectionSection(CartesianChart chart)
            {
                var section = new AxisSection()
                {
                    Stroke = new SolidColorBrush(Colors.Blue),
                    StrokeThickness = 1,
                    Fill = new SolidColorBrush(Color.FromArgb(60, 0, 0, 230)),
                    Visibility = Visibility.Hidden
                };
                ChartSelectionRangeSections.Add(section);
                chart.AxisX.First().Sections.Add(section);
            }
        }

        private void PrettifyCharts()
        {
            var healthEvents = Game.Events.Where(e => e is Current_Health).Cast<Current_Health>();

            if(healthEvents.Any())
            {
                float highestHealth = healthEvents.Max(e => e.PermanentHealth + e.TemporaryHealth);

                if (highestHealth > 455)
                {
                    CurrentHealthChart.AxisY[0].Separator.Step = 50;
                }
                else if (highestHealth > 355)
                {
                    CurrentHealthChart.AxisY[0].Separator.Step = 40;
                }
                else if (highestHealth > 255)
                {
                    CurrentHealthChart.AxisY[0].Separator.Step = 30;
                }
                else if (highestHealth > 155)
                {
                    CurrentHealthChart.AxisY[0].Separator.Step = 20;
                }
            }
        }

        private void SetAxisSections(SectionsCollection collection)
        {
            collection.Clear();
            foreach (var stateEvt in Game.Events.Where(evt => evt is Player_State).Cast<Player_State>())
            {
                collection.Add(CreatePlayerStateChangeMarker(stateEvt));
            }
        }

        private AxisSection CreatePlayerStateChangeMarker(Player_State evt)
        {
            switch (evt.State)
            {
                case PLAYER_STATE.Alive:
                    return CreatePlayerAlive(evt);
                case PLAYER_STATE.Downed:
                    return CreatePlayerDowned(evt);
                case PLAYER_STATE.Dead:
                    return CreatePlayerDead(evt);
            }
            return null;
        }

        private AxisSection CreatePlayerDowned(Player_State downedEvt)
        {
            var marker = CreateAxisSection(downedEvt.Time);
            marker.Stroke = (Brush)Util.StaticResource("NeonOrange");
            marker.ToolTip = "Downed";
            return marker;
        }

        private AxisSection CreatePlayerDead(Player_State deadEvt)
        {
            var marker = CreateAxisSection(deadEvt.Time);
            marker.Stroke = (Brush)Util.StaticResource("NeonRed");
            marker.ToolTip = "Killed";
            return marker;
        }

        private AxisSection CreatePlayerAlive(Player_State revivedEvt)
        {
            var marker = CreateAxisSection(revivedEvt.Time);
            marker.Stroke = (Brush)Util.StaticResource("NeonGreen");
            marker.ToolTip = "Revived";
            return marker;
        }

        private AxisSection CreateAxisSection(float value) =>
            new AxisSection()
            {
                Value = value,
                SectionWidth = 0,
                StrokeThickness = 1,
                SnapsToDevicePixels = true,
                UseLayoutRounding = true
            };

        private void RangeFilterUpdated()
        {
            // Recalculate game stats
            Stats.RangeFilterStart = RangeFilterStart;
            Stats.RangeFilterEnd = RangeFilterEnd;
            Stats?.RecalculateStats();

            // Update display for game stats
            foreach (var textBlock in
                GameStatsGrid.FindLogicalChildren<TextBlock>()
                .Concat(PlayerStateGridsPanel.FindLogicalChildren<TextBlock>()))
            {
                textBlock.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
            }

            // Recalculate stats and update display for weapons
            foreach (var weaponDisplay in this.FindVisualChildren<WeaponDataDisplay>())
            {
                weaponDisplay.UpdateRangeFilter(RangeFilterStart, RangeFilterEnd);
            }
        }

        private string DispDouble(double f) => f.ToString("F2");

        private List<UIElement> HideForScreenshot = new List<UIElement>();


        private void ScreenshotToClipboard()
        {
            if(Settings.Current.WatermarkScreenshots)
            {
                ScreenshotWatermark.Visibility = Visibility.Visible;
            }
            foreach(var elem in HideForScreenshot)
            {
                elem.Visibility = Visibility.Hidden;
            }

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)MainGrid.ActualWidth, (int)MainGrid.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(MainGrid);
            Clipboard.SetImage(renderTargetBitmap);

            ScreenshotWatermark.Visibility = Visibility.Hidden;
            foreach (var elem in HideForScreenshot)
            {
                elem.Visibility = Visibility.Visible;
            }

            ToastNotifier.ShowInformation("Detailed view copied to clipboard");
        }

        private void BannerScreenshotToClipboard()
        {
            TopGrid.Visibility = Visibility.Hidden;
            BannerView.Visibility = Visibility.Visible;
            if (Settings.Current.WatermarkScreenshots)
            {
                ScreenshotWatermark.Visibility = Visibility.Visible;
            }

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)BannerView.ActualWidth, (int)BannerView.ActualHeight, 96, 96, PixelFormats.Pbgra32);

            // Render the MainGrid still so we still can get the ScreenshotWatermark included
            renderTargetBitmap.Render(MainGrid);
            Clipboard.SetImage(renderTargetBitmap);

            ScreenshotWatermark.Visibility = Visibility.Hidden;
            BannerView.Visibility = Visibility.Hidden;
            TopGrid.Visibility = Visibility.Visible;

            ToastNotifier.ShowInformation("Quick view copied to clipboard");
        }

        private void SetVisibleChart(int index)
        {
            var destinationY = ChartScrollViewer.TranslatePoint(new Point(0, TotalChartHeight * index), ChartScrollViewer).Y;

            DoubleAnimation verticalAnimation = new DoubleAnimation();

            verticalAnimation.From = ChartScrollViewer.VerticalOffset;
            verticalAnimation.To = destinationY;
            verticalAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.35f));
            verticalAnimation.EasingFunction = new SineEase();

            Storyboard storyboard = new Storyboard();

            storyboard.Children.Add(verticalAnimation);
            Storyboard.SetTarget(verticalAnimation, ChartScrollViewer);
            Storyboard.SetTargetProperty(verticalAnimation, new PropertyPath(ScrollAnimationBehavior.VerticalOffsetProperty)); // Attached dependency property
            storyboard.Begin();

            VisibleChartIndex = index;

            foreach (var button in ChartSelectionControls.FindLogicalChildren<Border>().Where(b => b.Tag != null))
            {
                button.Background = (Brush)Util.StaticResource("ThemeDarker");
            }
            ChartSelectionControls.FindLogicalChildren<Border>().FirstOrDefault(b =>
                (string)b.Tag != "Next" &&
                (string)b.Tag != "Previous" &&
                int.Parse((string)b.Tag) == VisibleChartIndex)
                    .Background = (Brush)Util.StaticResource("ThemeLight");
        }

        #region Event Handlers
        private void Make_Note_For_Game_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new StringPromptDialog(this, "Notes:", GameModel.HasCustomNotes ? GameModel.CustomNotes : "");
            if (dialog.ShowDialog() == true)
            {
                if (string.IsNullOrWhiteSpace(dialog.ResponseText))
                {
                    GameRepository.Instance.GameNotes.Remove(Game.FilePath);
                }
                else if (GameRepository.Instance.GameNotes.ContainsKey(Game.FilePath))
                {
                    GameRepository.Instance.GameNotes[Game.FilePath] = dialog.ResponseText;
                }
                else
                {
                    GameRepository.Instance.GameNotes.Add(Game.FilePath, dialog.ResponseText);
                }

                // Update the game UI
                CustomNoteTextBlock.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
                CustomNoteTextBlock.GetBindingExpression(VisibilityProperty).UpdateTarget();
                MakeNoteTextBlock.GetBindingExpression(VisibilityProperty).UpdateTarget();

                // Update the game list UI
                (Navigation.Pages[NavPage.GameView] as GameListView)?.RefreshDisplay();

                GameRepository.Instance.WriteGameNotesToDisk();
            }
            e.Handled = true;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F12)
            {
                if(Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                {
                    BannerScreenshotToClipboard();
                }
                else
                {
                    ScreenshotToClipboard();
                }
            }
            else if(e.Key == Key.Down)
            {
                SetVisibleChart((VisibleChartIndex + 1) % NumCharts);
            }
            else if(e.Key == Key.Up)
            {
                SetVisibleChart((VisibleChartIndex - 1 + NumCharts) % NumCharts);
            }
        }

        private void ScreenshotInstructions_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ScreenshotToClipboard();
        }
        private void BannerScreenshotInstructions_MouseUp(object sender, MouseButtonEventArgs e)
        {
            BannerScreenshotToClipboard();
        }

        private void ChartScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(e.Delta > 0)
            {
                ChartNavigate(PREVIOUS);
            }
            else if(e.Delta < 0)
            {
                ChartNavigate(NEXT);
            }
            e.Handled = true;
        }

        private void ChartSelect_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var elem = sender as FrameworkElement;

            ChartNavigate((string)elem.Tag);
        }

        private const string NEXT = "Next";
        private const string PREVIOUS = "Previous";
        private void ChartNavigate(string tag)
        {
            var destination = VisibleChartIndex;
            if (tag == NEXT)
            {
                destination = (destination + 1) % NumCharts;
            }
            else if (tag == PREVIOUS)
            {
                destination = (destination - 1 + NumCharts) % NumCharts;
            }
            else
            {
                destination = int.Parse(tag);
            }
            SetVisibleChart(destination);
        }

        private Point MouseDownChartPosition = new Point(0, 0);
        private Point CurrentMouseChartPosition = new Point(0, 0);

        private List<AxisSection> ChartSelectionRangeSections = new List<AxisSection>();

        private void Chart_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is CartesianChart chart)
            {
                MouseDownChartPosition = chart.ConvertToChartValues(e.GetPosition(chart));
                CurrentMouseChartPosition = MouseDownChartPosition;

                foreach (var section in ChartSelectionRangeSections)
                {
                    section.SectionOffset = MouseDownChartPosition.X;
                    section.SectionWidth = 0;
                }
            }
        }

        private void Chart_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is CartesianChart chart)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    CurrentMouseChartPosition = chart.ConvertToChartValues(e.GetPosition(chart));
                    foreach (var section in ChartSelectionRangeSections)
                    {
                        section.SectionWidth = CurrentMouseChartPosition.X - MouseDownChartPosition.X;
                        section.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void Chart_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is CartesianChart chart)
            {
                if (CurrentMouseChartPosition.Equals(MouseDownChartPosition))
                {
                    // If the user just clicks, remove the range filter and section
                    foreach (var section in ChartSelectionRangeSections)
                    {
                        section.Visibility = Visibility.Hidden;
                    }
                    RangeFilterStart = float.MinValue;
                    RangeFilterEnd = float.MaxValue;
                }
                else
                {
                    // Otherwise, trigger data updates
                    RangeFilterStart = (float)Math.Min(MouseDownChartPosition.X, CurrentMouseChartPosition.X);
                    RangeFilterEnd = (float)Math.Max(MouseDownChartPosition.X, CurrentMouseChartPosition.X);
                }
                RangeFilterUpdated();
            }
        }
        #endregion

        #endregion
    }
}
