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
    public class RangedAttackUnitBase : ProducebleCombatUnitBase, IRangeAttackable
    {
        #region 战斗

        [Header("战斗")]

        #region 射击相关

        public Transform throwingPoint;

        [SerializeField] private FloatBuffableValue throwingSpeed    = new FloatBuffableValue();
        [SerializeField] private IntBuffableValue   throwingCount    = new IntBuffableValue();
        [SerializeField] private FloatBuffableValue throwingInterval = new FloatBuffableValue();
        [SerializeField] private FloatBuffableValue _accuracy        = new FloatBuffableValue();
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

        public int ThrowingCount
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

        private static   int              _maxEnemyFound        = 10;
        private readonly List<IDefenable> _visualFieldEnemyList = new List<IDefenable>();
        private          IDefenable       _curEnemy;
        private          UnitVisualField  _unitVisualField;
        private          bool             _attackEnemyBase = true;
        private          float            _attackTimer;
        private          Transform        _miscParent;


        public override void BeAttacked(IAttackable attacker)
        {
            base.BeAttacked(attacker);
            try
            {
                var defenableUnit                                    = (IDefenable)attacker;
                if (defenableUnit.CurHp < _curEnemy.CurHp) _curEnemy = defenableUnit;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public bool DoAttack(IDefenable attackTarget)
        {
            var u = (GameUnitBase)attackTarget;
            if (Vector3.Distance(u.transform.position, transform.position) < attackRange)
            {
                UnitMover.EnableMove = false;
                StartCoroutine(ThrowObjects(attackTarget));

                AttackEvent?.Invoke(this, attackTarget);
                return true;
            }

            UnitMover.EnableMove = true;
            UnitMover.Target     = u.transform;
            return false;
        }


        public IEnumerator ThrowObjects(IDefenable attackTarget)
        {
            var u               = (GameUnitBase)attackTarget;
            var targetTransform = ((GameUnitBase)attackTarget).transform;
            var wfs             = new WaitForSeconds(ThrowingInterval);
            for (var i = 0; i < ThrowingCount - 1; i++)
            {
                ThrowSingleObject(targetTransform, attackTarget);
                ThrowingEvent.Invoke(this, attackTarget);
                yield return wfs;
            }
            
            ThrowSingleObject(targetTransform, attackTarget);
            // this.UnitMover.EnableMove = true;
            if (attackTarget.IsDeath) SearchEnemy();

            try
            {
                AtEnemyHome = u is PlayerHomeUnit || u is TowerBase;
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        private void ThrowSingleObject(Transform target, IDefenable targetUnit = null)
        {
            transform.LookAt(target);
            if(!(targetUnit is FortificationUnitBase))
            {
                transform.localEulerAngles               = new Vector3(0, transform.localEulerAngles.y, 0);
                throwingPoint.transform.localEulerAngles = new Vector3(0, 0,                            0);
                var angle = GetShootAngle(target.position, Accuracy);
                throwingPoint.transform.Rotate(angle, 0, 0);
            } 
            var throwingObjectInstance = Instantiate(ThrowingObject, throwingPoint.position, throwingPoint.rotation, _miscParent);
            var throwingGameObject     = throwingObjectInstance.gameObject;
            var rigidbody              = throwingGameObject.GetComponent<Rigidbody>();
            throwingObjectInstance.shooter            =  this;
            throwingObjectInstance.UnitTeam           =  UnitTeam;
            throwingObjectInstance.UnitRoad           =  UnitRoad;
            throwingObjectInstance.target             =  targetUnit;
            throwingObjectInstance.ShotSuccessedEvent += (shooter, hitter) => { hitter.BeAttacked(this); };
            rigidbody.velocity                        =  throwingObjectInstance.transform.forward * (ThrowingSpeed + Random.Range(Accuracy - 1, 1 - Accuracy) * .1f);
        }


        protected float GetShootAngle(Vector3 targetPos)
        {
            var maxAngle = 45 * Mathf.Deg2Rad;
            var vMax     = ThrowingSpeed;
            var h        = Mathf.Abs(throwingPoint.position.y - targetPos.y);
            var s        = Vector3.Distance(throwingPoint.position, targetPos);
            // float h  = 0;
            var ud    = Mathf.Sqrt(vMax * vMax + 2 * Physics.gravity.magnitude * h);
            var angle = Mathf.Atan(vMax / ud)                                  * Mathf.Rad2Deg;
            return Mathf.LerpAngle(maxAngle, 0, Mathf.Max(0, angle - maxAngle) / 90);
        }

        protected float GetShootAngle(Vector3 targetPos, float accuracy, float heightOffset)
        {
            var maxAngle = 45 * Mathf.Deg2Rad;
            var vMax     = ThrowingSpeed;
            var s        = Vector3.Distance(throwingPoint.position, targetPos) + Random.Range(0.0f, 0.1f) * accuracy;
            var angle    = Mathf.Asin(Mathf.Min(s * Physics.gravity.magnitude / (vMax * vMax), 1))        / 2 * Mathf.Rad2Deg;
            // Debug.DrawRay(throwingPoint.position,
            //               new Matrix4x4(new Vector4(1, 0,                                0,                                 0),
            //                             new Vector4(0, Mathf.Cos(angle * Mathf.Deg2Rad), -Mathf.Sin(angle * Mathf.Deg2Rad), 0),
            //                             new Vector4(0, Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad),  0),
            //                             new Vector4(0, 0,                                0,                                 1)) * throwingPoint.forward,
            //               Color.cyan,
            //               10f);
            return Mathf.LerpAngle(maxAngle, -10, Mathf.Max(0, angle - maxAngle) / 90);
        }

        protected float GetShootAngle(Vector3 targetPos, float accuracy)
        {
            return GetShootAngle(targetPos, accuracy, 0);
        }

        public virtual IDefenable FindAEnemy()
        {
            var        minDist      = float.MaxValue;
            IDefenable closestEnemy = null;
            for (var i = 0; i < _visualFieldEnemyList.Count; i++)
            {
                var a = _visualFieldEnemyList[i];
                if (a.IsDeath || ((GameUnitBase)a).UnitTeam == UnitTeam)
                    continue;
                if (a is PlayerHomeUnit && _visualFieldEnemyList.Count > 1)
                    continue;
                var d = Vector3.Distance(transform.position, ((GameUnitBase)a).transform.position);
                minDist      = d < minDist ? d : minDist;
                closestEnemy = a;
            }

            return closestEnemy;
        }

        private void EnemyListGC()
        {
            for (var i = 0; i < _visualFieldEnemyList.Count; i++)
                if (_visualFieldEnemyList[i] == null)
                {
                    _visualFieldEnemyList.RemoveAt(i);
                    i--;
                }
        }

        private void SearchEnemy()
        {
            EnemyListGC();
            //当前进攻对象为敌人基地，且有预备敌人
            if (_attackEnemyBase && _visualFieldEnemyList.Count >= 0 || !_attackEnemyBase && !_visualFieldEnemyList.Contains(_curEnemy) || _curEnemy.IsDeath || _curEnemy == null || UnitMover.EnableMove)
            {
                _curEnemy = FindAEnemy();
                if (_curEnemy == null || _curEnemy.IsDeath)
                {
                    _attackEnemyBase = true;
                    _curEnemy        = EnemySide.homeUnit;
                }
                else
                {
                    _attackEnemyBase = false;
                    UnitMover.Target = ((GameUnitBase)_curEnemy).transform;
                }
            }
        }


        private void FindEnemyInit()
        {
            _unitVisualField = transform.Find("VisualField").GetComponent<UnitVisualField>();
            _unitVisualField.initEvent.AddListener(visualField => { visualField.VisualFieldSphere.radius = FindEnemyRange; });
            _unitVisualField.colliderHitEvent.AddListener((v, c) =>
                                                          {
                                                              IDefenable u = null;
                                                              if (c.gameObject.TryGetComponent(out u))
                                                                  // Debug.Log("Enemy In!");
                                                                  _visualFieldEnemyList.Add(u);
                                                          });
            _unitVisualField.colliderExitEvent.AddListener((v, c) =>
                                                           {
                                                               IDefenable u = null;
                                                               if (c.gameObject.TryGetComponent(out u))
                                                                   // Debug.Log("Enemy Lose!");
                                                                   _visualFieldEnemyList.Remove(u);
                                                           });
        }

        #endregion

        protected override void Awake()
        {
            base.Awake();
            ThrowingSpeed = Mathf.Sqrt(attackRange * Physics.gravity.magnitude);
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
            if (_curEnemy != null && !_curEnemy.IsDeath && _attackTimer > AttackInterval) _attackTimer = DoAttack(_curEnemy) ? 0 : _attackTimer;
        }
    }
}