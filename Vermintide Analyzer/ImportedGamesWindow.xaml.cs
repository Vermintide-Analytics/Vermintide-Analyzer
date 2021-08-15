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
using System.Windows.Shapes;
using Vermintide_Analyzer.Models;

namespace Vermintide_Analyzer
{
    /// <summary>
    /// Interaction logic for ImportedGamesWindow.xaml
    /// </summary>
    public partial class ImportedGamesWindow : Window
    {
        public IEnumerable<ImportedGameItem> Games { get; set; }

        public ImportedGamesWindow(IEnumerable<ImportedGameItem> games)
        {
            InitializeComponent();
            DataContext = this;

            Games = games;
            AllDataGrid.GetBindingExpression(DataGrid.ItemsSourceProperty).UpdateTarget();
        }
    }
}
