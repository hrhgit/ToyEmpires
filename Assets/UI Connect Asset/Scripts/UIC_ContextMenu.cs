using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIC_ContextMenu : MonoBehaviour
{
    static List<I_UIC_ContextItem> contextItemList;

    void Awake()
    {
        contextItemList = new List<I_UIC_ContextItem>();
        foreach (Transform t in transform)
        {
            I_UIC_ContextItem item = t.GetComponent<I_UIC_ContextItem>();
            if (item != null)
                contextItemList.Add(item);
        }
    }

    void Start()
    {
        UpdateContextMenu();
    }

    public static void UpdateContextMenu()
    {
        if (contextItemList != null)
        {
            foreach (I_UIC_ContextItem item in contextItemList)
            {
                item.OnChangeSelection();
            }
        }
    }
}
