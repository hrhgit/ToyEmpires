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

        public void InitBuff()
        {
            List<BuffBase> tmpBuffList = buffList;
            buffList = new List<BuffBase>();
            tmpBuffList.ForEach((b =>
                                 {
                                     this.AddBuff(b);
                                 }));
            
        }
        
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
            buff.Activate(this);
        }

        public void RemoveBuff(BuffBase buff)
        {
            if (!IsBuffAccessible(buff))
                return;
            int buffId = buffList.FindIndex((b => b.buffID == buff.buffID));
            if (buffId != -1)
            {
                buff.Deactivate(this);
                // buffList.RemoveAt(buffId);
            }
        }
        
        public void RemoveBuffByID(int buffID)
        {
            int buffIndex = buffList.FindIndex((b => b.buffID == buffID));
            if (buffIndex != -1)
            {
                buffList[buffIndex].Deactivate(this);
                // buffList.RemoveAt(buffId);
            }
        }
    }
}
