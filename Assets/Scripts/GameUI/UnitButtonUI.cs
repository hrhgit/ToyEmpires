using System;
using Gameplay;
using Gameplay.GameUnit;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.UI;


namespace GameUI
{
    public class UnitButtonUI : MonoBehaviour
    {
        public  int                   unitIndex;
        public  Text                  freeUnitTextUI;
        public  Text                  unitCountTextUI;
        public  Text                  unitFoodWoodCostTextUI;
        public  Text                  unitGoldCostTextUI;
        public  GameObject            selectedUI;
        public  Text                  selectedTextUI;
        public  Image                 unitProduceBarUI;
        
        private UnitDispatchManagerUI _unitDispatchManagerUI;
        private PlayerBase            _player;
        private UnitStatus            _status;

        private void Start()
        {
            _unitDispatchManagerUI = this.transform.parent.GetComponent<UnitDispatchManagerUI>();
            _player               = BattleGameManager.BattleGameManagerInstance.userPlayer;
            _player.onUnitProduceEventList[unitIndex].AddListener((unit, player, status) =>
                                                         {
                                                             this.unitProduceBarUI.fillAmount = status.unitProduceProcess;
                                                         });
            _status                     = _player.UnitStatusList[unitIndex];
            unitFoodWoodCostTextUI.text = ((IProduceable)_player.unitPrefabList[unitIndex]).CostFood + "食+" + ((IProduceable)_player.unitPrefabList[unitIndex]).CostWood + "木";
                unitGoldCostTextUI.text = ((IProduceable)_player.unitPrefabList[unitIndex]).CostGold + "金";
        }
        

        private void FixedUpdate()
        {
            try
            {
                freeUnitTextUI.text  = _player.UnitStatusList[unitIndex].freeUnitCount.ToString();
                unitCountTextUI.text = _player.UnitStatusList[unitIndex].curUnitCount.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public int FoodWoodBuyCount { get; private set; } = 0;
        public int GoldBuyCount { get; private set; } = 0;

        public void BuyUnit(bool useGold)
        {
            IProduceable unitProduceable = (IProduceable)_player.unitPrefabList[unitIndex];

            if(_status.freeUnitCount <=0 || _player.CurUnitPopulation + (this.GoldBuyCount + this.FoodWoodBuyCount) *unitProduceable.CostPopulation >= _player.maxBattleUnitCount)
                return;
            if (useGold)
            {
                if (_player.CanAfford(0, 0, unitProduceable.CostGold))
                {
                    _player.AddResource(ResourceType.Gold, -unitProduceable.CostGold );
                    GoldBuyCount++;
                    _status.freeUnitCount--;
                    _unitDispatchManagerUI.ShowBtn(true);
                    selectedUI.SetActive(true);
                    selectedTextUI.text = (this.GoldBuyCount + this.FoodWoodBuyCount).ToString();

                }
            }
            else
            {
                if (_player.CanAfford(unitProduceable.CostFood , unitProduceable.CostWood , 0))
                {
                    _player.AddResource(ResourceType.Wood, -unitProduceable.CostWood );
                    _player.AddResource(ResourceType.Food, -unitProduceable.CostFood );
                    FoodWoodBuyCount++;
                    _status.freeUnitCount--;
                    _unitDispatchManagerUI.ShowBtn(true);
                    selectedUI.SetActive(true);
                    selectedTextUI.text = (this.GoldBuyCount + this.FoodWoodBuyCount).ToString();

                }
            }
        }
        public void ReturnUnit(bool useGold)
        {
            IProduceable unit   = (IProduceable)_player.unitPrefabList[unitIndex];
            UnitStatus   status = _player.UnitStatusList[unitIndex];

            if (useGold && this.GoldBuyCount > 0)
            {
                _player.AddResource(ResourceType.Gold, unit.CostGold ); 
                GoldBuyCount--;
                status.freeUnitCount++;
            }
            else if(!useGold && this.FoodWoodBuyCount > 0)
            {
                _player.AddResource(ResourceType.Wood, unit.CostWood );
                _player.AddResource(ResourceType.Food, unit.CostFood );
                FoodWoodBuyCount--;
                status.freeUnitCount++;
            }

            if ((this.GoldBuyCount + this.FoodWoodBuyCount) == 0)
            {
                _unitDispatchManagerUI.ShowBtn(false);
            }
        }

        public void Reset()
        {
            selectedUI.SetActive(false);
            FoodWoodBuyCount = 0;
            GoldBuyCount     = 0;
        }
    }
}
