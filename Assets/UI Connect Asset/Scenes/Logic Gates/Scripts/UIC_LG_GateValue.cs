using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIC_LG_GateValue : UIC_LG_Gate
{
    protected override void Awake()
    {
        base.Awake();
    }

    public bool value;

    public override void Solve()
    {
        outValue = value == true ? 1 : 0;
    }
}
