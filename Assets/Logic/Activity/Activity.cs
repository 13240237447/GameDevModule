using System;
using System.Linq;
using System.Text;


namespace Logic.Activity
{
    public interface IActivityInterface
    {
        
    }
    
    public enum ActivityState { Queued, Active, Canceling, Done }

    /// <summary>
    /// 活动抽象基类
    /// </summary>
    public abstract class Activity : IActivityInterface
    {
        public ActivityState State { get; private set; }

        private Activity childActivity;

        protected Activity ChildActivity
        {
            get => SkipDoneActivities(childActivity);
            private set => childActivity = value;
        }

        private Activity nextActivity;

        public Activity NextActivity
        {
            get => SkipDoneActivities(nextActivity);
            private set => nextActivity = value;
        }
        
        internal static Activity SkipDoneActivities(Activity first)
        {
            while (first != null && first.State == ActivityState.Done)
                first = first.nextActivity;
            return first;
        }

        /// <summary>
        /// 是否可以中断
        /// </summary>
        public bool IsInterruptible { get; protected set; } = true;

        public bool ChildHasPriority { get; protected set; } = true;
        
        public bool IsCanceling => State == ActivityState.Canceling;

        bool finishing;
        bool firstRunCompleted;
        bool lastRun;

        internal Activity TickOuter(ActivityController controller)
        {
            if (State == ActivityState.Done)
                throw new InvalidOperationException($"ActivityOwner {controller} 尝试Tick一个已经完成的活动 {GetType()} ");

            if (State == ActivityState.Queued)
            {
                OnFirstRun(controller);
                firstRunCompleted = true;
                State = ActivityState.Active;
            }

            if (!firstRunCompleted)
            {
                throw new InvalidOperationException($"ActivityOwner {controller} 尝试在OnFirstRun之前Tick活动 {GetType()}");
            }

            if (ChildHasPriority)
            {
                lastRun = TickChild(controller) && (finishing || Tick(controller));
                finishing |= lastRun;
            }
            else
            {
                lastRun = Tick(controller);
            }

            var ca = ChildActivity;
            if (ca != null && ca.State == ActivityState.Queued)
            {
                if (ChildHasPriority)
                {
                    lastRun = TickChild(controller) && finishing;
                }
                else
                {
                    TickChild(controller);
                }
            }

            if (lastRun)
            {
                State = ActivityState.Done;
                OnLastRun(controller);
                return NextActivity;
            }

            return this;
        }

        protected bool TickChild(ActivityController controller)
        {
            ChildActivity = controller.RunActivity(ChildActivity);
            return ChildActivity == null;
        }
        
        
        /// <summary>
        /// 在第一个Tick之前立即执行一次
        /// </summary>
        /// <param name="controller"></param>
        protected virtual void OnFirstRun(ActivityController controller) { }

        
        /// <summary>
        /// 每帧调用来执行活动逻辑。返回false则表示活动仍需执行，返回true则代表活动执行完成
        /// 取消活动必须确保在返回true之前将ActivityOwner置回一致状态
        ///
        /// 子类活动可以通过QueueChild来入队，被激活时它们将会代替父类活动被Tick
        /// 如果需要和子类活动并行，需要将ChildHasPriority置位false 并且主动调用TickChildren
        ///
        /// 入队一个或多个活动并且立即返回完成是有效的，会使活动立即完成
        /// </summary>
        protected virtual bool Tick(ActivityController controller)
        {
            return true;
        }
        
        /// <summary>
        /// 在最后一次Tick完成之后立即执行一次
        /// </summary>
        protected virtual void OnLastRun(ActivityController controller) { }
     
        /// <summary>
        /// 当拥有者被销毁时调用一次
        /// </summary>
        protected virtual void OnOwnerDispose(ActivityController controller) { }
        
        internal void OnOwnerDisposeOuter(ActivityController controller)
        {
            ChildActivity?.OnOwnerDisposeOuter(controller);
            OnOwnerDispose(controller);
        }

        public virtual void Cancel(ActivityController controller, bool keepQueue = false)
        {
            if (!keepQueue)
            {
                NextActivity = null;
            }

            if (!IsInterruptible)
            {
                return;
            }
            
            ChildActivity?.Cancel(controller);

            State = State == ActivityState.Queued ? ActivityState.Done : ActivityState.Canceling;
        }

        /// <summary>
        /// 将活动入队
        /// </summary>
        /// <param name="activity"></param>
        public void Queue(Activity activity)
        {
            var it = this;
            while (it.nextActivity != null)
            {
                it = it.nextActivity;
            }
            it.nextActivity = activity;
        }

        /// <summary>
        /// 将活动入队到子活动
        /// </summary>
        /// <param name="activity"></param>
        public void QueueChild(Activity activity)
        {
            if (childActivity != null)
            {
                childActivity.Queue(activity);
            }
            else
            {
                childActivity = activity;
            }
        }
        
        
        public string PrintActivityTree(ActivityController controller, Activity origin = null, int level = 0)
        {
            if (origin == null)
            {
                return controller.CurrentActivity.PrintActivityTree(controller,this);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(new string(' ', level * 2));
                if (origin == this)
                {
                    sb.Append("*");
                }

                sb.AppendLine($"{GetType().ToString().Split('.').Last()}");
                if (ChildActivity != null)
                {
                    sb.Append(ChildActivity.PrintActivityTree(controller, origin, level + 1));
                }

                if (NextActivity != null)
                {
                    sb.Append(NextActivity.PrintActivityTree(controller, origin, level));
                }
                return sb.ToString();
            }
        }
    }
}
