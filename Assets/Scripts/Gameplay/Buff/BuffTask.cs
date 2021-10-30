using System;
using UnityEngine.Events;

namespace Gameplay.Buff
{
    [Serializable]
    public class BuffTask
    {
        public BuffBase              buff;
        public float                 startTime;
        public UnityEvent<BuffBase>  taskEvent;
        public UnityAction<BuffBase> additionalAction;
        
        public BuffTask(BuffBase buff, float startTime, UnityEvent<BuffBase> taskEvent)
        {
            this.buff      = buff;
            this.startTime = startTime;
            this.taskEvent = taskEvent;
        }

        public BuffTask(BuffBase buff, float startTime, UnityEvent<BuffBase> taskEvent, UnityAction<BuffBase> additionalAction)
        {
            this.buff             = buff;
            this.startTime        = startTime;
            this.taskEvent        = taskEvent;
            this.additionalAction = additionalAction;
        }

        public virtual bool Run(float curTime)
        {
            additionalAction.Invoke(buff);
            taskEvent.Invoke(buff);
            return true;
        }

    }
}