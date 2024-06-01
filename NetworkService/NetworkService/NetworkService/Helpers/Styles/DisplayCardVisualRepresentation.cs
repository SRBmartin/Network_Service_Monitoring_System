using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NetworkService.Helpers.Styles
{
    public class DisplayCardVisualRepresentation : BindableBase
    {
        private Visibility connectArrowVisibility;
        private Visibility alarmVisibility;
        private string imgPath;
        private Brush background;

        private readonly Brush AlarmBackground = new SolidColorBrush(Color.FromRgb(250, 204, 197));
        private readonly Brush NormalBackground = (Brush)Application.Current.FindResource("UIAccentColor");
        private readonly string ArrowPath = "pack://application:,,,/Resource/Images/ConnectArrow.png";
        private readonly string CancelPath = "pack://application:,,,/Resource/Images/Cancel.png";
        public DisplayCardVisualRepresentation(Visibility connectArrowVisibility = Visibility.Hidden, Visibility alarmVisibility = Visibility.Hidden, string imgPath = "pack://application:,,,/Resource/Images/ConnectArrow.png")
        {
            this.connectArrowVisibility = connectArrowVisibility;
            this.alarmVisibility = alarmVisibility;
            this.imgPath = imgPath;
            background = NormalBackground;
        }
        public Visibility ConnectArrowVisibility
        {
            get { return connectArrowVisibility; }
            set
            {
                connectArrowVisibility = value;
                OnPropertyChanged(nameof(ConnectArrowVisibility));
            }
        }
        public Visibility AlarmVisibility
        {
            get { return alarmVisibility; }
            set
            {
                alarmVisibility = value;
                OnPropertyChanged(nameof(AlarmVisibility));
            }
        }
        public string ImgPath
        {
            get { return imgPath; }
            set
            {
                imgPath = value;
                OnPropertyChanged(nameof(ImgPath));
            }
        }
        public Brush Background
        {
            get { return background; }
            set
            {
                background = value;
                OnPropertyChanged(nameof(Background));
            }
        }
        public void StartConnecting()
        {
            ConnectArrowVisibility = Visibility.Visible;
        }
        public void StopConnecting()
        {
            ConnectArrowVisibility = Visibility.Hidden;
            ImgPath = ArrowPath;
        }
        public void ProposeCancel()
        {
            ImgPath = CancelPath;
        }
        public void StopCancelPropose()
        {
            ImgPath = ArrowPath;
        }
        public void RaiseAlarm()
        {
            AlarmVisibility = Visibility.Visible;
            Background = AlarmBackground;
        }
        public void StopAlarm()
        {
            AlarmVisibility = Visibility.Hidden;
            Background = NormalBackground;
        }
    }
}
