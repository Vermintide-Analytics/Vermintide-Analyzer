using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public static string SplitCamelCase(this string str) =>
            Regex.Replace(
                Regex.Replace(
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );

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
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject parent) where T : DependencyObject
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            var queue = new Queue<DependencyObject>(new[] { parent });

            while (queue.Any())
            {
                var reference = queue.Dequeue();
                var count = VisualTreeHelper.GetChildrenCount(reference);

                for (var i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(reference, i);
                    if (child is T children)
                        yield return children;

                    queue.Enqueue(child);
                }
            }
        }

        public static bool ConfirmWithDialog(string prompt = "Are you sure?", string title = "") =>
            MessageBox.Show(prompt, title, MessageBoxButton.YesNo) == MessageBoxResult.Yes;

        public static void ErrorDialog(string description = "Unexpected error occurred.", string title = "") =>
            MessageBox.Show(description, title, MessageBoxButton.OK);

        public static bool OkCancelErrorDialog(string description = "Unexpected error occurred.", string title = "") =>
            MessageBox.Show(description, title, MessageBoxButton.OKCancel) == MessageBoxResult.OK;

        public static bool IsLocked(this FileInfo file)
        {
            if (!file.Exists)
            {
                return true;
            }

            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
    }
}
