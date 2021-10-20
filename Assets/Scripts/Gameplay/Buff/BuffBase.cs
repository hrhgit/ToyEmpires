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
        private bool                 _isActivated    = false;



        private void Activate()
        {
            buffStartEvent.Invoke(this);
            if (isTimeLimited)
            {
                container.buffTimingTaskList.Add(new BuffTimingTask(this,container.CurTime,maxTime,buffStopEvent,(buff =>
                                                                                                      {
                                                                                                          buff.isTimeLimited = false;
                                                                                                      })));
               
            }

            if (isOneOff)
            {
                IsActivated = false;
            }
            else
            {
                container.buffUpdateTaskList.Add(new BuffTask(this,container.CurTime,buffUpdateEvent));
            }
                
        }
        
        private void Deactivate()
        {
            buffStopEvent.Invoke(this);
        }

    }
}
