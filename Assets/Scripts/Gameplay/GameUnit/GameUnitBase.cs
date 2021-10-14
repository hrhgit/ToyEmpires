using System;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.GameUnit
{
    public abstract class GameUnitBase : MonoBehaviour
    {
        // 基本能力值
        [SerializeField] private Team _unitTeam;
        [SerializeField] private Road _unitRoad;

        protected PlayerBase UnitSide  { get; private set; }
        protected PlayerBase EnemySide { get; private set; }

        public Team UnitTeam
        {
            get => _unitTeam;
            internal set => _unitTeam = value;
        }

        public Road UnitRoad
        {
            get => _unitRoad;
            internal set => _unitRoad = value;
        }

        protected void BaseInit()
        {
            switch (UnitTeam)
            {
                case Team.Blue:
                    this.UnitSide  = BattleGameManager.BattleGameManagerInstance.bluePlayer;
                    this.EnemySide = BattleGameManager.BattleGameManagerInstance.redPlayer;
                    break;
                case Team.Red:
                    this.EnemySide = BattleGameManager.BattleGameManagerInstance.bluePlayer;
                    this.UnitSide  = BattleGameManager.BattleGameManagerInstance.redPlayer;

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
        
        protected virtual void Start()
        {
            BaseInit();
        }


    }
}