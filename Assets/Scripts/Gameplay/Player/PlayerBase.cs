using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Buff;
using Gameplay.Buildings;
using Gameplay.GameUnit;
using Gameplay.GameUnit.FortificationUnit;
using Gameplay.GameUnit.SoldierUnit;
using Gameplay.GameUnit.SoldierUnit.Worker;
using Gameplay.Policy;
using Gameplay.TechTree;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Gameplay.Player
{
    public class PlayerBase : MonoBehaviour, IBuffable<PlayerBuffContainer>
    {
        public Team playerTeam;

        [Header("特殊点")]
        #region 坐标

        public PlayerHomeUnit homeUnit;
        public Transform      foodWorkPos;
        public Transform      goldWorkPos;
        public Transform      woodWorkPos;
        public Transform      topPos;
        public Transform      midPos;
        public Transform      botPos;

        #endregion

        
        #region 经济
        [Header("经济")]
        //Economics
        private IntBuffableValue _food = new IntBuffableValue();
        private IntBuffableValue _gold = new IntBuffableValue();
        private IntBuffableValue _wood = new IntBuffableValue();
        public int Food
        {
            get => _food.Value;
            private set => _food.Value = value;
        }

        public int Gold
        {
            get => _gold.Value;
            private set => _gold.Value = value;
        }

        public int Wood
        {
            get => _wood.Value;
            private set => _wood.Value = value;
        }
        
        public int Productivity => this.workerStatus.freeUnitCount * workerPrefab.Productivity + this.UnitStatusList.Select(((status, i) => new {status,i})).Sum((s => s.status.freeUnitCount * unitPrefabList[s.i].Productivity));

        public void AddResource(ResourceType resourceType, int count)
        {
            switch (resourceType)
            {
                case ResourceType.Food:
                    this.Food += count;
                    break;
                case ResourceType.Gold:
                    this.Gold += count;
                    break;
                case ResourceType.Wood:
                    //TODO 粗暴地把木头换成了食物
                    this.Food += count;
                    // this.Wood += count;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null);
            }
        }

        public bool CanAfford(int food, int wood, int gold) => this.Food >= food && this.Wood >= wood && this.Gold >= gold;

        /// <summary>
        /// 是否能承担count个单位的成本
        /// 0 =》 不能
        /// 1 =》 用黄金
        /// 2 =》 用食物木材
        /// 3 =》 黄金/食物木材 都可以
        /// </summary>
        /// <param name="u">单位</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public int CanAfford(IProduceable u,int count)
        {
            uint r = 0x00;
            
            if (this.CanAfford(0, 0, u.CostGold * count))
            {
                r |= 0x01;
            }
            if (this.CanAfford(u.CostFood * count, u.CostWood * count, 0))
            {
                r |= 0x03;
            }

            return (int)r;
        }
        
        /// <summary>
        /// 是否能承担建筑的成本
        /// 0 =》 不能
        /// 1 =》 用黄金
        /// 2 =》 用食物木材
        /// 3 =》 黄金/食物木材 都可以
        /// </summary>
        /// <param name="b">建筑</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public int CanAfford(Building b,int count = 1)
        {
            uint r = 0x00;
            
            if (this.CanAfford(0, 0, b.BuildingCostGold * count))
            {
                r |= 0x01;
            }
            if (this.CanAfford(b.BuildingCostFood * count, b.BuildingCostWood * count, 0))
            {
                r |= 0x03;
            }
            return (int)r;
        }

        #endregion

        #region 单位
        
        private readonly List<FortificationUnitBase> _instanceFortificationList = new List<FortificationUnitBase>();

        
        #region 工人
        [Header("工人")]
        //Workers
        public int maxWorkerCount;
        public                   int             initWorkerCount;
        public                   SoldierUnitBase workerPrefab;
        public                   UnityEvent      onWorkerProduce = new UnityEvent();
        [HideInInspector] public Transform[]     resourceHome;
        
        public readonly  int[]        ResourceWorkerCount = new int[]{0, 0, 0};
        public           UnitStatus   workerStatus;
        private readonly int[]        _activeResourceWorkerCount = new int[]{0, 0, 0};
        private readonly List<Worker> _instanceWorkersList       = new List<Worker>();
        
        public List<Worker> InstanceWorkersList => _instanceWorkersList;

        public void DispatchWorker(ResourceType resourceType, bool isAdd)
        {
            if(isAdd)
            {
                // 有空闲工人
                if(workerStatus.freeUnitCount > 0)
                {
                    workerStatus.freeUnitCount--;
                    ResourceWorkerCount[(int) resourceType]++;
                    roadUnitsCount[(int) resourceType]++;
                    workerStatus.curUnitCount++;
                    AddWorker(resourceType);
                }else if (workerStatus.curUnitCount < _activeResourceWorkerCount.Sum())
                {
                    ResourceWorkerCount[(int) resourceType]++;
                    roadUnitsCount[(int) resourceType]++;
                    workerStatus.curUnitCount++;
                }
            }
            else
            {
                if (ResourceWorkerCount[(int)resourceType] > 0)
                {
                    ResourceWorkerCount[(int)resourceType] --;
                    roadUnitsCount[(int)resourceType]--;
                    workerStatus.curUnitCount--;
                }else
                {
                    ResourceWorkerCount[(int)resourceType] = 0;
                }
                
            }
        }

        private void AddWorker(ResourceType resourceType)
        {
            SoldierUnitBase workerUnit = Instantiate(this.workerPrefab, resourceType switch
                                                                        {
                                                                            ResourceType.Food => topPos,
                                                                            ResourceType.Gold => midPos,
                                                                            ResourceType.Wood => botPos,
                                                                            _                 => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null)
                                                                        });
            workerUnit.UnitTeam = this.playerTeam;
            workerUnit.UnitRoad = resourceType switch
                                  {
                                      ResourceType.Food => Road.Top,
                                      ResourceType.Gold => Road.Mid,
                                      ResourceType.Wood => Road.Bottom,
                                      _                 => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null)
                                  };
            ((Worker) workerUnit).WorkerLoadDoneFunc += WorkerLoadDone;
            InstanceWorkersList.Add( ((Worker) workerUnit));
            workerUnit.DeathEvent.AddListener((u =>
                                               {
                                                   _activeResourceWorkerCount[(int)resourceType]--;
                                                   ResourceWorkerCount[(int)resourceType]--;
                                                   roadUnitsCount[(int)resourceType]--;
                                                   workerStatus.curUnitCount--;
                                                   workerStatus.totalUnitCount--;
                                                   InstanceWorkersList.Remove( ((Worker) u));
                                               }));
            workerUnit.BuffContainer.buffList = this.workerPrefab.BuffContainer.buffList;
            workerUnit.BuffContainer.InitBuff();
            
            _activeResourceWorkerCount[(int) resourceType]++;
            workerUnit.gameObject.SetActive(true);
        }

        private void WorkerLoadDone(Worker worker)
        {
            if (ResourceWorkerCount[(int) worker.workResourceType] < _activeResourceWorkerCount[(int) worker.workResourceType])
            {
                workerStatus.freeUnitCount++;
                _activeResourceWorkerCount[(int) worker.workResourceType]--;
                Destroy(worker.gameObject);
                for (int i = 0; i < ResourceWorkerCount.Length; i++)
                {
                    if (ResourceWorkerCount[i] > _activeResourceWorkerCount[i])
                    {
                        workerStatus.freeUnitCount--;
                        AddWorker((ResourceType)i);
                        return;
                    }
                }

            }

        }

        private void ProduceWorker()
        {
            ((IProduceable)workerPrefab).Produce(workerPrefab, this, workerStatus);
        }

        private void InitWorker()
        {
            resourceHome = new Transform[]
                           {
                               this.topPos, this.midPos, this.botPos
                           };

            workerPrefab = Instantiate(workerPrefab, BattleGameManager.BattleGameManagerInstance.tempParent);
            workerPrefab.gameObject.SetActive(false);
            workerStatus.freeUnitCount  = this.initWorkerCount;
            workerStatus.totalUnitCount = this.initWorkerCount;
        }

        public void ChangeResourceHome(Road road, Transform newHome, bool isMandatory = false)
        {
            this.resourceHome[(int)road] = newHome;
            (from worker in this._instanceWorkersList
             where worker.UnitRoad == road
             select worker).ToList().ForEach((worker => worker.ChangeHomePos(newHome, isMandatory)));
        }
        
        #endregion

        
        #region 战斗单位
        [Header("战斗")]
        #region 生产

        public int maxBattleUnitCount;
        public List<SoldierUnitBase>                                     unitPrefabList         = new List<SoldierUnitBase>();
        public List<UnityEvent<SoldierUnitBase, PlayerBase, UnitStatus>> onUnitProduceEventList = new List<UnityEvent<SoldierUnitBase, PlayerBase, UnitStatus>>();

        [SerializeField]private  List<SoldierUnitBase> _instanceUnitsList = new List<SoldierUnitBase>();

        public List<UnitStatus> UnitStatusList { get; private set; } = new List<UnitStatus>();

        public int                   CurUnitPopulation { get; private set; } = 0;
        public List<SoldierUnitBase> InstanceUnitsList => _instanceUnitsList;



        private void InitUnitList()
        {
            
            for (int i = 0; i < unitPrefabList.Count; i++)
            {
                unitPrefabList[i] = Instantiate(unitPrefabList[i],BattleGameManager.BattleGameManagerInstance.tempParent);
                unitPrefabList[i].gameObject.SetActive(false);
                UnitStatusList.Add(new UnitStatus()
                                   {
                                       unitID = unitPrefabList[i].unitID
                                   });
                onUnitProduceEventList.Add((new UnityEvent<SoldierUnitBase, PlayerBase, UnitStatus>()));
            }
        }

        public virtual void InvokeUnitProduce(SoldierUnitBase gameunitbase, PlayerBase playerbase, UnitStatus status)
        {
            int index = unitPrefabList.IndexOf(gameunitbase);
            onUnitProduceEventList[index].Invoke(gameunitbase, playerbase, status);
        }

        private void ProduceUnit()  
        {
            if (CurUnitPopulation >= maxBattleUnitCount)
                return;
            for (var i = 0; i < unitPrefabList.Count; i++)
            {
                ((IProduceable)unitPrefabList[i]).Produce(unitPrefabList[i], this, UnitStatusList[i]);
            }
        }

        #endregion

        #region 派遣

        private static  float _dispatchOffset = .02f;
        public readonly int[] roadUnitsCount  = new int[3];

        public void DispatchUnits(SoldierUnitBase unit, int count, Road road, UnitStatus status)
        {
            Transform parent = road switch
                               {
                                   Road.Top    => this.topPos,
                                   Road.Mid    => this.midPos,
                                   Road.Bottom => this.botPos,
                                   _           => throw new ArgumentOutOfRangeException(nameof(road), road, null)
                               };
            for (int i = 0; i < count; i++)
            {
                float           angle        = Random.Range(0f, 2 *Mathf.PI);
                float           r            = _dispatchOffset * Random.Range(0f, 1f);
                Vector3         exactPos     = parent.position + new Vector3(r * Mathf.Cos(angle), 0, r * Mathf.Sin(angle));
                SoldierUnitBase unitInstance = Instantiate(unit, exactPos, Quaternion.Euler(0, 0, 0), parent);
                unitInstance.UnitTeam               = this.playerTeam;
                unitInstance.UnitRoad               = road;
                unitInstance.BuffContainer.buffList = unit.BuffContainer.buffList;
                unitInstance.BuffContainer.InitBuff();
                InstanceUnitsList.Add(unitInstance);
                try
                {
                    IDefenable defenable = (IDefenable)unitInstance;
                    defenable.DeathEvent.AddListener((u =>
                                                      {
                                                          this.CurUnitPopulation -= ((IProduceable)unit).CostPopulation;
                                                          UnitStatus s =UnitStatusList.Find((s => s.unitID == unitInstance.unitID));
                                                          s.curUnitCount--;
                                                          s.totalUnitCount--;
                                                          this.roadUnitsCount[(int)road]++;
                                                          this.InstanceUnitsList.Remove((SoldierUnitBase)u);
                                                      }));
                }
                catch (Exception e)
                {
                    // ignored
                }

                status.curUnitCount++;
                // status.freeUnitCount--;
                // status.totalUnitCount++;
                this.CurUnitPopulation += ((IProduceable)unit).CostPopulation;
                roadUnitsCount[(int)road]++;
                unitInstance.gameObject.SetActive(true);
            }
        }
        
        public void DispatchUnits(int index, int count, Road road)
        {
            DispatchUnits(unitPrefabList[index], count, road, UnitStatusList[index]);
        }

        #endregion

        #region 维护

        // [Header("维护")]
        
        
        public bool Maintain(SoldierUnitBase soldierUnitBase)
        {
            int totalMaintenanceCost = soldierUnitBase.MaintenanceCostFood;
            if (this.Food >= totalMaintenanceCost)
            {
                this.AddResource(ResourceType.Food, -totalMaintenanceCost);
                return true;
            }
            else
            {
                this.AddResource(ResourceType.Food, -this.Food);
                return false;
            }
        }

        

        #endregion

        #endregion

        #endregion

        
        #region Buff
        [Header("Buff")]
        [SerializeField]private PlayerBuffContainer _buffContainer;
        public PlayerBuffContainer BuffContainer
        {
            get => _buffContainer;
            set => _buffContainer = value;
        }

        public bool SetNumericalValueBuff(BuffNumericalValueType buffType, bool isAdditionalValue, float value)
        {
            //TODO 未完善
            switch (buffType)
            {
                default:
                    throw new UnityException("未找到Buff: " + buffType.ToString());
                    return false;
            }

            return true;
        }

        public void AddUnitsBuff(BuffBase buff)
        {
            this.workerPrefab.BuffContainer.AddBuff(buff);
            this.unitPrefabList.ForEach((unit => unit.BuffContainer.AddBuff(buff)));
            this.InstanceWorkersList.ForEach((worker => worker.BuffContainer.AddBuff(buff)));
            this.InstanceUnitsList.ForEach((unit => unit.BuffContainer.AddBuff(buff)));
        }


        #endregion
        
        
        #region 政策
        [Header("政策")]
        public                   PolicyManager    playerPolicyManager;

        [SerializeField] private IntBuffableValue policyCapacity = new IntBuffableValue(3);
        [SerializeField] private int              economyPolicyCapacity;
        [SerializeField] private int              militaryPolicyCapacity;
        [SerializeField] private int              specialPolicyCapacity;

        private List<PolicyBase> _activatedPolicies = new List<PolicyBase>();
        private int              _curPolicyActivatedCount = 0;
        public int EconomyPolicyCapacity
        {
            get => economyPolicyCapacity;
            set => economyPolicyCapacity = value;
        }

        public int MilitaryPolicyCapacity
        {
            get => militaryPolicyCapacity;
            set => militaryPolicyCapacity = value;
        }

        public int SpecialPolicyCapacity
        {
            get => specialPolicyCapacity;
            set => specialPolicyCapacity = value;
        }
        
        public int PolicyCapacity
        {
            get => policyCapacity;
        }

        


        public void ActivatePolicy(PolicyBase policyBase)
        {
            if (_curPolicyActivatedCount + policyBase.occupancy > PolicyCapacity)
                throw new Exception("政策满额");
            
            if (!_activatedPolicies.Contains(policyBase))
            {
                _activatedPolicies.Add(policyBase);
                policyBase.playerBuffs.ForEach((buff =>
                                                {
                                                    BuffContainer.AddBuff(buff);
                                                }));
                policyBase.unitBuffs.ForEach((buff =>
                                              {
                                                  AddUnitsBuff(buff);
                                              }));
                                                  
                _curPolicyActivatedCount+=policyBase.occupancy;
            }
        }


        public void DeactivatePolicy(PolicyBase policyBase)
        {
            
            if (_activatedPolicies.Contains(policyBase))
            {
                policyBase.playerBuffs?.ForEach((buff =>
                                                {
                                                    BuffContainer.RemoveBuff(buff);
                                                }));
                policyBase.unitBuffs?.ForEach((buff =>
                                              {
                                                  this.InstanceWorkersList.ForEach((worker => worker.BuffContainer.RemoveBuff(buff)));
                                                  this.InstanceUnitsList.ForEach((unit => unit.BuffContainer.RemoveBuff(buff)));
                                              }));
                _activatedPolicies.Remove(policyBase);
                _curPolicyActivatedCount -= policyBase.occupancy;
            }
        }

        #endregion

        #region 科技

        [Header("科技")] 
        public TechTree.TechTree techTree;

        public bool PurchaseTechNode(int nodeIdx,bool isUseGold)
        {
            TechTreeNode techNode = techTree.techTreeNodes[nodeIdx];
            if (techNode.IsDevelopable)
            {
                if (isUseGold && this.Gold >= techNode.CurCostGold)
                {
                    this.AddResource(ResourceType.Gold,-techNode.CurCostGold);
                    techNode.Purchase();
                    return true;
                }else if (!isUseGold && (this.Food >= techNode.CurCostFood && this.Wood >= techNode.CurCostWood))
                {
                    this.AddResource(ResourceType.Food, -techNode.CurCostFood);
                    this.AddResource(ResourceType.Wood, -techNode.CurCostWood);
                    techNode.Purchase();
                    return true;
                }
            }
            return false;
        }
        
        public void ActivateTech(int nodeIdx)
        {
            this.ActivateTech(techTree.techTreeNodes[nodeIdx].technology);
        }
        public void ActivateTech(TechTreeNode node)
        {
            this.ActivateTech(node.technology);
        }
        public void ActivateTech(Technology tech)
        {
            tech.playerBuffs?.ForEach((buff =>
                                      { 
                                          this.BuffContainer.AddBuff(buff);
                                      }));
            tech.unitBuffs?.ForEach((buff =>
                                    {
                                        this.AddUnitsBuff(buff);
                                    }));

        }

        #endregion

        #region 建筑

        [Header("建筑")]
        public BuildingsManager buildingManager;

        #endregion

        #region 执行逻辑

        private void Awake()
        {
            workerStatus = new UnitStatus()
                           {
                               unitID = workerPrefab.unitID
                           };
            InitWorker();
            InitUnitList();

        }

        private void Start()
        {
        }

        private void FixedUpdate()
        {

            ProduceWorker();
            ProduceUnit();
        }


        #endregion

        #region 杂项


        #endregion

    }
}
