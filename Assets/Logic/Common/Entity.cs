using System;
using System.Collections.Generic;


namespace Logic
{
    public sealed class Entity : IDisposable
    {
        public Scene Scene { private set;get; }

        public bool Disposed { get; private set; }

        public readonly uint EntityId;
        
                
        private Activity currentActivity;
        
        public Activity CurrentActivity { 
            private set => currentActivity = value;
            get => Activity.SkipDoneActivities(currentActivity);
        }

        public bool IsIdle => CurrentActivity == null;
        
        
        internal Entity(Scene scene,List<Activity> creationActivity = null) 
        {
            EntityId = scene.NextEntityID();
            Scene = scene;
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

        internal void UnBindScene(Scene scene)
        {
            if (Scene  == null || Scene != scene)
            {
                throw new Exception($"{this.GetType().Name} unbind scene {scene} fail!");
            }
            Scene = null;
        }

        private void OnDestroy()
        {
            CurrentActivity?.OnOwnerDisposeOuter(this);
        }
        
        public void Tick()
        {
            var wasIdle = IsIdle;
            CurrentActivity = RunActivity(CurrentActivity);
            if (!wasIdle && IsIdle)
            {
                CurrentActivity = RunActivity(CurrentActivity);
            }
            else if (wasIdle)
            {
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

        public void CancelActivity()
        {
            CurrentActivity?.Cancel(this);
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
        
        public void Dispose()
        {
            if (Scene != null)
            {
                Scene.AddEndFrameAction((s) =>
                {
                    if (Scene != null)
                    {
                        s.DestroyEntity(this);
                    }
                    OnDestroy();
                    Disposed = true;
                });
            }
            else
            {
                OnDestroy();
                Disposed = true;
            }
        }
    }
}
