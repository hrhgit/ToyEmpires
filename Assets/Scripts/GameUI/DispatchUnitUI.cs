using Gameplay;
using Gameplay.Player;
using UnityEngine;

namespace GameUI
{
    public class DispatchUnitUI : MonoBehaviour
    {
        public int[,] unitIndexCountArr;

        private PlayerBase _player;

        private void Start()
        {
            _player = BattleGameManager.BattleGameManagerInstance.userPlayer;
        }
    }
}
