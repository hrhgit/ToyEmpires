using Gameplay.GameUnit.SoldierUnit.CombatUnit;
using UnityEngine.Events;

namespace Gameplay.GameUnit
{
    public interface IDefenable
    {
        public int MaxHp   { get; }
        public int CurHp   { get; }
        
        public bool IsDeath { get; }
        public int  Defence { get; }

        public event AttackEventHandler BeAttackedEvent;
        public void                     BeAttacked(IAttackable attacker);

        public UnityEvent<IDefenable> DeathEvent { get; }

    }
}