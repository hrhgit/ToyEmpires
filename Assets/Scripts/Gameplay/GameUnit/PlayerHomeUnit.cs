using Gameplay.GameUnit.SoldierUnit.CombatUnit;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.GameUnit
{
    public class PlayerHomeUnit : GameUnitBase,IDefenable
    {
        public  PlayerBase playerBase;
        private int        _curHp;
        public  int        MaxHp { get; private set; }

        public int CurHp
        {
            get => _curHp;
            private set
            {
                _curHp = value;
                if (_curHp <= 0)
                {
                    IsDeath = true;
                    Destroy(this.gameObject);
                    DeathEvent.Invoke(this);
                }
            }
        }

        public bool IsDeath { get; private set; } = false;

        public int                      Defence { get; private set; }
        public event AttackEventHandler BeAttackedEvent;
        public void                     BeAttacked(IAttackable attacker)
        {
            this.CurHp -= Mathf.Max(attacker.Attack - this.Defence,1) ;
            BeAttackedEvent?.Invoke(attacker, this);
        }

        public UnityEvent<IDefenable> DeathEvent { get; }

        protected override void Start()
        {
            base.Start();
            this.CurHp   = playerBase.maxHp;
            this.MaxHp   = playerBase.maxHp;
            this.Defence = playerBase.defence;
        }
    }
}
