﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace Vermintide_Analyzer.Misc
{
    public static class Toast
    {
        public static Notifier MakeNotifier(Window window) =>
            new Notifier((cfg) =>
            {
                cfg.Dispatcher = Application.Current.Dispatcher;
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(TimeSpan.FromSeconds(5), MaximumNotificationCount.FromCount(5));
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: window,
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10);
            });
    }
}
