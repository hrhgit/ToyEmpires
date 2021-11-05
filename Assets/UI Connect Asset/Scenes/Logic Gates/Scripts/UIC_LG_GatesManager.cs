using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// v1.4 - Logic Gates scene added
// this scene uses a really simple solve method just as inspiration on using the asset. It is possible to use a more complex solver and expand the project.

// gates manager class responsible for calling the solve method of each gate and manage connections 
public class UIC_LG_GatesManager : MonoBehaviour
{
    public UIC_Manager uicManager;

    void Start()
    {
        // add listener for pointer event
        uicManager.pointer.e_OnPointerUpFirst.AddListener(
            delegate
            {
                CheckAndUpdateNodeConnections();
            });
    }

    void Update()
    {
        foreach (UIC_Entity entity in uicManager.EntityList)
        {
            UIC_LG_Gate gate = entity.GetComponent<UIC_LG_Gate>();
            gate.Solve();
            gate.UpdateConnections();
        }
    }

    void CheckAndUpdateNodeConnections()
    {
        UIC_Node clickedNode = uicManager.GetClickedObjectOfType<UIC_Node>();

        if (clickedNode)
        {
            UIC_Node foundNode = clickedNode.lastFoundNode;

            if (foundNode)
            {
                UIC_Connection outConn = clickedNode.connectionsList.Count > 0 ? clickedNode.connectionsList[0] : null;
                UIC_Connection inConn = foundNode.connectionsList.Count > 0 ? foundNode.connectionsList[0] : null;

                if (outConn != null)
                {
                    outConn.Remove();
                }

                if (inConn != null)
                {
                    inConn.Remove();
                }
            }
        }
    }
}
