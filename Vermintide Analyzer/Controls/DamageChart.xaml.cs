using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
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
using VA.LogReader;

namespace Vermintide_Analyzer.Controls
{
    /// <summary>
    /// Interaction logic for GameChart.xaml
    /// </summary>
    public partial class DamageChart : UserControl
    {
        private Game mGame;
        public Game Game
        {
            get => mGame;
            set { mGame = value; Init(); }
        }

        private List<(Damage_Dealt, ScatterPoint)> AllDamagePoints { get; set; } = new List<(Damage_Dealt, ScatterPoint)>();
        private List<(Damage_Taken, ScatterPoint)> AllDamageTakenPoints { get; set; } = new List<(Damage_Taken, ScatterPoint)>();

        public ScatterSeries PlainDamageSeries { get; set; } = new ScatterSeries();
        public ScatterSeries MonsterDamageSeries { get; set; } = new ScatterSeries();
        public ScatterSeries AllyDamageSeries { get; set; } = new ScatterSeries();


        public ScatterSeries Weapon1DamageSeries { get; set; } = new ScatterSeries();
        public ScatterSeries Weapon2DamageSeries { get; set; } = new ScatterSeries();
        public ScatterSeries CareerDamageSeries { get; set; } = new ScatterSeries();
        public ScatterSeries OtherDamageSeries { get; set; } = new ScatterSeries();


        public ScatterSeries DamageTakenSeries { get; set; } = new ScatterSeries();


        public SeriesCollection DamageSeriesCollection { get; set; } = new SeriesCollection();


        public DamageChart()
        {
            InitializeComponent();
        }

        private void Init()
        {
            ConstructSeries();

            // Set starting series
            DamageSeriesCollection.Add(PlainDamageSeries);
            DamageSeriesCollection.Add(MonsterDamageSeries);
            DamageSeriesCollection.Add(AllyDamageSeries);
            DamageSeriesCollection.Add(Weapon1DamageSeries);
            DamageSeriesCollection.Add(Weapon2DamageSeries);
            DamageSeriesCollection.Add(CareerDamageSeries);
            DamageSeriesCollection.Add(OtherDamageSeries);
            DamageSeriesCollection.Add(DamageTakenSeries);


            Chart.Series = DamageSeriesCollection;
        }

        private void ConstructSeries()
        {
            var allDamageEvents = Game.Events.Where(e => e.Type == EventType.Damage_Dealt).Cast<Damage_Dealt>();
            AllDamagePoints.AddRange(allDamageEvents.Select(e => (e, new ScatterPoint(e.Time, e.Damage))));
            var allDamageTakenEvents = Game.Events.Where(e => e.Type == EventType.Damage_Taken).Cast<Damage_Taken>();
            AllDamageTakenPoints.AddRange(allDamageTakenEvents.Select(e => (e, new ScatterPoint(e.Time, e.Damage))));

            PlainDamageSeries = MakeDamageDealtSeries("Regular", (Brush)App.Current.FindResource("DataEnemy"), p => p.Item1.Target == DAMAGE_TARGET.Enemy);
            MonsterDamageSeries = MakeDamageDealtSeries("Monsters", (Brush)App.Current.FindResource("DataMonster"), p => p.Item1.Target == DAMAGE_TARGET.Monster);
            AllyDamageSeries = MakeDamageDealtSeries("Allies", (Brush)App.Current.FindResource("DataBad"), p => p.Item1.Target == DAMAGE_TARGET.Ally);

            Weapon1DamageSeries = MakeDamageDealtSeries("Weapon 1", (Brush)App.Current.FindResource("DataWeapon1"), p => p.Item1.Source == DAMAGE_SOURCE.Weapon1);
            Weapon2DamageSeries = MakeDamageDealtSeries("Weapon 2", (Brush)App.Current.FindResource("DataWeapon2"), p => p.Item1.Source == DAMAGE_SOURCE.Weapon2);
            CareerDamageSeries = MakeDamageDealtSeries("Career", (Brush)App.Current.FindResource("DataCareer"), p => p.Item1.Source == DAMAGE_SOURCE.Career);
            OtherDamageSeries = MakeDamageDealtSeries("Other", (Brush)App.Current.FindResource("DataOther"), p => p.Item1.Source == DAMAGE_SOURCE.Other);

            DamageTakenSeries = MakeDamageTakenSeries("Not-Downed", (Brush)App.Current.FindResource("DataGood"), p => true);
        }

        private ScatterSeries MakeDamageDealtSeries(string name, Brush fill, Predicate<(Damage_Dealt, ScatterPoint)> points) =>
            new ScatterSeries() { Title = name, Fill = fill, Values = new ChartValues<ScatterPoint>(AllDamagePoints.Where(p => points(p)).Select(p => p.Item2)) };

        private ScatterSeries MakeDamageTakenSeries(string name, Brush fill, Predicate<(Damage_Taken, ScatterPoint)> points) =>
            new ScatterSeries() { Title = name, Fill = fill, Values = new ChartValues<ScatterPoint>(AllDamageTakenPoints.Where(p => points(p)).Select(p => p.Item2)) };


        #region Event Handlers
        private void BySourceButton_Checked(object sender, RoutedEventArgs e)
        {
            if(Chart != null)
            {
                //DamageSeriesCollection.Clear();
                //DamageSeriesCollection.Add(Weapon1DamageSeries);
                //DamageSeriesCollection.Add(Weapon2DamageSeries);
                //DamageSeriesCollection.Add(CareerDamageSeries);
                //DamageSeriesCollection.Add(OtherDamageSeries);

                PlainDamageSeries.Visibility = Visibility.Hidden;
                MonsterDamageSeries.Visibility = Visibility.Hidden;
                AllyDamageSeries.Visibility = Visibility.Hidden;

                DamageTakenSeries.Visibility = Visibility.Hidden;

                Weapon1DamageSeries.Visibility = Visibility.Visible;
                Weapon2DamageSeries.Visibility = Visibility.Visible;
                CareerDamageSeries.Visibility = Visibility.Visible;
                OtherDamageSeries.Visibility = Visibility.Visible;
            }
        }

        private void ByTargetButton_Checked(object sender, RoutedEventArgs e)
        {
            if (Chart != null)
            {
                //DamageSeriesCollection.Clear();
                //DamageSeriesCollection.Add(PlainDamageSeries);
                //DamageSeriesCollection.Add(MonsterDamageSeries);
                //DamageSeriesCollection.Add(AllyDamageSeries);

                PlainDamageSeries.Visibility = Visibility.Visible;
                MonsterDamageSeries.Visibility = Visibility.Visible;
                AllyDamageSeries.Visibility = Visibility.Visible;

                DamageTakenSeries.Visibility = Visibility.Hidden;

                Weapon1DamageSeries.Visibility = Visibility.Hidden;
                Weapon2DamageSeries.Visibility = Visibility.Hidden;
                CareerDamageSeries.Visibility = Visibility.Hidden;
                OtherDamageSeries.Visibility = Visibility.Hidden;
            }
        }

        private void DamageTakenButton_Checked(object sender, RoutedEventArgs e)
        {
            if (Chart != null)
            {
                //DamageSeriesCollection.Clear();
                //DamageSeriesCollection.Add(DamageTakenSeries);
                //DamageSeriesCollection.Add(DamageTakenWhileDownedSeries);

                PlainDamageSeries.Visibility = Visibility.Hidden;
                MonsterDamageSeries.Visibility = Visibility.Hidden;
                AllyDamageSeries.Visibility = Visibility.Hidden;

                DamageTakenSeries.Visibility = Visibility.Visible;

                Weapon1DamageSeries.Visibility = Visibility.Hidden;
                Weapon2DamageSeries.Visibility = Visibility.Hidden;
                CareerDamageSeries.Visibility = Visibility.Hidden;
                OtherDamageSeries.Visibility = Visibility.Hidden;
            }
        }
        #endregion
    }
}
