using System;
using UnityEngine.Events;

namespace Gameplay.Buff
{
    [Serializable]
    public class BuffTimingTask : BuffTask
    {
        public float                 timing;
        
        public override bool Run(float curTime)
        {
            if (curTime >= (startTime + timing))
            {
                additionalAction.Invoke(buff);
                taskEvent.Invoke(buff);
                return true;
            }

            return false;
        }

        public BuffTimingTask(BuffBase buff, float startTime, float timing, UnityEvent<BuffBase> taskEvent) : base(buff, startTime, taskEvent)
        {
            this.timing = timing;
        }

        public BuffTimingTask(BuffBase buff, float startTime, float timing, UnityEvent<BuffBase> taskEvent, UnityAction<BuffBase> additionalAction) : base(buff, startTime, taskEvent, additionalAction)
        {
            this.timing = timing;
        }
    }
}