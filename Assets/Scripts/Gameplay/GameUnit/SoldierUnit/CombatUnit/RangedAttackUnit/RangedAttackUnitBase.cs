using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.GameUnit.ThrowingObject;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.GameUnit.SoldierUnit.CombatUnit.RangedAttackUnit
{
    public class RangedAttackUnitBase : SoldierUnitBase, IProduceable, IRangeAttackable
    {


        #region 生产

        [SerializeField] private int _costTime;
        [SerializeField] private int _costFood;
        [SerializeField] private int _costWood;
        [SerializeField] private int _costGold;
        [SerializeField] private int _costPopulation = 1;
        [SerializeField] private int _maxReserveCount;

        public int CostTime => _costTime;

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

        public ThrowingObjectBase throwingObject;
        public Transform          throwingPoint;
        
        [SerializeField] private float throwingSpeed;
        [SerializeField] private int   throwingCount;
        [SerializeField] private float throwingInterval;

        public float ThrowingSpeed
        {
            get => throwingSpeed;
            private set => throwingSpeed = value;
        }
        public int   ThrowingCount
        {
            get => throwingCount;
            private set => throwingCount = value;
        }

        public float ThrowingInterval
        {
            get => throwingInterval;
            private set => throwingInterval = value;
        }

        public UnityEvent<IRangeAttackable, IDefenable> ThrowingEvent { get; set; } = new UnityEvent<IRangeAttackable, IDefenable>();


        #endregion

        [SerializeField] private int   attack;
        [SerializeField] private float attackRange;
        [SerializeField] private float attackInterval;
        [SerializeField] private float findEnemyRange;

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
                this.AtEnemyHome = ((PlayerHomeUnit)attackTarget) != null;
            }
            catch (Exception e)
            {
                // ignored
            }

        }
        
        private void ThrowSingleObject(Transform target,IDefenable targetUnit = null)
        {
            throwingPoint.transform.LookAt(target);
            float              angle                  = GetShootAngle(target.position);
            throwingPoint.transform.Rotate(-angle,0,0);
            ThrowingObjectBase throwingObjectInstance = Instantiate(throwingObject, throwingPoint.position, throwingPoint.rotation, _miscParent);
            GameObject         throwingGameObject     = throwingObjectInstance.gameObject;
            Rigidbody          rigidbody              = throwingGameObject.GetComponent<Rigidbody>();
            throwingObjectInstance.shooter            =  this;
            throwingObjectInstance.target             =  targetUnit;
            throwingObjectInstance.ShotSuccessedEvent += (shooter, hitter) =>
                                                         {
                                                             hitter.BeAttacked(this);
                                                         };
            rigidbody.velocity = (throwingObjectInstance.transform.forward * this.ThrowingSpeed);
        }


        protected float GetShootAngle(Vector3 targetPos)
        {
            float v0 = this.ThrowingSpeed;
            // float h  = Mathf.Abs(this.throwingPoint.position.y - targetPos.y);
            float h  = 0;
            float ud = Mathf.Sqrt(v0 * v0 + 2 * Physics.gravity.magnitude * h);
            return Mathf.Atan(v0 / ud) * Mathf.Rad2Deg;
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
