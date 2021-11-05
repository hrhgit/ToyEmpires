using System;
using System.Collections.Generic;
using Gameplay.Buff;
using UnityEngine;

namespace Gameplay.TechTree
{
    [Serializable]
    public class Technology
    {
        public int technologyID;

        public Technology() : this(0,0,0,0,0,0,1)
        {
            
        }
        public Technology(int   maxCostFood,
                          int   maxCostGold,
                          int   maxCostWood,
                          int   minCostFood,
                          int   minCostGold,
                          int   minCostWood,
                          float costTime)
        {
            this.maxCostFood.Value = maxCostFood;
            this.maxCostGold.Value = maxCostGold;
            this.maxCostWood.Value = maxCostWood;
            this.minCostFood.Value = minCostFood;
            this.minCostGold.Value = minCostGold;
            this.minCostWood.Value = minCostWood;
            this.costTime.Value    = costTime;
        }

        #region 消费

        [SerializeField] private IntBuffableValue   maxCostFood = new IntBuffableValue(0);
        [SerializeField] private IntBuffableValue   maxCostGold = new IntBuffableValue(0);
        [SerializeField] private IntBuffableValue   maxCostWood = new IntBuffableValue(0);
        [SerializeField] private IntBuffableValue   minCostFood = new IntBuffableValue(0);
        [SerializeField] private IntBuffableValue   minCostGold = new IntBuffableValue(0);
        [SerializeField] private IntBuffableValue   minCostWood = new IntBuffableValue(0);
        [SerializeField] private FloatBuffableValue costTime    = new FloatBuffableValue(1);
        
        public int   MaxCostFood => maxCostFood;
        public int   MaxCostGold => maxCostGold;
        public int   MaxCostWood => maxCostWood;
        public int   MinCostFood => minCostFood;
        public int   MinCostGold => minCostGold;
        public int   MinCostWood => minCostWood;
        public float CostTime    => costTime;

        #endregion
        
        #region 效果

        public List<PlayerBuffBase> playerBuffs = new List<PlayerBuffBase>();
        public List<UnitBuffBase>   unitBuffs   = new List<UnitBuffBase>();
        
        #endregion


    }
}
