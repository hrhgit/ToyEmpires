using Gameplay.GameUnit.SoldierUnit.CombatUnit;

namespace Gameplay.GameUnit
{
    public class PlayerHomeUnit : GameUnitBase,IAttackable
    {
        public int                      MaxHp   { get; }
        public int                      CurHp   { get; }
        public int                      Defence { get; }
        public event AttackEventHandler BeAttackedEvent;
        public void                     BeAttacked(ICombatable attacker)
        {
            throw new System.NotImplementedException();
        }
    }
}
