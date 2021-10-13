using System;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay
{
    public class BattleGameManager : MonoBehaviour
    {
        public static BattleGameManager BattleGameManagerInstance { get; private set; }
        
        // Players
        public PlayerBase bluePlayer;
        public PlayerBase redPlayer;
        public Team       userSide;
        public PlayerBase userPlayer;
        public PlayerBase userEnemyPlayer;
        

        private void Awake()
        {
            BattleGameManagerInstance = this;

            userPlayer      = userSide is Team.Blue ? bluePlayer : userSide is Team.Red ? redPlayer  : throw new ArgumentOutOfRangeException();
            userEnemyPlayer = userSide is Team.Blue ? redPlayer  : userSide is Team.Red ? bluePlayer : throw new ArgumentOutOfRangeException();
        }

        private void FixedUpdate()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                userPlayer.AddResource(ResourceType.Food,1);
                userPlayer.AddResource(ResourceType.Wood,1);
                userPlayer.AddResource(ResourceType.Gold,1);
            }
        }
    }
}
