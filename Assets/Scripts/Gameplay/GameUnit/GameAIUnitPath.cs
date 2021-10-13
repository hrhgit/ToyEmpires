using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;

public class GameAIUnitPath : AIPath
{
    public UnityEvent OnTargetReachedEvent = new UnityEvent();
    public override void OnTargetReached()
    {
        base.OnTargetReached();
        OnTargetReachedEvent.Invoke();
    }
}
