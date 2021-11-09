using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Buildings
{
    public class BuildingsGroup : MonoBehaviour
    {
        public BuildingsManager manager;
        public Building[]   buildings;
        public List<Transform>  buildingsPos = new List<Transform>();
        
        public Road             road;

        private void Start()
        {
            buildings = new Building[buildingsPos.Count];
        }

        public void SetBuilding(Building building,int posIdx,bool isUseGold)
        {
            int isResourceAvailable = manager.player.CanAfford(building);
            if (isUseGold)
            {
                if (isResourceAvailable == 1 || isResourceAvailable == 3)
                {
                    manager.player.AddResource(ResourceType.Gold, -building.BuildingCostGold);
                    buildings[posIdx]        = InstantiateBuilding(building, buildingsPos[posIdx]);
                    buildings[posIdx].posIdx = posIdx;

                }
            }
            else
            {
                if (isResourceAvailable == 2 || isResourceAvailable == 3)
                {
                    manager.player.AddResource(ResourceType.Food, -building.BuildingCostFood);
                    manager.player.AddResource(ResourceType.Wood, -building.BuildingCostWood);
                    buildings[posIdx]        = InstantiateBuilding(building, buildingsPos[posIdx]);
                    buildings[posIdx].posIdx = posIdx;
                }
            }
        }

        private Building InstantiateBuilding(Building building, Transform pos)
        {
            Building b = Instantiate(building, pos);
            b.Set();
            b.manager        = this.manager;
            b.road           = this.road;
            b.buildingsGroup = this;
            
            return b;
        }
    }
}
