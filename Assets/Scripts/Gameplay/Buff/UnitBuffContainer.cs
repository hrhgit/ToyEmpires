using Gameplay.GameUnit;
using UnityEngine;

namespace Gameplay.Buff
{
    public class UnitBuffContainer : BuffContainerBase
    {
        public GameUnitBase unit;
        
        protected override bool IsBuffAccessible(BuffBase buff)
        {
            return buff is UnitBuffBase;
        }

    }
}
