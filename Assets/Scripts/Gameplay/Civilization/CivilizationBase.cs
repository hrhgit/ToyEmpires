using System;
using System.Collections.Generic;
using Gameplay.GameUnit.SoldierUnit;
using Gameplay.GameUnit.SoldierUnit.Worker;
using Global;
using UnityEngine;
namespace Gameplay.Civilization {
	[Serializable]
	public class CivilizationBase {
		public int civilizationId;
		public int techTreeId;
		public int workerId = 1000;
		public List<int> availableUnitIds = new List<int>();
		public List<int> availablePoliciesIds = new List<int>();
		public CivilizationBase (int civilizationId, int techTreeId) {
			this.civilizationId = civilizationId;
			this.techTreeId = techTreeId;
		}

		public List<GameObject> GetUnitPrefabsList () {
			List<GameObject> soldierUnitList = new List<GameObject>();
			availableUnitIds.ForEach((i =>
			                          {
				                          soldierUnitList.Add(GlobalGameManager.GlobalGameManagerInstance.GetSoldierPrefab(i));
			                          } ));

			return soldierUnitList;
		}
		
		public GameObject GetWorkerPrefab () {
			return  GlobalGameManager.GlobalGameManagerInstance.GetSoldierPrefab(workerId);
		}
	}
}
