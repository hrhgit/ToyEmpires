using System;
using Gameplay.Buildings;

namespace Gameplay.GameUnit.FortificationUnit.BuildingUnit
{
    public class BuildingUnitBase : FortificationUnitBase
    {
        public Building building;
        

        public void Set()
        {
            this.MaxHp = building.Hp[building.level];
            this.CurHp = this.MaxHp;
        }

        protected override void Die()
        {
            base.Die();
            building.Destroy();
        }
    }
}