using System.Collections.Generic;
using Logic;
using UnityEngine;


namespace LogicTest
{
    public class RegenerationTrait : ITrait,ITraitTick
    {
        public void Tick(Entity entity)
        {
            var health = entity.Info.TraitInfo<HealthTraitInfo>();
            if (health.value < 100)
            {
                health.value++;
            }
        }
    }

    public class HealthTraitInfo : TraitInfo
    {
        public int value;
    }
    
    public class HealthTraitRender : ITrait, ITraitRenderTick
    {
        private int lastHealth = -1;
        public void RenderTick(Entity entity)
        {
            var health = entity.Info.TraitInfo<HealthTraitInfo>().value;
            if (health != lastHealth)
            {
                lastHealth = health;
                Debug.Log($"{health} {Time.frameCount}");
            }
        }
    }

    public class TestScene : Scene
    {
        
    }
}
