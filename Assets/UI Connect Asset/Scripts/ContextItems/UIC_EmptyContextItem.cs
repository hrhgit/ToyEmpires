using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIC_EmptyContextItem : MonoBehaviour, I_UIC_ContextItem, I_UIC_Object
{
    public string ID => "empty";
    public Color objectColor { get; set; }
    public int priority;
    public int Priority => priority;

    public void Action()
    {

    }

    public void OnChangeSelection()
    {

    }

    public void Remove()
    {

    }
}
