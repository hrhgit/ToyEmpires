using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.GameUnit.SoldierUnit.CombatUnit;
using Gameplay.GameUnit.SoldierUnit.CombatUnit.RangedAttackUnit;
using Gameplay.GameUnit.ThrowingObject;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Gameplay.GameUnit.FortificationUnit
{
    public class TowerBase : FortificationUnitBase, IRangeAttackable
    {
        #region 战斗

        #region 射击相关

        public   Transform          throwingPoint;
        
        [SerializeField] private float              throwingSpeed;
        [SerializeField] private int                throwingCount;
        [SerializeField] private float              throwingInterval;
        [SerializeField] private float              _accuracy;
        [SerializeField] private ThrowingObjectBase _throwingObject;

        public float ThrowingSpeed
        {
            get => throwingSpeed;
            private set => throwingSpeed = value;
        }

        public float Accuracy
        {
            get => _accuracy;
            private set => _accuracy = value;
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

        public ThrowingObjectBase ThrowingObject => _throwingObject;

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
        private        float            _attackTimer = 0f;
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
                StartCoroutine(this.ThrowObjects(attackTarget));
                AttackEvent?.Invoke(this, attackTarget);
                return true;
            }
            else
            {
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
        }
        
        private void ThrowSingleObject(Transform target,IDefenable targetUnit = null)
        {
            throwingPoint.transform.LookAt(target);
            float angle = GetShootAngle(target.position, Accuracy);
            throwingPoint.transform.Rotate(-angle,0,0);
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
            rigidbody.velocity = throwingObjectInstance.transform.forward * (this.ThrowingSpeed  + Random.Range(Accuracy-1,1-Accuracy) * .5f);
        }


        protected float GetShootAngle(Vector3 targetPos)
        {
            float v0 = this.ThrowingSpeed;
            // float h  = Mathf.Abs(this.throwingPoint.position.y - targetPos.y);
            float h  = 0;
            float ud = Mathf.Sqrt(v0 * v0 + 2 * Physics.gravity.magnitude * h);
            return Mathf.Atan(v0 / ud) * Mathf.Rad2Deg;
        }
        
        protected float GetShootAngle(Vector3 targetPos, float accuracy)
        {
            float v0       = this.ThrowingSpeed;
            float h  = Mathf.Abs(this.throwingPoint.position.y - targetPos.y);
            // float h  = 0;
            float ud = Mathf.Sqrt(v0 * v0 + 2 * Physics.gravity.magnitude * h);
            return Mathf.Atan(v0 / ud) * Mathf.Rad2Deg + Random.Range(accuracy-1,1-accuracy) * 30;
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
            if ((_visualFieldEnemyList.Count >= 0) || (!_visualFieldEnemyList.Contains(_curEnemy)) || (_curEnemy.IsDeath) || (_curEnemy == null))
            {
                _curEnemy = FindAEnemy();
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
