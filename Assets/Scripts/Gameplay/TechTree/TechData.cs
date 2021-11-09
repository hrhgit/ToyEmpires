using System;
using Gameplay.Policy;

namespace Gameplay.TechTree
{
    public struct TechData
    {
        public string     techName;
        public string     techContent;

        public TechData(string techName, string techContent)
        {
            this.techName    = techName;
            this.techContent = techContent;
        }

    }
}