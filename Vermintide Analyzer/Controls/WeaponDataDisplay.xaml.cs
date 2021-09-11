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
    /// Interaction logic for WeaponDataDisplay.xaml
    /// </summary>
    public partial class WeaponDataDisplay : UserControl
    {
        public WeaponData Weapon
        {
            get { return (WeaponData)GetValue(WeaponProperty); }
            set { SetValue(WeaponProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WeaponProperty =
            DependencyProperty.Register("Weapon", typeof(WeaponData), typeof(WeaponDataDisplay),
                new PropertyMetadata(
                    new WeaponData(HERO.Bardin, WEAPON_SLOT.Weapon1, 0, RARITY.Gray, 0),
                    new PropertyChangedCallback((obj, args) =>
                    { 
                        if(obj is WeaponDataDisplay disp)
                        {
                            disp.ItemDetailsDisplay.Item = disp.Weapon.ItemDetails;

                            disp.RecalculateStats();
                            disp.UpdateDisplay();
                        }
                    })));

        public float RangeFilterStart { get; set; } = float.MinValue;
        public float RangeFilterEnd { get; set; } = float.MaxValue;

        #region Display
        public string TotalDamageDisp => DispDouble(TotalDamage);
        public string DamagePerMinDisp => DispDouble(DamagePerMin);
        public string TotalMonsterDamageDisp => DispDouble(TotalMonsterDamage);
        public string MonsterDamagePerMinDisp => DispDouble(MonsterDamagePerMin);
        public string TotalAllyDamageDisp => DispDouble(TotalAllyDamage);
        public string AllyDamagePerMinDisp => DispDouble(AllyDamagePerMin);

        //public string TotalStaggerDisp => DispDouble(TotalStagger);
        //public string StaggerPerMinDisp => DispDouble(StaggerPerMin);

        public string TotalEnemiesKilledDisp => TotalEnemiesKilled.ToString();
        public string EnemiesKilledPerMinDisp => DispDouble(EnemiesKilledPerMin);
        public string TotalElitesKilledDisp => TotalElitesKilled.ToString();
        public string ElitesKilledPerMinDisp => DispDouble(ElitesKilledPerMin);
        public string TotalSpecialsKilledDisp => TotalSpecialsKilled.ToString();
        public string SpecialsKilledPerMinDisp => DispDouble(SpecialsKilledPerMin);

        public string TotalOverkillDamageDisp => DispDouble(TotalOverkillDamage);
        public string OverkillDamagePerMinDisp => DispDouble(OverkillDamagePerMin);

        public string HeadshotsDisp => Headshots.ToString();
        public string HeadshotsPerMinDisp => DispDouble(HeadshotsPerMin);
        public string HeadshotPercentageDisp => $"{DispDouble(HeadshotPercentage)}%";

        public string AverageCritMultiplierDisp => $"x{DispDouble(AverageCritMultiplier)}";
        public string CritPercentageDisp => $"{DispDouble(CritPercentage)}%";
        #endregion

        #region Calculated
        #region Damage Dealt
        public double TotalDamage { get; private set; } = double.NaN;
        public double DamagePerMin { get; private set; } = double.NaN;
        public double TotalMonsterDamage { get; private set; } = double.NaN;
        public double MonsterDamagePerMin { get; private set; } = double.NaN;
        public double TotalAllyDamage { get; private set; } = double.NaN;
        public double AllyDamagePerMin { get; private set; } = double.NaN;

        public double AvgDamage { get; private set; } = double.NaN;
        public double AvgMonsterDamage { get; private set; } = double.NaN;
        public double AvgAllyDamage { get; private set; } = double.NaN;
        #endregion

        //#region Stagger Dealt
        //public double TotalStagger { get; private set; } = double.NaN;
        //public double StaggerPerMin { get; private set; } = double.NaN;
        //public double AvgStagger { get; private set; } = double.NaN;

        //#endregion

        #region Enemies Killed
        public int TotalEnemiesKilled { get; private set; } = 0;
        public double EnemiesKilledPerMin { get; private set; } = double.NaN;
        public int TotalElitesKilled { get; private set; } = 0;
        public double ElitesKilledPerMin { get; private set; } = double.NaN;
        public int TotalSpecialsKilled { get; private set; } = 0;
        public double SpecialsKilledPerMin { get; private set; } = double.NaN;
        #endregion

        #region Overkill Damage
        public double TotalOverkillDamage { get; private set; } = double.NaN;
        public double OverkillDamagePerMin { get; private set; } = double.NaN;
        public double AvgOverkillDamage { get; private set; } = double.NaN;
        #endregion

        #region Headshots
        public int Headshots { get; private set; } = 0;
        public double HeadshotsPerMin { get; private set; } = double.NaN;
        public double HeadshotPercentage { get; private set; } = double.NaN;
        #endregion

        #region Crits
        public double AverageCritMultiplier { get; private set; } = double.NaN;
        public double CritPercentage { get; private set; } = double.NaN;
        #endregion
        #endregion

        public void UpdateRangeFilter(float rangeStart, float rangeEnd)
        {
            RangeFilterStart = rangeStart;
            RangeFilterEnd = rangeEnd;
            RecalculateStats();
            UpdateDisplay();
        }

        public void RecalculateStats()
        {
            var start = Math.Max(Weapon.StartTime, RangeFilterStart);
            var end = Math.Min(Weapon.StartTime + Weapon.Duration, RangeFilterEnd);

            var durationMinutes = Math.Max((end - start) / 60d, 0);

            var allEvents = Weapon.Events.Where(e => e.Time >= start && e.Time <= end);

            var damageEvents = allEvents.Where(e => e is Damage_Dealt).Cast<Damage_Dealt>().Where(e => IsDamageSourceThisWeapon(e.Source));
            var numDamageEvents = damageEvents.Count();
            TotalDamage = damageEvents.Sum(e => e.Damage);
            DamagePerMin = TotalDamage / durationMinutes;
            AvgDamage = TotalDamage / numDamageEvents;

            var nonCritDamageEvents = damageEvents.Where(e => !e.Crit);
            var critDamageEvents = damageEvents.Where(e => e.Crit);
            if (nonCritDamageEvents.Any() && critDamageEvents.Any())
            {
                var averageNonCritDamage = nonCritDamageEvents.Average(e => e.Damage);
                var averageCritDamage = critDamageEvents.Average(e => e.Damage);
                AverageCritMultiplier = averageCritDamage / averageNonCritDamage;
            }
            else
            {
                AverageCritMultiplier = double.NaN;
            }
            var numCritDamageEvents = critDamageEvents.Count();
            CritPercentage = 100f * numCritDamageEvents / numDamageEvents;

            var monsterDamageEvents = damageEvents.Where(e => e.Target == DAMAGE_TARGET.Monster);
            var numMonsterDamageEvents = monsterDamageEvents.Count();
            TotalMonsterDamage = monsterDamageEvents.Sum(e => e.Damage);
            MonsterDamagePerMin = TotalMonsterDamage / durationMinutes;
            AvgMonsterDamage = TotalMonsterDamage / numMonsterDamageEvents;

            var allyDamageEvents = damageEvents.Where(e => e.Target == DAMAGE_TARGET.Ally);
            var numAllyDamageEvents = allyDamageEvents.Count();
            TotalAllyDamage = allyDamageEvents.Sum(e => e.Damage);
            AllyDamagePerMin = TotalAllyDamage / durationMinutes;
            AvgAllyDamage = TotalAllyDamage / numAllyDamageEvents;

            //var staggerEvents = allEvents.Where(e => e is Enemy_Staggered).Cast<Enemy_Staggered>().Where(e => IsStaggerSourceThisWeapon(e.Source));
            //var numStaggerEvents = staggerEvents.Count();
            //TotalStagger = staggerEvents.Sum(e => e.StaggerDuration);
            //StaggerPerMin = TotalStagger / durationMinutes;
            //AvgStagger = TotalAllyDamage / numStaggerEvents;

            var killEvents = allEvents.Where(e => e is Enemy_Killed).Cast<Enemy_Killed>().Where(e => IsDamageSourceThisWeapon(e.Source));
            var eliteKillEvents = killEvents.Where(e => e.EnemyType == ENEMY_TYPE.Elite);
            var specialKillEvents = killEvents.Where(e => e.EnemyType == ENEMY_TYPE.Special);
            var numKillEvents = killEvents.Count();
            var numEliteKillEvents = eliteKillEvents.Count();
            var numSpecialKillEvents = specialKillEvents.Count();

            TotalEnemiesKilled = numKillEvents;
            TotalElitesKilled = numEliteKillEvents;
            TotalSpecialsKilled = numSpecialKillEvents;

            EnemiesKilledPerMin = TotalEnemiesKilled / durationMinutes;
            ElitesKilledPerMin = TotalElitesKilled / durationMinutes;
            SpecialsKilledPerMin = TotalSpecialsKilled / durationMinutes;

            TotalOverkillDamage = killEvents.Sum(e => e.OverkillDamage);
            OverkillDamagePerMin = TotalOverkillDamage / durationMinutes;
            AvgOverkillDamage = TotalOverkillDamage / numKillEvents;

            Headshots = damageEvents.Where(e => e.Headshot).Count();
            HeadshotsPerMin = Headshots / durationMinutes;
            HeadshotPercentage = 100f * (double)Headshots / numDamageEvents;


            #region Helper Functions
            bool IsDamageSourceThisWeapon(DAMAGE_SOURCE source) => (byte)source == (byte)Weapon.Slot;
            bool IsStaggerSourceThisWeapon(STAGGER_SOURCE source) => (byte)source == (byte)Weapon.Slot;
            #endregion
        }

        public void UpdateDisplay()
        {
            foreach(var textBlock in this.FindLogicalChildren<TextBlock>())
            {
                textBlock.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
            }
        }

        public WeaponDataDisplay()
        {
            InitializeComponent();
            RecalculateStats();
            UpdateDisplay();
        }

        private string DispDouble(double f) => f.ToString("F2");
    }
}
