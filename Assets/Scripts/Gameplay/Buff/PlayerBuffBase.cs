using System.Collections.Generic;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Buff
{
    public class PlayerBuffBase : BuffBase
    {
        public PlayerBuffBase() : base()
        {
            
        }
        
        public PlayerBuffBase(List<UnityAction<BuffBase,BuffContainerBase>> startEvent, List<UnityAction<BuffBase,BuffContainerBase>> updateEvent, List<UnityAction<BuffBase,BuffContainerBase>> stopEvent, float maxTime) : base(startEvent, updateEvent, stopEvent,maxTime) 
        {
            
        }
        public PlayerBuffBase(List<UnityAction<BuffBase,BuffContainerBase>> startEvent, List<UnityAction<BuffBase,BuffContainerBase>> stopEvent, bool isOneOff) : base(startEvent, stopEvent,isOneOff)
        {
            
        }

    }
}
