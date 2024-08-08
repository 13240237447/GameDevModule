using System.Collections.Generic;
using Logic;
using UnityEngine;


namespace LogicTest
{
    public class DataEntity : Activity
    {
        public int Num { private set; get; }
        
        protected override bool Tick(Entity entity)
        {
            if (Num < 100)
            {
                Num++;
                return false;
            }
            return true;
        }
    }
    
    public class RenderEntity 
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
