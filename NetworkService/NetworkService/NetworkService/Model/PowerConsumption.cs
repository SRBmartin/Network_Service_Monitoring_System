using NetworkService.Helpers;
using NetworkService.Helpers.Common;
using NetworkService.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class PowerConsumption : ValidationBase
    {
        private int id;
        private string idS;
        private string name;
        private double value;
        private string valueS;
        private MeterType type;
        private ObservableCollection<MeasurementHistory> measurementHistory = new ObservableCollection<MeasurementHistory>();
        private bool skip = true;
        public static readonly double MinValue = 0.34;
        public static readonly double MaxValue = 2.73;
        public PowerConsumption() { }
        public PowerConsumption(string idS, string name, MeterType type)
        {
            this.idS = idS;
            int.TryParse(idS, out id);
            this.name = name;
            this.type = type;
            value = Math.Round(GetRandomNumberValue(0.01, 5.50), 2);
            ValueS = value.ToString();
            MeasurementHistory.Add(new MeasurementHistory());
            MeasurementHistory.Add(new MeasurementHistory());
            MeasurementHistory.Add(new MeasurementHistory());
            MeasurementHistory.Add(new MeasurementHistory());
            MeasurementHistory.Add(new MeasurementHistory());
        }
        public PowerConsumption(string idS, string name, MeterType type, double value)
        {
            this.idS = idS;
            this.name = name;
            this.value = Math.Round(value, 2);
            ValueS = this.value.ToString();
            this.type = type;
            MeasurementHistory.Add(new MeasurementHistory());
            MeasurementHistory.Add(new MeasurementHistory());
            MeasurementHistory.Add(new MeasurementHistory());
            MeasurementHistory.Add(new MeasurementHistory());
            MeasurementHistory.Add(new MeasurementHistory());
        }
        public PowerConsumption(string idS, string name, MeterType type, double value, ObservableCollection<MeasurementHistory> measurementHistory)
        {
            this.idS = idS;
            this.name = name;
            this.value = Math.Round(value, 2);
            ValueS = this.value.ToString();
            this.type = type;
            this.measurementHistory = measurementHistory;
            if(measurementHistory.Count != 5)
            {
                MeasurementHistory.Add(new MeasurementHistory());
                MeasurementHistory.Add(new MeasurementHistory());
                MeasurementHistory.Add(new MeasurementHistory());
                MeasurementHistory.Add(new MeasurementHistory());
                MeasurementHistory.Add(new MeasurementHistory());
            }
        }
        public PowerConsumption(PowerConsumption powerConsumption)
        {
            id = powerConsumption.id;
            idS = powerConsumption.idS;
            name = powerConsumption.name;
            value = Math.Round(powerConsumption.value, 2);
            ValueS = value.ToString();
            type = powerConsumption.type;
            measurementHistory = powerConsumption.MeasurementHistory;
            if(measurementHistory.Count != 5)
            {
                MeasurementHistory.Add(new MeasurementHistory());
                MeasurementHistory.Add(new MeasurementHistory());
                MeasurementHistory.Add(new MeasurementHistory());
                MeasurementHistory.Add(new MeasurementHistory());
                MeasurementHistory.Add(new MeasurementHistory());
            }
        }
        public ObservableCollection<MeasurementHistory> MeasurementHistory
        {
            get { return measurementHistory; }
            set
            {
                measurementHistory = value;
                OnPropertyChanged(nameof(MeasurementHistory));
            }
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
                if (skip || measurementHistory == null)
                {
                    if (MainWindowViewModel.newValueIncoming)
                    {
                        if (measurementHistory.Count != 5)
                        {
                            MeasurementHistory.Add(new MeasurementHistory());
                            MeasurementHistory.Add(new MeasurementHistory());
                            MeasurementHistory.Add(new MeasurementHistory());
                            MeasurementHistory.Add(new MeasurementHistory());
                            MeasurementHistory.Add(new MeasurementHistory());
                        }
                        MeasurementHistory.Add(new MeasurementHistory(DateTime.Now.ToString("HH:mm"), this.value, 1));
                        if (MeasurementHistory.Count > 5)
                        {
                            MeasurementHistory.RemoveAt(0); //obrisi  prvi (najraniji)
                        }
                        for (int i = 0, j = 5; i < 5; i++)
                        {
                            MeasurementHistory[i].PositionIndex = j--;
                        }
                        MainWindowViewModel.newValueIncoming = false;
                    }
                }
                else
                {
                    skip = false;
                }
                OnPropertyChanged(nameof(MeasurementHistory));
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
                    return Math.Round(value, 2).ToString();
                }
            }
            set
            {
                valueS = value;
                OnPropertyChanged(nameof(ValueS));
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
        public void SetCardValue(PowerConsumption takeValues)
        {
            Id = takeValues.Id;
            IdS = takeValues.IdS;
            Name = takeValues.Name;
            Type = takeValues.Type;
            Value = takeValues.Value;
            ValueS = takeValues.ValueS;
        }
        private double GetRandomNumberValue(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
        public bool IsValueAlarming()
        {
            return value < MinValue || value > MaxValue;
        }
        public override string ToString()
        {
            return $"Id: {IdS}\nName: {name}\nType: {type.Name}\nCurrent Value: {(float)Math.Round(value, 2)}.";
        }
    }
}
