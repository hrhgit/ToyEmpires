using System.Collections;
using System.Collections.Generic;
using Gameplay;
using Gameplay.GameUnit;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    void Start()
    {
        Debug.Log(ResourceType.Wood.ToRoad());
    }
}
