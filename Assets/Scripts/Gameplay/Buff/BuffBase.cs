using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Buff
{
    [Serializable]
    public class BuffBase
    {
        public int   buffID;
        
        public bool isTimeLimited    = false;
        public bool isOneOff         = false;
        public bool isSuperimposable = false;




        public float maxTime;
        

        public  UnityEvent<BuffBase,BuffContainerBase> buffStartEvent  = new UnityEvent<BuffBase,BuffContainerBase>();
        public  UnityEvent<BuffBase,BuffContainerBase> buffUpdateEvent = new UnityEvent<BuffBase,BuffContainerBase>();
        public  UnityEvent<BuffBase,BuffContainerBase> buffStopEvent   = new UnityEvent<BuffBase,BuffContainerBase>();
        
        private bool           _isActivated = false;
        private BuffTask       _updateTask;
        private BuffTimingTask _timingTask;

        public BuffBase()
        {
            
        }

        private BuffBase(List<UnityAction<BuffBase,BuffContainerBase>> startEvent, List<UnityAction<BuffBase,BuffContainerBase>> updateEvent, List<UnityAction<BuffBase,BuffContainerBase>> stopEvent)
        {
            if (startEvent != null)
            {
                startEvent.ForEach((action => this.buffStartEvent.AddListener(action)));
            }
            if (updateEvent != null)
            {
                updateEvent.ForEach((action => this.buffUpdateEvent.AddListener(action)));
            }
            if (stopEvent != null)
            {
                stopEvent.ForEach((action => this.buffStopEvent.AddListener(action)));
            }
        }
        public BuffBase(List<UnityAction<BuffBase,BuffContainerBase>> startEvent, List<UnityAction<BuffBase,BuffContainerBase>> updateEvent, List<UnityAction<BuffBase,BuffContainerBase>> stopEvent,float maxTime) : this(startEvent,updateEvent,stopEvent)
        {
            isTimeLimited = true;
            this.maxTime  = maxTime;
        }
        
        public BuffBase(List<UnityAction<BuffBase,BuffContainerBase>> startEvent, List<UnityAction<BuffBase,BuffContainerBase>> stopEvent, bool isOneOff) : this(startEvent,null,stopEvent)
        {
            this.isOneOff = isOneOff;
        }

        public void Activate(BuffContainerBase container)
        {
            buffStartEvent.Invoke(this,container);
            

            if (isOneOff)
            {
                Deactivate(container);
            }
            else if(container.buffUpdateTaskList.Count > 0) //存在更新任务
            {
                _updateTask = new BuffTask(this,container, container.CurTime, buffUpdateEvent);
                container.buffUpdateTaskList.Add(_updateTask);
            }

            if (isTimeLimited)
            {
                _timingTask = new BuffTimingTask(this, container,container.CurTime, maxTime, buffStopEvent, ((buff,container) =>
                                                                                                   {
                                                                                                       Deactivate(container);
                                                                                                   }));
                container.buffTimingTaskList.Add(_timingTask);
            }

                
        }

        public void Deactivate(BuffContainerBase container)
        {
            buffStopEvent.Invoke(this,container);
            
            container.buffList.Remove(this);
            if(!isOneOff && container.buffUpdateTaskList.Count > 0)
            {
                container.buffUpdateTaskList.Remove(_updateTask);
            }

            if (isTimeLimited)
            {
                container.buffTimingTaskList.Remove(_timingTask);
            }
        }

    }
}
