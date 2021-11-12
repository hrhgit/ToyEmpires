using Gameplay.Buff;
using Gameplay.GameUnit.SoldierUnit.CombatUnit;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.GameUnit
{
    public class PlayerHomeUnit : GameUnitBase,IDefenable
    {
        public                   PlayerBase       playerBase;
        private                  IntBuffableValue _curHp   = new IntBuffableValue();
        [SerializeField] private IntBuffableValue _maxHp   = new IntBuffableValue();
        [SerializeField] private IntBuffableValue _defence = new IntBuffableValue();

        public int MaxHp => _maxHp.Value;

        public int CurHp
        {
            get => _curHp.Value;
            private set
            {
                _curHp.Value = value;
                if (_curHp.Value <= 0)
                {
                    IsDeath = true;
                    Destroy(this.gameObject);
                    DeathEvent.Invoke(this);
                }
            }
        }

        public bool IsDeath { get; private set; } = false;

        public int Defence => _defence.Value;

        public event AttackEventHandler BeAttackedEvent;
        public void                     BeAttacked(IAttackable attacker)
        {
            this.CurHp -= Mathf.Max(attacker.Attack - this.Defence,1) ;
            BeAttackedEvent?.Invoke(attacker, this);
        }

        public UnityEvent<IDefenable> DeathEvent { get; } = new UnityEvent<IDefenable>();

        protected override void Start()
        {
            base.Start();
            this.CurHp   = this.MaxHp;
            this.DeathEvent.AddListener((player => this.playerBase.playerDieEvent.Invoke(this.playerBase)));
        }
    }
}
