using System.Windows;
using System.Windows.Controls;
using VA.LogReader;

namespace Vermintide_Analyzer.Controls
{
    /// <summary>
    /// Interaction logic for ItemDetailsView.xaml
    /// </summary>
    public partial class ItemDetailsView : UserControl
    {
        public ItemDetails Item
        {
            get { return (ItemDetails)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register("Item", typeof(ItemDetails), typeof(ItemDetailsView),
                new PropertyMetadata(
                    new ItemDetails(),
                    new PropertyChangedCallback((obj, args) =>
                    {
                        if (obj is ItemDetailsView view)
                        {
                            view.UpdateDisplay();
                        }
                    })));

        public ItemDetailsView()
        {
            InitializeComponent();
        }

        public void UpdateDisplay()
        {
            TraitsItemsControl.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            PropertiesItemsControl.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
        }
    }
}
