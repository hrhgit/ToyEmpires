using System;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.GameUnit
{
    public delegate void GameUnitEvent(GameUnitBase gameUnitBase, PlayerBase playerBase,UnitStatus status);
    public abstract class  GameUnitBase : MonoBehaviour
    {
        // 基本能力值
        public int   maxHp;
        public float maxSpeed;
        public int   defence;
        


        //基本属性
        public int  CurHp    { get; private set; }
        public Team UnitTeam { get; set; }
        public Road UnitRoad { get; set; }

        public  GameUnitMover UnitMover { get; private set; }

        private void Awake()
        {
            this.UnitMover = this.GetComponent<GameUnitMover>();
        }

        protected virtual void Start()
        {
            this.UnitMover.Target = UnitTeam switch
                                    {
                                        Team.Blue => BattleGameManager.BattleGameManagerInstance.redPlayer.home.position,
                                        Team.Red  => BattleGameManager.BattleGameManagerInstance.bluePlayer.home.position,
                                        _         => throw new ArgumentOutOfRangeException()
                                    };
        }

        #region 生产


        #endregion
    }
}
