using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIC_LG_GateNOT : UIC_LG_Gate
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void Solve()
    {
        GetInputs();

        if (inputs.Count > 0)
        {
            outValue = inputs[0] == 0 ? 1 : 0;
        }
        else
        {
            outValue = -1;
            return;
        }
    }
}
