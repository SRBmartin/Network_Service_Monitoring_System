using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Helpers
{
    public enum ActionType
    {
        NoAction = -1,
        Add,
        Delete
    }
    public class UndoActionHolder : BindableBase
    {
        private ActionType actionId;
        private PowerConsumption entity;
        public UndoActionHolder(UndoActionHolder undoActionHolder)
        {
            actionId = undoActionHolder.actionId;
            entity = new PowerConsumption(undoActionHolder.entity);
        }
        public UndoActionHolder(PowerConsumption entity, ActionType actionId)
        {
            this.entity = new PowerConsumption();
            this.entity.IdS = entity.IdS;
            this.entity.Id = entity.Id;
            this.entity.Name = entity.Name;
            this.entity.Type = entity.Type;
            this.entity.Value = entity.Value;
            this.actionId = actionId;
        }
        public UndoActionHolder()
        {
            actionId = ActionType.NoAction;
        }

        public ActionType ActionId
        {
            set { actionId = value; }
            get { return actionId; }
        }
        public PowerConsumption Entity
        {
            get { return entity; }
            set
            {
                entity = value;
                OnPropertyChanged("Entity");
            }
        }
    }
}
