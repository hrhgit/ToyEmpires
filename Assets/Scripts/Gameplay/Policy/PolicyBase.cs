using System;
using System.Collections.Generic;
using Gameplay.Buff;

namespace Gameplay.Policy
{
    [Serializable]
    public class PolicyBase
    {
        public int        policyID;
        public PolicyType policyType;
        public int        occupancy;

        public List<PlayerBuffBase> playerBuffs = new List<PlayerBuffBase>();
        public List<UnitBuffBase>   unitBuffs = new List<UnitBuffBase>();
    }
}
