using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class LinesHolder : BindableBase
    {
        private PowerConsumption startingEntity;
        private PowerConsumption endingEntity;
        private Point from;
        private Point to;
        public LinesHolder(PowerConsumption startingEntity, PowerConsumption endingEntity, Point from, Point to)
        {
            this.startingEntity = startingEntity;
            this.endingEntity = endingEntity;
            this.from = from;
            this.to = to;
        }
        public PowerConsumption StartingEntity 
        {
            get {  return startingEntity; }
            set
            {
                startingEntity = value;
                OnPropertyChanged("StartingEntity");
            }
        }
        public PowerConsumption EndingEntity
        {
            get { return endingEntity; }
            set
            {
                endingEntity = value;
                OnPropertyChanged("EndingEntity");
            }
        }
        public Point From
        {
            get { return from; }
            set
            {
                from = value;
                OnPropertyChanged("From");
            }
        }
        public Point To
        {
            get { return to; }
            set
            {
                to = value;
                OnPropertyChanged("To");
            }
        }
    }
}
