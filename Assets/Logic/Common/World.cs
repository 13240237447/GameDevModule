using System;
using System.Collections.Generic;
using System.Linq;


namespace Logic
{
    /// <summary>
    /// 顶级场景 管理所有的场景
    /// </summary>
    public class World : Scene
    {
        private List<Scene> scenes = new();
        
        
        public void AddScene(Scene scene)
        {
            if (scenes.Contains(scene))
            {
                return;
            }
            scenes.Add(scene);
        }

        public void RemoveScene(Scene scene)
        {
            if (!scenes.Contains(scene))
            {
                return;
            }
            scenes.Remove(scene);
        }


        public void TickOuter()
        {
            var currScenes = scenes.ToList();
            currScenes.ForEach(s=>s.Tick());
            currScenes.ForEach(s=>s.TickRender());
        }
        
    }
}
