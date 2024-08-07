using System;


namespace Logic
{
    public abstract class Entity : IDisposable
    {
        public Scene Scene { private set;get; }

        public bool Disposed { get; private set; }

        internal void BindScene(Scene scene)
        {
            if (Scene != null)
            {
                throw new Exception($"{this.GetType().Name} had bind scene {Scene}");
            }
            Scene = scene;
            OnInit();
        }

        protected virtual void OnInit()
        {
            
        }

        internal void UnBindScene(Scene scene)
        {
            if (Scene  == null || Scene != scene)
            {
                throw new Exception($"{this.GetType().Name} unbind scene {scene} fail!");
            }
            OnDestroy();
            Scene = null;
        }

        protected virtual void OnDestroy()
        {
            
        }
        
        public void Dispose()
        {
            if (Scene != null)
            {
                Scene.AddEndFrameAction((s) =>
                {
                    if (Scene != null)
                    {
                        s.RemoveEntity(this);
                    }

                    Disposed = true;
                });
            }
            else
            {
                Disposed = true;
            }
        }
    }
}
