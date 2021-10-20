using System;
using System.Collections;
using Gameplay.Buff;
using Gameplay.GameUnit.SoldierUnit.CombatUnit;
using Gameplay.GameUnit.SoldierUnit.CombatUnit.RangedAttackUnit;
using Rendering;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.GameUnit.FortificationUnit
{
    public class FortificationUnitBase : GameUnitBase, IDefenable
    {
        [SerializeField] private IntBuffableValue _maxHp = new IntBuffableValue();
        [SerializeField] private IntBuffableValue _defence = new IntBuffableValue();
        private                  bool             _isDeath;
        private                  IntBuffableValue _curHp = new IntBuffableValue();

        public int  MaxHp   => _maxHp.Value;
        public int CurHp
        {
            get => _curHp.Value;
            private set
            {
                _curHp.Value = value;
                if (_curHp.Value <= 0)
                {
                    // _curHp = 0;
                    IsDeath = true;
                    Destroy(this.gameObject);
                    DeathEvent.Invoke(this);
                    
                }
            }
        }

        public bool IsDeath
        {
            get => _isDeath;
            private set => _isDeath = value;
        }

        public int  Defence => _defence;

        public event AttackEventHandler BeAttackedEvent;
        public UnityEvent<IDefenable>   DeathEvent { get; } = new UnityEvent<IDefenable>();

        public virtual void BeAttacked(IAttackable attacker)
        {
            this.CurHp -= attacker.Attack - this.Defence;
            BeAttackedEvent?.Invoke(attacker, this);
        }


        protected virtual void Awake()
        {
            
        }

        protected void InitHP()
        {
            this.CurHp = this.MaxHp;
        }
        
        protected void ChangeUnitMaterialColor()
        {
            this.transform.Find("Sprite").GetComponent<SpriteRendererPlus>().ChangeColor();
            DeathEvent.AddListener((u =>
                                    {
                                        Resources.UnloadUnusedAssets();
                                    }));
        }


        protected override void Start()
        {
            switch (UnitTeam)
            {
                case Team.Blue:
                    this.UnitSide         = BattleGameManager.BattleGameManagerInstance.bluePlayer;
                    this.EnemySide        = BattleGameManager.BattleGameManagerInstance.redPlayer;
                    this.gameObject.layer = LayerMask.NameToLayer("BlueFortification");
                    break;
                case Team.Red:
                    this.EnemySide        = BattleGameManager.BattleGameManagerInstance.bluePlayer;
                    this.UnitSide         = BattleGameManager.BattleGameManagerInstance.redPlayer;
                    this.gameObject.layer = LayerMask.NameToLayer("RedFortification");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            InitHP();
            // ChangeUnitMaterialColor();

        }
    }
}
