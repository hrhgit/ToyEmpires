using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_UIC_Draggable
{
    bool EnableDrag { get; set; }
    void OnDrag();
}
