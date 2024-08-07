namespace Logic
{
    public interface IEntityInterface
    {
        
    }

    public interface ITick : IEntityInterface
    {
        void Tick();
    }

    public interface ITickRender : IEntityInterface
    {
        void TickRender();
    }
}
