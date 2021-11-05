using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Buff;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.TechTree
{
    public class TechTree : MonoBehaviour
    {
        public  List<TechTreeNode> techTreeNodes = new List<TechTreeNode>();
        public  PlayerBase         player;
        private FloatBuffableValue additionalSpeed    = new FloatBuffableValue(0);
        private FloatBuffableValue magnificationSpeed = new FloatBuffableValue(1);

        

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
            this.techTreeNodes.Add(new TechTreeNode(
                                                    null,
                                                    (node => Debug.Log("0 is developing!")),
                                                    (node => Debug.Log("0 is done!!"))
                                                    )
                                   {
                                       technology = new Technology(),
                                   });
            this.techTreeNodes.Add(new TechTreeNode(
                                                    new List<TechTreeNode>(){this.techTreeNodes[0]},
                                                    (node => Debug.Log("1 is developing!")),
                                                    (node => Debug.Log("1 is done!!"))
                                                   )
                                   {
                                       developSpeed = new FloatBuffableValue(3f),
                                       technology = new Technology(0, 0, 0, 0, 0, 0, 2),
                                   });
            //
            Generate();
        }

        private void FixedUpdate()
        {
            DevelopTechs();
        }
        
        public void Generate()
        {
            techTreeNodes.ForEach((node =>
                                   {
                                       node.Generate();
                                       node.nodeIdx = techTreeNodes.IndexOf(node);
                                   }));
        }
        private void DevelopTechs()
        {
            techTreeNodes.Where((node => node.IsDevelopable)).Select((node => node)).ToList().ForEach((node => node.Develop(AdditionalSpeed,magnificationSpeed)));
        }

        #region 科技购买
        private Dictionary<int,Technology> _activatedTechsDict = new Dictionary<int,Technology>();

        public void Purchase(int nodeIdx,bool isUseGold)
        {
            if(this.techTreeNodes[nodeIdx].IsReady)
            {
                if(this.player.PurchaseTechNode(nodeIdx, isUseGold))
                {
                    Technology tech = this.techTreeNodes[nodeIdx].technology;
                    _activatedTechsDict.Add(nodeIdx,tech);
                    this.player.ActivateTech(tech);
                }
                
            }
            
        }

        

        #endregion
    }
}
