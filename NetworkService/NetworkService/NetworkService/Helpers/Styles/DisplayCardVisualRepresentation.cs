using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkService.Helpers.Styles
{
    public class DisplayCardVisualRepresentation : BindableBase
    {
        private Visibility connectArrowVisibility;
        private Visibility disconnectArrowVisibility;
        private string imgPath;

        private readonly string ArrowPath = "pack://application:,,,/Resource/Images/ConnectArrow.png";
        public DisplayCardVisualRepresentation(Visibility connectArrowVisibility = Visibility.Hidden, Visibility disconnectArrowVisibility = Visibility.Hidden, string imgPath = "pack://application:,,,/Resource/Images/ConnectArrow.png")
        {
            this.connectArrowVisibility = connectArrowVisibility;
            this.disconnectArrowVisibility = disconnectArrowVisibility;
            this.imgPath = imgPath;
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
        public Visibility DisconnectArrowVisibility
        {
            get { return disconnectArrowVisibility; }
            set
            {
                disconnectArrowVisibility = value;
                OnPropertyChanged(nameof(DisconnectArrowVisibility));
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
        public void StartConnecting()
        {
            ConnectArrowVisibility = Visibility.Visible;
        }
        public void StopConnecting()
        {
            ConnectArrowVisibility = Visibility.Hidden;
            DisconnectArrowVisibility = Visibility.Hidden;
        }
    }
}
