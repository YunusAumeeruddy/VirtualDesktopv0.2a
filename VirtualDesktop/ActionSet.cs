using System.Collections.Generic;
using VirtualDesktop.Actions;

namespace VirtualDesktop
{
    public class ActionSet
    {
        private List<Action> lstAction = new List<Action>();

        public List<Action> ActionList
        {
            get
            {
                return lstAction;
            }
        }

        public void AddAction(Action action)
        {
            lstAction.Add(action);
        }
    }
}
