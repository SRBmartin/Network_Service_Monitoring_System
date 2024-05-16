using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using NetworkService.Helpers;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        public UndoActionHolder undoAction;
        public StartingViewModel startingViewModel { get; set; }
        private TerminalViewModel terminalViewModel;
        private NetworkEntityViewModel networkEntityViewModel;
        public MyICommand<Window> ExitAppCommand { get; private set; }
        public MyICommand<string> NavigationCommand { get; private set; }

        private BindableBase currentViewModel;
        private int count = 15; // Inicijalna vrednost broja objekata u sistemu
                                // ######### ZAMENITI stvarnim brojem elemenata
                                //           zavisno od broja entiteta u listi

        public MainWindowViewModel()
        {
            createListener(); //Povezivanje sa serverskom aplikacijom
            UndoAction = new UndoActionHolder();

            ExitAppCommand = new MyICommand<Window>(ExitApp);
            NavigationCommand = new MyICommand<string>(Navigate);

            networkEntityViewModel = new NetworkEntityViewModel();
            terminalViewModel = new TerminalViewModel();
            startingViewModel = new StartingViewModel();
            CurrentViewModel = startingViewModel;
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
                SetProperty(ref  networkEntityViewModel, value);
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
                SetProperty(ref undoAction, value);
            }
        }

        private void createListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 25675);
            tcp.Start();

            var listeningThread = new Thread(() =>
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
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(count.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Console.WriteLine(incomming); //Na primer: "Entitet_1:272"

                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji

                        }
                    }, null);
                }
            });

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

                        break;

                    case "MeasurementGraphView":

                        break;
                }
            }
        }

        private void ExitApp(Window toClose)
        {
            toClose.Close();
        }
        
    }
}
