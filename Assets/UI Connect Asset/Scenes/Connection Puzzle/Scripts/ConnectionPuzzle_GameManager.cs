using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPuzzle_GameManager : MonoBehaviour
{
    public UIC_Manager uicManager;

    void Start()
    {
        // set the global line type
        uicManager.globalLineType = UIC_Connection.LineTypeEnum.Line;
        // disable click on connections
        uicManager.disableConnectionClick = true;

        // add listeners for pointer events
        uicManager.pointer.e_OnDragLast.AddListener(
            delegate
            {
                PerformVirtualClickOnFoundNode();
            });

        uicManager.pointer.e_OnPointerDownLast.AddListener(
            delegate
            {
                CheckAndUpdateCrossedConnections();
            });
    }

    List<UIC_Connection> alreadyCheckedConnection = new List<UIC_Connection>();
    private void CheckAndUpdateCrossedConnections()
    {
        UIC_Node clickedNode = uicManager.GetClickedObjectOfType<UIC_Node>();

        if (clickedNode)
        {
            alreadyCheckedConnection.Clear();

            // check every connection for a crossing, excluding the ones that were checked previously
            foreach (UIC_Connection conn in clickedNode.connectionsList)
            {
                if (!alreadyCheckedConnection.Contains(conn))
                {
                    alreadyCheckedConnection.Add(conn);
                    bool red = false;

                    // get connections that are crossed by conn and set their color red if exist
                    List<UIC_Connection> cross = conn.GetCrossedConnections();
                    if (cross.Count > 0)
                    {
                        alreadyCheckedConnection.AddRange(cross);
                        foreach (UIC_Connection c in cross)
                        {
                            c.line.color = Color.red;
                        }
                        red = true;
                    }

                    // alse check if the conn crosses any entity 
                    foreach (UIC_Entity entity in uicManager.EntityList)
                    {
                        if ((entity != conn.node0.entity && entity != conn.node1.entity) && UIC_Utility.ConnectionIntersectRect(conn, entity.GetComponent<RectTransform>()))
                        {
                            red = true;
                            break;
                        }
                    }

                    if (red)
                    {
                        conn.line.color = Color.red;
                    }
                    else
                    {
                        conn.line.color = conn.line.defaultColor;
                    }
                }
            }
        }
    }

    void PerformVirtualClickOnFoundNode()
    {
        UIC_Node clickedNode = uicManager.GetClickedObjectOfType<UIC_Node>();

        // check if clicked object is a node to perform virtual click
        if (clickedNode)
        {
            // get last node detected under the pointer
            UIC_Node foundNode = clickedNode.lastFoundNode;

            if (foundNode != null)
            {
                // if first clicked node is != than detected one, perform new simulated click
                if (foundNode != clickedNode)
                {
                    uicManager.pointer.OnPointerUp();
                    uicManager.pointer.OnPointerDown();
                }
            }
        }
        else
        {
            // call pointer down to mantain the click action
            uicManager.pointer.OnPointerDown();
        }
    }
}
