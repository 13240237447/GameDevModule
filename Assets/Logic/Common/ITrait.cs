namespace Logic
{
    /// <summary>
    /// 特性
    /// </summary>
    public interface ITrait
    {
        
    }

    public interface ITraitTick
    {
        void Tick(Entity entity);
    }
    
    public interface ITraitRenderTick 
    {
        void RenderTick(Entity entity);
    }
}
