using System.Collections.Generic;
using VirtualDesktop.Actions;

namespace VirtualDesktop
{
    public class ActionManager
    {
        private const int UNDO_STACK_LIMIT = 20;
        private static Stack<ActionSet> UndoStack = new Stack<ActionSet>();
        private static Stack<ActionSet> RedoStack = new Stack<ActionSet>();

        private static void AddActionSet(ActionSet actionSet)
        {
            if (UndoStack.Count == UNDO_STACK_LIMIT)
            {
                List<ActionSet> tempLstActionSet = new List<ActionSet>();
                while (UndoStack.Count > 0)
                {
                    tempLstActionSet.Add(UndoStack.Pop());
                }
                // Remove last item of list which was previously first item of Undo Stack
                tempLstActionSet.RemoveAt(tempLstActionSet.Count - 1);
                for (int i = tempLstActionSet.Count - 1; i >= 0; i--)
                {
                    UndoStack.Push(tempLstActionSet[i]);
                }
            }
            UndoStack.Push(actionSet);
        }

        public static void AddNewActionSet(ActionSet actionSet)
        {
            RedoStack.Clear();
            AddActionSet(actionSet);
            App.MainAppWindow.SliderWindow.btnUndo.IsEnabled = true;
            App.MainAppWindow.SliderWindow.btnRedo.IsEnabled = false;
        }

        public static int GetRedoStackCount()
        {
            return RedoStack.Count;
        }

        public static int GetUndoStackCount()
        {
            return UndoStack.Count;
        }

        public static void Undo()
        {
            ActionSet actionSet = UndoStack.Pop();
            RedoStack.Push(actionSet);

            List<Action> lstAction = actionSet.ActionList;
            for(int i = lstAction.Count - 1 ; i >= 0 ; i--)
            {
                Action action = lstAction[i];
                action.Undo();
            }
        }

        public static void Redo()
        {
            ActionSet actionSet = RedoStack.Pop();
            UndoStack.Push(actionSet);
            foreach (Action action in actionSet.ActionList)
            {
                action.Redo();
            }
        }
    }
}
