using System.Collections.Generic;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Buff
{
    public class PlayerBuffBase : BuffBase
    {
        public PlayerBase activatePlayer;
        public PlayerBuffBase() : base()
        {
            
        }
        
        public PlayerBuffBase(List<UnityAction<BuffBase>> startEvent, List<UnityAction<BuffBase>> updateEvent, List<UnityAction<BuffBase>> stopEvent, float maxTime) : base(startEvent, updateEvent, stopEvent,maxTime) 
        {
            
        }
        public PlayerBuffBase(List<UnityAction<BuffBase>> startEvent, List<UnityAction<BuffBase>> stopEvent) : base(startEvent, stopEvent)
        {
            
        }

    }
}
