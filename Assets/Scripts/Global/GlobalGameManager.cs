using System;
using System.Collections.Generic;
using System.Xml;
using Audio;
using Gameplay.GameUnit;
using Gameplay.GameUnit.SoldierUnit;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Global
{
    public class GlobalGameManager : MonoBehaviour
    {
        public static GlobalGameManager GlobalGameManagerInstance { get; private set; }

        public int                    playerCivilizationIdx;
        public int                    enemyCivilizationIdx;
        public string                 nextSceneName;
        public Dictionary<int,string> unitId2FileNameDict = new Dictionary<int, string>();
        public AudioManager           audioManager;
        private void Awake()
        {
            GlobalGameManagerInstance = this;
        }

        public void Init () {
            GenerateDict();
            DontDestroyOnLoad(this.gameObject);
        }
        private void GenerateDict () {
            TextAsset   textAsset = (TextAsset)Resources.Load("Data/Unit/UnitData0");
            XmlDocument xmlDoc    = new XmlDocument();
            xmlDoc.LoadXml(textAsset.text);
            XmlNodeList unitXmls = xmlDoc.GetElementsByTagName("Unit");
            for (int i = 0; i < unitXmls.Count; i++) {
                int unitId = int.Parse(unitXmls[i].Attributes["UID"].Value.Substring(1));
                string fileName = unitXmls[i].Attributes["fileName"].Value;
                unitId2FileNameDict.Add(unitId,fileName);
            }
        }

        public string GetFileName (int id) {
            return unitId2FileNameDict[id];
        }

        public GameObject GetSoldierPrefab (int id) {
            return  (GameObject)Resources.Load("Prefabs/Units/Soldiers/" + unitId2FileNameDict[id]);
        }

        public void Load (string sceneName) {
            nextSceneName = sceneName;

            SceneManager.LoadScene("Loading");
        }
    }
}
