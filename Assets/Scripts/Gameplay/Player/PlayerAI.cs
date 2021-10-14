using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Gameplay.GameUnit;
using Gameplay.GameUnit.SoldierUnit;
using Gameplay.Player;
using UnityEngine;

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

    private void AiDispatchUnit()
    {
        if (aiPlayer.workerStatus.freeUnitCount > 0)
        {
            ResourceType r = (ResourceType)Array.IndexOf(aiPlayer.ResourceWorkerCount, aiPlayer.ResourceWorkerCount.Min());
            aiPlayer.DispatchWorker(r,true);
        }
    }

    
    private void AiDispatchWorker()
    {
        var freeUnits = aiPlayer.UnitStatusList.Where(status => status.freeUnitCount > 0).Select((status,index) => index);
        foreach (int i in freeUnits)
        {
            SoldierUnitBase u  = aiPlayer.unitPrefabList[i];
            IProduceable    pu = (IProduceable)u;
            if(aiPlayer.CanAfford())
        }
        
    }
}
