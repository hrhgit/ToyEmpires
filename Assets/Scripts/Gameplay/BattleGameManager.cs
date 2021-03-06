using System;
using Gameplay.Civilization;
using Gameplay.Player;
using GameUI.PolicyUI;
using GameUI.TechTreeUI;
using Global;
using PathFindingPlus;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    public class BattleGameManager : MonoBehaviour
    {
        public static BattleGameManager BattleGameManagerInstance { get; private set; }

        // Players
        public PlayerBase        bluePlayer;
        public PlayerBase        redPlayer;
        public Team              userSide;
        public PlayerBase        userPlayer;
        public PlayerBase        userEnemyPlayer;
        public Transform         miscParent;
        public CustomPathFinding customPathFinding;
        public Transform         tempParent;

        public Camera          uiCamera;
        public PolicyManagerUI policyManagerUI;
        public TechTreePanelUI techTreePanelUI;

        public UnityEvent<bool> matchEndEvent = new UnityEvent<bool>();


        private void Awake()
        {
            BattleGameManagerInstance = this;

            userPlayer      = userSide is Team.Blue ? bluePlayer : userSide is Team.Red ? redPlayer  : throw new ArgumentOutOfRangeException();
            userEnemyPlayer = userSide is Team.Blue ? redPlayer  : userSide is Team.Red ? bluePlayer : throw new ArgumentOutOfRangeException();
            
            userPlayer.playerDieEvent.AddListener((p => matchEndEvent.Invoke(false)));
            userEnemyPlayer.playerDieEvent.AddListener((p => matchEndEvent.Invoke(true)));
            
            userPlayer.civilization = CivilizationGenerator.GenerateCivilization(GlobalGameManager.GlobalGameManagerInstance.playerCivilizationIdx);
            userEnemyPlayer.civilization = CivilizationGenerator.GenerateCivilization(GlobalGameManager.GlobalGameManagerInstance.enemyCivilizationIdx);
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
