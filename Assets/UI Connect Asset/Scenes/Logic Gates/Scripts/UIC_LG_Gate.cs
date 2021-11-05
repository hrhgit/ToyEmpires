using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// defines a logic gate, it is inherited by each specific gate and has the methods for get the input values and calculate output
// each gate overrides the solve method with its own logic
public class UIC_LG_Gate : MonoBehaviour
{
    public UIC_Entity entity;
    [HideInInspector]
    public int outValue; // 0(low), 1(high), -1(undefined)

    protected virtual void Awake()
    {
        entity = GetComponent<UIC_Entity>();
    }

    // update connection with the correspondent animation
    public virtual void UpdateConnections()
    {
        if (entity.nodeList.Count - 1 == entity.GetEntitiesConnectedToPolarity(UIC_Node.PolarityTypeEnum._in).Count && !inputs.Contains(-1))
        {
            if (outValue == 1)
            {
                foreach (UIC_Node node in entity.nodeList)
                {
                    if (node.polarityType == UIC_Node.PolarityTypeEnum._out)
                    {
                        foreach (UIC_Connection conn in node.connectionsList)
                        {
                            conn.line.animation.color = Color.red;
                            conn.line.animation.DrawType = UIC_Line.CapTypeEnum.Triangle;
                            conn.line.animation.isActive = true;
                        }
                    }
                }
            }
            else
            {
                foreach (UIC_Node node in entity.nodeList)
                {
                    if (node.polarityType == UIC_Node.PolarityTypeEnum._out)
                    {
                        foreach (UIC_Connection conn in node.connectionsList)
                        {
                            conn.line.animation.color = Color.white;
                            conn.line.animation.DrawType = UIC_Line.CapTypeEnum.Circle;
                            conn.line.animation.isActive = true;
                        }
                    }
                }
            }
        }
        else
        {
            foreach (UIC_Node node in entity.nodeList)
            {
                if (node.polarityType == UIC_Node.PolarityTypeEnum._out)
                {
                    foreach (UIC_Connection conn in node.connectionsList)
                    {
                        conn.line.animation.isActive = false;
                    }
                }
            }
        }
    }

    public List<int> inputs;
    
    public virtual void GetInputs()
    {
        inputs = new List<int>();

        foreach (UIC_Entity inEntity in entity.GetEntitiesConnectedToPolarity(UIC_Node.PolarityTypeEnum._in))
        {
            UIC_LG_Gate inGate = inEntity.GetComponent<UIC_LG_Gate>();

            inputs.Add(inGate.outValue);
        }
    }

    // ex AND
    public virtual void Solve()
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
