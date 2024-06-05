using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Helpers.Common;
using NetworkService.Helpers.Interface;
using NetworkService.Helpers.Styles;
using NetworkService.Model;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace NetworkService.ViewModel
{
    public class NetworkDisplayViewModel : BindableBase
    {
        private ObservableCollection<EntitiesByType> allEntities;
        private ObservableCollection<PowerConsumption> cardsEntities;
        private ObservableCollection<DisplayCardVisualRepresentation> cardsVisual;
        private ObservableCollection<LinesHolder> linesHolder;
        private PowerConsumption selectedEntity;
        private PowerConsumption draggedItem;
        private MyICommand<PowerConsumption> treeViewSelectionChangedCommand;
        private MyICommand<string> dropOnCardCommand;
        private MyICommand<string> startDragFromCardCommand;
        private MyICommand<string> removeEntityFromCardCommand;
        private MyICommand<string> rightClickCardCommand;
        private MyICommand<string> mouseEnterCard;
        private MyICommand<string> mouseLeaveCard;
        private MyICommand mouseButtonDownCommand;
        private MyICommand mouseButtonUpCommand;
        private MyICommand mouseButtonMoveCommand;
        private ConfirmationService _confirmationService;
        private bool isMouseDown;
        private int selectedCardIndexFirst;
        private int selectedCardIndexSecond;
        Thread t;

        private readonly string cardEntitiesSerializationPath = "../../Resource/json/cardEntities.json";
        private readonly string linesConnectionsSerializationPath = "../../Resource/json/linesConnections.json";

        public NetworkDisplayViewModel()
        {
            _confirmationService = new ConfirmationService();
            AllEntities = new ObservableCollection<EntitiesByType>();
            LinesHolder = new ObservableCollection<LinesHolder>();
            CardsVisual = new ObservableCollection<DisplayCardVisualRepresentation>();
            treeViewSelectionChangedCommand = new MyICommand<PowerConsumption>(OnTreeViewItemSelected);
            dropOnCardCommand = new MyICommand<string>(OnCardDrop);
            startDragFromCardCommand = new MyICommand<string>(OnStartDraggingFromCard);
            removeEntityFromCardCommand = new MyICommand<string>(RemoveFromCard);
            rightClickCardCommand = new MyICommand<string>(OnCardRightClick);
            MouseEnterCard = new MyICommand<string>(OnMouseEnterCard);
            MouseLeaveCard = new MyICommand<string>(OnMouseLeaveCard);
            MouseButtonDownCommand = new MyICommand(MouseButtonDownHandler);
            MouseButtonUpCommand = new MyICommand(MouseButtonUpHandler);
            MouseButtonMoveCommand = new MyICommand(MouseMoveHandler);
            
            draggedItem = null;
            ReloadEntities(true);
            isMouseDown = false;
            selectedCardIndexFirst = -1;
            selectedCardIndexSecond = -1;
            t = new Thread(Update);
            t.IsBackground = true;
            t.Start();
            Messenger.Default.Register<bool>(this, ReloadEntities);
        }
        public ObservableCollection<EntitiesByType> AllEntities
        {
            get { return allEntities; }
            set
            {
                allEntities = value;
                OnPropertyChanged("AllEntities");
            }
        }
        public ObservableCollection<PowerConsumption> CardsEntities
        {
            get { return cardsEntities; }
            set
            {
                cardsEntities = value;
                OnPropertyChanged("CardsEntities");
            }
        }
        public ObservableCollection<DisplayCardVisualRepresentation> CardsVisual
        {
            get { return cardsVisual; }
            set
            {
                cardsVisual = value;
                OnPropertyChanged(nameof(CardsVisual));
            }
        }
        public ObservableCollection<LinesHolder> LinesHolder
        {
            get { return linesHolder; }
            set
            {
                linesHolder = value;
                OnPropertyChanged("LinesHolder");
            }
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
        public MyICommand<PowerConsumption> TreeViewSelectionChangedCommand
        {
            get { return treeViewSelectionChangedCommand; }
            set { treeViewSelectionChangedCommand = value; }
        }
        public MyICommand<string> DropOnCardCommand
        {
            get { return dropOnCardCommand; }
            private set
            {
                dropOnCardCommand = value;
            }
        }
        public MyICommand<string> StartDragFromCardCommand
        {
            get { return startDragFromCardCommand; }
            private set
            {
                startDragFromCardCommand = value;
            }
        }
        public MyICommand<string> RemoveEntityFromCardCommand
        {
            get { return removeEntityFromCardCommand; }
            private set
            {
                removeEntityFromCardCommand = value;
            }
        }
        public MyICommand<string> RightClickCardCommand
        {
            get { return rightClickCardCommand; }
            private set
            {
                rightClickCardCommand = value;
            }
        }
        public MyICommand<string> MouseEnterCard
        {
            get { return mouseEnterCard; }
            private set
            {
                mouseEnterCard = value;
            }
        }
        public MyICommand<string> MouseLeaveCard
        {
            get { return mouseLeaveCard; }
            private set
            {
                mouseLeaveCard = value;
            }
        }
        public MyICommand MouseButtonDownCommand
        {
            get { return mouseButtonDownCommand; }
            set
            {
                mouseButtonDownCommand = value;
            }
        }
        public MyICommand MouseButtonUpCommand
        {
            get { return mouseButtonUpCommand; }
            set
            {
                mouseButtonUpCommand = value;
            }
        }
        public MyICommand MouseButtonMoveCommand
        {
            get { return mouseButtonMoveCommand; }
            set
            {
                mouseButtonMoveCommand = value;
            }
        }
        public void ReloadEntities(bool reload)
        {
            if (reload)
            {
                try
                {
                    AllEntities.Clear();
                }
                catch (NotSupportedException)
                {
                    return; //preskociti jer se nekad duplira Thread bez ociglednog razloga i to je Slepi Thread
                }
                EntitiesByType intervalType = new EntitiesByType("Interval Meter");
                EntitiesByType smartType = new EntitiesByType("Smart Meter");
                foreach (PowerConsumption i in MainWindowViewModel.Entities)
                {
                    if (i.Type.Name.Equals("Interval Meter"))
                    {
                        intervalType.Entities.Add(new PowerConsumption(i));
                    }
                    else
                    {
                        smartType.Entities.Add(new PowerConsumption(i));
                    }
                }
                if(intervalType.Entities.Count > 0)
                {
                    SelectedEntity = intervalType.Entities[0];
                }else if(smartType.Entities.Count > 0)
                {
                    selectedEntity = smartType.Entities[0];
                }
                AllEntities.Add(intervalType);
                AllEntities.Add(smartType);
                InitCards();
                InitLinesConnections();
            }
        }
        private void OnTreeViewItemSelected(object item)
        {
            if(item is PowerConsumption selectedPC)
            {
                SelectedEntity = selectedPC;
            }
        }
        private void MouseButtonDownHandler()
        {
            isMouseDown = true;
            draggedItem = selectedEntity;
        }
        private void MouseButtonUpHandler()
        {
            isMouseDown = false;
            draggedItem = null;
        }
        private void MouseMoveHandler()
        {
            if (isMouseDown && draggedItem != null)
            {
            }
        }
        private void InitCards()
        {
            CardsEntities = new ObservableCollection<PowerConsumption>();
            CardsEntities = SerializationHandler.DeserializeEntitiesFromFile(cardEntitiesSerializationPath);
            foreach(PowerConsumption i in CardsEntities)
            {
                if (MainWindowViewModel.Entities.Any(pc => pc.Id == i.Id))
                {
                    RemoveFromEntitiesGroup(i);
                }
                else
                {
                    i.SetDefaultCardValue();
                }
                CardsVisual.Add(new DisplayCardVisualRepresentation());
            }
            //await Task.Run(async () => MainWindowViewModel.SaveCards(new ObservableCollection<PowerConsumption>(CardsEntities)));
            MainWindowViewModel.SaveCards(new ObservableCollection<PowerConsumption>(CardsEntities));
        }
        private void OnCardDrop(string cardIndex)
        {
            if (draggedItem != null)
            {
                int index = int.Parse(cardIndex);
                if(draggedItem.Id == cardsEntities[index].Id)
                {
                    draggedItem = null;
                    return;
                }
                if (cardsEntities[index].Id == -1)
                {
                    PowerConsumption tmp = new PowerConsumption(draggedItem);
                    if (cardsEntities.Any(e => e.Id == draggedItem.Id))
                    {
                        RemoveFromCardEntities(draggedItem);
                        OnChangedPosition(draggedItem, index);
                    }
                    else
                    {
                        RemoveFromEntitiesGroup(draggedItem);
                    }
                    CardsEntities[index] = new PowerConsumption(tmp);
                    //await Task.Run(async () => MainWindowViewModel.SaveCards(new ObservableCollection<PowerConsumption>(CardsEntities)));
                    MainWindowViewModel.SaveCards(new ObservableCollection<PowerConsumption>(CardsEntities));
                    CardsVisual.ToList().ForEach(cv => cv.StopConnecting());
                    selectedCardIndexFirst = -1;
                    selectedCardIndexSecond = -1;
                    CheckAlarmingValues();
                    Messenger.Default.Send<NotificationContent>(NotificationHandler.CreateNotification(NotificationType.Success, "Drag & Drop", $"Entity \"{tmp.Name}\" was dropped on a card."));
                }
                else
                {
                    Messenger.Default.Send<NotificationContent>(NotificationHandler.CreateNotification(NotificationType.Error, "Drag & Drop", "There is an entity on that card already."));
                }
                draggedItem = null;
            }
        }
        private void OnStartDraggingFromCard(string cardIndex)
        {
            int index = int.Parse(cardIndex);
            if (CardsEntities[index].Id != -1)
            {
                draggedItem = new PowerConsumption(cardsEntities[index]);
                if(selectedCardIndexFirst != -1)
                {
                    CardsVisual[selectedCardIndexFirst].StopConnecting();
                    selectedCardIndexFirst = -1;
                    if(selectedCardIndexSecond != -1)
                    {
                        CardsVisual[selectedCardIndexSecond].StopConnecting();
                        selectedCardIndexSecond = -1;
                    }
                }
            }
            else
            {
                draggedItem = null;
            }
        }
        private void RemoveFromCardEntities(PowerConsumption toRemove)
        {
            for(int i = 0; i < cardsEntities.Count; ++i)
            {
                if (cardsEntities[i].Id == toRemove.Id)
                {
                    cardsEntities[i].SetDefaultCardValue();
                    break;
                }
            }
        }
        private void RemoveFromEntitiesGroup(PowerConsumption toRemove)
        {
            int index = -1;
            if (toRemove.Type.Name.Equals("Smart Meter"))
            {
                index = 1;
            }
            else
            {
                index = 0;
            }
            AllEntities[index].Entities.Remove(AllEntities[index].Entities.FirstOrDefault(e => e.IdS.Equals(toRemove.IdS)));
        }
        private void RemoveFromCard(string cardIndex)
        {
            int index = int.Parse(cardIndex);
            if (CardsEntities[index].Id != -1)
            {
                if (_confirmationService.Confirm($"Are you sure you want to remove the following entity from the card?\n{CardsEntities[index]}\nConfirm?"))
                {
                    int inx = 0;
                    if (cardsEntities[index].Type.Name.Equals("Smart Meter"))
                    {
                        inx = 1;
                    }
                    else
                    {
                        inx = 0;
                    }
                    AllEntities[inx].Entities.Add(new PowerConsumption(cardsEntities[index]));
                    LinesOnRemoveFromCard(cardsEntities[index]);
                    CardsVisual[index].StopConnecting();
                    cardsEntities[index].SetDefaultCardValue();
                    //await Task.Run(async () => MainWindowViewModel.SaveCards(new ObservableCollection<PowerConsumption>(CardsEntities)));
                    MainWindowViewModel.SaveCards(new ObservableCollection<PowerConsumption>(CardsEntities));
                    CheckAlarmingValues();
                    Messenger.Default.Send<NotificationContent>(NotificationHandler.CreateNotification(NotificationType.Success, "Remove from card", "Removing entity from the card was successful."));
                }
                else
                {
                    Messenger.Default.Send<NotificationContent>(NotificationHandler.CreateNotification(NotificationType.Information, "Remove from card", "Removing entity from the card was cancelled."));
                }
            }
            else
            {
                Messenger.Default.Send<NotificationContent>(NotificationHandler.CreateNotification(NotificationType.Error, "Remove from card", "There is no entity in the card to remove."));
            }
        }

        private void LinesOnRemoveFromCard(PowerConsumption removedEntity)
        {
            List<LinesHolder> toRemove = LinesHolder.Where(lh => lh.StartingEntity.Id == removedEntity.Id || lh.EndingEntity.Id == removedEntity.Id).ToList();
            if(toRemove.Count > 0)
            {
                foreach(LinesHolder i in toRemove)
                {
                    LinesHolder.Remove(i);
                }
            }
            SerializationHandler.SerializeEntitiesToFile(LinesHolder, linesConnectionsSerializationPath);
        }

        private void OnCardRightClick(string cardIndex)
        {
            int index = int.Parse(cardIndex);
            if (CardsEntities[index].Id != -1)
            {
                if(selectedCardIndexFirst == -1)
                {
                    selectedCardIndexFirst = index;
                    CardsVisual[index].StartConnecting();
                    Messenger.Default.Send<NotificationContent>(NotificationHandler.CreateNotification(NotificationType.Information, "Connect", "You have selected the first entity to connect."));
                }
                else
                {
                    if (selectedCardIndexFirst != index)
                    {
                        selectedCardIndexSecond = index;
                        if (!IsAlreadyConnected(CardsEntities[selectedCardIndexFirst], CardsEntities[selectedCardIndexSecond]))
                        {
                            LinesHolder.Add(new LinesHolder(
                                new PowerConsumption(CardsEntities[selectedCardIndexFirst]),
                                new PowerConsumption(CardsEntities[selectedCardIndexSecond]),
                                GetCardCenterPosition(selectedCardIndexFirst),
                                GetCardCenterPosition(selectedCardIndexSecond)
                            ));
                            Messenger.Default.Send<NotificationContent>(NotificationHandler.CreateNotification(NotificationType.Success, "Connect", "You connected the two entities."));
                            SerializationHandler.SerializeEntitiesToFile(linesHolder, linesConnectionsSerializationPath);
                        }
                        else
                        {
                            Messenger.Default.Send<NotificationContent>(NotificationHandler.CreateNotification(NotificationType.Error, "Connect", "Those two entities are already connected."));
                        }
                        CardsVisual[selectedCardIndexFirst].StopConnecting();
                        CardsVisual[selectedCardIndexSecond].StopConnecting();
                        selectedCardIndexFirst = -1;
                        selectedCardIndexSecond = -1;
                    }
                    else
                    {
                        Messenger.Default.Send<NotificationContent>(NotificationHandler.CreateNotification(NotificationType.Information, "Connect", "You cancelled connection of entities."));
                        CardsVisual[selectedCardIndexFirst].StopConnecting();
                        selectedCardIndexFirst = -1;
                    }
                }
            }
            else
            {
                Messenger.Default.Send<NotificationContent>(NotificationHandler.CreateNotification(NotificationType.Error, "Connect", "There is no entity in the card to start/stop connection."));
            }
        }

        private System.Drawing.Point GetCardCenterPosition(int cardIndex)
        {
            int row = cardIndex / 4;
            int column = cardIndex % 4;
            double cardWidth = 150;
            double cardHeight = 133.33;
            double x = column * cardWidth + cardWidth / 2;
            double y = row * cardHeight + cardHeight / 2;
            return new System.Drawing.Point((int)x + 200, (int)y);
        }
        
        private bool IsAlreadyConnected(PowerConsumption first, PowerConsumption second)
        {
            for (int i = 0; i < linesHolder.Count; ++i)
            {
                if ((linesHolder[i].StartingEntity.Id == first.Id || linesHolder[i].EndingEntity.Id == first.Id) &&
                    (linesHolder[i].StartingEntity.Id == second.Id || linesHolder[i].EndingEntity.Id == second.Id))
                {
                    return true;
                }
            }
            return false;
        }
        private void OnChangedPosition(PowerConsumption changedPosition, int cardIndex)
        {
            List<LinesHolder> list = new List<LinesHolder>();
            for (int i = 0; i < LinesHolder.Count; ++i)
            {
                if (linesHolder[i].StartingEntity.Id == changedPosition.Id || linesHolder[i].EndingEntity.Id == changedPosition.Id)
                {
                    PowerConsumption tmp = null;
                    if (linesHolder[i].StartingEntity.Id == changedPosition.Id)
                    {
                        tmp = linesHolder[i].EndingEntity;
                    }
                    else
                    {
                        tmp = linesHolder[i].StartingEntity;
                    }
                    int otherIndex = -1;
                    for (int j = 0; j < CardsEntities.Count; ++j)
                    {
                        if (cardsEntities[j].Id == tmp.Id)
                        {
                            otherIndex = j;
                            break;
                        }
                    }
                    linesHolder.RemoveAt(i);
                    list.Add(new LinesHolder(
                                new PowerConsumption(changedPosition),
                                new PowerConsumption(tmp),
                                GetCardCenterPosition(cardIndex),
                                GetCardCenterPosition(otherIndex)
                            ));
                    --i;
                }
            }
            if (list.Count > 0)
            {
                for(int i = 0; i < list.Count; ++i)
                {
                    LinesHolder.Add(list[i]);
                }
                SerializationHandler.SerializeEntitiesToFile(LinesHolder, linesConnectionsSerializationPath);
            }
        }

        private void InitLinesConnections()
        {
            LinesHolder = SerializationHandler.DeserializeEntitiesFromFileLines();
            List<LinesHolder> list = linesHolder.Where(line => !MainWindowViewModel.Entities.Any(e => e.Id == line.StartingEntity.Id && e.Name.Equals(line.StartingEntity.Name)) ||
                                                               !MainWindowViewModel.Entities.Any(e => e.Id == line.EndingEntity.Id && e.Name.Equals(line.EndingEntity.Name))).ToList();
            if(list.Count > 0)
            {
                foreach(LinesHolder i in list)
                {
                    LinesHolder.Remove(i);
                }
            }
        }

        private void OnMouseEnterCard(string cardIndex)
        {
            int index = int.Parse(cardIndex);
            if(selectedCardIndexFirst != -1)
            {
                if(selectedCardIndexFirst == index)
                {
                    CardsVisual[index].ProposeCancel();
                }
                else if (CardsEntities[index].Id != -1 && !LinesHolder.Any(lh => (lh.StartingEntity.Id == CardsEntities[index].Id && lh.EndingEntity.Id == CardsEntities[selectedCardIndexFirst].Id) ||
                                                                                 (lh.StartingEntity.Id == CardsEntities[selectedCardIndexFirst].Id && lh.EndingEntity.Id == CardsEntities[index].Id)))
                {
                    CardsVisual[index].StartConnecting();
                }
            }
        }

        private void OnMouseLeaveCard(string cardIndex)
        {
            int index = int.Parse(cardIndex);
            if(selectedCardIndexFirst != -1)
            {
                if(selectedCardIndexFirst == index)
                {
                    CardsVisual[index].StopCancelPropose();
                }
                else if (CardsEntities[index].Id != -1)
                {
                    CardsVisual[index].StopConnecting();
                }
            }
        }

        private void Update()
        {
            while (true)
            {
                for (int i = 0; i < CardsEntities.Count; ++i)
                {
                    if (CardsEntities[i].Id != -1)
                    {
                        if (CardsEntities[i].Value != MainWindowViewModel.Entities.FirstOrDefault(pc => pc.Id == CardsEntities[i].Id).Value)
                        {
                            CardsEntities[i].Value = MainWindowViewModel.Entities.FirstOrDefault(pc => pc.Id == CardsEntities[i].Id).Value;
                        }
                    }
                }
                CheckAlarmingValues();
                Thread.Sleep(2000);
            }
        }

        private void CheckAlarmingValues()
        {
            for(int i = 0; i < CardsEntities.Count; ++i)
            {
                if (CardsEntities[i].Id != -1 && CardsEntities[i].IsValueAlarming())
                {
                    CardsVisual[i].RaiseAlarm();
                }
                else
                {
                    cardsVisual[i].StopAlarm();
                }
            }
        }

    }
}
