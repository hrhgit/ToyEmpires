using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.GameUnit;
using Gameplay.GameUnit.SoldierUnit;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Player.AI
{
    public partial class PlayerAI : MonoBehaviour
    {
        public PlayerBase aiPlayer;
        public PlayerBase enemyPlayer;
        public float      aiRestTime;

        private float _timer = 0f;

    
        private void FixedUpdate()
        {
            _timer += Time.fixedDeltaTime;
            if (_timer > aiRestTime)
            {
                AiDispatchWorker();
                AiDispatchUnit();
                _timer = 0f;
            }
        }

        private void AiDispatchWorker()
        {
            if (aiPlayer.workerStatus.freeUnitCount > 0)
            {
                ResourceType r = (ResourceType)Array.IndexOf(aiPlayer.ResourceWorkerCount, aiPlayer.ResourceWorkerCount.Min());
                aiPlayer.DispatchWorker(r, true);
            }
        }

    
        private void AiDispatchUnit()
        {
            // var freeUnits     = aiPlayer.UnitStatusList.Where(status => status.freeUnitCount > 0).Select((status, index) => index).ToArray();
            // var freeStatusArr = (freeUnits.Select(freeIndex => aiPlayer.UnitStatusList[freeIndex])).ToArray();
            // // 0维表示食物木材派遣、1维表示黄金派遣
            // int[,] dispatchUnitArr = new int[freeUnits.Length, 2];
            // for (var i = 0; i < freeUnits.Length; i++)
            // {
            //     SoldierUnitBase u                   = aiPlayer.unitPrefabList[freeUnits[i]];
            //     IProduceable    pu                  = (IProduceable)u;
            //     
            //     //数量计算
            //     int             maxPurchaseFoodWood = Mathf.Min(aiPlayer.Food / pu.CostFood, aiPlayer.Wood / pu.CostWood);
            //     int             maxPurchaseGold     = aiPlayer.Gold / pu.CostGold;
            //     int             maxSend             = aiPlayer.maxBattleUnitCount - aiPlayer.CurUnitPopulation;
            //     if (maxPurchaseFoodWood + maxPurchaseGold > maxSend)
            //     {
            //         int sum = maxPurchaseGold + maxPurchaseFoodWood;
            //         maxPurchaseFoodWood = (int)(maxSend * ((float)maxPurchaseFoodWood / sum));
            //         maxPurchaseGold     = (int)(maxSend * ((float)maxPurchaseGold     / sum));
            //     }
            //     //食物木材派遣
            //     dispatchUnitArr[i, 0] += Random.Range(0, maxPurchaseFoodWood);
            //     //黄金派遣
            //     dispatchUnitArr[i, 1] += Random.Range(0, maxPurchaseGold);
            //
            //     aiPlayer.AddResource(ResourceType.Food, -pu.CostFood * dispatchUnitArr[i, 0]);
            //     aiPlayer.AddResource(ResourceType.Wood, -pu.CostWood * dispatchUnitArr[i, 0]);
            //     aiPlayer.AddResource(ResourceType.Gold, -pu.CostGold * dispatchUnitArr[i, 1]);
            //
            //     int dispatchCount = dispatchUnitArr[i, 0] + dispatchUnitArr[i, 1];
            //     freeStatusArr[i].freeUnitCount -= dispatchCount;
            //
            //     var sortEnemyRoad = enemyPlayer.roadUnitsCount.Select((x, i) => new
            //                                                                     {
            //                                                                         count = x, idx = i
            //                                                                     }).OrderByDescending(x => x.count).ToArray();
            //
            //     for (int j = 0; j < 2; j++)
            //     {
            //         int roadCount = Random.Range(0, dispatchCount);
            //         aiPlayer.DispatchUnits(freeUnits[i], roadCount, (Road)sortEnemyRoad[j].idx);
            //         dispatchCount -= roadCount;
            //     }
            //     aiPlayer.DispatchUnits(freeUnits[i], dispatchCount, (Road)sortEnemyRoad[2].idx);
            //}
            if (Random.value < ((float)enemyPlayer.CurUnitPopulation / (this.aiPlayer.CurUnitPopulation + enemyPlayer.CurUnitPopulation) + .1))
                return;
            if (aiPlayer.Gold == 0 && (aiPlayer.Food == 0 || aiPlayer.Wood == 0) )
                return;
            UnitTmpData[]      selectableUnits;
            DispatchPlanDict[] plans           = SelectUnits(out selectableUnits);
            int                mostDecisionIdx = plans.GroupBy((dict => dict.strength)).Max((g => g.Count()));
            DispatchPlanDict   randPlan        = plans[Random.Range(0, (plans.Length - mostDecisionIdx) +1)];
            
            //路况检测
            var sortEnemyRoad = enemyPlayer.roadUnitsCount.Select((x, i) => new
                                                                            {
                                                                                count = x, idx = i
                                                                            }).OrderByDescending(x => x.count).ToArray();

            for (var i = 0; i < randPlan.plans.Length; i++)
            {
                DispatchPlan plan   = randPlan.plans[i];
                int          amount = plan.count;
                
                // 分配资源使用
                if (Random.Range(0, 2) == 0) //优先使用食物判定
                {
                    int n = Mathf.Min(amount, selectableUnits[i].foodwoodMax);
                    aiPlayer.AddResource(ResourceType.Food, -selectableUnits[i].produceable.CostFood * n);
                    aiPlayer.AddResource(ResourceType.Wood, -selectableUnits[i].produceable.CostWood * n);
                    aiPlayer.AddResource(ResourceType.Gold, -selectableUnits[i].produceable.CostGold * (amount - n));
                }
                else
                {
                    int n = Mathf.Min(amount, selectableUnits[i].goldMax);
                    aiPlayer.AddResource(ResourceType.Gold, -selectableUnits[i].produceable.CostGold * n);
                    aiPlayer.AddResource(ResourceType.Food, -selectableUnits[i].produceable.CostFood * (amount - n));
                    aiPlayer.AddResource(ResourceType.Wood, -selectableUnits[i].produceable.CostWood * (amount - n));
                }
                
                //分配路
                int m = amount;
                if (Random.Range(0, 2) == 0) //随机换路
                {
                    sortEnemyRoad = sortEnemyRoad.Reverse().ToArray();
                } 
                for (int j = 0; j < 3; j++)
                {
                    int roadCount = Random.Range(0, m);
                    aiPlayer.DispatchUnits(plan.playerListIdx, roadCount, (Road)sortEnemyRoad[j].idx);
                    m -= roadCount;
                }
                //收尾
                aiPlayer.DispatchUnits(plan.playerListIdx, m, (Road)Random.Range(0, 3));

                selectableUnits[i].status.freeUnitCount -= amount;
            }


        }

        private DispatchPlan[] CopyPlans(DispatchPlan[] ori)
        {
            return ori.Select(p => new DispatchPlan(p.playerListIdx, p.count)).ToArray();
        }

        private void OneByOnePlan(DispatchPlanDict[] planDicts, UnitTmpData[] units, int unitIdx, int amount, int maxCapacity)
        {
            int   cost  = units[unitIdx].produceable.CostPopulation;
            float value = units[unitIdx].unit.UnitValue;
            for (int i = maxCapacity; i >= cost; i--)
            {
                try
                {

                    if (i - cost * amount < 0) return;
                    if (planDicts[i].strength < planDicts[i - cost * amount].strength + value * amount)
                    {
                        planDicts[i].strength             =  planDicts[i - cost * amount].strength + value * amount;
                        planDicts[i].plans                =  CopyPlans(planDicts[i - cost * amount].plans);
                        planDicts[i].plans[unitIdx].count += amount;
                    }



                }
                catch (Exception e)
                {
                    Debug.Log("Idx:" + (i - cost * amount) + ", cost:" + cost + ", amoun:" + amount);
                }
            }
        }
        
        
        private void AllInPlan(DispatchPlanDict[] planDicts, UnitTmpData[] units, int unitIdx, int maxCapacity)
        {
            int   cost  = units[unitIdx].produceable.CostPopulation;
            float value = units[unitIdx].unit.UnitValue;
            
            for (int i = cost; i <= maxCapacity; i++)
            {
                if (planDicts[i].strength < planDicts[i - cost].strength + value)
                {
                    planDicts[i].strength             =  planDicts[i - cost].strength + value;
                    planDicts[i].plans                =  CopyPlans(planDicts[i - cost].plans);
                    planDicts[i].plans[unitIdx].count ++;
                }
            }
        }



        private DispatchPlanDict[] SelectUnits(out UnitTmpData[] selectableUnitsOut)
        {
            int           populationAvailable = aiPlayer.maxBattleUnitCount - aiPlayer.CurUnitPopulation;
            UnitTmpData[] selectableUnits     = aiPlayer.UnitStatusList.Where(status => status.freeUnitCount > 0).Select((status, index) => new UnitTmpData(index, aiPlayer.unitPrefabList[index], Mathf.Min(aiPlayer.Food / ((IProduceable)aiPlayer.unitPrefabList[index]).CostFood, aiPlayer.Wood / ((IProduceable)aiPlayer.unitPrefabList[index]).CostWood), aiPlayer.Gold / ((IProduceable)aiPlayer.unitPrefabList[index]).CostGold, status)).ToArray();
            DispatchPlanDict[] planDicts = Enumerable.Range(0, populationAvailable + 1).Select(i =>
                                                                                                   new DispatchPlanDict(selectableUnits.Select(j =>
                                                                                                                                                   new DispatchPlan(j.playerListidx, 0)).ToArray())
                                                                                              ).ToArray();
            selectableUnitsOut = selectableUnits;
            // 完全背包问题
            for (int i = 0; i < selectableUnits.Length; i++)
            {
                int amount = Mathf.Min(selectableUnits[i].foodwoodMax + selectableUnits[i].goldMax, selectableUnits[i].status.freeUnitCount);
                if (selectableUnits[i].produceable.CostPopulation * amount >= populationAvailable)
                {
                    AllInPlan(planDicts, selectableUnits, i, populationAvailable);
                    continue;
                }

                int k = 1;
                while (k < amount)
                {
                    OneByOnePlan(planDicts, selectableUnits, i, k, populationAvailable);
                    amount -= k;
                    k      *= 2;
                }

                OneByOnePlan(planDicts, selectableUnits, i, amount, populationAvailable);
            }




            return planDicts;
        }
    }
}
