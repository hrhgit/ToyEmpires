using System.Linq;
using Gameplay.GameUnit;
using UnityEngine;

namespace Gameplay.Buff
{
    public class UnitBuffContainer : BuffContainerBase
    {
        public GameUnitBase unit;
        public bool         isUnitLimited   = false;
        public int[]        availableUnitID = new int[10];
        protected override bool IsBuffAccessible(BuffBase buff)
        {
            return buff is UnitBuffBase;
        }

        
        public virtual bool IsAvailableUnit(GameUnitBase u)
        {
            return availableUnitID.Contains(u.unitID);
        }

    }
}
