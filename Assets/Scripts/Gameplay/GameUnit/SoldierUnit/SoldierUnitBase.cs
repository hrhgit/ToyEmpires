using System;
using Gameplay.GameUnit.SoldierUnit.CombatUnit;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.GameUnit.SoldierUnit
{
    public delegate void GameUnitEvent(SoldierUnitBase soldierUnitBase, PlayerBase playerBase,UnitStatus status);
    public abstract class  SoldierUnitBase : GameUnitBase, IAttackable, IMovable
    {


        protected virtual void Awake()
        {
            this.UnitMover = this.GetComponent<GameUnitMover>();
        }

        protected override void Start()
        {
            base.Start();
            switch (UnitTeam)
            {
                case Team.Blue:
                    this.UnitMover.Target = BattleGameManager.BattleGameManagerInstance.redPlayer.homeUnit.transform.position;
                    break;
                case Team.Red:
                    this.UnitMover.Target = BattleGameManager.BattleGameManagerInstance.bluePlayer.homeUnit.transform.position;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }


        #region 移动

        public                   GameUnitMover UnitMover { get; private set; }
        [SerializeField] private float         maxSpeed;
        public                   float         MaxSpeed => maxSpeed;

        #endregion

        #region 生产


        #endregion

        #region 受击

        [SerializeField] protected int defence;
        [SerializeField] private   int _maxHp;


        public int MaxHp => _maxHp;

        public int CurHp   { get; private set; }
        public int Defence => defence;

        public event AttackEventHandler BeAttackedEvent;
        public virtual void BeAttacked(ICombatable attacker)
        {
            this.CurHp -= attacker.Attack - this.Defence;
            BeAttackedEvent?.Invoke(attacker, this);
        }

        #endregion

    }
}
