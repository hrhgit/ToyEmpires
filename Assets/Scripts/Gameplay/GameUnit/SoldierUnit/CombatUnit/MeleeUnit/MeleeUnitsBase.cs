using System;
using System.Collections.Generic;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.GameUnit.SoldierUnit.CombatUnit.MeleeUnit
{
    public class MeleeUnitsBase : SoldierUnitBase, IProduceable, IAttackable 
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
        [SerializeField] private int   attack;
        [SerializeField] private float attackRange;
        [SerializeField] private float attackInterval;
        [SerializeField] private float findEnemyRange;

        public int   Attack         => attack;
        public float AttackRange    => attackRange;
        public float AttackInterval => attackInterval;

        public float FindEnemyRange => findEnemyRange;

        public event AttackEventHandler AttackEvent;

        private static int               _maxEnemyFound        = 10;
        private        List<IDefenable> _visualFieldEnemyList = new List<IDefenable>();
        private        IDefenable       _curEnemy             = null;
        private        UnitVisualField   _unitVisualField;
        private        bool              _attackEnemyBase = true;
        private        float             _attackTimer     = 0f;
        

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
                attackTarget.BeAttacked(this);
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

        public virtual IDefenable FindAEnemy()
        {
            // int          enemyFoundCount = Physics.OverlapSphereNonAlloc(this.transform.position, AttackRange, _enemyFoundArr, 1 <<LayerMask.NameToLayer("Unit"));
            float       minDist     = float.MaxValue;
            IDefenable closestEnemy = null;
            for (int i = 0; i < _visualFieldEnemyList.Count; i++)
            {
                IDefenable a = _visualFieldEnemyList[i];
                try
                {
                    if((a.IsDeath) || (!(((SoldierUnitBase)a).AtEnemyHome) && (((GameUnitBase)a).UnitTeam == this.UnitTeam || ((GameUnitBase)a).UnitRoad != this.UnitRoad)))
                        continue;
                }
                catch (Exception e)
                {
                    //  is home
                    if((a.IsDeath) || ((GameUnitBase)a).UnitTeam == this.UnitTeam)
                        continue;
                }
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
                                                                  Debug.Log("Enemy In!");
                                                                  _visualFieldEnemyList.Add(u);
                                                              }
                                                          });
            _unitVisualField.colliderExitEvent.AddListener((v, c) =>
                                                           {
                                                               IDefenable u = null;
                                                               if (c.gameObject.TryGetComponent<IDefenable>(out u))
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
            SearchEnemy();
            Debug.Log(((GameUnitBase)_curEnemy).name);

            _attackTimer += Time.fixedDeltaTime;
            if (_curEnemy != null && !_curEnemy.IsDeath && _attackTimer > AttackInterval)
            {
                _attackTimer = this.DoAttack(_curEnemy) ? 0 : _attackTimer;
            }
            
            
        }

    }
}