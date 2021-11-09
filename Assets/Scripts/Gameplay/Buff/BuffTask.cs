using System;
using UnityEngine.Events;

namespace Gameplay.Buff
{
    // [Serializable]
    public class BuffTask
    {
        public BuffBase                                buff;
        public BuffContainerBase                       container;
        public float                                   startTime;
        public UnityEvent<BuffBase,BuffContainerBase>  taskEvent;
        public UnityAction<BuffBase,BuffContainerBase> additionalAction;
        
        public BuffTask(BuffBase buff, BuffContainerBase container,float startTime, UnityEvent<BuffBase,BuffContainerBase> taskEvent)
        {
            this.buff      = buff;
            this.container = container;
            this.startTime = startTime;
            this.taskEvent = taskEvent;
        }

        public BuffTask(BuffBase buff, BuffContainerBase container, float startTime, UnityEvent<BuffBase,BuffContainerBase> taskEvent, UnityAction<BuffBase,BuffContainerBase> additionalAction)
        {
            this.buff             = buff;
            this.container        = container;
            this.startTime        = startTime;
            this.taskEvent        = taskEvent;
            this.additionalAction = additionalAction;
        }

        public virtual bool Run(float curTime)
        {
            additionalAction.Invoke(buff, container);
            taskEvent.Invoke(buff, container);
            return true;
        }

    }
}