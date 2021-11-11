using System.Collections.Generic;
using System.Xml;
using Gameplay.Buildings;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI.BuildingUI
{
    public class BuildingDetailUI : MonoBehaviour
    {
        public BuildingListUI listUI;
        public RectTransform  rect;
        public GameObject     root;
        public RectTransform  rootRect;
        public Text           nameUI;
        public List<Text>     infoUIList = new List<Text>();
        public Text           detailUI;

        public void Show(BuildingLineUI buildingLine)
        {
            InitContent(buildingLine);
            this.root.SetActive(true);
            _isTracing = true;
        }

        public void Close()
        {
            this.root.SetActive(false);
            _isTracing = false;
        }
        

        public void InitContent(BuildingLineUI buildingLine)
        {
            List<InfoLine> info2Show = new List<InfoLine>();
            
            info2Show.Add(new InfoLine("生命值",  buildingLine.building.Hp[buildingLine.building.level].ToString()));
            info2Show.Add(new InfoLine("建造耗时", buildingLine.building.BuildingCostTime[buildingLine.building.level].ToString()));

            for (int i = 0; i < infoUIList.Count; i++)
            {
                infoUIList[i].text = info2Show[i].ToString();
            }

            BuildingData data = new BuildingData(buildingLine.name,buildingLine.detail);
            nameUI.text   = data.buildingName;
            detailUI.text = data.buildingContent;
        }
        
        private bool _isRightPanel = true;

        public void TraceDetail()
        {
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, null, out mousePos);
            rootRect.pivot         = new Vector2(_isRightPanel ? 0 : 1, 1);
            rootRect.localPosition = mousePos;
            if (-rootRect.offsetMax.x < -10) //Right
                _isRightPanel = false;
            else if(rootRect.offsetMin.x < -210) //Left
                _isRightPanel = true;

        }

        
        private bool _isTracing = false;
        private void Update()
        {
            if (_isTracing)
            {
                this.TraceDetail();

            }
        }
    }
}

