using System.Collections.Generic;
using Gameplay.Buff;
using Gameplay.GameUnit.SoldierUnit;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.TechTree
{
    public delegate Technology GenerateTechFunc();

    public static class TechGenerator
    {
        private static readonly Dictionary<int, GenerateTechFunc> _techDict = new Dictionary<int, GenerateTechFunc>
                                                                              {
                                                                                  {
                                                                                      0, () =>
                                                                                         {
                                                                                             return new Technology(0, 10, 0, 5, 4, 0,2)
                                                                                                    {
                                                                                                        playerBuffs = new List<PlayerBuffBase>
                                                                                                                                        {
                                                                                                                                            new PlayerBuffBase(new List<UnityAction<BuffBase,BuffContainerBase>>
                                                                                                                                                               {
                                                                                                                                                                   (b, c) =>
                                                                                                                                                                   {
                                                                                                                                                                       var buff = b as PlayerBuffBase;
                                                                                                                                                                       ((PlayerBuffContainer)c).player.unitPrefabList.ForEach(u =>
                                                                                                                                                                                                                               {
                                                                                                                                                                                                                                   u.SetNumericalValueBuff(BuffNumericalValueType.Defence, true,
                                                                                                                                                                                                                                                           3);
                                                                                                                                                                                                                               });
                                                                                                                                                                   }
                                                                                                                                                               }, new List<UnityAction<BuffBase,BuffContainerBase>>
                                                                                                                                                                  {
                                                                                                                                                                      (b, c) =>
                                                                                                                                                                      {
                                                                                                                                                                          var buff = b as PlayerBuffBase;
                                                                                                                                                                          ((PlayerBuffContainer)c).player.unitPrefabList.ForEach(u => { u.SetNumericalValueBuff(BuffNumericalValueType.Defence, true, -3); });
                                                                                                                                                                      }
                                                                                                                                                                  }, false) { buffID = 1001, isSuperimposable = false }
                                                                                                                                        },
                                                                                                        unitBuffs = null
                                                                                                    };
                                                                                         }
                                                                                  },
                                                                                  {
                                                                                      1, () =>
                                                                                         {
                                                                                             return new Technology(1, 10, 0, 5, 4, 0, 2)
                                                                                                    {
                                                                                                        playerBuffs = new List<PlayerBuffBase>
                                                                                                                                        {
                                                                                                                                            new PlayerBuffBase(new List<UnityAction<BuffBase,BuffContainerBase>>
                                                                                                                                                               {
                                                                                                                                                                   (b, c) =>
                                                                                                                                                                   {
                                                                                                                                                                       var buff = b as PlayerBuffBase;
                                                                                                                                                                       ((PlayerBuffContainer)c).player.InstanceWorkersList.ForEach(worker =>
                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                        worker.SetNumericalValueBuff(BuffNumericalValueType.MaxLoadFood, true, 2);
                                                                                                                                                                                                                                        worker.SetNumericalValueBuff(BuffNumericalValueType.MaxLoadWood, true, 2);
                                                                                                                                                                                                                                        worker.SetNumericalValueBuff(BuffNumericalValueType.MaxLoadGold, true,
                                                                                                                                                                                                                                                                     2);
                                                                                                                                                                                                                                    });
                                                                                                                                                                   }
                                                                                                                                                               }, new List<UnityAction<BuffBase,BuffContainerBase>>
                                                                                                                                                                  {
                                                                                                                                                                      (b, c) =>
                                                                                                                                                                      {
                                                                                                                                                                          var buff = b as PlayerBuffBase;
                                                                                                                                                                          ((PlayerBuffContainer)c).player.InstanceWorkersList.ForEach(worker =>
                                                                                                                                                                                                                                      {
                                                                                                                                                                                                                                          worker.SetNumericalValueBuff(BuffNumericalValueType.MaxLoadFood, true, -2);
                                                                                                                                                                                                                                          worker.SetNumericalValueBuff(BuffNumericalValueType.MaxLoadWood, true, -2);
                                                                                                                                                                                                                                          worker.SetNumericalValueBuff(BuffNumericalValueType.MaxLoadGold, true,
                                                                                                                                                                                                                                                                       -2);
                                                                                                                                                                                                                                      });
                                                                                                                                                                      }
                                                                                                                                                                  }, false)
                                                                                                                                            {
                                                                                                                                                buffID           = 1003,
                                                                                                                                                isSuperimposable = true
                                                                                                                                            }
                                                                                                                                        },
                                                                                                        unitBuffs = null
                                                                                                    };
                                                                                         }
                                                                                  },
                                                                                  {
                                                                                      2, () =>
                                                                                         {
                                                                                             return new Technology(2, 10, 0, 5, 4, 0, 2)
                                                                                                    {
                                                                                                        playerBuffs = new List<PlayerBuffBase>
                                                                                                                      {
                                                                                                                          new PlayerBuffBase(new List<UnityAction<BuffBase,BuffContainerBase>>
                                                                                                                                             {
                                                                                                                                                 (b,c) =>
                                                                                                                                                 {
                                                                                                                                                     var buff = b as PlayerBuffBase;
                                                                                                                                                     ((PlayerBuffContainer)c).player.unitPrefabList.ForEach(u =>
                                                                                                                                                                                                {
                                                                                                                                                                                                    u.SetNumericalValueBuff(BuffNumericalValueType.Productivity,
                                                                                                                                                                                                                            true,
                                                                                                                                                                                                                            1);
                                                                                                                                                                                                });
                                                                                                                                                 }
                                                                                                                                             }, new List<UnityAction<BuffBase,BuffContainerBase>>
                                                                                                                                                {
                                                                                                                                                    (b, c) =>
                                                                                                                                                    {
                                                                                                                                                        var buff = b as PlayerBuffBase;
                                                                                                                                                        ((PlayerBuffContainer)c).player.unitPrefabList.ForEach(u =>
                                                                                                                                                                                                               {
                                                                                                                                                                                                                   u.SetNumericalValueBuff(BuffNumericalValueType.Productivity,
                                                                                                                                                                                                                                           true,
                                                                                                                                                                                                                                           -1);
                                                                                                                                                                                                               });
                                                                                                                                                    }
                                                                                                                                                }, false)
                                                                                                                          {
                                                                                                                              buffID           = 1004,
                                                                                                                              isSuperimposable = true
                                                                                                                          }
                                                                                                                      },
                                                                                                        unitBuffs = null
                                                                                                    };
                                                                                         }
                                                                                  },
                                                                                  {
                                                                                      3, () =>
                                                                                         {
                                                                                             return new Technology(3, 20, 0, 10, 8, 0, 4,10)
                                                                                                    {
                                                                                                        unitBuffs = new List<UnitBuffBase>
                                                                                                                      {
                                                                                                                          new UnitBuffBase(new List<UnityAction<BuffBase,BuffContainerBase>>
                                                                                                                                           {
                                                                                                                                               (b, c) =>
                                                                                                                                               {
                                                                                                                                                   var buff = b as UnitBuffBase;
                                                                                                                                                   if (((UnitBuffContainer)c).unit is SoldierUnitBase)
                                                                                                                                                   {
                                                                                                                                                       ((SoldierUnitBase)((UnitBuffContainer)c).unit).SetNumericalValueBuff(BuffNumericalValueType.MaintenanceTime,
                                                                                                                                                                                                                            false,
                                                                                                                                                                                                                            2);

                                                                                                                                                   }
                                                                                                                                               }
                                                                                                                                           }, new List<UnityAction<BuffBase,BuffContainerBase>>
                                                                                                                                              {
                                                                                                                                                  (b,c) =>
                                                                                                                                                  {
                                                                                                                                                      var buff = b as UnitBuffBase;
                                                                                                                                                      if (((UnitBuffContainer)c).unit is SoldierUnitBase)
                                                                                                                                                      {
                                                                                                                                                          ((SoldierUnitBase)((UnitBuffContainer)c).unit).SetNumericalValueBuff(BuffNumericalValueType.MaintenanceTime,
                                                                                                                                                                                                                               false,
                                                                                                                                                                                                                               .5f);

                                                                                                                                                      }
                                                                                                                                                  }
                                                                                                                                              }, false)
                                                                                                                          {
                                                                                                                              buffID           = 1005,
                                                                                                                              isSuperimposable = true
                                                                                                                          }
                                                                                                                      },
                                                                                                        playerBuffs = null,
                                                                                                    };
                                                                                         }
                                                                                  }
                                                                              };

        public static Technology GenerateTechnology(int techID)
        {
            return _techDict[techID]();
        }
    }
}