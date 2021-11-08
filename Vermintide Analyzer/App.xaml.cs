using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

namespace Vermintide_Analyzer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {

#if DEBUG   // DEBUG only so this can never be accidentally left uncommented and affect release versions

            // UNCOMMENT THIS TO TEST DIFFERENT CULTURES

            //var vCulture = new CultureInfo("de-DE");
            //Thread.CurrentThread.CurrentCulture = vCulture;
            //Thread.CurrentThread.CurrentUICulture = vCulture;
            //CultureInfo.DefaultThreadCurrentCulture = vCulture;
            //CultureInfo.DefaultThreadCurrentUICulture = vCulture;
            //FrameworkElement.LanguageProperty.OverrideMetadata(
            //    typeof(FrameworkElement),
            //    new FrameworkPropertyMetadata(
            //        XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

#endif

            base.OnStartup(e);
        }
    }
}
