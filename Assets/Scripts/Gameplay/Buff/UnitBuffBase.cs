using System.Collections.Generic;
using Gameplay.GameUnit;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Buff
{
    public class UnitBuffBase : BuffBase
    {
        public GameUnitBase activateUnit;
        public UnitBuffBase() : base()
        {
            
        }
        public UnitBuffBase(List<UnityAction<BuffBase>> startEvent, List<UnityAction<BuffBase>> updateEvent, List<UnityAction<BuffBase>> stopEvent, float maxTime) : base(startEvent, updateEvent, stopEvent,maxTime)
        {
            
        }
        public UnitBuffBase(List<UnityAction<BuffBase>> startEvent, List<UnityAction<BuffBase>> stopEvent) : base(startEvent, stopEvent)
        {
            
        }
    }
}
