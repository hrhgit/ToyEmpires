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
                ((UnitBuffBase)buff).activateUnit = unit;
            }
            
            buff.IsActivated = true;
        }

        public virtual bool IsAvailableUnit(GameUnitBase u)
        {
            return availableUnitID.Contains(u.unitID);
        }

    }
}
