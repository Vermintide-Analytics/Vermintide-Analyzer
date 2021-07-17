using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.ObjectModel;

namespace Vermintide_Analyzer.Controls
{
    /// <summary>
    /// Interaction logic for MultiSelectComboBox.xaml
    /// </summary>
    public partial class MultiSelectComboBox : UserControl
    {
        #region Events
        public delegate void SelectionChangedEvent(MultiSelectComboBox source, List<string> newSelection);
        public event SelectionChangedEvent SelectionChanged;
        #endregion

        #region DP Props
        public IEnumerable ItemSource
        {
            get { return (IEnumerable)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register("ItemSource", typeof(IEnumerable), typeof(MultiSelectComboBox), new PropertyMetadata(null));

        public List<string> Selected
        {
            get { return (List<string>)GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Selected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedProperty =
            DependencyProperty.Register("Selected", typeof(List<string>), typeof(MultiSelectComboBox), new PropertyMetadata(null));

        public List<string> InitiallySelected
        {
            get { return (List<string>)GetValue(InitiallySelectedProperty); }
            set { SetValue(InitiallySelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InitiallySelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InitiallySelectedProperty =
            DependencyProperty.RegisterAttached("InitiallySelected", typeof(List<string>), typeof(MultiSelectComboBox), new PropertyMetadata(null,
                (o, e) =>
                {
                    foreach (var item in new List<string>(e.NewValue as List<string>))
                    {
                        ((MultiSelectComboBox)o).items.SelectedItems.Add(item);
                    }
                }));

        public List<string> AllValues
        {
            get { return (List<string>)GetValue(AllValuesProperty); }
            set { SetValue(AllValuesProperty, new List<string>(value)); }
        }

        // Using a DependencyProperty as the backing store for InitiallySelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllValuesProperty =
            DependencyProperty.RegisterAttached("AllValues", typeof(List<string>), typeof(MultiSelectComboBox), new PropertyMetadata(null));
        #endregion

        #region Binding
        public virtual string SelectedString =>
            Selected is null || !Selected.Any() ?
                "NONE" :
                (ContainsAll() ?
                    "ANY" :
                    string.Join(", ", Selected));
        #endregion

        public MultiSelectComboBox()
        {
            InitializeComponent();
        }

        private void ShowListBox(object sender, RoutedEventArgs e)
        {
            pop.IsOpen = true;
            items.Focus();
        }

        private void Items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selected.Clear();
            Selected.AddRange(items.SelectedItems.Cast<string>());

            txtValues.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();

            SelectionChanged?.Invoke(this, Selected);
        }

        private bool ContainsAll()
        {
            if (AllValues is null) return true;
            if (Selected is null) return false;

            foreach(var val in AllValues)
            {
                if(!Selected.Contains(val))
                {
                    return false;
                }
            }

            return true;
        }

        private void None_Button_Click(object sender, RoutedEventArgs e)
        {
            items.SelectedItems.Clear();
            items.Focus();
        }
        private void All_Button_Click(object sender, RoutedEventArgs e)
        {
            ResetSelection();
            items.Focus();
        }

        public void ResetSelection()
        {
            items.SelectedItems.Clear();
            foreach (var item in AllValues)
            {
                items.SelectedItems.Add(item);
            }
        }
    }
}
