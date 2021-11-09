using System;
using Gameplay.Buff;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.GameUnit.SoldierUnit.CombatUnit
{
    public class ProducebleCombatUnitBase : SoldierUnitBase,IProduceable
    {
        #region 生产

        [Header("生产")] [SerializeField] private FloatBuffableValue _costTime        = new FloatBuffableValue();
        [SerializeField]                private IntBuffableValue   _costFood        = new IntBuffableValue();
        [SerializeField]                private IntBuffableValue   _costWood        = new IntBuffableValue();
        [SerializeField]                private IntBuffableValue   _costGold        = new IntBuffableValue();
        [SerializeField]                private IntBuffableValue   _costPopulation  = new IntBuffableValue(1);
        [SerializeField]                private IntBuffableValue   _maxReserveCount = new IntBuffableValue();

        public float CostTime => _costTime;

        public int CostFood => _costFood;

        public int CostWood => _costWood;

        public int CostGold => _costGold;

        public int CostPopulation => _costPopulation;

        public int MaxReserveCount => _maxReserveCount;


        public void Produce(SoldierUnitBase unit, PlayerBase player, UnitStatus status)
        {
            if (status.freeUnitCount >= MaxReserveCount)
                return;
            if (status.unitProduceProcess < 1)
            {
                status.unitProduceProcess += Time.fixedDeltaTime / ((IProduceable)unit).CostTime;
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
                    case BuffNumericalValueType.CostTime:
                        if (isAdditionalValue)
                            this._costTime.AddAdditionalValue(value);
                        else
                            this._costTime.AddMagnification(value);
                        break;
                    
                    case BuffNumericalValueType.MaxReserveCount:
                        if (isAdditionalValue)
                            this._maxReserveCount.AddAdditionalValue((int)value);
                        else
                            this._maxReserveCount.AddMagnification(value);
                        break;

                    default:
                        throw new UnityException("未找到Buff: " + buffType.ToString());
                        return false;
                }
            }
            return true;
        }

        #endregion


    }
}