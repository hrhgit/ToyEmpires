using System;
using System.Collections.Generic;
using Gameplay.Buff;
using Gameplay.GameUnit.SoldierUnit.CombatUnit.RangedAttackUnit;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Policy
{
    public class PolicyManager : MonoBehaviour
    {
        public  List<PolicyBase> availablePolicies              = new List<PolicyBase>();
        private List<int>        _economyActivatedPolicyIndexes  = new List<int>();
        private List<int>        _militaryActivatedPolicyIndexes = new List<int>();
        private List<int>        _specialActivatedPolicyIndexes  = new List<int>();

        public PlayerBase targetPlayer;

        public void AddPolicy(PolicyBase policyBase)
        {
            this.availablePolicies.Add(policyBase);
            int index = this.availablePolicies.IndexOf(policyBase);
            switch (policyBase.policyType)
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

        public void RemovePolicy(PolicyBase policyBase)
        {
            int index = this.availablePolicies.IndexOf(policyBase);
            switch (policyBase.policyType)
            {
                case PolicyType.Economy:
                    _economyActivatedPolicyIndexes[index] = -1;
                    break;
                case PolicyType.Military:
                    _militaryActivatedPolicyIndexes[index] = -1;
                    break;
                case PolicyType.Special:
                    _specialActivatedPolicyIndexes[index] = -1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            this.availablePolicies[index] = null;
        }

        public void ActivatePolicy(int index)
        {
            if(availablePolicies[index] != null) 
                targetPlayer.ActivatePolicy(availablePolicies[index]);
        }
        public void DeactivatePolicy(int index)
        {
            if(availablePolicies[index] != null) 
                targetPlayer.DeactivatePolicy(availablePolicies[index]);
        }

        
        private void Start()
        {
            /********测试***************/
            this.AddPolicy(new PolicyBase()
                           {
                               playerBuffs = new List<PlayerBuffBase>()
                                             {
                                                 new PlayerBuffBase(new List<UnityAction<BuffBase>>()
                                                                    {
                                                                        (b =>
                                                                         {
                                                                             PlayerBuffBase buff = b as PlayerBuffBase;
                                                                             Debug.Log(buff.activatePlayer);
                                                                             buff.activatePlayer.unitPrefabList.ForEach((u =>
                                                                                                                         {
                                                                                                                             if (u.unitID == 3000)
                                                                                                                             {
                                                                                                                                 Archer archer = u as Archer;
                                                                                                                                 archer.SetNumericalValueBuff(BuffNumericalValueType.CostTime, false, .1f);
                                                                                                                             }
                                                                                                                         }));
                                                                         })
                                                                    },null)
                                                 {
                                                     buffID = 1000,
                                                     isSuperimposable = true,
                                                 }
                                             },
                               occupancy = 1,
                               policyType = PolicyType.Economy,
                           });

            /********测试***************/
        }
    }
}
