using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Buff;
using UnityEngine;

namespace Gameplay.TechTree
{
    public delegate void TechDevelopFunc(TechTreeNode techTreeNode);
    
    [Serializable]
    public class TechTreeNode
    {
        public                            int                nodeIdx;
        public                            Technology         technology;
        [SerializeField] private readonly List<TechTreeNode> _formerNodes = new List<TechTreeNode>();
        [SerializeField] private readonly List<TechTreeNode> _afterNodes  = new List<TechTreeNode>();


        public int depth = 0;


        public TechTreeNode()
        {
            
        }
        
        public TechTreeNode(List<TechTreeNode> formerNodes, TechDevelopFunc developingFunc = null, TechDevelopFunc readyFunc = null, TechDevelopFunc developedFunc = null)
        {
            formerNodes?.ForEach((node => node.AddAfterNode(this)));
            TechDevelopingEvent += developingFunc;
            TechReadyEvent      += readyFunc;
            TechDevelopedEvent  += developedFunc;
        }
        public void AddAfterNode(TechTreeNode node)
        {
            this._afterNodes.Add(node);
            node._formerNodes.Add(this);
            node.depth          =  this.depth >= node.depth ? this.depth + 1 : node.depth;
            this.TechReadyEvent += node.CheckSelfDevelopable;
        }
        
        
        public void Generate()
        {
            this.IsDevelopable = CheckDevelopable();
            this._curCostFood  = technology.MaxCostFood;
            this._curCostWood  = technology.MaxCostWood;
            this._curCostGold  = technology.MaxCostGold;
        }
        


        #region 研究
        
        public FloatBuffableValue    developSpeed = new FloatBuffableValue(1f);
        public event TechDevelopFunc TechDevelopingEvent;
        public event TechDevelopFunc TechReadyEvent;
        public event TechDevelopFunc TechDevelopedEvent;
        
        private bool  _isDeveloped    = false;
        private bool  _isReady  = false;
        private bool  _isDevelopable  = false;
        private float _developProcess = 0f;
        
        public float DevelopProcess
        {
            get => _developProcess;
            private set
            {
                if (value <= this.technology.CostTime)
                {
                    _developProcess = value;
                }
                else
                {
                    this.IsReady = true;
                    TechReadyEvent?.Invoke(this);
                }
            }
        }

        public bool IsDeveloped
        {
            get => _isDeveloped;
            private set
            {
                _isDeveloped = value;
                if (value)
                {
                    TechDevelopedEvent?.Invoke(this);
                }
                
            }
        }

        public bool IsDevelopable
        {
            get => _isDevelopable;
            private set
            {
                _isDevelopable = value;
            }
        }

        public bool IsReady
        {
            get => _isReady;
            private set
            {
                _isReady = value;
                if (value)
                {
                    this.CurCostFood = technology.MinCostFood;
                    this.CurCostWood = technology.MinCostWood;
                    this.CurCostGold = technology.MinCostGold;
                    TechDevelopedEvent?.Invoke(this);
                }
                
            }
        }


        public void Develop()
        {
            if (_isReady) return;
            this.DevelopProcess += developSpeed * Time.fixedDeltaTime;
            
            this.CurCostFood    =  (int)Mathf.Ceil(Mathf.Lerp(technology.MaxCostFood, technology.MinCostFood, DevelopProcess));
            this.CurCostWood    =  (int)Mathf.Ceil(Mathf.Lerp(technology.MaxCostWood, technology.MinCostWood, DevelopProcess));
            this.CurCostGold    =  (int)Mathf.Ceil(Mathf.Lerp(technology.MaxCostGold, technology.MinCostGold, DevelopProcess));

            this.TechDevelopingEvent?.Invoke(this);
        }
        
        public void Develop(float additionalSpeed, float magnificationSpeed)
        {
            if (_isReady) return;
            this.DevelopProcess += (developSpeed + additionalSpeed) * magnificationSpeed * Time.fixedDeltaTime;
            
            this.CurCostFood    =  (int)Mathf.Ceil(Mathf.Lerp(technology.MaxCostFood, technology.MinCostFood, DevelopProcess));
            this.CurCostWood    =  (int)Mathf.Ceil(Mathf.Lerp(technology.MaxCostWood, technology.MinCostWood, DevelopProcess));
            this.CurCostGold    =  (int)Mathf.Ceil(Mathf.Lerp(technology.MaxCostGold, technology.MinCostGold, DevelopProcess));
            
            this.TechDevelopingEvent?.Invoke(this);
        }
        public bool CheckDevelopable()
        {
            return this._formerNodes.Count == 0 || this._formerNodes.All((node => node._isDeveloped));
        }
        
        public void CheckSelfDevelopable(TechTreeNode techTreeNode)
        {
            this.IsDevelopable = CheckDevelopable();
        }

        #endregion

        #region 购买

        private int _curCostWood;
        private int _curCostFood;
        private int _curCostGold;
        
        
        public int CurCostWood
        {
            get => _curCostWood;
            private set => _curCostWood = value;
        }

        public int CurCostFood
        {
            get => _curCostFood;
            private set => _curCostFood = value;
        }

        public int CurCostGold
        {
            get => _curCostGold;
            private set => _curCostGold = value;
        }


        public void Purchase()
        {
            this.IsDeveloped = true;
        }

        #endregion


    }
}
