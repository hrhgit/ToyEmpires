using System;
using System.Collections.Generic;
using Gameplay.Buff;
using Gameplay.GameUnit.FortificationUnit;
using Gameplay.GameUnit.SoldierUnit.CombatUnit.RangedAttackUnit;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.GameUnit.SoldierUnit.CombatUnit.MeleeUnit
{
    public class MeleeUnitsBase : ProducebleCombatUnitBase, IProduceable, IAttackable 
    {

        #region 战斗
        [Header("战斗")]
        [SerializeField] private IntBuffableValue   attack         = new IntBuffableValue();
        [SerializeField] private FloatBuffableValue attackRange    = new FloatBuffableValue();
        [SerializeField] private FloatBuffableValue attackInterval = new FloatBuffableValue();
        [SerializeField] private FloatBuffableValue findEnemyRange = new FloatBuffableValue();

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

                bool       isFortification = _curEnemy is FortificationUnitBase;
                bool       isRangeAttacker = attacker is IRangeAttackable;
                IDefenable defenableUnit   = attacker as IDefenable;
                if (!isRangeAttacker && defenableUnit.CurHp < this._curEnemy.CurHp)
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
                    this.AtEnemyHome = (u is PlayerHomeUnit) || (u is TowerBase);
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
                if(a.IsDeath || ((GameUnitBase)a).UnitTeam == this.UnitTeam)
                    continue;

                bool isSoldier       = a is SoldierUnitBase;
                bool isFortification = a is FortificationUnitBase;
                bool isHome = a is PlayerHomeUnit;

                if (isSoldier)
                {
                    if ( !((SoldierUnitBase)a).AtEnemyHome && ((GameUnitBase)a).UnitRoad != this.UnitRoad)
                        continue;
                }

                if (isFortification)
                {
                    bool isTower = a is TowerBase;
                    if (!isTower && ((GameUnitBase)a).UnitRoad != this.UnitRoad)
                    {
                        continue;
                    }
                }

                if (isHome)
                {
                    if(_visualFieldEnemyList.Count > 1)
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
                                                                  _visualFieldEnemyList.Add(u);
                                                              }
                                                          });
            _unitVisualField.colliderExitEvent.AddListener((v, c) =>
                                                           {
                                                               IDefenable u = null;
                                                               if (c.gameObject.TryGetComponent<IDefenable>(out u))
                                                               {
                                                                   _visualFieldEnemyList.Remove(u);
                                                               }
                                                           });
        }

        #endregion

        #region Buff

        public override bool SetNumericalValueBuff(BuffNumericalValueType buffType, bool isAdditionalValue, float value)
        {
            try
            {
                base.SetNumericalValueBuff(buffType, isAdditionalValue, value);
            }
            catch (Exception e)
            {
                switch (buffType)
                {
                    case BuffNumericalValueType.Attack:
                        if (isAdditionalValue)
                            this.attack.AddAdditionalValue((int)value);
                        else
                            this.attack.AddMagnification(value);
                        break;

                    default:
                        throw new UnityException("未找到Buff: " + buffType.ToString());
                        return false;
                }
            }
            return true;
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


        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            SearchEnemy();

            _attackTimer += Time.fixedDeltaTime;
            if (_curEnemy != null && !_curEnemy.IsDeath && _attackTimer > AttackInterval)
            {
                _attackTimer = this.DoAttack(_curEnemy) ? 0 : _attackTimer;
            }
            
            
        }

    }
}