using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using VA.LogReader;

namespace Vermintide_Analyzer
{
    public static class Util
    {
        public static object StaticResource(object key) => Application.Current.FindResource(key);

        public static void SafeInvoke(Action action) =>
            Application.Current.Dispatcher.Invoke(action);

        public static void SafeInvoke<T>(Action<T> action, object arg) =>
            Application.Current.Dispatcher.Invoke(action, arg);

        public static Task ContinueWithSafe(this Task t, Action<Task> continuation) =>
            t.ContinueWith((task) => { SafeInvoke(continuation, task); });


        public static IEnumerable<string> FilterOptions(Type enumType, params Enum[] exclusions)
        {
            var list = Enum.GetValues(enumType)
                        .Cast<Enum>()
                        .Where(val => !exclusions.Contains(val))
                        .Select(val => val.ForDisplay());

            return list;
        }

        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
                yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                if (child is T t)
                    yield return t;

                foreach (T childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }

        public static IEnumerable<T> FindLogicalChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
                yield break;

            foreach(var child in LogicalTreeHelper.GetChildren(depObj))
            {
                if (child is T t)
                    yield return t;

                if(child is DependencyObject childDep)
                {
                    foreach (T childOfChild in FindLogicalChildren<T>(childDep))
                        yield return childOfChild;
                }
            }
        }
    }
}
