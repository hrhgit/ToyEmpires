using System;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.GameUnit.MeleeUnits
{
    public class MeleeUnitsBase : GameUnitBase, IProduceable, ICombatable 
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

        public void Produce(GameUnitBase unit, PlayerBase player, UnitStatus status)
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

        public int                      Attack         { get; }
        public int                      AttackRange    { get; }
        public int                      AttackInterval { get; }
        public int                      Defence        { get; }
        public event AttackEventHandler AttackEvent;
        public event AttackEventHandler BeAttackedEvent;

        public void DoAttack(GameUnitBase attackTarget)
        {
            throw new NotImplementedException();
        }

        public void BeAttacked(ICombatable attacker)
        {
            throw new NotImplementedException();
        }

        public void BeAttacked(GameUnitBase attacker)
        {
            throw new NotImplementedException();
        }

        #endregion


        private void FixedUpdate()
        {
            this.UnitMover.Target = BattleGameManager.BattleGameManagerInstance.userEnemyPlayer.home.position;
        }

    }
}