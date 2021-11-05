using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeCycles_GameManager : MonoBehaviour
{
    public UIC_Manager uicManager;

    int lastConnCount;

    void Start()
    {
        foreach (UIC_Entity entity in uicManager.EntityList)
        {
            entity.DisableClick = true;
        }

        lastConnCount = uicManager.ConnectionsList != null ? uicManager.ConnectionsList.Count : 0;

        // set pointer detection range for nodes
        //uicManager.MaxPointerDetectionDistance = 130;
        // set the line type
        uicManager.globalLineType = UIC_Connection.LineTypeEnum.Line;

        // add listener for pointer event
        uicManager.pointer.e_OnPointerUpFirst.AddListener(
            delegate
            {
                CheckAndUpdateNodeConnections();
            });
    }

    private void CheckAndUpdateNodeConnections()
    {
        UIC_Node clickedNode = uicManager.GetClickedObjectOfType<UIC_Node>();

        if (clickedNode)
        {
            UIC_Node foundNode = clickedNode.lastFoundNode;

            if (foundNode)
            {
                UIC_Connection outConn = clickedNode.GetOutConnections().Count > 0 ? clickedNode.GetOutConnections()[0] : null;
                UIC_Connection inConn = foundNode.GetInConnections().Count > 0 ? foundNode.GetInConnections()[0] : null;

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

    void Update()
    {
        if (lastConnCount != uicManager.ConnectionsList.Count)
        {
            lastConnCount = uicManager.ConnectionsList.Count;
        }
    }
}
