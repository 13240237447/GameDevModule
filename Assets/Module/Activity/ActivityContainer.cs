using System;
using System.Collections.Generic;


namespace Module.Activity
{
    public interface IActivityOwner
    {
        /// <summary>
        /// 在活动即将变为idle的时候调用一次
        /// </summary>
        /// <param name="container"></param>
        void OnBecomingIdle(ActivityContainer container);

        /// <summary>
        /// idle的时候每帧调用
        /// </summary>
        /// <param name="container"></param>
        void TickIdle(ActivityContainer container);
    }
    
    /// <summary>
    /// 活动的容器
    /// </summary>
    public class ActivityContainer : IDisposable 
    {
        private Activity currentActivity;
        public Activity CurrentActivity { 
            private set => currentActivity = value;
            get => Activity.SkipDoneActivities(currentActivity);
        }

        public bool IsIdle => CurrentActivity == null;

        /// <summary>
        /// 容器的拥有者
        /// </summary>
        public readonly IActivityOwner Owner;

        /// <summary>
        /// 活动拥有者
        /// </summary>
        /// <param name="creationActivity">初始化需要创建的活动</param>
        /// <param name="owner"></param>
        public ActivityContainer(List<Activity> creationActivity = null,IActivityOwner owner = null)
        {
            this.Owner = owner;
            if (creationActivity != null)
            {
                for (int i = creationActivity.Count - 1; i >= 0; i--)
                {
                    var activity = creationActivity[i];
                    activity.Queue(CurrentActivity);
                    CurrentActivity = activity;
                }
            }
        }

        public void Tick()
        {
            var wasIdle = IsIdle;
            CurrentActivity = RunActivity(CurrentActivity);
            if (!wasIdle && IsIdle)
            {
                Owner?.OnBecomingIdle(this);
                CurrentActivity = RunActivity(CurrentActivity);
            }
            else if (wasIdle)
            {
                Owner?.TickIdle(this);
            }
        }
        
        public Activity RunActivity(Activity act)
        {
            if (act == null)
            {
                return null;
            }

            while (act != null)
            {
                var prev = act;
                act = act.TickOuter(this);
                if (act == prev)
                {
                    break;
                }
            }
            return act;
        }


        public void QueueActivity(Activity nextActivity)
        {
            if (CurrentActivity == null)
            {
                CurrentActivity = nextActivity;
            }
            else
            {
                CurrentActivity.Queue(nextActivity);
            }
        }

        public void CancelActivity()
        {
            CurrentActivity?.Cancel(this);
        }
        
        public void Dispose()
        {
            CurrentActivity?.OnOwnerDisposeOuter(this);
        }
    }
}
