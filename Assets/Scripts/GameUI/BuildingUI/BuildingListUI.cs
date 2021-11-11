using System.Collections.Generic;
using System.Xml;
using Gameplay;
using Gameplay.Buildings;
using UnityEngine;

namespace GameUI.BuildingUI
{
    public class BuildingListUI : MonoBehaviour
    {
        public                   BuildingPanelUI  panelUI;
        [HideInInspector] public Road             tmpRoad;
        [HideInInspector] public int              tmpIdx;
        public                   BuildingDetailUI detailUI;

        
        [Header("建筑列表")]
        public GameObject     buildingListUI;
        public Transform      buildingListContent;
        public BuildingLineUI buildingLinePrefab;

        public void InitBuildingLine()
        {
            Clear();
            List<Building> buildings = panelUI.manager.buildingPrefabs;
            for (int i = 0; i < buildings.Count; i++)
            {
                AddBuildingLine(buildings[i], i);
            }
            
        }

        private void Clear()
        {
            for (int i = 0; i < buildingListContent.childCount; i++)
            {
                Destroy(buildingListContent.GetChild(i));
            }
        }

        private static float _yOffset = 50;

        private void AddBuildingLine(Building building, int idx)
        {
            BuildingLineUI line = Instantiate(buildingLinePrefab, buildingListContent);
            RectTransform  rect = line.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, -(idx + 1) * _yOffset - idx * rect.sizeDelta.y);
            line.building         = building;
            line.buildingListUI   = this;
            BuildingData data = GetBuildingData(idx);
            line.name   = data.buildingName;
            line.detail = data.buildingContent;
            line.Init();
        }

        public BuildingData GetBuildingData(int buildingIdx)
        {
            int         techID    = panelUI.manager.buildingPrefabs[buildingIdx].buildingID;
            TextAsset   textAsset = (TextAsset)Resources.Load("Data/Buildings/BuildingsData0");
            XmlDocument xmlDoc    = new XmlDocument();
            xmlDoc.LoadXml(textAsset.text);
            XmlNode buildingXml = xmlDoc.GetElementById("b" + techID.ToString("d4"));

            return new BuildingData(buildingXml["Name"].InnerText.Trim(), buildingXml["Content"].InnerText.Trim());
        }

        public void ClosePanel()
        {
            buildingListUI.SetActive(false);
        }


        public void Purchase(Building building, bool isUseGold)
        {
            this.panelUI.manager.buildingsGroups[(int)tmpRoad].SetBuilding(building,tmpIdx,isUseGold);
            this.panelUI.listUI.ClosePanel();
        }
    }
}
