using System;
using Gameplay;
using Gameplay.GameUnit;
using Gameplay.Player;
using GameUI.UnitDispatchMenuUI;
using Global;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;


namespace GameUI
{
    public class UnitButtonUI : MonoBehaviour
    {
        public int        unitIndex;
        public Text       freeUnitTextUI;
        public Text       unitCountTextUI;
        public Text       unitFoodWoodCostTextUI;
        public Button     foodBtn;
        public Button     goldBtn;
        public Text       unitGoldCostTextUI;
        public GameObject selectedUI;
        public Text       selectedTextUI;
        public Image unitAvatar;
        public Image unitProduceBarUI;
        
        private UnitDispatchManagerUI _unitDispatchManagerUI;
        private PlayerBase            _player;
        private UnitStatus            _status;

        private void Start()
        {
            _unitDispatchManagerUI = this.transform.parent.GetComponent<UnitDispatchManagerUI>();
            _player               = BattleGameManager.BattleGameManagerInstance.userPlayer;

            Object spriteObj = Resources.Load("UI/UnitAvatars/" + GlobalGameManager.GlobalGameManagerInstance.GetFileName(_player.unitPrefabList[unitIndex].unitID), typeof(Sprite));
            Sprite sprite = null;
            try {
                sprite = Instantiate(spriteObj) as Sprite;
            } catch (Exception e) {

            }
            unitAvatar.sprite = sprite;
            
            _player.onUnitProduceEventList[unitIndex].AddListener((unit, player, status) =>
                                                         {
                                                             this.unitProduceBarUI.fillAmount = status.unitProduceProcess;
                                                         });
            _status                     = _player.UnitStatusList[unitIndex];
            _unitProduceable = (IProduceable)_player.unitPrefabList[unitIndex];
            //TODO 粗暴地把木材换成了食物
            // unitFoodWoodCostTextUI.text = ((IProduceable)_player.unitPrefabList[unitIndex]).CostFood + "食+" + ((IProduceable)_player.unitPrefabList[unitIndex]).CostWood + "木";
            unitFoodWoodCostTextUI.text = (_unitProduceable).CostFood + "食";
                unitGoldCostTextUI.text = (_unitProduceable).CostGold + "金";
                
        }
        

        private void FixedUpdate()
        {
            try
            {
                freeUnitTextUI.text  = _player.UnitStatusList[unitIndex].freeUnitCount.ToString();
                unitCountTextUI.text = _player.UnitStatusList[unitIndex].curUnitCount.ToString();
                SetInteractable();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        IProduceable _unitProduceable;
        private void SetInteractable()
        {
            goldBtn.interactable = _player.CanAfford(0, 0, _unitProduceable.CostGold);
            foodBtn.interactable =
                _player.CanAfford(_unitProduceable.CostFood, _unitProduceable.CostWood, 0);

        }

        public int FoodWoodBuyCount { get; private set; } = 0;
        public int GoldBuyCount { get; private set; } = 0;

        public void BuyUnit(bool useGold)
        {

            if(_status.freeUnitCount <=0 || _player.CurUnitPopulation + (this.GoldBuyCount + this.FoodWoodBuyCount) * _unitProduceable.CostPopulation >= _player.maxBattleUnitCount)
                return;
            if (useGold)
            {
                if (_player.CanAfford(0, 0, _unitProduceable.CostGold))
                {
                    _player.AddResource(ResourceType.Gold, -_unitProduceable.CostGold );
                    GoldBuyCount++;
                    _status.freeUnitCount--;
                    _unitDispatchManagerUI.ShowBtn(true);
                    selectedUI.SetActive(true);
                    selectedTextUI.text = (this.GoldBuyCount + this.FoodWoodBuyCount).ToString();

                }
            }
            else
            {
                if (_player.CanAfford(_unitProduceable.CostFood , _unitProduceable.CostWood , 0))
                {
                    _player.AddResource(ResourceType.Wood, -_unitProduceable.CostWood );
                    _player.AddResource(ResourceType.Food, -_unitProduceable.CostFood );
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

        public void OnMouseHover()
        {
            _unitDispatchManagerUI.unitDetailUI.Show(this._player.unitPrefabList[this.unitIndex]);
        }
        public void OnMouseLeave()
        {
            _unitDispatchManagerUI.unitDetailUI.Close();
        }
    }
}
