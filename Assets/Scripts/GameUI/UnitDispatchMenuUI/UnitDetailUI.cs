using System.Collections.Generic;
using System.Xml;
using Gameplay.GameUnit;
using Gameplay.GameUnit.SoldierUnit;
using Gameplay.GameUnit.SoldierUnit.CombatUnit.MeleeUnit;
using Gameplay.GameUnit.SoldierUnit.CombatUnit.RangedAttackUnit;
using Gameplay.GameUnit.SoldierUnit.Worker;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI.UnitDispatchMenuUI
{
    public class UnitDetailUI : MonoBehaviour
    {
        struct InfoLine
        {
            
            public string     title;
            public string     content;

            public InfoLine(string title, string content)
            {
                this.title   = title;
                this.content = content;
            }

            public override string ToString()
            {
                return title + ":" + content;
            }
        }

        public RectTransform rect;
        public GameObject    root;
        public RectTransform rootRect;
        public Text          nameUI;
        public List<Text>    infoUIList = new List<Text>();
        public Text          detailUI;

        public void Show(GameUnitBase unit)
        {
            InitContent(unit);
            this.root.SetActive(true);
            _isTracing = true;
        }

        public void Close()
        {
            this.root.SetActive(false);
            _isTracing = false;
        }


        public void InitContent(GameUnitBase unit)
        {
            List<InfoLine> info2Show = new List<InfoLine>();
            if (unit is Worker)
            {
                Worker worker = unit as Worker;
                info2Show.Add(new InfoLine("生命值",   worker.MaxHp.ToString()));
                info2Show.Add(new InfoLine("防御力",   worker.Defence.ToString()));
                info2Show.Add(new InfoLine("速度",    worker.MaxSpeed.ToString()));
                info2Show.Add(new InfoLine("食物运载量", worker.maxLoad[0].ToString()));
                info2Show.Add(new InfoLine("黄金运载量", worker.maxLoad[1].ToString()));
            }else if (unit is MeleeUnitsBase)
            {
                MeleeUnitsBase u = unit as MeleeUnitsBase;
                info2Show.Add(new InfoLine("生命值",   u.MaxHp.ToString()));
                info2Show.Add(new InfoLine("防御力",   u.Defence.ToString()));
                info2Show.Add(new InfoLine("速度",    u.MaxSpeed.ToString()));
                info2Show.Add(new InfoLine("攻击力", u.Attack.ToString()));
                info2Show.Add(new InfoLine("补给消耗", u.MaintenanceCostFood.ToString() + "食 / " + u.MaintenanceTime.ToString() + "秒"));
            }else if (unit is RangedAttackUnitBase)
            {
                RangedAttackUnitBase u = unit as RangedAttackUnitBase;
                info2Show.Add(new InfoLine("生命值",  u.MaxHp.ToString()));
                info2Show.Add(new InfoLine("防御力",  u.Defence.ToString()));
                info2Show.Add(new InfoLine("速度",   u.MaxSpeed.ToString()));
                info2Show.Add(new InfoLine("攻击力",  u.Attack.ToString()));
                info2Show.Add(new InfoLine("补给消耗", u.MaintenanceCostFood.ToString() + "食 / " + u.MaintenanceTime.ToString() + "秒"));
            }

            UnitData data = GetBuildingDataData(unit.unitID);

            for (int i = 0; i < infoUIList.Count; i++)
            {
                infoUIList[i].text = info2Show[i].ToString();
            }

            nameUI.text   = data.name;
            detailUI.text = data.content;
        }
        
        private bool _isRightPanel = true;

        public void TraceDetail()
        {
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, null, out mousePos);
            rootRect.pivot         = new Vector2(_isRightPanel ? 0 : 1, 0);
            rootRect.localPosition = mousePos;
            if (-rootRect.offsetMax.x < -10) //Right
                _isRightPanel = false;
            else if(rootRect.offsetMin.x < -210) //Left
                _isRightPanel = true;

        }

        
        private UnitData GetBuildingDataData(int unitId)
        {
            int         techID    = unitId;
            TextAsset   textAsset = (TextAsset)Resources.Load("Data/Unit/UnitData0");
            XmlDocument xmlDoc    = new XmlDocument();
            xmlDoc.LoadXml(textAsset.text);
            XmlNode unitXml = xmlDoc.GetElementById("u" + techID.ToString("d4"));

            return new UnitData(unitId, unitXml["Name"].InnerText.Trim(), unitXml["Content"].InnerText.Trim());
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
