using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Vermintide_Analyzer.Controls
{
    public class NavButton : Button
    {
        public NavPage Destination
        {
            get { return (NavPage)GetValue(DestinationProperty); }
            set { SetValue(DestinationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Destination.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DestinationProperty =
            DependencyProperty.Register("Destination", typeof(NavPage), typeof(NavButton), new PropertyMetadata(null));

        public void ColorOff() =>
            Background = (Brush)Util.StaticResource("ThemeDarker");
        public void ColorOn() =>
            Background = (Brush)Util.StaticResource("ThemeMidtone");

        public override void EndInit()
        {
            base.EndInit();
            Navigation.RegisterNavButton(this);
        }

        protected override void OnClick()
        {
            base.OnClick();
            Navigation.NavigateTo(Destination);
        }
    }
}
