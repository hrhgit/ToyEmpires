using System.Collections.Generic;
using Gameplay.Buff;
using Gameplay.GameUnit.SoldierUnit.CombatUnit;
using Gameplay.GameUnit.SoldierUnit.CombatUnit.MeleeUnit;
using UnityEngine.Events;

namespace Gameplay.Policy
{
    public delegate PolicyBase GeneratePolicyFunc();

    public static class PolicyGenerator
    {
        private static readonly Dictionary<int, GeneratePolicyFunc> _policyDict = new Dictionary<int, GeneratePolicyFunc>
                                                                              {
                                                                                  {
                                                                                      2000, () =>
                                                                                            {
                                                                                                return new PolicyBase
                                                                                                       {
                                                                                                           policyID   = 2000,
                                                                                                           occupancy  = 1,
                                                                                                           policyType = PolicyType.Military,
                                                                                                           playerBuffs = new List<PlayerBuffBase>
                                                                                                                         {
                                                                                                                             new PlayerBuffBase(new List<UnityAction<BuffBase,BuffContainerBase>>
                                                                                                                                                {
                                                                                                                                                    (b, c) =>
                                                                                                                                                    {
                                                                                                                                                        var buff = b as PlayerBuffBase;
                                                                                                                                                        // Debug.Log(buff.activatePlayer);
                                                                                                                                                        ((PlayerBuffContainer)c).player.unitPrefabList.ForEach(u =>
                                                                                                                                                                                                               {
                                                                                                                                                                                                                   if (u.unitID == 2000)
                                                                                                                                                                                                                   {
                                                                                                                                                                                                                       var swordsman = u as Swordsman;
                                                                                                                                                                                                                       swordsman.SetNumericalValueBuff(BuffNumericalValueType.CostTime, false, .5f);
                                                                                                                                                                                                                   }
                                                                                                                                                                                                               });
                                                                                                                                                    }
                                                                                                                                                }, new List<UnityAction<BuffBase,BuffContainerBase>>
                                                                                                                                                   {
                                                                                                                                                       (b, c) =>
                                                                                                                                                       {
                                                                                                                                                           var buff = b as PlayerBuffBase;
                                                                                                                                                           // Debug.Log(buff.activatePlayer);
                                                                                                                                                           ((PlayerBuffContainer)c).player.unitPrefabList.ForEach(u =>
                                                                                                                                                                                                                  {
                                                                                                                                                                                                                      if (u.unitID == 2000)
                                                                                                                                                                                                                      {
                                                                                                                                                                                                                          var swordsman = u as Swordsman;
                                                                                                                                                                                                                          swordsman.SetNumericalValueBuff(BuffNumericalValueType.CostTime, false, 2f);
                                                                                                                                                                                                                      }
                                                                                                                                                                                                                  });
                                                                                                                                                       }
                                                                                                                                                   }, false)
                                                                                                                             {
                                                                                                                                 buffID           = 1000,
                                                                                                                                 isSuperimposable = false
                                                                                                                             }
                                                                                                                         }
                                                                                                       };
                                                                                            }
                                                                                  },
                                                                                  {
                                                                                      2001, () =>
                                                                                            {
                                                                                                return new PolicyBase
                                                                                                       {
                                                                                                           policyID   = 2001,
                                                                                                           occupancy  = 3,
                                                                                                           policyType = PolicyType.Military,
                                                                                                           playerBuffs = new List<PlayerBuffBase>
                                                                                                                         {
                                                                                                                             new PlayerBuffBase(new List<UnityAction<BuffBase,BuffContainerBase>>
                                                                                                                                                {
                                                                                                                                                    (b, c) =>
                                                                                                                                                    {
                                                                                                                                                        var buff = b as PlayerBuffBase;
                                                                                                                                                        // Debug.Log(buff.activatePlayer);
                                                                                                                                                        ((PlayerBuffContainer)c).player.unitPrefabList.ForEach(u =>
                                                                                                                                                                                                               {
                                                                                                                                                                                                                   if (u is ProducebleCombatUnitBase) ((ProducebleCombatUnitBase)u).SetNumericalValueBuff(BuffNumericalValueType.MaxReserveCount, true, 2);
                                                                                                                                                                                                               });
                                                                                                                                                    }
                                                                                                                                                }, new List<UnityAction<BuffBase,BuffContainerBase>>
                                                                                                                                                   {
                                                                                                                                                       (b, c) =>
                                                                                                                                                       {
                                                                                                                                                           var buff = b as PlayerBuffBase;
                                                                                                                                                           // Debug.Log(buff.activatePlayer);
                                                                                                                                                           ((PlayerBuffContainer)c).player.unitPrefabList.ForEach(u =>
                                                                                                                                                                                                                  {
                                                                                                                                                                                                                      if (u is ProducebleCombatUnitBase) ((ProducebleCombatUnitBase)u).SetNumericalValueBuff(BuffNumericalValueType.MaxReserveCount, true, -2);
                                                                                                                                                                                                                  });
                                                                                                                                                       }
                                                                                                                                                   }, false)
                                                                                                                             {
                                                                                                                                 buffID           = 1001,
                                                                                                                                 isSuperimposable = false
                                                                                                                             }
                                                                                                                         }
                                                                                                       };
                                                                                            }
                                                                                  }
                                                                              };

        public static PolicyBase GeneratePolicy(int policyID)
        {
            return _policyDict[policyID]();
        }
    }
}