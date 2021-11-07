using System;
using Gameplay.Policy;

namespace Gameplay.TechTree
{
    public struct TechData
    {
        public int        techIdx;
        public Technology tech;
        public String     techName;
        public String     techContent;

        public TechData(int techIdx, Technology tech, string techName, string techContent)
        {
            this.techIdx     = techIdx;
            this.tech        = tech;
            this.techName    = techName;
            this.techContent = techContent;
        }

    }
}