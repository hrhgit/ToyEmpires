using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Gameplay;
using Gameplay.TechTree;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GameUI.TechTreeUI
{
    public class TechTreePanelUI : MonoBehaviour
    {
        public UIC_LineRenderer        connectionLineRenderer;
        public UIC_Manager             connectionManager;
        public List<TechTreeNodeUI>    techTreeNodeUis = new List<TechTreeNodeUI>();
        public TechTree                techTree;

        private void Start()
        {
            techTree = BattleGameManager.BattleGameManagerInstance.userPlayer.techTree;
            techTree.techTreeInitializedEvent.AddListener(GenerateLayout);
            
        }
        

        private void Update()
        {
            // foreach (UIC_Connection conn in connectionManager.ConnectionsList)
            // {
            //     conn.UpdateLine();
            // }
            if (_isTracing)
            {
                this.TraceDetail();

            }
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
                                         connectionManager.AddEntity(nodeUI.entity);
                                         nodeUI.entity.UicManager      = this.connectionManager;
                                         nodeUI.inNode.uILineRenderer  = this.connectionLineRenderer;
                                         nodeUI.outNode.uILineRenderer = this.connectionLineRenderer;
                                     }));
        }

        #region 布局

        public TechTreeNodeUI nodeUiPrefab;
        public RectTransform  nodesRect;

        //高： +- 413
        //宽：1349
        private static float _height = 350f;
        private static float _width  = 1349f;

        public void Clear()
        {
            connectionManager.EntityList.Clear();
            connectionManager.ConnectionsList.Clear();
            this.techTreeNodeUis.Clear();
            for (int i = 0; i < this.nodesRect.transform.childCount; i++)
            {
                TechTreeNodeUI tmp;
                bool           isNode = this.nodesRect.transform.GetChild(i).TryGetComponent<TechTreeNodeUI>(out tmp);
                if(!isNode) continue;
                DestroyImmediate(this.nodesRect.transform.GetChild(i).gameObject);
            }
            
        }
        public void GenerateLayout(TechTree techTree)
        {
            // panel.SetActive(true);
            connectionManager.OnValidate();

            Clear();
            List<TechTreeNodeUI> nodeUIList = new List<TechTreeNodeUI>();
            var depthGroup = (from node in techTree.techTreeNodes.NodeList
                              group node by node.depth
                              into nodeDepthGroup
                              orderby nodeDepthGroup.Key
                              select new
                                     {
                                         depth = nodeDepthGroup.Key,
                                         nodes = nodeDepthGroup.ToList()
                                     }).ToList();
            float colWidth = _width / depthGroup.Count;
            foreach (var depthGrp in depthGroup)
            {
                float nodeHeight = _height * 2 / depthGrp.nodes.Count;
                float heightOri  = _height;
                for (int i = 0; i < depthGrp.nodes.Count; i++)
                {
                    TechTreeNodeUI node     = Instantiate<TechTreeNodeUI>(nodeUiPrefab, nodesRect.transform);
                    TechData       techData = GetTechData(depthGrp.nodes[i].technology.technologyID);
                    node.GetComponent<UIC_Entity>().OnValidate();
                    node.techTreePanelUI = this;
                    node.techName        = techData.techName;
                    node.techDetail      = techData.techContent;
                    node.techID          = depthGrp.nodes[i].technology.technologyID;
                    nodeUIList.Add(node);
                    RectTransform  nodeRect = node.gameObject.GetComponent<RectTransform>();
                    node.techTreeNode         = depthGrp.nodes[i];
                    node.gameObject.name      = depthGrp.nodes[i].depth + "-" + depthGrp.nodes[i].nodeIdx;
                    nodeRect.anchoredPosition = new Vector2(colWidth * depthGrp.depth, heightOri - i * nodeHeight);
                    techTreeNodeUis.Add(node);
                }
            }

            GenerateNodeLinks(nodeUIList);
            GenerateNodes(nodeUIList);
            GenerateConection();
            // panel.SetActive(false);
        }
        
        public void GenerateNodes(List<TechTreeNodeUI> nodeUIList)
        {
            nodeUIList.ForEach((nodeUI =>
                                {
                                    
                                    connectionManager.AddEntity(nodeUI.entity);
                                    nodeUI.inNode.OnValidate();
                                    nodeUI.outNode.OnValidate();
                                    nodeUI.entity.UicManager      = this.connectionManager;
                                    nodeUI.inNode.uILineRenderer  = this.connectionLineRenderer;
                                    nodeUI.outNode.uILineRenderer = this.connectionLineRenderer;
                                    nodeUI.inNode.entity          = nodeUI.entity;
                                    nodeUI.outNode.entity          = nodeUI.entity;
                                }));
        }

        private void GenerateNodeLinks(List<TechTreeNodeUI> nodeUIList)
        {
            foreach (TechTreeNodeUI nodeUI in nodeUIList)
            {
                nodeUI.formerNodes = (from node in nodeUIList
                                      where nodeUI.techTreeNode.FormerNodes.Exists((treeNode => treeNode.nodeIdx == node.techTreeNode.nodeIdx))
                                      select node).ToList();
                nodeUI.afterNodes = (from node in nodeUIList
                                     where nodeUI.techTreeNode.AfterNodes.Exists((treeNode => treeNode.nodeIdx == node.techTreeNode.nodeIdx))
                                     select node).ToList();
            }
        }
        
        private void GenerateConection()
        {
            // Debug.Log(techTreeNodeUis);
            foreach (var node in techTreeNodeUis)
            {
                // Debug.Log(node);
                node.formerNodes.ForEach((n =>
                                          {
                                              // var conn = connectionManager.AddConnection(n.outNode, node.inNode, UIC_Connection.LineTypeEnum.Spline);
                                              // Debug.Log(n.outNode);
                                              // Debug.Log(n.outNode.entity);
                                              // Debug.Log(n.outNode.entity.uicManager);
                                              // Debug.Log(node.inNode);
                                              // Debug.Log(n.outNode.entity.UicManager.globalLineType);
                                              // _connection = entity.UicManager.AddConnection(this, otherNode, entity.UicManager.globalLineType);
                                              n.outNode.ConnectTo(node.inNode);
                                              // Debug.Log(conn);
                                              // Connect(n, node);
                                          }));
                
            }
        }

        #endregion

        #region 详情

        [Header("Content")]
        public RectTransform detailCanvasRect;
        public RectTransform detailRect;
        public Text          detailNameTextUI;
        public Text          detailTextUI;
        public Text          detailCostTextUI;
        public Text          detailProcessTextUI;

        private TechData GetTechData(int techId)
        {
            int         techID    = techId;
            TextAsset   textAsset = (TextAsset)Resources.Load("Data/Tech/TechData0");
            XmlDocument xmlDoc    = new XmlDocument();
            xmlDoc.LoadXml(textAsset.text);
            XmlNode techXml = xmlDoc.GetElementById("t" + techID.ToString("d4"));
            return new TechData(techXml["Name"].InnerText.Trim(), techXml["Content"].InnerText.Trim());
        }

        private bool           _isTracing = false;
        private TechTreeNodeUI _curShowNode;
        public void ShowDetail(TechTreeNodeUI nodeUI)
        {
            _curShowNode               = nodeUI;
            this.detailNameTextUI.text = _curShowNode.techName;
            this.detailTextUI.text     = _curShowNode.techDetail;
            _isTracing                 = true;
            detailRect.gameObject.SetActive(true);
        }

        public void CloseDetail()
        {
            _isTracing = false;
            detailRect.gameObject.SetActive(false);
        }

        private bool _isRightPanel = true;
        public void TraceDetail()
        {
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(detailCanvasRect, Input.mousePosition, null, out mousePos);
            detailRect.pivot         = new Vector2(_isRightPanel ? 0 : 1, 1);
            detailRect.localPosition = mousePos;
            if (-detailRect.offsetMax.x < -60) //Right
                _isRightPanel = false;
            else if(detailRect.offsetMin.x < -60) //Left
                _isRightPanel = true;

            this.detailProcessTextUI.text = (_curShowNode.techTreeNode.Process * 100).ToString("f0") + "%";
            this.detailCostTextUI.text = "当前购买需要花费 <color=#E76868>" + _curShowNode.techTreeNode.CurCostFood + "食</color> 或 <color=#E99F20>" +
                                         _curShowNode.techTreeNode.CurCostGold + "金</color>";

        }

        #endregion

        #region 开关

        public GameObject panel;
        public void Open()
        {
            panel.SetActive(true);
        }

        public void Close()
        {
            panel.SetActive(false);
        }

        #endregion
    }
}
