using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Gameplay.TechTree;
using UnityEditor;
using UnityEngine;

namespace GameUI.TechTreeUI
{
    public class TechTreePanelUI : MonoBehaviour
    {
        public UIC_LineRenderer        connectionLineRenderer;
        public UIC_Manager             connectionManager;
        public List<TechTreeNodeUI>    techTreeNodeUis = new List<TechTreeNodeUI>();
        public TechTree                techTree;
        public SetupInitialConnections initialConnections;

        private void Start()
        {
            techTree = BattleGameManager.BattleGameManagerInstance.userPlayer.techTree;
            InitConection();
        }

        private void InitConection()
        {
            foreach (var node in techTreeNodeUis)
            {
                node.formerNodes.ForEach((n =>
                                          {
                                              // Connect(n, node);
                                              initialConnections.size++;
                                              initialConnections.fromNodes.Add(node.inNode);
                                              initialConnections.toNodes.Add(n.outNode);
                                          }));
                
            }
            initialConnections.Refresh();
        }

        private void Connect(TechTreeNodeUI from, TechTreeNodeUI to)
        {
            from.outNode.ConnectTo(to.inNode);
        }

        [ContextMenu("Generate Nodes")]
        public void GenerateNodes()
        {
            techTreeNodeUis = this.transform.GetComponentsInChildren<TechTreeNodeUI>().ToList();
            techTreeNodeUis.ForEach((nodeUI =>
                                     {
                                         nodeUI.entity.UicManager      = this.connectionManager;
                                         nodeUI.inNode.uILineRenderer  = this.connectionLineRenderer;
                                         nodeUI.outNode.uILineRenderer = this.connectionLineRenderer;
                                     }));
        }


        #region 布局

        public RectTransform nodesRect;

        public void GenerateLayout()
        {
            var depthGroup = (from node in techTree.techTreeNodes
                              group node by node.depth
                              into nodeDepthGroup
                              orderby nodeDepthGroup.Key
                              select new
                                     {
                                         depth = nodeDepthGroup.Key,
                                         nodes = nodeDepthGroup.ToList()
                                     }).ToList();

        }

        #endregion
    }
}
