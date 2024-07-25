using System;


namespace Module.Activity
{
    public class CallFunc : Activity
    {
        private readonly Action a;
        
        public CallFunc(Action a,bool interruptible)
        {
            this.a = a;
            IsInterruptible = interruptible;
        }

        protected override bool Tick(ActivityContainer container)
        {
            a?.Invoke();
            return true;
        }
    }
}
