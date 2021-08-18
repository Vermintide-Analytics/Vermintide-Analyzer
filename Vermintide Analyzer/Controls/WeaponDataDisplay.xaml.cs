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

                            disp.UpdateDisplay();
                        }
                    })));

        public string DamageDealt => DispDouble(Weapon.TotalDamage);
        public string DamageDealtPerMin => DispDouble(Weapon.DamagePerMin);
        public string MonsterDamageDealt => DispDouble(Weapon.TotalMonsterDamage);
        public string MonsterDamageDealtPerMin => DispDouble(Weapon.MonsterDamagePerMin);
        public string AllyDamageDealt => DispDouble(Weapon.TotalAllyDamage);
        public string AllyDamageDealtPerMin => DispDouble(Weapon.AllyDamagePerMin);

        //public string StaggerDealt => DispDouble(Weapon.TotalStagger);
        //public string StaggerDealtPerMin => DispDouble(Weapon.StaggerPerMin);

        public string EnemiesKilled => Weapon.TotalEnemiesKilled.ToString();
        public string EnemiesKilledPerMin => DispDouble(Weapon.EnemiesKilledPerMin);
        public string ElitesKilled => Weapon.TotalElitesKilled.ToString();
        public string ElitesKilledPerMin => DispDouble(Weapon.ElitesKilledPerMin);
        public string SpecialsKilled => Weapon.TotalSpecialsKilled.ToString();
        public string SpecialsKilledPerMin => DispDouble(Weapon.SpecialsKilledPerMin);

        public string OverkillDamage => DispDouble(Weapon.TotalOverkillDamage);
        public string OverkillDamagePerMin => DispDouble(Weapon.OverkillDamagePerMin);

        public string Headshots => Weapon.Headshots.ToString();
        public string HeadshotsPerMin => DispDouble(Weapon.HeadshotsPerMin);
        public string HeadshotPercent => $"{DispDouble(Weapon.HeadshotPercentage)}%";

        public string AverageCritMultiplier => $"x{DispDouble(Weapon.AverageCritMultiplier)}";
        public string CritPercent => $"{DispDouble(Weapon.CritPercentage)}%";

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
        }

        private string DispDouble(double f) => f.ToString("F2");
    }
}
