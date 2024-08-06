using System;


namespace Logic.Activity
{
    public class CallFunc : Activity
    {
        private readonly Action a;
        
        public CallFunc(Action a,bool interruptible)
        {
            this.a = a;
            IsInterruptible = interruptible;
        }

        protected override bool Tick(ActivityController controller)
        {
            a?.Invoke();
            return true;
        }
    }
}
