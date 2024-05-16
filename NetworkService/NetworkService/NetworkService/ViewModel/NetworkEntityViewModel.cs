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

        public NetworkEntityViewModel()
        {
            entities = new List<PowerConsumption>();
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
    }
}
