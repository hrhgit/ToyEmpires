using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Buff;
using Gameplay.GameUnit;
using Gameplay.GameUnit.FortificationUnit;
using Gameplay.GameUnit.SoldierUnit;
using Gameplay.GameUnit.SoldierUnit.Worker;
using Gameplay.Policy;
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

        [Header("经济")]
        #region 经济

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
                    this.Wood += count;
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
            uint r = 00;
            
            if (this.CanAfford(0, 0, u.CostGold * count))
            {
                r |= 01;
            }
            if (this.CanAfford(u.CostFood * count, u.CostWood * count, 0))
            {
                r |= 010;
            }

            return (int)r;
        }

        #endregion

        #region 单位
        
        private readonly List<FortificationUnitBase> _instanceFortificationList = new List<FortificationUnitBase>();

        [Header("工人")]
        #region 工人
        //Workers
        public int maxWorkerCount;
        public int             initWorkerCount;
        public SoldierUnitBase workerPrefab;
        public UnityEvent      onWorkerProduce = new UnityEvent();
        
        public readonly  int[]        ResourceWorkerCount = new int[]{0, 0, 0};
        public           UnitStatus   workerStatus;
        private readonly int[]        _activeResourceWorkerCount = new int[]{0, 0, 0};
        private readonly List<Worker> _instanceWorkersList       = new List<Worker>();

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
            _instanceWorkersList.Add( ((Worker) workerUnit));
            workerUnit.DeathEvent.AddListener((u =>
                                               {
                                                   _activeResourceWorkerCount[(int)resourceType]--;
                                                   ResourceWorkerCount[(int)resourceType]--;
                                                   roadUnitsCount[(int)resourceType]--;
                                                   workerStatus.curUnitCount--;
                                                   workerStatus.totalUnitCount--;
                                                   _instanceWorkersList.Remove( ((Worker) u));
                                               }));
            
            _activeResourceWorkerCount[(int) resourceType]++;
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
            workerStatus.freeUnitCount  = this.initWorkerCount;
            workerStatus.totalUnitCount = this.initWorkerCount;
        }
        

        #endregion

        [Header("战斗")]
        #region 战斗单位

        #region 生产

        public int maxBattleUnitCount;
        public List<SoldierUnitBase>                                     unitPrefabList         = new List<SoldierUnitBase>();
        public List<UnityEvent<SoldierUnitBase, PlayerBase, UnitStatus>> onUnitProduceEventList = new List<UnityEvent<SoldierUnitBase, PlayerBase, UnitStatus>>();

        private readonly List<SoldierUnitBase> _instanceUnitsList = new List<SoldierUnitBase>();

        public List<UnitStatus> UnitStatusList { get; private set; } = new List<UnitStatus>();

        public int CurUnitPopulation { get; private set; } = 0;



        private void InitUnitList()
        {
            for (int i = 0; i < unitPrefabList.Count; i++)
            {
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
                unitInstance.UnitTeam = this.playerTeam;
                unitInstance.UnitRoad = road;
                _instanceUnitsList.Add(unitInstance);
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
                                                          this._instanceUnitsList.Remove((SoldierUnitBase)u);
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
            }
        }
        
        public void DispatchUnits(int index, int count, Road road)
        {
            DispatchUnits(unitPrefabList[index], count, road, UnitStatusList[index]);
        }

        #endregion

        #endregion

        #endregion

        [Header("Buff")]
        #region Buff

        private PlayerBuffContainer _buffContainer;
        public PlayerBuffContainer BuffContainer
        {
            get => _buffContainer;
            set => _buffContainer = value;
        }

        #endregion
        
        [Header("政策")]
        #region 政策

        public                   PolicyManager    playerPolicyManager;
        [SerializeField] private int              economyPolicyCapacity;
        [SerializeField] private int              militaryPolicyCapacity;
        [SerializeField] private int              specialPolicyCapacity;

        private List<PolicyBase> _activatedPolicies = new List<PolicyBase>();
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


        public void ActivatePolicy(PolicyBase policyBase)
        {
            if (!_activatedPolicies.Contains(policyBase))
            {
                _activatedPolicies.Add(policyBase);
                policyBase.playerBuffs.ForEach((buff =>
                                                {
                                                    BuffContainer.AddBuff(buff);
                                                }));
                policyBase.unitBuffs.ForEach((buff =>
                                              {
                                                  this._instanceWorkersList.ForEach((worker => worker.BuffContainer.AddBuff(buff)));
                                                  this._instanceUnitsList.ForEach((unit => unit.BuffContainer.AddBuff(buff)));
                                              }));
            }
        }

        public void DeactivatePolicy(PolicyBase policyBase)
        {
            
        }

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
