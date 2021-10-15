using Gameplay.GameUnit.SoldierUnit.CombatUnit;
using UnityEngine.Events;

namespace Gameplay.GameUnit
{
    public interface IAttackable
    {
        public int MaxHp   { get; }
        public int CurHp   { get; }
        public int Defence { get; }

        public event AttackEventHandler BeAttackedEvent;
        public void                     BeAttacked(ICombatable attacker);

        public UnityEvent<IAttackable> DeathEvent { get; }

    }
}