using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Gameplay;
using Gameplay.Policy;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI.PolicyUI
{
    public class PolicyManagerUI : MonoBehaviour
    {
        public PolicyManager policyManager;
        public PolicyLineUI  policyLineUI;
        
        public Transform selectedPolicyScrollContent;
        public Transform availablePolicyScrollContent;
        public Transform panelUI;

        public Text  policyCountTextUI;
        public float offset = -20;
        
        [Header("Content")]
        public RectTransform ContentRect;
        public Text          ContentNameTextUI;
        public Text          ContentTextUI;
        public Image         ContentTypeUI;
        public Text          ContentOccTextUI;

        private RectTransform _selectedPolicyRect;
        private RectTransform _availablePolicyRect;
        private RectTransform _rectTrans;
        private float         height;

        private void Start()
        {
            policyManager        = BattleGameManager.BattleGameManagerInstance.userPlayer.playerPolicyManager;
            _selectedPolicyRect  = selectedPolicyScrollContent.GetComponent<RectTransform>();
            _availablePolicyRect = availablePolicyScrollContent.GetComponent<RectTransform>();
            _rectTrans           = this.transform.GetComponent<RectTransform>();
            height               = policyLineUI.GetComponent<RectTransform>().sizeDelta.y;
        }

        private PolicyData GetPolicyData(int policyIdx)
        {
            int         policyID  = policyManager.availablePolicies[policyIdx].policyID;
            TextAsset   textAsset = (TextAsset)Resources.Load("Data/Policy/PolicyData0");
            XmlDocument xmlDoc    = new XmlDocument();
            xmlDoc.LoadXml(textAsset.text);
            XmlNode policyXml = xmlDoc.GetElementById("p" + policyID);
            return new PolicyData(policyIdx,policyManager.availablePolicies[policyIdx], policyXml["Name"].InnerText.Trim(), policyXml["Content"].InnerText.Trim());
        }

        public void UpdatePolicies()
        {
            ClearPolicies();
            int selectedCount  = 0;
            int availableCount = 0;

            foreach (PolicyBase policy in policyManager.availablePolicies)
            {
                int        index  = policyManager.availablePolicies.IndexOf(policy);
                PolicyData data   = GetPolicyData(index);
                
                if (policyManager.activatedPoliciesIndexes.Contains(index))
                {
                    PolicyLineUI line = Instantiate<PolicyLineUI>(policyLineUI, selectedPolicyScrollContent);
                    line.title                                          = data.policyName;
                    line.content                                        = data.policyContent;
                    line.type                                           = data.policyBase.policyType;
                    line.occ                                            = data.policyBase.occupancy;
                    line.policyManagerUI                                = this;
                    line.policyData                                     = data;
                    line.isSelected                                     = true;
                    line.GetComponent<RectTransform>().anchoredPosition = new Vector2(_selectedPolicyRect.anchoredPosition.x, offset * (selectedCount + 1) - height * selectedCount);
                    selectedCount++;
                }
                else
                {
                    PolicyLineUI line = Instantiate<PolicyLineUI>(policyLineUI, availablePolicyScrollContent);
                    line.title                                          = data.policyName;
                    line.content                                        = data.policyContent;
                    line.type                                           = data.policyBase.policyType;
                    line.occ                                            = data.policyBase.occupancy;
                    line.policyManagerUI                                = this;
                    line.isSelected                                     = false;
                    line.policyData                                     = data;
                    line.GetComponent<RectTransform>().anchoredPosition = new Vector2(_availablePolicyRect.anchoredPosition.x, offset * (availableCount + 1) - height * availableCount);
                    availableCount++;

                }
                
            }

            policyCountTextUI.text = policyManager.activatedPoliciesIndexes.Sum((i => policyManager.availablePolicies[i].occupancy)) + "/" + policyManager.targetPlayer.PolicyCapacity;

        }

        
        public void EnableDetailPanel(PolicyLineUI policyLineUI)
        {
            Color color = policyLineUI.type switch
                          {
                              PolicyType.Normal   => PolicyLineUI.NormalPolicyColor,
                              PolicyType.Economy  => PolicyLineUI.EconomyPolicyColor,
                              PolicyType.Military => PolicyLineUI.MilitaryPolicyColor,
                              PolicyType.Special  => PolicyLineUI.SpeciaPolicyColor,
                              _                   => throw new ArgumentOutOfRangeException()
                          };

            this.ContentNameTextUI.text  = policyLineUI.title;
            this.ContentTextUI.text      = policyLineUI.content;
            this.ContentOccTextUI.text   = policyLineUI.occ.ToString();
            this.ContentNameTextUI.color = color;
            this.ContentOccTextUI.color  = color;
            this.ContentTypeUI.color     = color;
            ContentRect.gameObject.SetActive(true);
        }
        
        public void DisableDetailPanel(PolicyLineUI policyLineUI)
        {
            ContentRect.gameObject.SetActive(false);
        }
        private bool _isRightPanel = true;

        public void TraceDetail(PolicyLineUI policyLineUI)
        {
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTrans, Input.mousePosition, null, out mousePos);
            ContentRect.pivot         = new Vector2(_isRightPanel ? 0 : 1, 1);
            ContentRect.localPosition = mousePos;
            if (-ContentRect.offsetMax.x < -10) //Right
                _isRightPanel = false;
            else if(ContentRect.offsetMin.x < -210) //Left
                _isRightPanel = true;

        }

        private void ClearPolicies()
        {
            for (int i = 0; i < _selectedPolicyRect.transform.childCount; i++) {
                Destroy (_selectedPolicyRect.transform.GetChild (i).gameObject);
            }
            for (int i = 0; i < _availablePolicyRect.transform.childCount; i++) {
                Destroy (_availablePolicyRect.transform.GetChild (i).gameObject);
            }
        }

        public void SelectPolicy(PolicyLineUI policyLineUI)
        {
            policyManager.ActivatePolicy(policyLineUI.policyData.policyIdx);
        }
        
        public void DeselectPolicy(PolicyLineUI policyLineUI)
        {
            policyManager.DeactivatePolicy(policyLineUI.policyData.policyIdx);
        }

        public void Close()
        {
            panelUI.gameObject.SetActive(false);
        }
    }
}
