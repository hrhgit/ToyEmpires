using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// v1.3 - connect defined nodes on start of the project
public class SetupInitialConnections : MonoBehaviour
{
    public UIC_Manager uicManager;

    [SerializeField] public List<UIC_Node> fromNodes = new List<UIC_Node>();
    [SerializeField] public List<UIC_Node> toNodes = new List<UIC_Node>();
    public int size = 1;

    void Awake()
    {
        uicManager = GetComponentInParent<UIC_Manager>();
    }

    bool lateStart = true;
    public void Refresh()
    {
        for (int i = 0; i < fromNodes.Count; i++)
        {
            if (fromNodes[i] && toNodes[i])
            {
                fromNodes[i].ConnectTo(toNodes[i]);
            }
        }

        lateStart = false;
        //Destroy(gameObject);
    }

    void Update()
    {
        if (lateStart)
        {
            Refresh();
        }

        foreach (UIC_Connection conn in uicManager.ConnectionsList)
        {
            conn.UpdateLine();
        }
    }
}
