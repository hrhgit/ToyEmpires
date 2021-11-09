using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Buff;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Buildings
{
    public class BuildingsManager : MonoBehaviour
    {
        public PlayerBase           player;
        public List<BuildingsGroup> buildingsGroups = new List<BuildingsGroup>();
        public List<Building>       buildingPrefabs = new List<Building>();

        [SerializeField] private FloatBuffableValue baseBuildingSpeed = new FloatBuffableValue(1);
        

        public float BuildingSpeed =>
            baseBuildingSpeed + .02f * player.Productivity / buildingsGroups.Sum((group => group.buildings.Count(building => building != null && building.IsProdutivityRequiring)));
    }
}
