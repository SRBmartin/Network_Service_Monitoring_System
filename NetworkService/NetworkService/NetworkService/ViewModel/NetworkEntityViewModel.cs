using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Helpers.Common;
using NetworkService.Model;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NetworkService.ViewModel
{
    public class NetworkEntityViewModel : BindableBase
    {
        //private ObservableCollection<PowerConsumption> entities;
        private ObservableCollection<PowerConsumption> filteredEntities;
        private List<MeterType> availableTypes;
        private MeterType selectedFilterMeterType;
        private MeterType selectedAddingMeterType;
        private PowerConsumption enteredPowerConsumption;
        private PowerConsumption selectedPowerConsumption;
        private MyICommand addNewEntityCmd;
        private MyICommand deleteEntityCmd;
        private MyICommand filterEntityCmd;
        private MyICommand filterOffEntityCmd;
        private FilterHandler currentFilter; //ono sto je trenutno selektovano
        private FilterHandler activeFilter; //onaj filter koji je aktivan trenutno
        private string filterIdString;
        private string addingIdString;
        private string addingNameString;
        private bool undoResist { get; set; }
        private Visibility iDErrorVisibility;
        private Visibility addTypeErrorVisibility;
        private Visibility addNameErrorVisibility;
        private Visibility idFilterErrorVisibility;
        private Visibility buttonsFilterErrorVisibility;
        private readonly ConfirmationService _confirmationService;
        public NetworkEntityViewModel()
        {
            filteredEntities = MainWindowViewModel.Entities; //inicijalno prikaz u tabeli je isti kao i prikaz svih entiteta
            addNewEntityCmd = new MyICommand(AddNewEntity);
            deleteEntityCmd = new MyICommand(DeleteSelectedEntity);
            filterEntityCmd = new MyICommand(FilterOn);
            filterOffEntityCmd = new MyICommand(FilterOff);
            availableTypes = new List<MeterType>() 
            {
                new MeterType("Select the meter type",""),
                new MeterType("Interval Meter", "pack://application:,,,/Resource/Images/IntervalMeter.png"),
                new MeterType("Smart Meter", "pack://application:,,,/Resource/Images/SmartMeter.png")
            };
            enteredPowerConsumption = new PowerConsumption(string.Empty, string.Empty, availableTypes[0]);
            currentFilter = new FilterHandler();
            activeFilter = null;
            currentFilter.Type = availableTypes[0];
            selectedFilterMeterType = availableTypes[0];
            selectedAddingMeterType = availableTypes[0];
            undoResist = false;
            iDErrorVisibility = Visibility.Hidden;
            AddTypeErrorVisibility = Visibility.Hidden;
            AddNameErrorVisibility = Visibility.Hidden;
            idFilterErrorVisibility = Visibility.Hidden;
            buttonsFilterErrorVisibility = Visibility.Hidden;
            this._confirmationService = new ConfirmationService();
            Messenger.Default.Register<PowerConsumption>(this, ResolveUndoActiveFilter);
            Messenger.Default.Register<CommandID>(this, ResolveTerminalCommand);
        }
        public ObservableCollection<PowerConsumption> FilteredEntities
        {
            get { return filteredEntities; }
            set
            {
                filteredEntities = value;
                OnPropertyChanged("FilteredEntities");
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
        public MyICommand AddNewEntityCmd
        {
            get { return addNewEntityCmd; }
            set
            {
                addNewEntityCmd = value;
            }
        }
        public MyICommand DeleteEntityCmd
        {
            get { return deleteEntityCmd; }
            set
            {
                deleteEntityCmd = value;
            }
        }
        public MyICommand FilterEntityCmd
        {
            get { return filterEntityCmd; }
            set
            {
                filterEntityCmd = value;
                OnPropertyChanged("FilterEntityCmd");
            }
        }
        public MyICommand FilterOffEntityCmd
        {
            get { return filterOffEntityCmd; }
            set
            {
                filterOffEntityCmd = value;
                OnPropertyChanged("FilterOffEntityCmd");
            }
        }
        public PowerConsumption EnteredPowerConsumption
        {
            get { return enteredPowerConsumption; }
            set
            {
                enteredPowerConsumption = value;
                OnPropertyChanged("EnteredPowerConsumption");
            }
        }
        public PowerConsumption SelectedPowerConsumption
        {
            get { return selectedPowerConsumption; }
            set
            {
                selectedPowerConsumption = value;
                OnPropertyChanged("SelectedPowerConsumption");
            }
        }
        public FilterHandler CurrentFilter
        {
            get { return currentFilter; }
            set
            {
                currentFilter = value;
                OnPropertyChanged("CurrentFilter");
            }
        }
        public FilterHandler ActiveFilter
        {
            get { return activeFilter; }
            set
            {
                activeFilter = value;
                OnPropertyChanged("ActiveFilter");
            }
        }
        public Visibility IDErrorVisibility
        {
            get { return iDErrorVisibility; }
            set
            {
                iDErrorVisibility = value;
                OnPropertyChanged("IDErrorVisibility");
            }
        }
        public Visibility AddTypeErrorVisibility
        {
            get { return addTypeErrorVisibility; }
            set
            {
                addTypeErrorVisibility = value;
                OnPropertyChanged("AddTypeErrorVisibility");
            }
        }
        public Visibility AddNameErrorVisibility
        {
            get { return addNameErrorVisibility; }
            set
            {
                addNameErrorVisibility = value;
                OnPropertyChanged("AddNameErrorVisibility");
            }
        }
        public Visibility IdFilterErrorVisibility
        {
            get { return idFilterErrorVisibility; }
            set
            {
                idFilterErrorVisibility = value;
                OnPropertyChanged("IdFilterErrorVisibility");
            }
        }
        public Visibility ButtonsFilterErrorVisibility
        {
            get { return buttonsFilterErrorVisibility; }
            set
            {
                buttonsFilterErrorVisibility = value;
                OnPropertyChanged("ButtonsFilterErrorVisibility");
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
        public void AddNewEntity()
        {
            enteredPowerConsumption.Validate();
            if (enteredPowerConsumption.IsValid)
            {
                if (!MainWindowViewModel.Entities.Any(pc => pc.IdS.Equals(enteredPowerConsumption.IdS)))
                {
                    MainWindowViewModel.Entities.Add(new PowerConsumption(enteredPowerConsumption));
                    Messenger.Default.Send<UndoActionHolder>(new UndoActionHolder(new PowerConsumption(enteredPowerConsumption), ActionType.Add));
                    Messenger.Default.Send<bool>(true);
                    EnteredPowerConsumption.Type = availableTypes[0];
                    EnteredPowerConsumption.IdS = string.Empty;
                    EnteredPowerConsumption.Name = string.Empty;
                    Messenger.Default.Send<NotificationContent>(NotificationHandler.CreateNotification(NotificationType.Success, "Entity addition", "Entity was added successfully."));
                    SerializationHandler.SerializeEntitiesToFile(MainWindowViewModel.Entities);
                    if(activeFilter != null)
                    {
                        FilteredEntities = activeFilter.FilterAction(MainWindowViewModel.Entities);
                    }
                    IDErrorVisibility = Visibility.Hidden;
                    AddTypeErrorVisibility = Visibility.Hidden;
                    AddNameErrorVisibility = Visibility.Hidden;
                }
                else
                {
                    Messenger.Default.Send<NotificationContent>(NotificationHandler.CreateNotification(NotificationType.Error, "ID duplicate", "That ID already exists."));
                }
            }
            else
            {
                if (enteredPowerConsumption.ValidationErrors["ID"] != string.Empty)
                {
                    IDErrorVisibility = Visibility.Visible;
                }
                else
                {
                    IDErrorVisibility = Visibility.Hidden;
                }
                if (enteredPowerConsumption.ValidationErrors["Type"] != string.Empty)
                {
                    AddTypeErrorVisibility = Visibility.Visible;
                }
                else
                {
                    AddTypeErrorVisibility = Visibility.Hidden;
                }
                if (enteredPowerConsumption.ValidationErrors["Name"] != string.Empty)
                {
                    AddNameErrorVisibility = Visibility.Visible;
                }
                else
                {
                    AddNameErrorVisibility = Visibility.Hidden;
                }
            }
        }
        public void DeleteSelectedEntity()
        {
            if(selectedPowerConsumption == null)
            {
                Messenger.Default.Send<NotificationContent>(NotificationHandler.CreateNotification(NotificationType.Error, "Nothing selected", "No entity was selected from the table to be deleted."));
            }
            else
            {
                if(_confirmationService.Confirm($"You are about to delete entity:\n{selectedPowerConsumption}\nContinue?"))
                {
                    Messenger.Default.Send<UndoActionHolder>(new UndoActionHolder(selectedPowerConsumption, ActionType.Delete));
                    MainWindowViewModel.Entities.Remove(selectedPowerConsumption);
                    Messenger.Default.Send<bool>(true);
                    Messenger.Default.Send<NotificationContent>(NotificationHandler.CreateNotification(NotificationType.Success, "Delete", "Entity was deleted from the system."));
                    SerializationHandler.SerializeEntitiesToFile(MainWindowViewModel.Entities);
                    if(activeFilter != null)
                    {
                        FilteredEntities = activeFilter.FilterAction(MainWindowViewModel.Entities);
                    }
                }
                else
                {
                    Messenger.Default.Send<NotificationContent>(NotificationHandler.CreateNotification(NotificationType.Information, "Cancelled", "You cancelled a delete operation."));
                }
            }
        }
        public void FilterOn()
        {
            currentFilter.Validate();
            if (currentFilter.IsValid)
            {
                FilteredEntities = currentFilter.FilterAction(MainWindowViewModel.Entities);
                activeFilter = currentFilter;
                IdFilterErrorVisibility = Visibility.Hidden;
                ButtonsFilterErrorVisibility = Visibility.Hidden;
            }
            else
            {
                if (currentFilter.ValidationErrors["ID"] != string.Empty)
                {
                    IdFilterErrorVisibility = Visibility.Visible;
                }
                else
                {
                    IdFilterErrorVisibility = Visibility.Hidden;
                }
                if (currentFilter.ValidationErrors["Buttons"] != string.Empty)
                {
                    ButtonsFilterErrorVisibility = Visibility.Visible;
                }
                else
                {
                    ButtonsFilterErrorVisibility = Visibility.Hidden;
                }
            }
        }
        public void FilterOff()
        {
            if (activeFilter != null)
            {
                activeFilter = null;
                FilteredEntities = MainWindowViewModel.Entities;
                Messenger.Default.Send<NotificationContent>(NotificationHandler.CreateNotification(NotificationType.Information, "Filter off", "You removed the active filter."));
            }
            else
            {
                Messenger.Default.Send<NotificationContent>(NotificationHandler.CreateNotification(NotificationType.Information, "Filter off", "There is no active filter."));
            }
        }

        private void ResolveUndoActiveFilter(PowerConsumption toUndo)
        {
            if(activeFilter != null)
            {
                FilteredEntities = activeFilter.FilterAction(MainWindowViewModel.Entities);
            }
        }

        public void ResolveTerminalCommand(CommandID commandID)
        {
            if(commandID == CommandID.RefreshFilter)
            {
                if (activeFilter != null)
                {
                    FilteredEntities = activeFilter.FilterAction(MainWindowViewModel.Entities);
                }
            }
        }

    }
}
