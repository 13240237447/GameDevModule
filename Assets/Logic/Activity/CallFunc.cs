using System;


namespace Logic
{
    public class CallFunc : Activity
    {
        private readonly Action a;
        
        public CallFunc(Action a,bool interruptible)
        {
            this.a = a;
            IsInterruptible = interruptible;
        }

        protected override bool Tick(Entity entity)
        {
            a?.Invoke();
            return true;
        }
    }
}
