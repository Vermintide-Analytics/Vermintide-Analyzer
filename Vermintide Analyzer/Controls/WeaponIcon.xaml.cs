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
