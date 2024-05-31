using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class EntitiesByType : BindableBase
    {
        public string TypeName { get; set; }
        private ObservableCollection<PowerConsumption> entities;
        public EntitiesByType(string typeName)
        {
            TypeName = typeName;
            Entities = new ObservableCollection<PowerConsumption>();
        }
        public ObservableCollection<PowerConsumption> Entities
        {
            get
            {
                return entities;
            }
            set
            {
                entities = value;
                OnPropertyChanged(nameof(Entities));
            }
        }
    }
}
