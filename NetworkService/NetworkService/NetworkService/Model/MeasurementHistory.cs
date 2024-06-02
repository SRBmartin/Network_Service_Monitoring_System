using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NetworkService.Model
{
    public class MeasurementHistory : BindableBase
    {
        private string time;
        private double value;
        int positionIndex;
        //private Brush AlarmBackground = new SolidColorBrush(Color.FromRgb(250, 204, 197));
        //private Brush NormalBackground = new SolidColorBrush(Color.FromRgb(138, 175, 255));
        public MeasurementHistory()
        {
            time = string.Empty;
            value = 0;
            positionIndex = 0;
        }
        public MeasurementHistory(string time, double value, int positionIndex)
        {
            this.time = time;
            this.value = value;
            this.positionIndex = positionIndex;
        }
        public string Time
        {
            get { return time; }
            set
            {
                time = value;
                OnPropertyChanged(nameof(Time));
            }
        }
        public  int PositionIndex
        {
            get { return positionIndex; }
            set
            {
                positionIndex = value;
                OnPropertyChanged(nameof(positionIndex));
            }
        }
        public double Value
        {
            get { return value; }
            set
            {
                this.value = value;
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(Diametar));
                OnPropertyChanged(nameof(CanvasBottom));
                OnPropertyChanged(nameof(CanvasRight));
                OnPropertyChanged(nameof(Background));
            }
        }
        public double Diametar
        {
            get { return (((value - 0.01) / (5.5 - 0.01)) * 56); }
        }
        public double CanvasRight
        {
            get
            {
                double centerRight = 82;
                switch (positionIndex)
                {
                    case 1:
                        centerRight = 82;
                        break;
                    case 2:
                        centerRight = 166;
                        break;
                    case 3:
                        centerRight = 250;
                        break;
                    case 4:
                        centerRight = 334;
                        break;
                    case 5:
                        centerRight = 418;
                        break;
                }
                return centerRight - Diametar / 2;
            }
        }
        public  double CanvasBottom
        {
            get
            {
                return 200 - Diametar / 2;
            }
        }
        public Brush Background
        {
            get
            {
                if(value >= PowerConsumption.MinValue && value <= PowerConsumption.MaxValue)
                {
                    return new SolidColorBrush(Color.FromRgb(138, 175, 255)); ;
                }
                return new SolidColorBrush(Color.FromRgb(250, 204, 197)); ;
            }
        }
    }
}
