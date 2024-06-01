using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Helpers.Common;
using NetworkService.Helpers.Undo;
using NetworkService.Model;
using Notification.Wpf;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        private static ObservableCollection<PowerConsumption> entities;
        public UndoActionHolder undoAction;
        public StartingViewModel startingViewModel { get; set; }
        private TerminalViewModel terminalViewModel;
        private NetworkEntityViewModel networkEntityViewModel;
        private NetworkDisplayViewModel networkDisplayViewModel;
        public MyICommand<Window> ExitAppCommand { get; private set; }
        public MyICommand<string> NavigationCommand { get; private set; }
        private MyICommand undoActionCommand;

        private BindableBase currentViewModel;

        private NotificationManager notificationManager;
        private ConfirmationService _confirmationService;

        public MainWindowViewModel()
        {
            Entities = SerializationHandler.DeserializeEntitiesFromFile();
            createListener(); //Povezivanje sa serverskom aplikacijom
            UndoAction = new UndoActionHolder();

            ExitAppCommand = new MyICommand<Window>(ExitApp);
            NavigationCommand = new MyICommand<string>(Navigate);
            UndoActionCommand = new MyICommand(DoUndo);

            networkEntityViewModel = new NetworkEntityViewModel();
            networkDisplayViewModel = new NetworkDisplayViewModel();
            terminalViewModel = new TerminalViewModel();
            startingViewModel = new StartingViewModel();
            notificationManager = new NotificationManager();
            _confirmationService = new ConfirmationService();
            CurrentViewModel = startingViewModel;
            Messenger.Default.Register<NotificationContent>(this, NotificationHandler.ShowToastNotification);
            Messenger.Default.Register<UndoActionHolder>(this, ReceiveUndoActionHolder);
        }

        public static ObservableCollection<PowerConsumption> Entities
        {
            get
            {
                return entities;
            }
            set
            {
                entities = value;
                OnStaticPropertyChanged(nameof(Entities));
                Messenger.Default.Send<bool>(true);
            }
        }
        public BindableBase CurrentViewModel
        {
            get
            {
                return currentViewModel;
            }
            set
            {
                SetProperty(ref currentViewModel, value);
            }
        }
        public TerminalViewModel TerminalViewModel
        {
            get
            {
                return terminalViewModel;
            }
            set
            {
                SetProperty(ref terminalViewModel, value);
            }
        }

        public NetworkEntityViewModel NetworkEntityViewModel
        {
            get
            {
                return networkEntityViewModel;
            }
            set
            {
                SetProperty(ref networkEntityViewModel, value);
            }
        }
        public NetworkDisplayViewModel NetworkDisplayViewModel
        {
            get
            {
                return networkDisplayViewModel;
            }
            set
            {
                SetProperty(ref networkDisplayViewModel, value);
            }
        }

        public UndoActionHolder UndoAction
        {
            get
            {
                return undoAction;
            }
            set
            {
                undoAction = value;
                OnPropertyChanged("UndoAction");
            }
        }
        public MyICommand UndoActionCommand
        {
            get { return undoActionCommand; }
            private set
            {
                undoActionCommand = value;
            }
        }

        private void func(TcpListener tcp)
        {
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                 ThreadPool.QueueUserWorkItem(param =>
                    {
                        //Prijem poruke
                        NetworkStream stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        //Primljena poruka je sacuvana u incomming stringu
                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            //Response
                            /* Umesto sto se ovde salje count.ToString(), potrebno je poslati 
                             * duzinu liste koja sadrzi sve objekte pod monitoringom, odnosno
                             * njihov ukupan broj (NE BROJATI OD NULE, VEC POSLATI UKUPAN BROJ)
                             * */
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(entities.Count.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            Console.WriteLine(incomming); //Na primer: "Entitet_1:272"
                            int index = int.Parse(incomming.Split(':')[0].Split('_')[1]);
                            float value = float.Parse(incomming.Split(':')[1]);
                            Entities[index].Value = value;
                            SerializationHandler.SerializeEntitiesToFile(Entities);
                        }
                    }, null);
                }
            }
        }

        private void createListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 25675);
            tcp.Start();

            var listeningThread = new Thread(() => func(tcp));

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }

        private void Navigate(string pageName)
        {
            {
                switch (pageName)
                {
                    case "NetworkEntityView":
                        CurrentViewModel = networkEntityViewModel;
                        break;
                    case "NetworkDisplayView":
                        CurrentViewModel = networkDisplayViewModel;
                        Messenger.Default.Send<bool>(true);
                        break;

                    case "MeasurementGraphView":

                        break;
                }
            }
        }
        private void ReceiveUndoActionHolder(UndoActionHolder toUndo)
        {
            UndoAction = new UndoActionHolder(toUndo);
        }
        private void DoUndo()
        {
            if(undoAction.ActionId != ActionType.NoAction)
            {
                switch (undoAction.ActionId)
                {
                    case ActionType.Add:
                        if(_confirmationService.Confirm($"Are you sure you want to undo the addition of:\n{undoAction.Entity}\nContinue?"))
                        {
                            PowerConsumption toDelete = Entities.FirstOrDefault(e => e.Id == undoAction.Entity.Id);
                            if(toDelete != null)
                            {
                                Entities.Remove(toDelete);
                                Messenger.Default.Send<PowerConsumption>(UndoAction.Entity);
                                Messenger.Default.Send<bool>(true);
                                SerializationHandler.SerializeEntitiesToFile(Entities);
                                NotificationHandler.ShowToastNotification(NotificationHandler.CreateNotification(NotificationType.Success, "Undo", "Undo option was executed with success."));
                            }
                        }
                        else
                        {
                            NotificationHandler.ShowToastNotification(NotificationHandler.CreateNotification(NotificationType.Information, "Undo", "Undo option was cancelled."));
                        }
                        break;
                    case ActionType.Delete:
                        if(_confirmationService.Confirm($"Are you sure you want to undo the deletion of:\n{undoAction.Entity}\n"))
                        {
                            Entities.Add(undoAction.Entity);
                            Messenger.Default.Send<PowerConsumption>(UndoAction.Entity);
                            Messenger.Default.Send<bool>(true);
                            SerializationHandler.SerializeEntitiesToFile(Entities);
                            NotificationHandler.ShowToastNotification(NotificationHandler.CreateNotification(NotificationType.Success, "Undo", "Undo option was executed with success."));
                        }
                        else
                        {
                            NotificationHandler.ShowToastNotification(NotificationHandler.CreateNotification(NotificationType.Information, "Undo", "Undo option was cancelled."));
                        }
                        break;
                }
                UndoAction = new UndoActionHolder();
            }
        }
        private void ExitApp(Window toClose)
        {
            toClose.Close();
        }

    }
}
