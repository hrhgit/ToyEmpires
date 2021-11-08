using System.Collections.Generic;
using Gameplay.Buff;
using Gameplay.GameUnit;
using Gameplay.GameUnit.SoldierUnit;
using UnityEngine.Events;

namespace Gameplay.TechTree
{
    public delegate Technology GenerateTech();
    public static class TechGenerator
    {
        private static readonly Dictionary<int, GenerateTech> _techDict = new Dictionary<int, GenerateTech>
                                                                            {
                                                                                {0,(() =>
                                                                                            {
                                                                                                return
                                                                                                    new
                                                                                                    Technology(5, 5,
                                                                                                        3, 5,
                                                                                                        2, 1,
                                                                                                        2)
                                                                                                    {
                                                                                                        technologyID
                                                                                                            = 0,
                                                                                                        playerBuffs
                                                                                                            = new
                                                                                                                List
                                                                                                                <PlayerBuffBase
                                                                                                                >
                                                                                                                {
                                                                                                                    new
                                                                                                                    PlayerBuffBase(new
                                                                                                                            List
                                                                                                                            <UnityAction
                                                                                                                                <BuffBase
                                                                                                                                >>
                                                                                                                            {
                                                                                                                                b =>
                                                                                                                                {
                                                                                                                                    var
                                                                                                                                        buff
                                                                                                                                            = b
                                                                                                                                                as
                                                                                                                                                PlayerBuffBase;
                                                                                                                                    buff
                                                                                                                                       .activatePlayer
                                                                                                                                       .unitPrefabList
                                                                                                                                       .ForEach(u =>
                                                                                                                                        {
                                                                                                                                            u.SetNumericalValueBuff(BuffNumericalValueType.Defence,
                                                                                                                                                true,
                                                                                                                                                3);
                                                                                                                                        });
                                                                                                                                }
                                                                                                                            },
                                                                                                                        new
                                                                                                                        List
                                                                                                                        <UnityAction
                                                                                                                            <BuffBase
                                                                                                                            >>
                                                                                                                        {
                                                                                                                            b =>
                                                                                                                            {
                                                                                                                                var
                                                                                                                                    buff
                                                                                                                                        = b
                                                                                                                                            as
                                                                                                                                            PlayerBuffBase;
                                                                                                                                buff
                                                                                                                                   .activatePlayer
                                                                                                                                   .unitPrefabList
                                                                                                                                   .ForEach(u =>
                                                                                                                                    {
                                                                                                                                        u.SetNumericalValueBuff(BuffNumericalValueType.Defence,
                                                                                                                                            true,
                                                                                                                                            -3);
                                                                                                                                    });
                                                                                                                            }
                                                                                                                        },
                                                                                                                        false)
                                                                                                                    {
                                                                                                                        buffID
                                                                                                                            = 1001,
                                                                                                                        isSuperimposable
                                                                                                                            = false
                                                                                                                    }
                                                                                                                },
                                                                                                        unitBuffs =
                                                                                                            null
                                                                                                    };
                                                                                            })},
                                                                                {1,(() =>
                                                                                    {
                                                                                        return new Technology(5,5,3,5,2,1,2)
                                                                                               {
                                                                                                   technologyID = 1,
                                                                                                   playerBuffs  = new List<PlayerBuffBase>()
                                                                                                       {
                                                                                                           new PlayerBuffBase(new List<UnityAction<BuffBase>>()
                                                                                                               {
                                                                                                                   (b =>
                                                                                                                       {
                                                                                                                           var buff = b as PlayerBuffBase;
                                                                                                                           buff.activatePlayer.InstanceWorkersList.ForEach((worker =>
                                                                                                                                       {
                                                                                                                                           worker
                                                                                                                                              .SetNumericalValueBuff(BuffNumericalValueType.MaxLoadFood,
                                                                                                                                                   true,
                                                                                                                                                   2);
                                                                                                                                           worker
                                                                                                                                              .SetNumericalValueBuff(BuffNumericalValueType.MaxLoadWood,
                                                                                                                                                   true,
                                                                                                                                                   2);
                                                                                                                                           worker
                                                                                                                                              .SetNumericalValueBuff(BuffNumericalValueType.MaxLoadGold,
                                                                                                                                                   true,
                                                                                                                                                   2);
                                                                                                                                       }));
                                                                                                                       })
                                                                                                               },new List<UnityAction<BuffBase>>()
                                                                                                               {
                                                                                                                   (b =>
                                                                                                                       {
                                                                                                                           var buff = b as PlayerBuffBase;
                                                                                                                           buff.activatePlayer.InstanceWorkersList.ForEach((worker =>
                                                                                                                                       {
                                                                                                                                           worker
                                                                                                                                              .SetNumericalValueBuff(BuffNumericalValueType.MaxLoadFood,
                                                                                                                                                   true,
                                                                                                                                                   -2);
                                                                                                                                           worker
                                                                                                                                              .SetNumericalValueBuff(BuffNumericalValueType.MaxLoadWood,
                                                                                                                                                   true,
                                                                                                                                                   -2);
                                                                                                                                           worker
                                                                                                                                              .SetNumericalValueBuff(BuffNumericalValueType.MaxLoadGold,
                                                                                                                                                   true,
                                                                                                                                                   -2);
                                                                                                                                       }));
                                                                                                                       })
                                                                                                               },false )
                                                                                                           {
                                                                                                               buffID = 1003,
                                                                                                               isSuperimposable = true,
                                                                                                           }
                                                                                                       },
                                                                                                   unitBuffs    = null,
                                                                                               };
                                                                                    })},
                                                                                {2,(() =>
                                                                                    {
                                                                                        return new Technology(5,5,3,5,2,1,2)
                                                                                               {
                                                                                                   technologyID = 2,
                                                                                                   playerBuffs  = new List<PlayerBuffBase>()
                                                                                                       {
                                                                                                           new PlayerBuffBase(new List<UnityAction<BuffBase>>()
                                                                                                               {
                                                                                                                   (b =>
                                                                                                                       {
                                                                                                                           var buff = b as PlayerBuffBase;
                                                                                                                           buff.activatePlayer.unitPrefabList.ForEach((u =>
                                                                                                                                       {
                                                                                                                                           u.SetNumericalValueBuff(BuffNumericalValueType.Productivity,
                                                                                                                                               true,
                                                                                                                                               1);
                                                                                                                                       }));
                                                                                                                       })
                                                                                                               },new List<UnityAction<BuffBase>>()
                                                                                                               {
                                                                                                                   (b =>
                                                                                                                       {
                                                                                                                           var buff = b as PlayerBuffBase;
                                                                                                                           buff.activatePlayer.unitPrefabList.ForEach((u =>
                                                                                                                                       {
                                                                                                                                           u.SetNumericalValueBuff(BuffNumericalValueType.Productivity,
                                                                                                                                               true,
                                                                                                                                               -1);
                                                                                                                                       }));
                                                                                                                       })
                                                                                                               },false )
                                                                                                           {
                                                                                                               buffID = 1004,
                                                                                                               isSuperimposable = true,
                                                                                                           }
                                                                                                       },
                                                                                                   unitBuffs    = null,
                                                                                               };
                                                                                    })},
                                                                            };

        public static Technology GenerateTechnology(int techID)
        {
            return _techDict[techID]();
        }
    }
}