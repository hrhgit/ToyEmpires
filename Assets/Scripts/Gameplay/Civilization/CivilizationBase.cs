using System.Collections.Generic;
using Gameplay.GameUnit.SoldierUnit;
using Global;
namespace Gameplay.Civilization {
	public class CivilizationBase {
		public int civilizationId;
		public int techTreeId;
		public List<int> availableUnitIds = new List<int>();

		public List<SoldierUnitBase> GetUnitPrefabsList () {
			List<SoldierUnitBase> soldierUnitList = new List<SoldierUnitBase>();
			availableUnitIds.ForEach((i =>
			                          {
				                          soldierUnitList.Add(GlobalGameManager.GlobalGameManagerInstance.GetSoldierPrefab(i));
			                          } ));

			return soldierUnitList;
		}
	}
}
