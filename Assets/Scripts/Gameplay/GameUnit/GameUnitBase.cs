using System;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.GameUnit
{
    public abstract class GameUnitBase : MonoBehaviour
    {
        public int unitID;
        // 基本能力值
        [SerializeField] private Team  _unitTeam;
        [SerializeField] private Road  _unitRoad;
        protected                  float _unitValue = float.MinValue;

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

        public virtual float UnitValue
        {
            get
            {
                if (_unitValue <= 0)
                {
                    _unitValue = 1;
                }
                return _unitValue;
            }
        }

        protected void BaseInit()
        {
            switch (UnitTeam)
            {
                case Team.Blue:
                    this.UnitSide         = BattleGameManager.BattleGameManagerInstance.bluePlayer;
                    this.EnemySide        = BattleGameManager.BattleGameManagerInstance.redPlayer;
                    this.gameObject.layer = LayerMask.NameToLayer("BlueUnit");
                    break;
                case Team.Red:
                    this.EnemySide        = BattleGameManager.BattleGameManagerInstance.bluePlayer;
                    this.UnitSide         = BattleGameManager.BattleGameManagerInstance.redPlayer;
                    this.gameObject.layer = LayerMask.NameToLayer("RedUnit");
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