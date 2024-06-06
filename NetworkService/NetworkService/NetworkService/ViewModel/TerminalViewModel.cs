using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Helpers.Common;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.ViewModel
{
    public enum CommandID
    {
        NoResponse,
        RequestUndoInfo,
        WaitingUserUndoConfirm,
        WaitingUserDeleteConfirm,
        Undo,
        RefreshFilter,
        NavigateToNEntities,
        NavigateToNDisplay,
        NavigateToMGraph,
        WaitingNavigationChange,
        GoBack,
        WaitingGoBackResponse
    }
    public class TerminalViewModel : BindableBase
    {
        private Terminal terminal;
        private MyICommand enterCommand;
        private CommandID expectingResponse = CommandID.NoResponse;
        private int idHolder = -1;
        public TerminalViewModel()
        {
            terminal = new Terminal();
            EnterCommand = new MyICommand(TerminalCommandParse);
            Messenger.Default.Register<string>(this, ReceiveStringResponseFromMaster);
        }

        public Terminal Terminal
        {
            get
            {
                return terminal;
            }
            set
            {
                SetProperty(ref terminal, value);
            }
        }
        public MyICommand EnterCommand
        {
            get { return enterCommand; }
            private set
            {       
                enterCommand = value;
            }
        }
        public void TerminalCommandParse()
        {
            if (Terminal.ConsumeTerminalCommand())
            {
                string[] commandParts = Terminal.ConsoleContent.Split(' ');
                switch (commandParts[0].Trim().ToLower())
                {
                    case "undo":
                        expectingResponse = CommandID.RequestUndoInfo;
                        Messenger.Default.Send<CommandID>(CommandID.RequestUndoInfo);
                        break;
                    case "yes":
                        if (expectingResponse == CommandID.WaitingUserUndoConfirm)
                        {
                            expectingResponse = CommandID.Undo;
                            Messenger.Default.Send<CommandID>(CommandID.Undo);
                        }
                        else if (expectingResponse == CommandID.WaitingUserDeleteConfirm)
                        {
                            PowerConsumption toDelete = MainWindowViewModel.Entities.FirstOrDefault(pc => pc.Id == idHolder);
                            if (toDelete != null && idHolder != -1)
                            {
                                Messenger.Default.Send<UndoActionHolder>(new UndoActionHolder(toDelete, ActionType.Delete));
                                MainWindowViewModel.Entities.Remove(toDelete);
                                Messenger.Default.Send<bool>(true);
                                SerializationHandler.SerializeEntitiesToFile(MainWindowViewModel.Entities);
                                Messenger.Default.Send<CommandID>(CommandID.RefreshFilter);
                                Terminal.TerminalContent += "The entity was deleted with a success.\n";
                                idHolder = -1;
                                expectingResponse = CommandID.NoResponse;
                            }
                        }
                        break;
                    case "no":
                        if (expectingResponse == CommandID.WaitingUserUndoConfirm)
                        {
                            Terminal.TerminalContent += "~ You cancelled Undo action.\n";
                            expectingResponse = CommandID.NoResponse;
                        }
                        else if (expectingResponse == CommandID.WaitingUserDeleteConfirm)
                        {
                            Terminal.TerminalContent += "You cancelled the deletion operation.\n";
                        }
                        break;
                    case "add":
                        if(commandParts.Length < 4)
                        {
                            Terminal.TerminalContent += Terminal.AddCommandHelp;
                        }
                        else
                        {
                            if (!int.TryParse(commandParts[1], out int meterType))
                            {
                                Terminal.TerminalContent += Terminal.AddCommandHelp;
                            }
                            else
                            {
                                MeterType mType = new MeterType(string.Empty, string.Empty);
                                switch(meterType)
                                {
                                    case 0:
                                        mType = new MeterType("Interval Meter", "pack://application:,,,/Resource/Images/IntervalMeter.png");
                                        break;
                                    case 1:
                                        mType = new MeterType("Smart Meter", "pack://application:,,,/Resource/Images/SmartMeter.png");
                                        break;
                                    default:
                                        Terminal.TerminalContent += Terminal.AddCommandHelp;
                                        break;
                                }
                                if (!int.TryParse(commandParts[2], out int id))
                                {
                                    Terminal.TerminalContent += Terminal.AddCommandHelp;
                                }
                                else
                                {
                                    string name = string.Empty;
                                    for(int i = 3; i < commandParts.Length; ++i)
                                    {
                                        name += commandParts[i] + " ";
                                    }
                                    if (string.IsNullOrEmpty(name.Trim()))
                                    {
                                        Terminal.TerminalContent += Terminal.AddCommandHelp;
                                    }
                                    else
                                    {
                                        if(MainWindowViewModel.Entities.Any(pc => pc.Id == id))
                                        {
                                            Terminal.TerminalContent += "The entity with that ID already exists.\n";
                                        }
                                        else
                                        {
                                            MainWindowViewModel.Entities.Add(new PowerConsumption(id.ToString(), name, mType));
                                            Messenger.Default.Send<UndoActionHolder>(new UndoActionHolder(new PowerConsumption(id.ToString(), name, mType), ActionType.Add));
                                            Messenger.Default.Send<bool>(true);
                                            SerializationHandler.SerializeEntitiesToFile(MainWindowViewModel.Entities);
                                            Messenger.Default.Send<CommandID>(CommandID.RefreshFilter);
                                            Terminal.TerminalContent += "Added new entity with success.\n";
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "delete":
                        if(commandParts.Length != 2)
                        {
                            Terminal.TerminalContent += Terminal.DeleteCommandHelp;
                        }
                        else
                        {
                            if (!int.TryParse(commandParts[1], out idHolder))
                            {
                                Terminal.TerminalContent += "ID must be a number.\n";
                            }
                            else
                            {
                                PowerConsumption toDelete = MainWindowViewModel.Entities.FirstOrDefault(pc => pc.Id == idHolder);
                                if (toDelete == null)
                                {
                                    Terminal.TerminalContent += "That ID doesn't exist.\n";
                                }
                                else
                                {
                                    Terminal.TerminalContent += $"Are you sure you want to delete:\n{toDelete}\n(yes/no)\n";
                                    expectingResponse = CommandID.WaitingUserDeleteConfirm;
                                }
                            }
                        }
                        break;
                    case "nav":
                        if(commandParts.Length != 2)
                        {
                            Terminal.TerminalContent += Terminal.NavCommandHelp;
                        }
                        else
                        {
                            if (int.TryParse(commandParts[1], out int navId))
                            {
                                if(navId >= 0 && navId <= 2)
                                {
                                    CommandID toSend = CommandID.NoResponse;
                                    switch (navId)
                                    {
                                        case 0:
                                            toSend = CommandID.NavigateToNEntities;
                                            break;
                                        case 1:
                                            toSend = CommandID.NavigateToNDisplay;
                                            break;
                                        case 2:
                                            toSend = CommandID.NavigateToMGraph;
                                            break;
                                    }
                                    expectingResponse = CommandID.WaitingNavigationChange;
                                    Messenger.Default.Send<CommandID>(toSend);
                                }
                                else
                                {
                                    Terminal.TerminalContent += "~ Tab ID must be in range [0-2].\n";
                                }
                            }
                            else
                            {
                                Terminal.TerminalContent += "~ Tab ID must be a number.\n";
                            }
                        }
                        break;
                    case "back":
                        expectingResponse = CommandID.WaitingGoBackResponse;
                        Messenger.Default.Send<CommandID>(CommandID.GoBack);
                        break;
                    default:
                        Terminal.TerminalContent += "~ That command doesn't exist.\n";
                        break;
                }
            }
            Terminal.ClearTerminalConsole();
        }
        public void ReceiveStringResponseFromMaster(string response)
        {
            switch (expectingResponse)
            {
                case CommandID.RequestUndoInfo:
                    Terminal.TerminalContent += $"~ {response}\n";
                    expectingResponse = CommandID.WaitingUserUndoConfirm;
                    break;
                case CommandID.Undo:
                case CommandID.WaitingNavigationChange:
                case CommandID.WaitingGoBackResponse:
                    Terminal.TerminalContent += $"~ {response}\n";
                    expectingResponse = CommandID.NoResponse;
                    break;
                default:
                    //skipping, probably won't ever happend 
                break;
            }
        }
    }
}
