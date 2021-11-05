using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_UIC_Selectable
{
    Vector3[] Handles { get; set; }
    void Select();
    void Unselect();
    void Remove();
}
