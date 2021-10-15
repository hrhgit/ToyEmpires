using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Gameplay.GameUnit;
using Gameplay.GameUnit.SoldierUnit;
using Gameplay.Player;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAI : MonoBehaviour
{
    public PlayerBase aiPlayer;
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
            aiPlayer.DispatchWorker(r,true);
        }
    }

    
    private void AiDispatchUnit()
    {
        var freeUnits     = aiPlayer.UnitStatusList.Where(status => status.freeUnitCount > 0).Select((status, index) => index).ToArray();
        var freeStatusArr = (freeUnits.Select(freeIndex => aiPlayer.UnitStatusList[freeIndex])).ToArray();
        // 0维表示食物木材派遣、1维表示黄金派遣
        int[,] dispatchUnitArr = new int[freeUnits.Length, 2];
        for (var i = 0; i < freeUnits.Length; i++)
        {
            SoldierUnitBase u                   = aiPlayer.unitPrefabList[freeUnits[i]];
            IProduceable    pu                  = (IProduceable)u;
            int             maxPurchaseFoodWood = Mathf.Min(aiPlayer.Food / pu.CostFood, aiPlayer.Wood / pu.CostWood);
            int             maxPurchaseGold     = aiPlayer.Gold / pu.CostGold;
            //食物木材派遣
            dispatchUnitArr[i, 0] += Random.Range(0,maxPurchaseFoodWood);
            //黄金派遣
            dispatchUnitArr[i, 1] += Random.Range(0, maxPurchaseGold);

            aiPlayer.AddResource(ResourceType.Food, -pu.CostFood * dispatchUnitArr[i, 0]);
            aiPlayer.AddResource(ResourceType.Wood, -pu.CostWood * dispatchUnitArr[i, 0]);
            aiPlayer.AddResource(ResourceType.Gold, -pu.CostGold * dispatchUnitArr[i, 1]);

            int dispatchCount = dispatchUnitArr[i, 0] + dispatchUnitArr[i, 1];
            freeStatusArr[i].freeUnitCount -= dispatchCount;

            for (int j = 0; j < 3; j++)
            {
                int roadCount = Random.Range(0, dispatchCount);
                aiPlayer.DispatchUnits(freeUnits[i], roadCount,(Road)j);
                dispatchCount -= roadCount;
            }
            

        }
    }
}
