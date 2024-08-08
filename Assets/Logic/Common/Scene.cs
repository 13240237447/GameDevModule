using System;
using System.Collections.Generic;
using System.Linq;


namespace Logic
{
    /// <summary>
    /// 场景基类,用来管理实体
    /// </summary>
    public class Scene : IDisposable
    {
        private Queue<Action<Scene>> frameEndActions = new();
        public bool IsPaused { private set; get; }
        public bool Disposed { get; private set; }

        private List<Entity> entities = new();
        
        uint nextEntityId = 0;
        internal uint NextEntityID()
        {
            return nextEntityId++;
        }

        public Entity CreateEntity(List<Activity> creationActivity = null)
        {
            var entity = new Entity(this,creationActivity);
            entities.Add(entity);
            return entity;
        }
        
        public void DestroyEntity(Entity entity)
        {
            if (Disposed || entity.Disposed || !entities.Contains(entity))
            {
                return;
            }
            entity.UnBindScene(this);
            entities.Remove(entity);
        }
        
        internal void Tick()
        {
            if (Disposed ||IsPaused)
            {
                return;
            }

            entities.ToList().ForEach(s=>s.Tick());
            
            while (frameEndActions.Count != 0)
                frameEndActions.Dequeue()(this);
        }

        internal void TickRender()
        {
            if (Disposed)
            {
                return;
            }
        }
        
        public void AddEndFrameAction(Action<Scene> action)
        {
            if (Disposed)
            {
                return;
            }
            frameEndActions.Enqueue(action);
        }
        
        //场景销毁时 会将所有实体也销毁
        public void Dispose()
        {
            frameEndActions.Clear();
            entities.ToList().ForEach(e =>
            {
                if (!e.Disposed)
                {
                    e.Dispose();
                }
            });
            
            while (frameEndActions.Count != 0)
                frameEndActions.Dequeue()(this);
            
            Disposed = true;
            frameEndActions = null;
            entities = null;
        }
    }
}
