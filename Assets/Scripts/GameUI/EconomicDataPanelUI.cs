using System;
using Gameplay;
using Gameplay.Player;
using UnityEngine;

namespace GameUI
{
    public class EconomicDataPanelUI : MonoBehaviour
    {
        public DataLineUI foodUI;
        public DataLineUI woodUI;
        public DataLineUI goldUI;
        public DataLineUI populationUI;

        private PlayerBase _player;

        private void Start()
        {
            _player = BattleGameManager.BattleGameManagerInstance.userPlayer;
        }

        private void FixedUpdate()
        {
            foodUI.content       = _player.Food.ToString();
            woodUI.content       = _player.Wood.ToString();
            goldUI.content       = _player.Gold.ToString();
            populationUI.content = _player.CurUnitCount + "/" + _player.maxBattleUnitCount;
        }
    }
}
