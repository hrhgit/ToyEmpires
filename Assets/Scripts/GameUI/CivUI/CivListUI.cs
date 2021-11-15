using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Gameplay.Civilization;
using Global;
using UnityEngine;
using UnityEngine.UI;
namespace GameUI.CivUI {
    public class CivListUI : MonoBehaviour {
        public CivLineUI       civLineUIPrefab;
        public Transform       civLineParent;
        public int             curCivId;
        public CivDetailUI     detailUI;
        public List<CivLineUI> civLineUiList = new List<CivLineUI>();
        public Button          confirmBtn;

        private void Start () {
            GenerateCivList();
        }
        public void GenerateCivList () {
            int[] civIds = CivilizationGenerator.CivDict.Keys.ToArray();
            for (var i = 0; i < civIds.Length; i++) {
                int       id      = civIds[i];
                CivLineUI lineUI  = Instantiate(civLineUIPrefab, civLineParent);
                var       civData = GetCivData(id);
                lineUI.civId     = id;
                lineUI.listUI    = this;
                lineUI.civName   = civData.civilizationName;
                lineUI.civDetail = civData.civilizationContent;
                RectTransform lineRect = lineUI.GetComponent<RectTransform>();
                lineRect.anchoredPosition = new Vector2(0, -i*(lineRect.sizeDelta.y + 10));
                civLineUiList.Add(lineUI);
            }
        }

        public CivilizationData GetCivData (int civID) {
            TextAsset   textAsset = (TextAsset)Resources.Load("Data/Civilizations/CivsData0");
            XmlDocument xmlDoc    = new XmlDocument();
            xmlDoc.LoadXml(textAsset.text);
            XmlNode techXml = xmlDoc.GetElementById("c" + civID.ToString("d3"));
            return new CivilizationData(techXml["Name"].InnerText.Trim(), techXml["Content"].InnerText.Trim());

        }

        
        public void Select (CivLineUI civLineUI) {
            this.curCivId = civLineUI.civId;
            detailUI.Show(civLineUI);
            confirmBtn.gameObject.SetActive(true);
        }

        public void Confirm () {
            GlobalGameManager.GlobalGameManagerInstance.playerCivilizationIdx = this.curCivId;
            GlobalGameManager.GlobalGameManagerInstance.Load("BattleScene0");
        }
    }
}
