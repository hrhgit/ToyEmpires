using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using Gameplay.Player;
using UnityEngine;

public class DispatchUnitUI : MonoBehaviour
{
    public int[,] unitIndexCountArr;

    private PlayerBase _player;

    private void Start()
    {
        _player = BattleGameManager.BattleGameManagerInstance.userPlayer;
    }
}
