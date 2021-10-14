using System;
using System.Collections.Generic;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.GameUnit.SoldierUnit.CombatUnit.MeleeUnit
{
    public class MeleeUnitsBase : SoldierUnitBase, IProduceable, ICombatable 
    {
        #region 生产

        [SerializeField] private int _costTime;
        [SerializeField] private int _costFood;
        [SerializeField] private int _costWood;
        [SerializeField] private int _costGold;
        [SerializeField] private int _maxReserveCount;

        public int CostTime => _costTime;

        public int CostFood => _costFood;

        public int CostWood => _costWood;

        public int CostGold => _costGold;

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
        [SerializeField] private int   attack;
        [SerializeField] private float attackRange;
        [SerializeField] private float attackInterval;
        [SerializeField] private float findEnemyRange;

        public int   Attack         => attack;
        public float AttackRange    => attackRange;
        public float AttackInterval => attackInterval;

        public float FindEnemyRange => findEnemyRange;

        public event AttackEventHandler AttackEvent;
        public event AttackEventHandler BeAttackedEvent;
        
        private static int               _maxEnemyFound        = 10;
        private        List<IAttackable> _visualFieldEnemyList = new List<IAttackable>();
        private        IAttackable       _curEnemy             = null;
        private        UnitVisualField   _unitVisualField;
        private        bool              _attackEnemyBase = true;
        

        public override void BeAttacked(ICombatable attacker)
        {
            base.BeAttacked(attacker);
        }

        public void DoAttack(IAttackable attackTarget)
        {
            throw new NotImplementedException();
        }

        public IAttackable FindEnemy()
        {
            // int          enemyFoundCount = Physics.OverlapSphereNonAlloc(this.transform.position, AttackRange, _enemyFoundArr, 1 <<LayerMask.NameToLayer("Unit"));
            float       minDist     = float.MaxValue;
            IAttackable closestEnemy = null;
            for (int i = 0; i < _visualFieldEnemyList.Count; i++)
            {
                IAttackable a = _visualFieldEnemyList[i];
                if(((GameUnitBase)a).UnitTeam == this.UnitTeam || ((GameUnitBase)a).UnitRoad != this.UnitRoad)
                    continue;
                float d = Vector3.Distance(this.transform.position, ((GameUnitBase)a).transform.position);
                minDist     = d < minDist ? d : minDist;
                closestEnemy = a;
            }
            return closestEnemy;
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
                                                              IAttackable u = null;
                                                              if (c.gameObject.TryGetComponent<IAttackable>(out u))
                                                              {
                                                                  Debug.Log("Enemy In!");
                                                                  _visualFieldEnemyList.Add(u);
                                                              }
                                                          });
            _unitVisualField.colliderExitEvent.AddListener((v, c) =>
                                                           {
                                                               IAttackable u = null;
                                                               if (c.gameObject.TryGetComponent<IAttackable>(out u))
                                                               {
                                                                   Debug.Log("Enemy Lose!");
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
            
        }


        private void FixedUpdate()
        {
            //当前进攻对象为敌人基地，且有预备敌人
            if ((_attackEnemyBase && _visualFieldEnemyList.Count >= 0) || (!_attackEnemyBase && !_visualFieldEnemyList.Contains(_curEnemy)))
            {
                _curEnemy = FindEnemy();
                if (_curEnemy == null)
                {
                    _attackEnemyBase      = true;
                    _curEnemy             = this.EnemySide.homeUnit as IAttackable;
                    
                }
                else
                {
                    _attackEnemyBase      = false;
                    this.UnitMover.Target = ((GameUnitBase)_curEnemy).transform.position;
                }
            }
            Debug.Log(this.gameObject.name + ":" + ((GameUnitBase)_curEnemy).gameObject.name);
        }

    }
}