using Pathfinding;
using UnityEngine.Events;

namespace Gameplay.GameUnit
{
    public class GameAIUnitPath : AIPath
    {
        public             UnityEvent OnTargetReachedEvent = new UnityEvent();

        public override void  OnTargetReached()
        {
            base.OnTargetReached();
            OnTargetReachedEvent.Invoke();
        }
    }
}
