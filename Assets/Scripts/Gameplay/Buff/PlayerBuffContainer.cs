using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Buff
{
    public class PlayerBuffContainer : BuffContainerBase
    {
        public PlayerBase player;

        protected override bool IsBuffAccessible(BuffBase buff)
        {
            return buff is PlayerBuffBase;
        }
    }
}
