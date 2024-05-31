using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NetworkService.Helpers.Common
{
    public class NotificationHandler
    {
        public static NotificationContent CreateNotification(NotificationType notificationType, string title, string content)
        {
            SolidColorBrush background = new SolidColorBrush();
            SolidColorBrush foreground = new SolidColorBrush();
            switch (notificationType)
            {
                case NotificationType.Success:
                    background = new SolidColorBrush(Colors.LimeGreen);
                    foreground = new SolidColorBrush(Colors.Wheat);
                    break;
                case NotificationType.Error:
                    background = new SolidColorBrush(Colors.MediumVioletRed);
                    foreground = new SolidColorBrush(Colors.Wheat);
                    break;
                case NotificationType.Information:
                    background = new SolidColorBrush(Colors.LightGoldenrodYellow);
                    foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
            }
            return new NotificationContent
            {
                Title = title,
                Message = content,
                Type = notificationType,
                TrimType = NotificationTextTrimType.AttachIfMoreRows,
                RowsCount = 2,
                CloseOnClick = true,
                Background = background,
                Foreground = foreground
            };
        }
        public static void ShowToastNotification(NotificationContent notificationContent)
        {
            NotificationManager notificationManager = new NotificationManager();
            notificationManager.Show(notificationContent, "WindowNotificationArea");
        }
    }
}
