using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;

public class GameAIUnitPath : RichAI
{
    public             UnityEvent OnTargetReachedEvent = new UnityEvent();
    protected override void  OnTargetReached()
    {
        base.OnTargetReached();
        OnTargetReachedEvent.Invoke();
    }
}
