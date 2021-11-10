using Gameplay;
using Gameplay.Player;
using GameUI.UnitDispatchMenuUI;
using UnityEngine;
using UnityEngine.UI;


namespace GameUI
{
    public class WorkerPanelUI : MonoBehaviour
    {
        public  UnitDetailUI detailUI;
        public  Text         freeWorkerTextUI;
        public  Text         workerCountTextUI;
        public  Image        workerProduceBarUI;
        private PlayerBase   _player;

        private void Start()
        {
            _player = BattleGameManager.BattleGameManagerInstance.userPlayer;
            _player.onWorkerProduce.AddListener((() =>
                                                 {
                                                     workerProduceBarUI.fillAmount = _player.workerStatus.unitProduceProcess;
                                                 }));
        }

        private void FixedUpdate()
        {
            freeWorkerTextUI.text  = _player.workerStatus.freeUnitCount.ToString();
            workerCountTextUI.text = _player.workerStatus.totalUnitCount + " / " + _player.maxWorkerCount;
        }
        
        public void OnMouseHover()
        {
            detailUI.Show(this._player.workerPrefab);
        }

        public void OnMouseLeave()
        {
            detailUI.Close();
        }

    }
}
