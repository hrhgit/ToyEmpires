using System.Collections.Generic;
using UnityEngine;

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
        
        public UIC_Entity entity;

        public List<TechTreeNodeUI> formerNodes = new List<TechTreeNodeUI>();
        public List<TechTreeNodeUI> afterNodes  = new List<TechTreeNodeUI>();
    }
}
