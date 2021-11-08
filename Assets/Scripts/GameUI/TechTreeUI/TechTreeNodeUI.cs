using System;
using System.Collections.Generic;
using Gameplay.TechTree;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI.TechTreeUI
{
    public class TechTreeNodeUI : MonoBehaviour
    {
        public int        techID;
        
        /// <summary>
        /// 进的
        /// </summary>
        public UIC_Node   outNode;
        /// <summary>
        /// 出的
        /// </summary>
        public UIC_Node   inNode;
        
        public UIC_Entity           entity;
        public TechTreePanelUI      techTreePanelUI;
        public List<TechTreeNodeUI> formerNodes = new List<TechTreeNodeUI>();
        public List<TechTreeNodeUI> afterNodes  = new List<TechTreeNodeUI>();
        public TechTreeNode         techTreeNode;
        public Image                techImage;
        public GameObject           outline;
        public GameObject           goldBtn;
        public GameObject           foodWoodBtn;

        private void Update()
        {
            techImage.fillAmount = techTreeNode.Process;

            if (this.techTreeNode.IsDeveloped)
            {
                this.outNode.connectionsList.ForEach((connection => connection.line.color             = new Color(1f,    1f,    1f)));
                this.inNode.connectionsList.ForEach((connection => connection.line.color              = new Color(0.25f, 0.81f, 0.45f)));
                this.inNode.connectionsList.ForEach((connection => connection.line.animation.isActive = false));
                goldBtn.SetActive(false);
                foodWoodBtn.SetActive(false);
                outline.SetActive(true);
            }else if (this.techTreeNode.IsReady)
            {
                this.inNode.connectionsList.ForEach((connection => connection.line.animation.isActive = false));
                this.inNode.connectionsList.ForEach((connection => connection.line.color              = new Color(0.25f, 0.81f, 0.45f)));
                goldBtn.SetActive(true);
                foodWoodBtn.SetActive(true);
                outline.SetActive(true);
            }else if (this.techTreeNode.IsDevelopable)
            {
                this.outNode.connectionsList.ForEach((connection => connection.line.animation.isActive = false));
                this.inNode.connectionsList.ForEach((connection => connection.line.animation.isActive = true));
                goldBtn.SetActive(true);
                foodWoodBtn.SetActive(true);
            }
            else
            {
                this.outNode.connectionsList.ForEach((connection => connection.line.animation.isActive = false));
                goldBtn.SetActive(false);
                foodWoodBtn.SetActive(false);
                // this.inNode.connectionsList.ForEach((connection => connection.line.color               = new Color(0.45f, 0.45f, 0.44f)));
            }
        }

        public void Purchase(bool useGold)
        {
            if (techTreeNode.techTree.Purchase(techTreeNode.nodeIdx, useGold))
            {
                goldBtn.SetActive(false);
                foodWoodBtn.SetActive(false);
            }
        }

        #region 详情信息

        public String techName;
        [TextArea]
        public String techDetail;

        public void OnMouseIn()
        {
            this.techTreePanelUI.ShowDetail(this);
        }
        
        public void OnMouseLeft()
        {
            this.techTreePanelUI.CloseDetail();
        }

        #endregion
    }
}
