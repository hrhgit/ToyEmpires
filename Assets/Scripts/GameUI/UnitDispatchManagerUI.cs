using System;
using System.Collections.Generic;
using Gameplay;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.UI;


namespace GameUI
{
    public class UnitDispatchManagerUI : MonoBehaviour
    {
        public GameObject dispatchBtns;

        private PlayerBase         _player;
        private List<UnitButtonUI> unitButtonUiList = new List<UnitButtonUI>();

        private void Start()
        {
            _player = BattleGameManager.BattleGameManagerInstance.userPlayer;
            foreach (UnitButtonUI unitButtonUI in this.transform.GetComponentsInChildren<UnitButtonUI>())
            {
                unitButtonUiList.Add(unitButtonUI);
            }
        }

        public void ShowBtn(bool enableShow)
        {
            dispatchBtns.SetActive(enableShow);
        }
        

        public void Dispatch(int road)
        {
            foreach (UnitButtonUI unitButtonUI in unitButtonUiList)
            {
                if (unitButtonUI.FoodWoodBuyCount + unitButtonUI.GoldBuyCount == 0)
                    continue;
                _player.DispatchUnits(unitButtonUI.unitIndex,unitButtonUI.GoldBuyCount + unitButtonUI.FoodWoodBuyCount,(Road)road);
                unitButtonUI.Reset();
            }
            ShowBtn(false);
            
        }
    }
}
