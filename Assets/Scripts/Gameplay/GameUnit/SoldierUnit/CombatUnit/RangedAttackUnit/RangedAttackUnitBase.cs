using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Buff;
using Gameplay.GameUnit.FortificationUnit;
using Gameplay.GameUnit.ThrowingObject;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Gameplay.GameUnit.SoldierUnit.CombatUnit.RangedAttackUnit
{
    public class RangedAttackUnitBase : SoldierUnitBase, IProduceable, IRangeAttackable
    {


        #region 生产

        [SerializeField] private FloatBuffableValue _costTime = new FloatBuffableValue();
        [SerializeField] private IntBuffableValue   _costFood = new IntBuffableValue();
        [SerializeField] private IntBuffableValue   _costWood = new IntBuffableValue();
        [SerializeField] private IntBuffableValue   _costGold = new IntBuffableValue();
        [SerializeField] private IntBuffableValue   _costPopulation = new IntBuffableValue(1);
        [SerializeField] private IntBuffableValue   _maxReserveCount = new IntBuffableValue();

        public float CostTime => _costTime;

        public int CostFood => _costFood;

        public int CostWood => _costWood;

        public int CostGold       => _costGold;

        public int CostPopulation => _costPopulation;

        public int MaxReserveCount => _maxReserveCount;

        public void Produce(SoldierUnitBase unit, PlayerBase player, UnitStatus status)
        {
            if (status.freeUnitCount >= MaxReserveCount)
                return;
            if (status.unitProduceProcess < 1)
            {
                status.unitProduceProcess += Time.fixedDeltaTime / (((IProduceable)unit).CostTime);
                player.InvokeUnitProduce(unit, player, status);
            }
            else
            {
                status.unitProduceProcess = 0;
                status.totalUnitCount++;
                status.freeUnitCount++;
            }

        }

        #endregion
        
        #region 战斗

        #region 射击相关
        public Transform          throwingPoint;
        
        [SerializeField] private FloatBuffableValue              throwingSpeed = new FloatBuffableValue();
        [SerializeField] private IntBuffableValue                throwingCount = new IntBuffableValue();
        [SerializeField] private FloatBuffableValue              throwingInterval = new FloatBuffableValue();
        [SerializeField] private FloatBuffableValue              _accuracy = new FloatBuffableValue();
        [SerializeField] private ThrowingObjectBase _throwingObject;


        public float ThrowingSpeed
        {
            get => throwingSpeed;
            private set => throwingSpeed.Value = value;
        }

        public float Accuracy
        {
            get => _accuracy;
            private set => _accuracy.Value = value;
        }

        public int   ThrowingCount
        {
            get => throwingCount;
            private set => throwingCount.Value = value;
        }

        public float ThrowingInterval
        {
            get => throwingInterval;
            private set => throwingInterval.Value = value;
        }
        public ThrowingObjectBase ThrowingObject => _throwingObject;


        public UnityEvent<IRangeAttackable, IDefenable> ThrowingEvent { get; set; } = new UnityEvent<IRangeAttackable, IDefenable>();


        #endregion

        [SerializeField] private IntBuffableValue   attack         = new IntBuffableValue();
        [SerializeField] private FloatBuffableValue attackRange    = new FloatBuffableValue();
        [SerializeField] private FloatBuffableValue attackInterval = new FloatBuffableValue();
        [SerializeField] private FloatBuffableValue findEnemyRange = new FloatBuffableValue();

        public int   Attack         => attack;
        public float AttackRange    => attackRange;
        public float AttackInterval => attackInterval;
        public float FindEnemyRange => findEnemyRange;

        public event AttackEventHandler AttackEvent;

        private static int              _maxEnemyFound        = 10;
        private        List<IDefenable> _visualFieldEnemyList = new List<IDefenable>();
        private        IDefenable       _curEnemy             = null;
        private        UnitVisualField  _unitVisualField;
        private        bool             _attackEnemyBase = true;
        private        float            _attackTimer     = 0f;
        private        Transform        _miscParent;



        public override void BeAttacked(IAttackable attacker)
        {
            base.BeAttacked(attacker);
            try
            {
                IDefenable defenableUnit = (IDefenable)attacker;
                if (defenableUnit.CurHp < this._curEnemy.CurHp)
                {
                    this._curEnemy = defenableUnit;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public bool DoAttack(IDefenable attackTarget)
        {
            GameUnitBase u = (GameUnitBase)attackTarget;
            if(Vector3.Distance(u.transform.position,this.transform.position) < attackRange)
            {
                this.UnitMover.EnableMove = false;
                StartCoroutine(this.ThrowObjects(attackTarget));

                AttackEvent?.Invoke(this, attackTarget);
                return true;
            }
            else
            {
                this.UnitMover.EnableMove = true;
                this.UnitMover.Target     = u.transform;
                return false;
            }
        }
        
        
        public IEnumerator ThrowObjects(IDefenable attackTarget)
        {
            GameUnitBase   u               = (GameUnitBase)attackTarget;
            Transform      targetTransform = ((GameUnitBase)attackTarget).transform;
            WaitForSeconds wfs             = new WaitForSeconds(ThrowingInterval);
            for (int i = 0; i < ThrowingCount - 1; i++)
            {
                ThrowSingleObject(targetTransform, attackTarget);
                ThrowingEvent.Invoke(this,attackTarget);
                yield return wfs;
            }
            Debug.Log("Shoot Count:" );
            ThrowSingleObject(targetTransform, attackTarget);
            // this.UnitMover.EnableMove = true;
            if(attackTarget.IsDeath)
            {
                SearchEnemy();
            }

            try
            {
                this.AtEnemyHome = (u is PlayerHomeUnit) || (u is TowerBase);
            }
            catch (Exception e)
            {
                // ignored
            }

        }
        
        private void ThrowSingleObject(Transform target,IDefenable targetUnit = null)
        {
            this.transform.LookAt(target);
            this.transform.localEulerAngles          = new Vector3(0, this.transform.localEulerAngles.y, 0);
            throwingPoint.transform.localEulerAngles = new Vector3(0, 0, 0);
            float angle = GetShootAngle(target.position, Accuracy,.01f);
            throwingPoint.transform.Rotate(angle,0,0);
            ThrowingObjectBase throwingObjectInstance = Instantiate(ThrowingObject, throwingPoint.position, throwingPoint.rotation, _miscParent);
            GameObject         throwingGameObject     = throwingObjectInstance.gameObject;
            Rigidbody          rigidbody              = throwingGameObject.GetComponent<Rigidbody>();
            throwingObjectInstance.shooter  = this;
            throwingObjectInstance.UnitTeam = this.UnitTeam;
            throwingObjectInstance.UnitRoad = this.UnitRoad;
            throwingObjectInstance.target   = targetUnit;
            throwingObjectInstance.ShotSuccessedEvent += (shooter, hitter) =>
                                                         {
                                                             hitter.BeAttacked(this);
                                                         };
            rigidbody.velocity = throwingObjectInstance.transform.forward * (this.ThrowingSpeed  + Random.Range(Accuracy-1,1-Accuracy) * .1f);
        }


        protected float GetShootAngle(Vector3 targetPos)
        {
            float v0 = this.ThrowingSpeed;
            float h  = Mathf.Abs(this.throwingPoint.position.y - targetPos.y);
            // float h  = 0;
            float ud = Mathf.Sqrt(v0 * v0 + 2 * Physics.gravity.magnitude * h);
            return Mathf.Atan(v0 / ud) * Mathf.Rad2Deg;
        }

        protected float GetShootAngle(Vector3 targetPos, float accuracy, float heightOffset)
        {
            float v0 = this.ThrowingSpeed;
            float h  = (this.throwingPoint.position.y + heightOffset) - targetPos.y;
            // float h  = 0;
            float ud = Mathf.Sqrt(v0 * v0 + 2 * Physics.gravity.magnitude * h);
            return Mathf.Atan(v0 / ud) * Mathf.Rad2Deg + Random.Range(accuracy -1, 1 -accuracy) * 30;
        }
        
        protected float GetShootAngle(Vector3 targetPos, float accuracy)
        {
            return GetShootAngle(targetPos, accuracy, 0);
        }

        public virtual IDefenable FindAEnemy()
        {
            float       minDist     = float.MaxValue;
            IDefenable closestEnemy = null;
            for (int i = 0; i < _visualFieldEnemyList.Count; i++)
            {
                IDefenable a = _visualFieldEnemyList[i];
                if((a.IsDeath) || ((GameUnitBase)a).UnitTeam == this.UnitTeam)
                    continue;
                if(a is PlayerHomeUnit && _visualFieldEnemyList.Count > 1)
                    continue;
                float d = Vector3.Distance(this.transform.position, ((GameUnitBase)a).transform.position);
                minDist     = d < minDist ? d : minDist;
                closestEnemy = a;
            }
            return closestEnemy;
        }

        private void EnemyListGC()
        {
            for (int i = 0; i < _visualFieldEnemyList.Count; i++)
            {
                if (_visualFieldEnemyList[i] == null)
                {
                    _visualFieldEnemyList.RemoveAt(i);
                    i--;
                }
            }
        }
        
        private void SearchEnemy()
        {
            EnemyListGC();
            //当前进攻对象为敌人基地，且有预备敌人
            if ((_attackEnemyBase && _visualFieldEnemyList.Count >= 0) || (!_attackEnemyBase && !_visualFieldEnemyList.Contains(_curEnemy)) || (_curEnemy.IsDeath) || (_curEnemy == null))
            {
                _curEnemy = FindAEnemy();
                if (_curEnemy == null || _curEnemy.IsDeath)
                {
                    _attackEnemyBase = true;
                    _curEnemy        = this.EnemySide.homeUnit as IDefenable;
                }
                else
                {
                    _attackEnemyBase      = false;
                    this.UnitMover.Target = ((GameUnitBase)_curEnemy).transform;
                }
            }
        }


        private void FindEnemyInit()
        {
            _unitVisualField = this.transform.Find("VisualField").GetComponent<UnitVisualField>();
            _unitVisualField.initEvent.AddListener((visualField =>
                                                    {
                                                        visualField.VisualFieldSphere.radius = FindEnemyRange;
                                                    }));
            _unitVisualField.colliderHitEvent.AddListener((v, c) =>
                                                          {
                                                              IDefenable u = null;
                                                              if (c.gameObject.TryGetComponent<IDefenable>(out u))
                                                              {
                                                                  // Debug.Log("Enemy In!");
                                                                  _visualFieldEnemyList.Add(u);
                                                              }
                                                          });
            _unitVisualField.colliderExitEvent.AddListener((v, c) =>
                                                           {
                                                               IDefenable u = null;
                                                               if (c.gameObject.TryGetComponent<IDefenable>(out u))
                                                               {
                                                                   // Debug.Log("Enemy Lose!");
                                                                   _visualFieldEnemyList.Remove(u);
                                                               }
                                                           });
        }

        #endregion

        protected override void Awake()
        {
            base.Awake();
            FindEnemyInit();
        }


        protected override void Start()
        {
            base.Start();
            _miscParent = BattleGameManager.BattleGameManagerInstance.miscParent;
        }


        private void FixedUpdate()
        {
            SearchEnemy();

            _attackTimer += Time.fixedDeltaTime;
            if (_curEnemy != null && !_curEnemy.IsDeath && _attackTimer > AttackInterval)
            {
                _attackTimer = this.DoAttack(_curEnemy) ? 0 : _attackTimer;
            }
            
            
        }
    }
}
