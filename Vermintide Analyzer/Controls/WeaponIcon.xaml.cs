using System.Windows;
using System.Windows.Controls;
using VA.LogReader;

namespace Vermintide_Analyzer.Controls
{
    /// <summary>
    /// Interaction logic for WeaponIcon.xaml
    /// </summary>
    public partial class WeaponIcon : UserControl
    {
        public WeaponData Weapon
        {
            get { return (WeaponData)GetValue(WeaponProperty); }
            set { SetValue(WeaponProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WeaponProperty =
            DependencyProperty.Register("Weapon", typeof(WeaponData), typeof(WeaponIcon), new PropertyMetadata(null));


        public WeaponIcon()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
