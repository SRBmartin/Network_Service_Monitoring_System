using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class PowerConsumption : ValidationBase
    {
        private int id;
        private string idS;
        private string name;
        private double value;
        MeterType type;
        public PowerConsumption() { }
        public PowerConsumption(string idS, string name, MeterType type)
        {
            this.idS = idS;
            this.name = name;
            this.type = type;
            value = GetRandomNumberValue(0.01, 5.50);
        }
        public PowerConsumption(string idS, string name, MeterType type, double value)
        {
            this.idS = idS;
            this.name = name;
            this.value = value;
            this.type = type;
        }
        public PowerConsumption(PowerConsumption powerConsumption)
        {
            id = powerConsumption.id;
            idS = powerConsumption.idS;
            name = powerConsumption.name;
            value = powerConsumption.value;
            type = powerConsumption.type;
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
        public string IdS
        {
            get { return idS; }
            set
            {
                idS = value;
                OnPropertyChanged("IdS");
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
            get { return (double)Math.Round(value, 2); }
            set
            {
                this.value = value;
                OnPropertyChanged("Value");
                OnPropertyChanged("ValueS");
            }
        }
        public  string ValueS
        {
            get
            {
                if(value == -1)
                {
                    return "NaN";
                }
                else
                {
                    return value.ToString();
                }
            }
        }
        public MeterType Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged("Type");
            }
        }

        protected override void ValidateSelf()
        {
            if (string.IsNullOrEmpty(idS))
            {
                ValidationErrors["ID"] = "ID cannot be empty.\n";
            }
            else
            {
                if (!int.TryParse(IdS, out id))
                {
                    ValidationErrors["ID"] = "ID must be a number.\n";
                }

                else
                {
                    if (id <= 0)
                    {
                        ValidationErrors["ID"] += "ID value cannot be negative.\n";
                    }
                }
            }
            if (string.IsNullOrEmpty(name))
            {
                ValidationErrors["Name"] = "Name cannot be empty.";
            }
            if(type.Name.Equals("Select the meter type"))
            {
                ValidationErrors["Type"] = "Type wasn't selected.";
            }
        }

        public void SetDefaultCardValue()
        {
            Id = -1;
            IdS = "NaN";
            Name = string.Empty;
            Value = -1;
            Type = new MeterType(string.Empty, "pack://application:,,,/Resource/Images/NotExists.png");
        }
        private double GetRandomNumberValue(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
        public override string ToString()
        {
            return $"Id: {IdS}\nName: {name}\nType: {type.Name}\nCurrent Value: {(float)Math.Round(value, 2)}.";
        }
    }
}
