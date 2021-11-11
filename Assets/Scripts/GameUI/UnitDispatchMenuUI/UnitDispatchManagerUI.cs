using System;
using System.Collections.Generic;
using Gameplay;
using Gameplay.GameUnit.SoldierUnit;
using Gameplay.Player;
using UnityEngine;

namespace GameUI.UnitDispatchMenuUI
{
    public class UnitDispatchManagerUI : MonoBehaviour
    {
        public GameObject   dispatchBtns;
        
        public Transform unitLine;
        public UnitButtonUI buttonPrefab;
        
        public UnitDetailUI unitDetailUI;

        private PlayerBase         _player;
        private List<UnitButtonUI> unitButtonUiList = new List<UnitButtonUI>();
        

        private void Start()
        {
            _player = BattleGameManager.BattleGameManagerInstance.userPlayer;
            for (var i = 0; i < _player.unitPrefabList.Count; i++) {
                SoldierUnitBase unitBase = _player.unitPrefabList[i];
                UnitButtonUI uiInstance = Instantiate(buttonPrefab, unitLine);
                uiInstance.unitIndex = i;
                RectTransform rect = uiInstance.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(i*rect.sizeDelta.x, 0);
                unitButtonUiList.Add(uiInstance);
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
