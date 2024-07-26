using System;
using UnityEngine;


namespace Module.Activity
{
    public class Wait : Activity
    {
        private float remainSeconds;

        private readonly bool ignoreTimeScale;
        
        public Wait(float seconds, bool isIgnoreTimeScale)
        {
            remainSeconds = seconds;
            ignoreTimeScale = isIgnoreTimeScale;
        }

        public void SetInterruptible(bool canInterruptible)
        {
            IsInterruptible = canInterruptible;
        }

        protected override bool Tick(ActivityController controller)
        {
            if (IsCanceling)
            {
                return true;
            }

            remainSeconds -= ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            return remainSeconds <= 0;
        }
    }

    public class WaitFor : Activity
    {
        private readonly Func<bool> f;

        public WaitFor(Func<bool> f)
        {
            this.f = f;
        }
        
        public void SetInterruptible(bool canInterruptible)
        {
            IsInterruptible = canInterruptible;
        }

        protected override bool Tick(ActivityController controller)
        {
            if (IsCanceling)
            {
                return true;
            }
            
            return f == null || f();
        }
    }
}
