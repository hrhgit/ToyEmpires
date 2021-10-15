using Gameplay.GameUnit.SoldierUnit.CombatUnit;
using UnityEngine.Events;

namespace Gameplay.GameUnit
{
    public class PlayerHomeUnit : GameUnitBase,IAttackable
    {
        private int _curHp;
        public  int MaxHp { get; }

        public int CurHp
        {
            get => _curHp;
            private set
            {
                _curHp = value;
                if (_curHp <= 0)
                {
                    _curHp = 0;
                    DeathEvent.Invoke(this);
                }
            }
        }

        public           int            Defence { get; }
        public event AttackEventHandler BeAttackedEvent;
        public void                     BeAttacked(ICombatable attacker)
        {
            throw new System.NotImplementedException();
        }

        public UnityEvent<IAttackable> DeathEvent { get; }
    }
}
