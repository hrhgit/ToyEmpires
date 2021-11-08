namespace Gameplay.TechTree
{
    public class TechTreeNodeStat
    {
        public readonly TechTree        techTree;
        public readonly int[]           formerNodesIdxs;
        public readonly Technology      tech;
        public readonly TechDevelopFunc developingFunc;
        public readonly TechDevelopFunc readyFunc;
        public readonly TechDevelopFunc developedFunc;

        public TechTreeNodeStat(TechTree techTree, int[] formerNodesIdxs, Technology tech,TechDevelopFunc developingFunc = null, TechDevelopFunc readyFunc = null, TechDevelopFunc developedFunc = null)
        {
            this.techTree        = techTree;
            this.formerNodesIdxs = formerNodesIdxs;
            this.tech            = tech;
            this.developingFunc  = developingFunc;
            this.readyFunc       = readyFunc;
            this.developedFunc   = developedFunc;
        }
        public TechTreeNodeStat(TechTree techTree, Technology tech, TechDevelopFunc developingFunc = null, TechDevelopFunc readyFunc = null, TechDevelopFunc developedFunc = null)
        {
            this.techTree        = techTree;
            this.tech            = tech;
            this.formerNodesIdxs = null;
            this.developingFunc  = developingFunc;
            this.readyFunc       = readyFunc;
            this.developedFunc   = developedFunc;
        }
    }
}