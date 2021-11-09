using System.Collections.Generic;
using Gameplay;
using UnityEngine;

namespace GameUI.BuildingUI
{
    public class BuildingGrpUI : MonoBehaviour
    {
        public int                 grpIdx;
        public List<BuildingPosUI> buildingPosUis = new List<BuildingPosUI>();
        public Road                road;
        public BuildingPanelUI     panelUI;

    }
}
