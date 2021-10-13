using System;
using Gameplay;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class WorkerDispatchUI : MonoBehaviour
    {
        public  Text              workerInPlaceUI;
        public  ResourceType      resourceType;
        private BattleGameManager _battleGameManager;
        private PlayerBase        _player;
        private Animator          _animator;
        private bool              _isShowingSubMenu = false;

        private void Start()
        {
            _battleGameManager = BattleGameManager.BattleGameManagerInstance;
            _player = _battleGameManager.userSide switch
                      {
                          Team.Blue => _battleGameManager.bluePlayer,
                          Team.Red  => _battleGameManager.redPlayer,
                          _         => throw new ArgumentOutOfRangeException()
                      };
            _animator = this.GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            workerInPlaceUI.text = _player.ResourceWorkerCount[(int) resourceType].ToString();
        }

        public void SwitchSubMenu()
        {
            if (_isShowingSubMenu)
            {
                _animator.SetTrigger("Close");
                _isShowingSubMenu = false;
            }
            else
            {
                _animator.SetTrigger("Show");
                _isShowingSubMenu = true;
            }
        }


        public void AddWorker()
        {
            _player.DispatchWorker((ResourceType)resourceType, true);
        }
        public void RemoveWorker()
        {
            _player.DispatchWorker((ResourceType)resourceType, false);
        }
    }
}
