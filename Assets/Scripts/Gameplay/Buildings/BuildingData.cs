using System;

namespace Gameplay.Buildings
{
    public struct BuildingData
    {
        public string     buildingName;
        public string     buildingContent;

        public BuildingData(string buildingName, string buildingContent)
        {
            this.buildingName    = buildingName;
            this.buildingContent = buildingContent;
        }
    }
}