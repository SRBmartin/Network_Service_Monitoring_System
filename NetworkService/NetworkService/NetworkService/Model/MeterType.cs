using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class MeterType : BindableBase
    {
        private string name;
        private string img_path;
        public MeterType(string name, string img_path)
        {
            this.name = name;
            this.img_path = img_path;
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
        public string Img_Path
        {
            get { return img_path; }
            set
            {
                img_path = value;
                OnPropertyChanged("Img_Path");
            }
        }

        public static ObservableCollection<PowerConsumption> FilterByType(ObservableCollection<PowerConsumption> toFilter, string typeName)
        {
            return new ObservableCollection<PowerConsumption>(toFilter.Where(pc => pc.Type != null && pc.Type.Name.Equals(typeName)).ToList());
        }
    }
}
