using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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
