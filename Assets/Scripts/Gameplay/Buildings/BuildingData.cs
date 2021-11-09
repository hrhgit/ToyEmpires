using System;

namespace Gameplay.Buildings
{
    public struct BuildingData
    {
        public int        buildingIdx;
        public Building   building;
        public string     buildingName;
        public string     buildingContent;

        public BuildingData(int buildingIdx, Building building, string buildingName, string buildingContent)
        {
            this.buildingIdx     = buildingIdx;
            this.building        = building;
            this.buildingName    = buildingName;
            this.buildingContent = buildingContent;
        }
    }
}