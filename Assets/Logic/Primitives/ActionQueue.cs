using System;
using System.Collections.Generic;


namespace Logic.Primitives
{
    public class ActionQueue
    {

        private List<DelayAction> waitingActions = new();

        private DelayAction cacheAction = new ();

        /// <summary>
        /// 添加回调
        /// </summary>
        /// <param name="action"></param>
        /// <param name="runTime">回调触发的时间</param>
        public void Add(Action action, long runTime)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            DelayAction delayAction = new DelayAction()
            {
                action = action,
                runTime = runTime,
            };
            var index = Index(delayAction);
            waitingActions.Insert(index,cacheAction);
        }

        /// <summary>
        /// 执行小于等于runTime的回调
        /// </summary>
        /// <param name="runTime"></param>
        public void PerformsActions(long runTime)
        {
            cacheAction.runTime = runTime;
            var index = Index(cacheAction);
            if (index <= 0)
            {
                return;
            }
            DelayAction[] preformActions = new DelayAction[index];
            waitingActions.CopyTo(0,preformActions,0,index);
            waitingActions.RemoveRange(0,index);
            foreach (var delayAction in preformActions)
            {
                delayAction.action();
            }
        }

        private int Index(DelayAction action)
        {
            var index = waitingActions.BinarySearch(action);
            if (index < 0)
            {
                return ~index;
            }

            while (index < waitingActions.Count && action.CompareTo(waitingActions[index]) > 0)
            {
                index++;
            }
            return index;
        }
        
        
        class DelayAction : IComparable<DelayAction>
        {
            public long runTime;

            public Action action;
            
            public int CompareTo(DelayAction other)
            {
                return runTime.CompareTo(other.runTime);
            }
        }
    }
}
