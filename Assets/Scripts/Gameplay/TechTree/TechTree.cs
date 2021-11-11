using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Buff;
using Gameplay.Player;
using GameUI.TechTreeUI;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.TechTree
{
    public class TechTree : MonoBehaviour
    {
        public  TechTreeNodeList     techTreeNodes = new TechTreeNodeList();
        public  PlayerBase           player;
        public  UnityEvent<TechTree> techTreeInitializedEvent = new UnityEvent<TechTree>();
        private FloatBuffableValue   additionalSpeed          = new FloatBuffableValue(0);
        private FloatBuffableValue   magnificationSpeed       = new FloatBuffableValue(1);
        private TechTreePanelUI      _techTreePanelUI;

        

        public float AdditionalSpeed
        {
            get => additionalSpeed;
        }
        public float MagnificationSpeed
        {
            get => magnificationSpeed;
        }

        private void Start()
        {
            //测试
            // InitTechTree();
            //
            
            
        }


        public void InitTechTree()
        {
            if(BattleGameManager.BattleGameManagerInstance.userPlayer == this.player) {
                _techTreePanelUI = BattleGameManager.BattleGameManagerInstance.techTreePanelUI;
                techTreeInitializedEvent.AddListener((tech => _techTreePanelUI.GenerateLayout(tech)));
            }

            this.techTreeNodes = TechTreeGenerator.GenerateTechTreeNodeList(this.player.civilization.techTreeId, this);
            techTreeInitializedEvent.Invoke(this);
        }

        private void FixedUpdate()
        {
            DevelopTechs();
        }
        
        public void Generate()
        {
            techTreeNodes.NodeList.ForEach((node =>
                                   {
                                       node.Generate();
                                       node.nodeIdx = techTreeNodes.NodeList.IndexOf(node);
                                   }));
        }
        private void DevelopTechs()
        {
            techTreeNodes.NodeList.Where((node => node.IsDevelopable)).Select((node => node)).ToList().ForEach((node => node.Develop(AdditionalSpeed,magnificationSpeed)));
        }

        #region 科技购买
        private Dictionary<int,Technology> _activatedTechsDict = new Dictionary<int,Technology>();

        public bool Purchase(int nodeIdx,bool isUseGold)
        {
            if (_activatedTechsDict.ContainsKey(nodeIdx))
                return false;
            if(this.techTreeNodes[nodeIdx].IsDevelopable)
            {
                if(this.player.PurchaseTechNode(nodeIdx, isUseGold))
                {
                    Technology tech = this.techTreeNodes[nodeIdx].technology;
                    _activatedTechsDict.Add(nodeIdx,tech);
                    this.player.ActivateTech(tech);
                    return true;
                }
                
            }

            return false;
        }

        

        #endregion
    }
}
