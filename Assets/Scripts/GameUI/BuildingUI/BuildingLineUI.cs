using System;
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

        public Button goldBtn;
        public Button foodBtn;

        public string     name;
        [TextArea]
        public string     detail;

        public void Init()
        {
            this.nameUI.text     = name;
            this.costFoodUI.text = building.BuildingCostFood[building.level] + "食";
            this.costGoldUI.text = building.BuildingCostGold[building.level] + "金";
        }

        private void FixedUpdate()
        {
            goldBtn.interactable = buildingListUI.panelUI.manager.player.CanAfford(0, 0, building.BuildingCostGold[building.level]);
            
            foodBtn.interactable = buildingListUI.panelUI.manager.player.CanAfford(building.BuildingCostFood[building.level], building.BuildingCostWood[building.level], 0);
        }

        public void Purchase(bool isUseGold)
        {
            this.buildingListUI.Purchase(this.building, isUseGold);
            buildingListUI.ClosePanel();
        }

        public void OnMouseHover()
        {
            buildingListUI.detailUI.Show(this);
        }
        public void OnMouseLeave()
        {
            buildingListUI.detailUI.Close();
        }
    }
}
