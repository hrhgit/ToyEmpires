using System;
using System.Collections.Generic;
using Gameplay.Buff;
using Gameplay.GameUnit.SoldierUnit.CombatUnit.RangedAttackUnit;
using Gameplay.Player;
using GameUI.PolicyUI;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Policy
{
    public class PolicyManager : MonoBehaviour
    {
        public  List<PolicyBase> availablePolicies        = new List<PolicyBase>();
        public  List<int>        activatedPoliciesIndexes = new List<int>();
        public  PlayerBase       targetPlayer;

        public  UnityEvent<PolicyManager> policyUpdateEvent               = new UnityEvent<PolicyManager>(); 
        private List<int>                 _economyActivatedPolicyIndexes  = new List<int>();
        private List<int>                 _militaryActivatedPolicyIndexes = new List<int>();
        private List<int>                 _specialActivatedPolicyIndexes  = new List<int>();

        
        private PolicyManagerUI _policyManagerUI;

        public void AddPolicy(PolicyBase policyBase)
        {
            this.availablePolicies.Add(policyBase);
            // int index = this.availablePolicies.IndexOf(policyBase);
            policyUpdateEvent.Invoke(this);
        }

        public void RemovePolicy(PolicyBase policyBase)
        {
            int index = this.availablePolicies.IndexOf(policyBase);
            this.availablePolicies[index] = null;
            policyUpdateEvent.Invoke(this);
        }

        public void ActivatePolicy(int index)
        {
            if(availablePolicies[index] != null)
            {
                targetPlayer.ActivatePolicy(availablePolicies[index]);
                activatedPoliciesIndexes.Add(index);
                switch (availablePolicies[index].policyType)
                {
                    case PolicyType.Economy:
                        _economyActivatedPolicyIndexes.Add(index);
                        break;
                    case PolicyType.Military:
                        _militaryActivatedPolicyIndexes.Add(index);
                        break;
                    case PolicyType.Special:
                        _specialActivatedPolicyIndexes.Add(index);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
            policyUpdateEvent.Invoke(this);
        }
        public void DeactivatePolicy(int index)
        {
            if(availablePolicies[index] != null)
            {
                targetPlayer.DeactivatePolicy(availablePolicies[index]);
                activatedPoliciesIndexes.Remove(index);
                switch (availablePolicies[index].policyType)
                {
                    case PolicyType.Economy:
                        _economyActivatedPolicyIndexes.Remove(index);
                        break;
                    case PolicyType.Military:
                        _militaryActivatedPolicyIndexes.Remove(index);
                        break;
                    case PolicyType.Special:
                        _specialActivatedPolicyIndexes.Remove(index);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            policyUpdateEvent.Invoke(this);
        }

        
        private void Start()
        {
            if(BattleGameManager.BattleGameManagerInstance.userPlayer == this.targetPlayer)
            {
                _policyManagerUI = BattleGameManager.BattleGameManagerInstance.policyManagerUI;
                policyUpdateEvent.AddListener((manager => _policyManagerUI.UpdatePolicies()));
            }
            /********测试***************/
            this.AddPolicy(PolicyGenerator.GeneratePolicy(2000));
            this.AddPolicy(PolicyGenerator.GeneratePolicy(2001));
            /********测试***************/
        }
    }
}
