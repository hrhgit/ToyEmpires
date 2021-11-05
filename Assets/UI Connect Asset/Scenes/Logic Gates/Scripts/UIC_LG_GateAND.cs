using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIC_LG_GateAND : UIC_LG_Gate
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void Solve()
    {
        GetInputs();

        int result;
        if (inputs.Count > 0)
            result = inputs[0];
        else
        {
            outValue = -1;
            return;
        }

        for (int i = 1; i < inputs.Count; i++)
        {
            if (inputs[i] == -1)
            {
                outValue = -1;
                return;
            }
            else
            {
                bool boolInput = inputs[i] == 1;
                bool boolResult = result == 1;
                boolResult = boolResult & boolInput;
                outValue = boolResult == true ? 1 : 0;
            }
        }
    }
}
