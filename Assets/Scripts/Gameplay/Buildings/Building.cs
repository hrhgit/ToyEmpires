using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Buff;
using Gameplay.GameUnit.FortificationUnit.BuildingUnit;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Buildings
{
    public class Building : MonoBehaviour
    {
        public  int              buildingID;
        public  BuildingUnitBase buildingUnit;
        public  BuildingsGroup   buildingsGroup;
        public  BuildingsManager manager;
        private float            buildingProcess = 0f;
        public  int              maxLevel;
        public  int              level = 0;
        
        public  Road             road;
        public  int              posIdx;
        

        #region 建造

        [Header("建造")] 
        [SerializeField] private IntBuffableValue buildingCostFood = new IntBuffableValue(0);
        [SerializeField] private IntBuffableValue buildingCostWood = new IntBuffableValue(0);
        [SerializeField] private IntBuffableValue buildingCostGold = new IntBuffableValue(0);
        [SerializeField] private IntBuffableValue[] buildingCostTime;
        
        private bool _isBuilding  = true;
        private bool _isBuilt     = false;
        private bool _isFixing    = false;
        private bool _isUpgrading = false;
        
        public int BuildingCostFood => buildingCostFood;
        public int BuildingCostWood => buildingCostWood;
        public int BuildingCostGold => buildingCostGold;
        public int[] BuildingCostTime => (from t in buildingCostTime 
                                          select t.Value).ToArray();

        public float process => IsProdutivityRequiring ? this.buildingProcess / this.BuildingCostTime[level] : 1f;

        public bool IsBuilding
        {
            get => _isBuilding;
            set => _isBuilding = value;
        }

        public bool IsBuilt
        {
            get => _isBuilt;
            set => _isBuilt = value;
        }

        public bool IsFixing
        {
            get => _isFixing;
            set => _isFixing = value;
        }

        public bool IsUpgrading
        {
            get => _isUpgrading;
            set => _isUpgrading = value;
        }

        public virtual bool IsProdutivityRequiring => this.IsBuilding || this.IsFixing || this.IsUpgrading;

        public virtual void Set()
        {
            OnBuildingSet.Invoke(this);
            this.level         = 0;
            buildingUnit.Set();
            this.Build();
        }

        public virtual void Build()
        {
            this.IsBuilt     = false;
            this.IsFixing    = false;
            this.IsUpgrading = false;
            this.IsBuilding  = true;
        }

        protected virtual void BuildingUpdate()
        {
            this.buildingProcess += Time.fixedDeltaTime * manager.BuildingSpeed;
            if (this.buildingProcess > (this.BuildingCostTime[level]))
            {
                this.IsBuilding = false;
                this.level++;
                this.IsBuilt    = true;
                OnBuildingBuilt.Invoke(this);
            }
        }

        public virtual void Fix()
        {
            
        }

        protected virtual void FixingUpdate()
        {
            
        }


        public virtual void Upgrade()
        {
            this.buildingProcess = 0f;
            this.IsUpgrading     = true;
        }

        protected virtual void UpgradingUpdate()
        {
            
        }

        public virtual void Destroy()
        {
            this.IsBuilt     = false;
            this.IsFixing    = false;
            this.IsUpgrading = false;
            this.IsBuilding  = false;
            OnBuildingDestroy.Invoke(this);
        }
        

        #endregion

        #region 血量

        public int[] hp = new int[10];

        #endregion
        
        #region 效果

        public UnityEvent<Building> OnBuildingSet       = new UnityEvent<Building>();
        public UnityEvent<Building> OnBuildingBuilt     = new UnityEvent<Building>();
        public UnityEvent<Building> OnBuildingAvailable = new UnityEvent<Building>();
        public UnityEvent<Building> OnBuildingUpgraded  = new UnityEvent<Building>();
        public UnityEvent<Building> OnBuildingDestroy   = new UnityEvent<Building>();

        #endregion

        #region 逻辑

        protected virtual void FixedUpdate()
        {
            if (IsBuilding)
            {
                BuildingUpdate();
            }else if (IsFixing)
            {
                FixingUpdate();
            }else if (IsUpgrading)
            {
                UpgradingUpdate();
            }

            if (IsBuilt)
            {
                OnBuildingAvailable.Invoke(this);
            }
        }

        protected virtual void Start()
        {
            
        }

        #endregion


    }
}
