using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class PowerConsumption : ValidationBase
    {
        private int id;
        private string name;
        private double value;
        MeterType type;
        public PowerConsumption(int id, string name, MeterType type)
        {
            this.id = id;
            this.name = name;
            this.value = value;
            this.type = type;
        }

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public double Value
        {
            get { return value; }
            set
            {
                this.value = value;
                OnPropertyChanged("Value");
            }
        }
        public MeterType Type
        {
            get { return type; }
            set
            {
                type = value;
                SetProperty(ref type, value);
            }
        }

        protected override void ValidateSelf()
        {
            throw new NotImplementedException();
        }
    }
}
