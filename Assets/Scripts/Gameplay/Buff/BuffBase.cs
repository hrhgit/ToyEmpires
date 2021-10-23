using System;
using System.Collections;
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

        public bool IsActivated
        {
            get => _isActivated;

            set
            {
                _isActivated = value;
                if (_isActivated)
                {
                    this.Activate();
                }
                else
                {
                    this.Deactivate();
                }
            }
        }


        public float maxTime;

        public BuffContainerBase container;

        public  UnityEvent<BuffBase> buffStartEvent  = new UnityEvent<BuffBase>();
        public  UnityEvent<BuffBase> buffUpdateEvent = new UnityEvent<BuffBase>();
        public  UnityEvent<BuffBase> buffStopEvent   = new UnityEvent<BuffBase>();
        
        private bool           _isActivated = false;
        private BuffTask       _updateTask;
        private BuffTimingTask _timingTask;


        private void Activate()
        {
            buffStartEvent.Invoke(this);
            
            if (isOneOff)
            {
                IsActivated = false;
            }
            else if(container.buffUpdateTaskList.Count > 0)
            {
                _updateTask = new BuffTask(this, container.CurTime, buffUpdateEvent);
                container.buffUpdateTaskList.Add(_updateTask);
            }

            if (isTimeLimited)
            {
                _timingTask = new BuffTimingTask(this, container.CurTime, maxTime, buffStopEvent, (buff =>
                                                                                                   {
                                                                                                       buff.IsActivated = false;
                                                                                                   }));
                container.buffTimingTaskList.Add(_timingTask);
            }

                
        }
        
        private void Deactivate()
        {
            buffStopEvent.Invoke(this);
            
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
