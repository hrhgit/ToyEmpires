using System;
using Gameplay.Buff;
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
            this._curHp    = new IntBuffableValue(0);
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

        public                   GameUnitMover      UnitMover { get; private set; }
        [SerializeField] private FloatBuffableValue maxSpeed = new FloatBuffableValue();
        public                   float              MaxSpeed    => maxSpeed;
        public                   bool               AtEnemyHome { get; protected set; } = false;

        #endregion

        #region 生产
        [SerializeField] private IntBuffableValue productivity = new IntBuffableValue();
        public                   int Productivity
        {
            get => productivity;
            private set => productivity.Value = value;
        }

        #endregion

        #region 受击

        [SerializeField] protected IntBuffableValue       defence = new IntBuffableValue();
        [SerializeField] private   IntBuffableValue       _maxHp = new IntBuffableValue();
        private                    IntBuffableValue       _curHp = new IntBuffableValue();
        private                    UnityEvent<IDefenable> _deathEvent;


        public int MaxHp => _maxHp.Value;

        public int CurHp
        {
            get
            {
                return _curHp.Value;
            }
            private set
            {
                _curHp.Value =   value;
                if (_curHp.Value <= 0)
                {
                    // _curHp = 0;
                    IsDeath = true;
                    Destroy(this.gameObject);
                    DeathEvent.Invoke(this);
                    
                }
            }
        }

        public bool IsDeath { get; private set; } = false;

        public int Defence => defence.Value;

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
