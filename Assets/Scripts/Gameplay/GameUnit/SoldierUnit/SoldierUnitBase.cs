using System;
using Gameplay.GameUnit.SoldierUnit.CombatUnit;
using Gameplay.Player;
using Rendering;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.GameUnit.SoldierUnit
{
    public delegate void GameUnitEvent(SoldierUnitBase soldierUnitBase, PlayerBase playerBase,UnitStatus status);
    public abstract class  SoldierUnitBase : GameUnitBase, IDefenable, IMovable
    {
        public override float UnitValue
        {
            get
            {
                if (_unitValue <= 0)
                {
                    _unitValue = this.MaxHp * this.Defence;
                }
                return _unitValue;
            }
        }

        protected virtual void Awake()
        {
            this.UnitMover = this.GetComponent<GameUnitMover>();
        }

        protected override void Start()
        {
            base.Start();
            InitHP();
            switch (UnitTeam)
            {
                case Team.Blue:
                    this.UnitMover.Target = BattleGameManager.BattleGameManagerInstance.redPlayer.homeUnit.transform;
                    break;
                case Team.Red:
                    this.UnitMover.Target = BattleGameManager.BattleGameManagerInstance.bluePlayer.homeUnit.transform;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            ChangeUnitMaterialColor();

        }

        protected void InitHP()
        {
            this.CurHp = this.MaxHp;
        }


        #region 移动

        public                   GameUnitMover UnitMover { get; private set; }
        [SerializeField] private float         maxSpeed;
        public                   float         MaxSpeed    => maxSpeed;
        public                   bool          AtEnemyHome { get; protected set; } = false;

        #endregion

        #region 生产


        #endregion

        #region 受击

        [SerializeField] protected int                    defence;
        [SerializeField] private   int                    _maxHp;
        private                    int                    _curHp;
        private                    UnityEvent<IDefenable> _deathEvent;


        public int MaxHp => _maxHp;

        public int CurHp
        {
            get => _curHp;
            private set
            {
                _curHp = value;
                if (_curHp <= 0)
                {
                    // _curHp = 0;
                    IsDeath = true;
                    Destroy(this.gameObject);
                    DeathEvent.Invoke(this);
                    
                }
            }
        }

        public bool IsDeath { get; private set; } = false;

        public int Defence => defence;

        public UnityEvent<IDefenable> DeathEvent
        {
            get
            {
                if (_deathEvent == null)
                {
                    _deathEvent = new UnityEvent<IDefenable>();
                }
                
                return _deathEvent;
            }
        }


        public event AttackEventHandler BeAttackedEvent;
        public virtual void BeAttacked(IAttackable attacker)
        {
            this.CurHp -= attacker.Attack - this.Defence;
            BeAttackedEvent?.Invoke(attacker, this);
        }


        #endregion

        #region 杂项

        protected void ChangeUnitMaterialColor()
        {
            this.transform.Find("Sprite").GetComponent<SpriteRendererPlus>().ChangeColor();
            DeathEvent.AddListener((u =>
                                    {
                                        Resources.UnloadUnusedAssets();
                                    }));
        }


        #endregion

    }
}
