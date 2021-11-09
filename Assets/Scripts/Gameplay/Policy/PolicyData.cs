using System;

namespace Gameplay.Policy
{
    // [Serializable]
    public struct PolicyData
    {
        public int        policyIdx;
        public PolicyBase policyBase;
        public String     policyName;
        public String     policyContent;

        public PolicyData(int policyIdx,PolicyBase policyBase, string policyName, string policyContent)
        {
            this.policyIdx     = policyIdx;
            this.policyBase    = policyBase;
            this.policyName    = policyName;
            this.policyContent = policyContent;
        }
    }
}