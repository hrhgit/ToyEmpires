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
    public abstract class  SoldierUnitBase : GameUnitBase, IDefenable, IMovable, IMaintenanceRequired, IBuffable<UnitBuffContainer>
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
        [Header("移动")]
        [SerializeField] private FloatBuffableValue maxSpeed = new FloatBuffableValue();
        public                   GameUnitMover      UnitMover { get; private set; }
        public                   float              MaxSpeed    => maxSpeed;
        public                   bool               AtEnemyHome { get; protected set; } = false;

        #endregion

        #region 生产
        [Header("生产力")]
        [SerializeField] private IntBuffableValue productivity = new IntBuffableValue();
        public                   int Productivity
        {
            get => productivity;
            private set => productivity.Value = value;
        }

        #endregion

        #region 受击
        [Header("防御")]
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
                    Die();
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
        protected virtual void Die()
        {
            IsDeath = true;
            Destroy(this.gameObject);
            DeathEvent.Invoke(this);
        }


        #endregion

        #region Buff
        [Header("Buff")]
        [SerializeField] private UnitBuffContainer _buffContainer;
        public UnitBuffContainer BuffContainer
        {
            get => _buffContainer;
            set => _buffContainer = value;
        }

        public virtual bool SetNumericalValueBuff(BuffNumericalValueType buffType, bool isAdditionalValue, float value)
        {
            //TODO 未完善
            switch (buffType)
            {
                case BuffNumericalValueType.Defence:
                    if (isAdditionalValue)
                        this.defence.AddAdditionalValue((int)value);
                    else
                        this.defence.AddMagnification(value);
                    break;
                case BuffNumericalValueType.Productivity:
                    if (isAdditionalValue)
                        this.productivity.AddAdditionalValue((int)value);
                    else
                        this.productivity.AddMagnification(value);
                    break;
                case BuffNumericalValueType.MaintenanceTime:
                    if (isAdditionalValue)
                        this.maintenanceTime.AddAdditionalValue((int)value);
                    else
                        this.maintenanceTime.AddMagnification(value);
                    break;
                default:
                    throw new UnityException("未找到Buff: " + buffType.ToString());
                    return false;
            }

            return true;
        }


        #endregion
        
        #region 维护费用

        [Header("维护费用")]
        [SerializeField] private IntBuffableValue maintenanceCostFood = new IntBuffableValue(0);
        [SerializeField] private FloatBuffableValue maintenanceTime = new FloatBuffableValue(5f);
        
        private                  bool               _isWellResourced;
        public                   int                MaintenanceCostFood => maintenanceCostFood;

        protected      int          hungerBuffId = 240000;
        public virtual UnitBuffBase NonResourceDeBuff => BuffGenerator.GenerateBuff(hungerBuffId) as UnitBuffBase;

        private float _maintenanceTimer = 0f;

        public virtual bool IsWellResourced
        {
            get => _isWellResourced;
            set
            {
                if (value && !_isWellResourced)  //伙食改产
                {
                    this.BuffContainer.RemoveBuffByID(hungerBuffId);
                }
                else if(!value && _isWellResourced) //伙食变差
                {
                    this.BuffContainer.AddBuff(this.NonResourceDeBuff);
                }

                _isWellResourced = value;
            }
        }
        

        public void RequireMaintenance()
        {
            this.IsWellResourced = this.UnitSide.Maintain(this);
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

        protected virtual void FixedUpdate()
        {
            _maintenanceTimer += Time.fixedDeltaTime;
            if (_maintenanceTimer >= maintenanceTime)
            {
                RequireMaintenance();
                _maintenanceTimer = 0f;
            }
        }
    }
}
