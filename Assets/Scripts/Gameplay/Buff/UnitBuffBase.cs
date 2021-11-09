using System.Collections.Generic;
using Gameplay.GameUnit;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Buff
{
    public class UnitBuffBase : BuffBase
    {
        public UnitBuffBase() : base()
        {
            
        }
        public UnitBuffBase(List<UnityAction<BuffBase,BuffContainerBase>> startEvent, List<UnityAction<BuffBase,BuffContainerBase>> updateEvent, List<UnityAction<BuffBase,BuffContainerBase>> stopEvent, float maxTime) : base(startEvent, updateEvent, stopEvent,maxTime)
        {
            
        }
        public UnitBuffBase(List<UnityAction<BuffBase,BuffContainerBase>> startEvent, List<UnityAction<BuffBase,BuffContainerBase>> stopEvent, bool isOneOff) : base(startEvent, stopEvent, isOneOff)
        {
            
        }
    }
}
