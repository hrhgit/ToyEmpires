using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;

public class MainTest : MonoBehaviour
{
    void Start() {
        var dict = GlobalGameManager.GlobalGameManagerInstance.unitId2FileNameDict;
        foreach (var d in dict) {
            Debug.Log(d.Key + " : " + d.Value);
        }
    }


}
