using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Helpers
{
    public enum ActionType
    {
        NoAction = -1
    }
    public class UndoActionHolder
    {
        private ActionType actionId;

        public UndoActionHolder()
        {
            actionId = ActionType.NoAction;
        }

        public ActionType ActionId
        {
            private set { }
            get { return actionId; }
        }
    }
}
