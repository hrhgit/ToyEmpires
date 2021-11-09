using System;
using System.Collections.Generic;
using Gameplay;
using Gameplay.Buildings;
using UnityEngine;

namespace GameUI.BuildingUI
{
    public class BuildingPanelUI : MonoBehaviour
    {
        public BuildingsManager    manager;
        public BuildingListUI      listUI;
        public GameObject          root;
        public List<BuildingGrpUI> grpUis;


        private void Start()
        {
            this.manager = BattleGameManager.BattleGameManagerInstance.userPlayer.buildingManager;
            listUI.InitBuildingLine();
        }

        public void OpenListPanel(Road road,int idx)
        {
            listUI.tmpRoad = road;
            listUI.tmpIdx  = idx;
            listUI.gameObject.SetActive(true);
        }

        public void Open()
        {
            root.SetActive(true);
        }
        
        public void Close()
        {
            root.SetActive(false);
        }


    }
}
