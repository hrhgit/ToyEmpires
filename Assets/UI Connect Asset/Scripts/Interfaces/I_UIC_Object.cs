using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_UIC_Object
{
    string ID { get; }
    Color objectColor { get; set; }
    int Priority { get; }
    void Remove();
}
