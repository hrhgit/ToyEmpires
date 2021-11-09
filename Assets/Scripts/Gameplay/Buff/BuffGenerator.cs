using System.Collections.Generic;
using Gameplay.GameUnit.SoldierUnit;
using UnityEngine.Events;

namespace Gameplay.Buff
{
    public delegate BuffBase GenerateBuffFunc();

    public static class BuffGenerator
    {
        /// <summary>
        ///     ID: 六位000000
        ///     首位：
        ///     0 : 未定
        ///     1 ： 玩家Buff
        ///     2 ： 单位buff
        ///     次位：
        ///     0：民族buff
        ///     1：政策buff
        ///     2：科技buff
        ///     3：单位技能buff
        ///     4：平常buff
        ///     240000 : 饥饿
        /// </summary>
        private static readonly Dictionary<int, GenerateBuffFunc> _buffDict = new Dictionary<int, GenerateBuffFunc>
                                                                              {
                                                                                  {
                                                                                      240000, () => new UnitBuffBase(new List<UnityAction<BuffBase,BuffContainerBase>>
                                                                                                                     {
                                                                                                                         (b,c) =>
                                                                                                                         {
                                                                                                                             var buff = b as UnitBuffBase;
                                                                                                                             ((SoldierUnitBase)((UnitBuffContainer)c).unit).SetNumericalValueBuff(BuffNumericalValueType.Attack, false, .5f);
                                                                                                                         }
                                                                                                                     },
                                                                                                                     new List<UnityAction<BuffBase,BuffContainerBase>>
                                                                                                                     {
                                                                                                                         (b, c) =>
                                                                                                                         {
                                                                                                                             var buff = b as UnitBuffBase;
                                                                                                                             ((SoldierUnitBase)((UnitBuffContainer)c).unit).SetNumericalValueBuff(BuffNumericalValueType.Attack, false, 2f);
                                                                                                                         }
                                                                                                                     },
                                                                                                                     false)
                                                                                                    {
                                                                                                        buffID = 240000,
                                                                                                        isSuperimposable =
                                                                                                            false,
                                                                                                    }
                                                                                  }
                                                                              };

        public static BuffBase GenerateBuff(int buffID)
        {
            return _buffDict[buffID]();
        }
    }
}