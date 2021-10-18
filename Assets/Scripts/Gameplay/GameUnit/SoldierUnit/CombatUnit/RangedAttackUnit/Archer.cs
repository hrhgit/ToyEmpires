using UnityEngine;

namespace Gameplay.GameUnit.SoldierUnit.CombatUnit.RangedAttackUnit
{
    public class Archer : RangedAttackUnitBase
    {
        public override float UnitValue { get => base.UnitValue * 5; }
    }
}