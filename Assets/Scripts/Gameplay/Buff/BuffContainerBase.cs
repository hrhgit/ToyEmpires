using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Buff
{
    public abstract class BuffContainerBase : MonoBehaviour
    {
        public List<BuffBase>       buffList           = new List<BuffBase>();
        
        public List<BuffTask>       buffUpdateTaskList = new List<BuffTask>();
        public List<BuffTimingTask> buffTimingTaskList = new List<BuffTimingTask>();

        public float CurTime { get; protected set; } = 0;


        private void FixedUpdate()
        {
            CurTime += Time.fixedDeltaTime;
            buffTimingTaskList.ForEach((buff =>
                                        {
                                            buff.Run(CurTime);
                                        }));
            buffUpdateTaskList.ForEach((buff =>
                                        {
                                            buff.Run(CurTime);
                                        }));
        }
    }
}
