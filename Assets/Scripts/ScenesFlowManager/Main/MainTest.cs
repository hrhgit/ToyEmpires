using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;

public class MainTest : MonoBehaviour
{
    void Start() {
        GlobalGameManager.GlobalGameManagerInstance.Load("BattleScene0");
    }


}
