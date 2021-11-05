using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_UIC_Clickable
{
    bool DisableClick { get; }
    void OnPointerDown();
    void OnPointerUp();
}
