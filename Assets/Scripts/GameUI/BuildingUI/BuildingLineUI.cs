using Gameplay.Buildings;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI.BuildingUI
{
    public class BuildingLineUI : MonoBehaviour
    {
        public Building       building;
        public BuildingListUI buildingListUI;
        public Text           nameUI;
        public Text           costFoodUI;
        public Text           costGoldUI;

        public string     name;
        [TextArea]
        public string     detail;

        public void Init()
        {
            this.nameUI.text     = name;
            this.costFoodUI.text = building.BuildingCostFood + "食";
            this.costGoldUI.text = building.BuildingCostGold + "金";
        }

        public void Purchase(bool isUseGold)
        {
            this.buildingListUI.Purchase(this.building, isUseGold);
            buildingListUI.ClosePanel();
        }
    }
}
