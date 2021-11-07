using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Buff;
using Gameplay.Player;
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
            InitTechTree();
            //
            // Generate();
            techTreeInitializedEvent.Invoke(this);
        }
        

        private void InitTechTree()
        {
            // this.techTreeNodes.AddNode(new TechTreeNode(
            //                                             this,
            //                                             new Technology(),
            //                                             (node => Debug.Log("0 is developing!")),
            //                                             (node => Debug.Log("0 is done!!")),
            //                                             (node => Debug.Log("0 is developed!!"))
            //                                            )
            //                            {
            //                                
            //                            });
            //
            // this.techTreeNodes.AddNode(new TechTreeNode(
            //                                             this,
            //                                             new Technology(0, 0, 0, 0, 0, 0, 5),
            //                                             new[] { this.techTreeNodes[0] },
            //                                             (node => Debug.Log("1 is developing!")),
            //                                             (node => Debug.Log("1 is done!!")),
            //                                             (node => Debug.Log("1 is developed!!"))
            //                                            )
            //                            {
            //                                developSpeed = new FloatBuffableValue(3f),
            //                            });
            //
            // this.techTreeNodes.AddNode(new TechTreeNode(
            //                                             this,
            //                                             new Technology(),
            //                                             new[] { this.techTreeNodes[0] },
            //                                             (node => Debug.Log("2 is developing!")),
            //                                             (node => Debug.Log("2 is done!!")),
            //                                             (node => Debug.Log("2 is developed!!"))
            //                                            )
            //                            {
            //                            });
            // this.techTreeNodes.AddNode(new TechTreeNode(
            //                                             this,
            //                                             new Technology(),
            //                                             new[] { this.techTreeNodes[1] },
            //                                             (node => Debug.Log("3 is developing!")),
            //                                             (node => Debug.Log("3 is done!!")),
            //                                             (node => Debug.Log("3 is developed!!"))
            //                                            )
            //                            {
            //                            });
            // this.techTreeNodes.AddNode(new TechTreeNode(
            //                                             this,
            //                                             new Technology(),
            //                                             new[] { this.techTreeNodes[1] },
            //                                             (node => Debug.Log("4 is developing!")),
            //                                             (node => Debug.Log("4 is done!!")),
            //                                             (node => Debug.Log("4 is developed!!"))
            //                                            )
            //                            {
            //                            });
            // this.techTreeNodes.AddNode(new TechTreeNode(
            //                                             this,
            //                                             new Technology(),
            //                                             new[] { this.techTreeNodes[1], this.techTreeNodes[2] },
            //                                             (node => Debug.Log("5 is developing!")),
            //                                             (node => Debug.Log("5 is done!!")),
            //                                             (node => Debug.Log("5 is developed!!"))
            //                                            )
            //                            {
            //
            //                            });
            this.techTreeNodes = TechTreeGenerator.GenerateTechTreeNodeList(0, this);
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
