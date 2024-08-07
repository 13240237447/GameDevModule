using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;


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
        
        private List<ITick> ticks = new();

        private List<ITickRender> renderTicks = new();

        
        public void AddEntity(Entity entity)
        {
            if (Disposed || entity.Disposed || entities.Contains(entity))
            {
                return;
            }
            entity.BindScene(this);
            entities.Add(entity);
            if (entity is ITick tick)
            {
                AddTick(tick);
            }

            if (entity is ITickRender tickRender)
            {
                AddTickRender(tickRender);
            }
        }
        
        public void RemoveEntity(Entity entity)
        {
            if (Disposed || entity.Disposed || !entities.Contains(entity))
            {
                return;
            }
            entity.UnBindScene(this);
            entities.Remove(entity);
            if (entity is ITick tick)
            {
                RemoveTick(tick);
            }
            if (entity is ITickRender tickRender)
            {
                RemoveTickRender(tickRender);
            }
        }

        public void AddTick(ITick tick)
        {
            if (ticks.Contains(tick))
            {
                return;
            }
            ticks.Add(tick);
        }

        public void RemoveTick(ITick tick)
        {
            if (!ticks.Contains(tick))
            {
                return;
            }
            ticks.Remove(tick);
        }
        
        
        public void AddTickRender(ITickRender tick)
        {
            if (renderTicks.Contains(tick))
            {
                return;
            }
            renderTicks.Add(tick);
        }

        public void RemoveTickRender(ITickRender tick)
        {
            if (!renderTicks.Contains(tick))
            {
                return;
            }
            renderTicks.Remove(tick);
        }
        
        internal void Tick()
        {
            if (Disposed ||IsPaused)
            {
                return;
            }

            ticks.ToList().ForEach(s=>s.Tick());
            
            while (frameEndActions.Count != 0)
                frameEndActions.Dequeue()(this);
        }

        internal void TickRender()
        {
            if (Disposed)
            {
                return;
            }
            
            renderTicks.ToList().ForEach(s=>s.TickRender());
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
            ticks = null;
            entities = null;
        }
    }
}
