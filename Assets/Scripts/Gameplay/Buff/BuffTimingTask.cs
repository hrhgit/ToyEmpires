using System;
using UnityEngine.Events;

namespace Gameplay.Buff
{
    // [Serializable]
    public class BuffTimingTask : BuffTask
    {
        public float                 timing;
        
        public override bool Run(float curTime)
        {
            if (curTime >= (startTime + timing))
            {
                additionalAction.Invoke(buff,this.container);
                taskEvent.Invoke(buff,this.container);
                return true;
            }

            return false;
        }

        public BuffTimingTask(BuffBase buff, BuffContainerBase container, float startTime, float timing, UnityEvent<BuffBase,BuffContainerBase> taskEvent) : base(buff,container,startTime, taskEvent)
        {
            this.timing = timing;
        }

        public BuffTimingTask(BuffBase buff, BuffContainerBase container, float startTime, float timing, UnityEvent<BuffBase,BuffContainerBase> taskEvent, UnityAction<BuffBase,BuffContainerBase> additionalAction) : base(buff, container,startTime, taskEvent, additionalAction)
        {
            this.timing = timing;
        }
    }
}