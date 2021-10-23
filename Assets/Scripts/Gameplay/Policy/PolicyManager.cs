using System.Collections.Generic;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Policy
{
    public class PolicyManager : MonoBehaviour
    {
        public  List<PolicyBase> availablePolicies              = new List<PolicyBase>();
        private List<int>        _economyActivatedPolicyIndexes  = new List<int>();
        private List<int>        _militaryActivatedPolicyIndexes = new List<int>();
        private List<int>        _specialActivatedPolicyIndexes  = new List<int>();

        public PlayerBase targetPlayer;

    }
}
