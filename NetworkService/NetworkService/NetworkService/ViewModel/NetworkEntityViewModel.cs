using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.ViewModel
{
    public class NetworkEntityViewModel : BindableBase
    {
        private List<PowerConsumption> entities;
        private List<MeterType> availableTypes;
        private MeterType selectedFilterMeterType;
        private MeterType selectedAddingMeterType;
        private bool isLessThanChecked;
        private bool isGreaterThanChecked;
        private bool isEqualChecked;
        private string filterIdString;
        private string addingIdString;
        private string addingNameString;

        public NetworkEntityViewModel()
        {
            entities = new List<PowerConsumption>();
            availableTypes = new List<MeterType>() 
            {
                new MeterType("Select the meter type",""),
                new MeterType("Interval Meter", "../../Resources/IntervalMeter.png"),
                new MeterType("Smart Meter", "../../Resources/SmartMeter.png")
            };
            selectedFilterMeterType = availableTypes[0];
            selectedAddingMeterType = availableTypes[0];
            isLessThanChecked = false;
            isGreaterThanChecked = false;
            isEqualChecked = false;
            filterIdString = string.Empty;
            addingIdString = string.Empty;
            addingNameString = string.Empty;
        }
        public List<PowerConsumption> Entities
        {
            get
            {
                return entities;
            }
            set
            {
                entities = value;
                OnPropertyChanged("Entities");
            }
        }
        public List<MeterType> AvailableTypes
        {
            get { return availableTypes; }
            set
            {
                availableTypes = value;
                SetProperty(ref availableTypes, value);
            }
        }
        public MeterType SelectedFilterMeterType
        {
            get { return  selectedFilterMeterType; }
            set
            {
                selectedFilterMeterType = value;
                OnPropertyChanged("SelectedFilterMeterType");
            }
        }
        public MeterType SelectedAddingMeterType
        {
            get { return selectedAddingMeterType; }
            set
            {
                selectedAddingMeterType = value;
                OnPropertyChanged("SelectedAddingMeterType");
            }
        }
        public bool IsLessThanChecked
        {
            get { return isLessThanChecked; }
            set
            {
                if (isLessThanChecked != value)
                {
                    isLessThanChecked = value;
                }
                else
                {
                    isLessThanChecked = !value;
                }
                OnPropertyChanged("IsLessThanChecked");
            }
        }
        public bool IsGreaterThanChecked
        {
            get { return isGreaterThanChecked; }
            set
            {
                if (isGreaterThanChecked != value)
                {
                    isGreaterThanChecked = value;
                }
                else
                {
                    isGreaterThanChecked = !value;
                }
                OnPropertyChanged("IsGreaterThanChecked");
            }
        }
        public bool IsEqualChecked
        {
            get { return isEqualChecked; }
            set
            {
                if (isEqualChecked != value)
                {
                    isEqualChecked = value;
                }
                else
                {
                    isEqualChecked = !value;
                }
                OnPropertyChanged("IsEqualChecked");
            }
        }
        public string FilterIdString
        {
            get { return filterIdString; }
            set
            {
                if (value[value.Length - 1] >= '0' && value[value.Length - 1] <= '9')
                {
                    filterIdString = value;
                    OnPropertyChanged("FilterIdString");
                }
            }
        }
        public string AddingIdString
        {
            get { return addingIdString; }
            set
            {
                addingIdString = value;
                OnPropertyChanged("AddingIdString");
            }
        }
        public string AddingNameString
        {
            get { return addingIdString; }
            set
            {
                addingNameString = value;
                OnPropertyChanged("AddingNameString");
            }
        }
    }
}
