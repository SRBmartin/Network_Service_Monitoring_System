using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Helpers.Common
{
    public class FilterHandler : ValidationBase
    {
        private bool isLessThan;
        private bool isGreaterThan;
        private bool isEqual;
        private string idS;
        private MeterType type;
        public FilterHandler()
        {
            isLessThan = false;
            isGreaterThan = false;
            isEqual = false;
            idS = string.Empty;
            type = new MeterType("Select the meter type", "");
        }
        public FilterHandler(FilterHandler filterHandler)
        {
            isLessThan = filterHandler.isLessThan;
            isGreaterThan = filterHandler.isGreaterThan;
            isEqual = filterHandler.isEqual;
            idS = filterHandler.idS;
            type = filterHandler.type;
        }

        public bool IsLessThan 
        {
            get { return isLessThan; }
            set
            {
                if (isLessThan != value)
                {
                    isLessThan= value;
                }
                else
                {
                    isLessThan= !value;
                }
                OnPropertyChanged("IsLessThan");
            }
        }
        public bool IsGreaterThan
        {
            get { return isGreaterThan; }
            set
            {
                if (isGreaterThan != value)
                {
                    isGreaterThan= value;
                }
                else
                {
                    isGreaterThan= !value;
                }
                OnPropertyChanged("IsGreaterThan");
            }
        }
        public bool IsEqual
        {
            get { return isEqual; }
            set
            {
                if (isEqual != value)
                {
                    isEqual= value;
                }
                else
                {
                    isEqual= !value;
                }
                OnPropertyChanged("IsEqual");
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
        public MeterType Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged("Type");
            }
        }

        public ObservableCollection<PowerConsumption> FilterAction(ObservableCollection<PowerConsumption> toFilter)
        {
            ObservableCollection<PowerConsumption> retVal = new ObservableCollection<PowerConsumption>();
            if(type.Name.Equals("Select the meter type"))
            {
                retVal = toFilter;
            }
            else
            {
                retVal = MeterType.FilterByType(toFilter, type.Name);
            }
            int id = -1;
            if (int.TryParse(IdS, out id))
            {
                if (isEqual)
                {
                    if (isLessThan)
                    {
                        retVal = new ObservableCollection<PowerConsumption>(retVal.Where(pc => pc.Id <= id).ToList());
                    }
                    else if (isGreaterThan)
                    {
                        retVal = new ObservableCollection<PowerConsumption>(retVal.Where(pc => pc.Id >= id).ToList());
                    }
                    else
                    {
                        retVal = new ObservableCollection<PowerConsumption>(retVal.Where(pc => pc.Id == id).ToList());
                    }
                }
                else
                {
                    if (IsLessThan)
                    {
                        retVal = new ObservableCollection<PowerConsumption>(retVal.Where(pc => pc.Id < id).ToList());
                    }
                    else
                    {
                        retVal = new ObservableCollection<PowerConsumption>(retVal.Where(pc => pc.Id > id).ToList());
                    }
                }
            }
            return retVal;
        }

        protected override void ValidateSelf()
        {
            if ((isLessThan || isGreaterThan || isEqual) && string.IsNullOrEmpty(idS))
            {
                ValidationErrors["ID"] = "You must enter an ID.";
            }
            else if (!(isLessThan || isGreaterThan || isEqual) && !string.IsNullOrEmpty(idS))
            {
                ValidationErrors["Buttons"] = "You must select an operation.";
            }
        }
    }
}
