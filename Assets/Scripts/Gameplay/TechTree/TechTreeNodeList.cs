using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.TechTree
{
    // [Serializable]
    public class TechTreeNodeList
    {
        public  UnityEvent<TechTreeNodeList> nodeListUpdateEvent = new UnityEvent<TechTreeNodeList>();
        [SerializeField]private List<TechTreeNode>           nodeList            = new List<TechTreeNode>();
        public TechTreeNode this[int idx]
        {
            get { return NodeList[idx]; }
        }

        public TechTreeNodeList()
        {

        }
        public TechTreeNodeList(TechTreeNodeStat[] nodes)
        {
            NodeList = new List<TechTreeNode>();
            foreach (TechTreeNodeStat nodeStat in nodes)
            {
                if(nodeStat.formerNodesIdxs != null)
                {
                    var formerNodes = (from idx in nodeStat.formerNodesIdxs
                                       select NodeList[idx]).ToList();
                    // for (int i = 0; i < nodeStat.formerNodesIdxs.Length; i++)
                    // {
                    //     Debug.Log("former idx:" + nodeStat.formerNodesIdxs[i] + " : " + NodeList[i].nodeIdx);
                    // }
                    // formerNodes = new List<TechTreeNode>();
                    AddNode(new TechTreeNode(nodeStat.techTree, nodeStat.tech,formerNodes, nodeStat.developingFunc, nodeStat.readyFunc, nodeStat.developedFunc));
                }                
                else
                {
                    AddNode(new TechTreeNode(nodeStat.techTree, nodeStat.tech, nodeStat.developingFunc, nodeStat.readyFunc, nodeStat.developedFunc));
                }
                
            }
        }

        public List<TechTreeNode> NodeList
        {
            get => nodeList;
            private set => nodeList = value;
        }

        public int Count
        {
            get => NodeList.Count;
        }

        public void AddNode(TechTreeNode node)
        {
            node.nodeIdx = NodeList.Count;
            NodeList.Add(node);
            nodeListUpdateEvent.Invoke(this);
        }


    }
}