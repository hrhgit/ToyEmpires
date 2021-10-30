using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Buff
{
    public class PlayerBuffContainer : BuffContainerBase
    {
        public PlayerBase player;

        public override void AddBuff(BuffBase buff)
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
            if (IsBuffAccessible(buff))
            {
                ((PlayerBuffBase)buff).activatePlayer = player;
            }
            buff.IsActivated = true;
        }

        protected override bool IsBuffAccessible(BuffBase buff)
        {
            return buff is PlayerBuffBase;
        }
    }
}
