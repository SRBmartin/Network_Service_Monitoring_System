using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkService.ViewModel
{
    public class MeasurementGraphViewModel : BindableBase
    {
        private PowerConsumption selectedEntity;
        private ObservableCollection<PowerConsumption> availableEntities;
        private Thread t;
        public MeasurementGraphViewModel()
        {
            AvailableEntities = MainWindowViewModel.Entities;
            if(availableEntities.Count != 0)
            {
                SelectedEntity = AvailableEntities[0];
            }
            else
            {
                SelectedEntity = new PowerConsumption();
            }
            t = new Thread(UpdateEntities);
            t.IsBackground = true;
            t.Start();
        }
        public PowerConsumption SelectedEntity
        {
            get { return selectedEntity; }
            set
            {
                selectedEntity = value;
                OnPropertyChanged(nameof(SelectedEntity));
            }
        }
        public ObservableCollection<PowerConsumption> AvailableEntities
        {
            get { return availableEntities; }
            set
            {
                availableEntities = value;
                OnPropertyChanged(nameof(AvailableEntities));
            }
        }
        public void UpdateEntities()
        {
            AvailableEntities = MainWindowViewModel.Entities;
            if (!MainWindowViewModel.Entities.Any(pc => pc.Id == selectedEntity.Id) && MainWindowViewModel.Entities.Count > 0)
            {
                SelectedEntity = availableEntities[0];
            }
            else
            {
                SelectedEntity = MainWindowViewModel.Entities.FirstOrDefault(pc => pc.Id == SelectedEntity.Id);
            }
            Thread.Sleep(2000);
        }
    }
}
