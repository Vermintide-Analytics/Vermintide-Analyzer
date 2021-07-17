using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Vermintide_Analyzer.Controls;

namespace Vermintide_Analyzer
{
    public static class Navigation
    {
        public static Dictionary<NavPage, UIElement> Pages { get; } = new Dictionary<NavPage, UIElement>()
        {
            { NavPage.Dashboard, new Dashboard() },
            { NavPage.GameView, new GameListView() },
            { NavPage.StatComparison, new GameCompareView() },
            { NavPage.About, new AboutView() },
        };

        public static Dictionary<NavPage, List<NavButton>> NavButtons { get; } = new Dictionary<NavPage, List<NavButton>>()
        {
            { NavPage.Dashboard, new List<NavButton>() },
            { NavPage.GameView, new List<NavButton>() },
            { NavPage.StatComparison, new List<NavButton>() },
            { NavPage.About, new List<NavButton>() },
        };

        public static NavPage? CurrentPage { get; set; }
        public static Decorator ContentPane { get; set; }

        public static void NavigateTo(NavPage page)
        {
            if (page == CurrentPage) return;

            if(ContentPane != null)
            {
                CurrentPage = page;
                ContentPane.Child = Pages[page];

                foreach(var kvp in NavButtons)
                {
                    foreach(var val in kvp.Value)
                    {
                        val.ColorOff();
                    }
                }

                foreach(var val in NavButtons[page])
                {
                    val.ColorOn();
                }

                if(Pages[page] is IAnalyticsPage vaPage)
                {
                    vaPage.OnNavigatedTo();
                }
            }
        }

        public static void RegisterNavButton(NavButton button)
        {
            // Make sure we don't keep duplicate registrations
            foreach(var kvp in NavButtons)
            {
                kvp.Value.Remove(button);
            }

            NavButtons[button.Destination].Add(button);
            if(button.Destination == CurrentPage)
            {
                button.ColorOn();
            }
            else
            {
                button.ColorOff();
            }
        }
            
    }

    public enum NavPage
    {
        Dashboard,
        GameView,
        StatComparison,
        About
    }
}
