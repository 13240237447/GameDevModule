using Logic;
using UnityEngine;


namespace LogicTest
{
    public class DataEntity : Entity, ITick
    {
        public int Num { private set; get; }
        public void Tick()
        {
            Num++;
            if (Num >= 100)
            {
                Scene.RemoveEntity(this);
            }
        }
    }
    
    public class RenderEntity : Entity, ITickRender
    {
        private DataEntity dataEntity;
        
        public RenderEntity(DataEntity dataEntity)
        {
            this.dataEntity = dataEntity;
        }
        
        public void TickRender()
        {
            if (dataEntity != null)
            {
                Debug.Log($"{dataEntity.Num} {Time.frameCount}");
            }
        }
    }

    public class TestScene : Scene
    {
        
    }
}
