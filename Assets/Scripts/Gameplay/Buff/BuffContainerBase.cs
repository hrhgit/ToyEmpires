using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Buff
{
    public abstract class BuffContainerBase: MonoBehaviour
    {
        public List<BuffBase> buffList = new List<BuffBase>();
        
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

        protected virtual bool IsBuffAccessible(BuffBase buff)
        {
            return true;
        }
        public virtual void AddBuff(BuffBase buff)
        {
            if (!IsBuffAccessible(buff))
                return;
            if (!buff.isSuperimposable)
            {
                if (buffList.Exists((b => b.buffID == buff.buffID)))
                    return;
            }
            buffList.Add(buff);
            buff.container   = this;
            buff.IsActivated = true;
        }

        public void RemoveBuff(BuffBase buff)
        {
            if (!IsBuffAccessible(buff))
                return;
            int buffId = buffList.FindIndex((b => b.buffID == buff.buffID));
            if (buffId != -1)
            {
                buff.IsActivated = false;
                buffList.RemoveAt(buffId);
            }
        }
    }
}
