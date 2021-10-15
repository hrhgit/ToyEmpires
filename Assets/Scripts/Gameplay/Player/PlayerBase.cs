using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.GameUnit;
using Gameplay.GameUnit.SoldierUnit;
using Gameplay.GameUnit.SoldierUnit.Worker;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Gameplay.Player
{
    public class PlayerBase : MonoBehaviour
    {
        public Team playerTeam;
        public int  maxHp;
        public int  defence;

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

        //Economics
        private int _food;
        private int _gold;
        private int _wood;
        public int Food
        {
            get => _food;
            private set => _food = value;
        }

        public int Gold
        {
            get => _gold;
            private set => _gold = value;
        }

        public int Wood
        {
            get => _wood;
            private set => _wood = value;
        }
        
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

        #region 工人
        //Workers
        public int          maxWorkerCount;
        public int          initWorkerCount;
        public SoldierUnitBase workerPrefab;
        public UnityEvent   onWorkerProduce = new UnityEvent();
        
        public readonly  int[]      ResourceWorkerCount        = new int[]{0, 0, 0};
        public           UnitStatus workerStatus               = new UnitStatus();
        private readonly int[]      _activeResourceWorkerCount = new int[]{0, 0, 0};

        public void DispatchWorker(ResourceType resourceType, bool isAdd)
        {
            if(isAdd)
            {
                // 有空闲工人
                if(workerStatus.freeUnitCount> 0)
                {
                    workerStatus.freeUnitCount--;
                    ResourceWorkerCount[(int) resourceType]++;
                    workerStatus.curUnitCount++;
                    AddWorker(resourceType);
                }else if (workerStatus.curUnitCount < _activeResourceWorkerCount.Sum())
                {
                    ResourceWorkerCount[(int) resourceType]++;
                    workerStatus.curUnitCount++;
                }
            }
            else
            {
                if (ResourceWorkerCount[(int)resourceType] > 0)
                {
                    ResourceWorkerCount[(int)resourceType] = ResourceWorkerCount[(int)resourceType] - 1;
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
            ((IProduceable)workerPrefab).Produce(workerPrefab,this,workerStatus);
        }

        private void InitWorker()
        {
            workerStatus.freeUnitCount    = this.initWorkerCount;
            workerStatus.totalUnitCount   = this.initWorkerCount;
        }
        

        #endregion

        #region 战斗单位

        #region 生产

        public int                 maxBattleUnitCount;
        public List<SoldierUnitBase>  unitPrefabList = new List<SoldierUnitBase>();
        public event GameUnitEvent onUnitProduce;

        public List<UnitStatus> UnitStatusList { get; private set; } = new List<UnitStatus>();

        public int CurUnitCount { get; private set; } = 0;

        private void InitUnitList()
        {
            for (int i = 0; i < unitPrefabList.Count; i++)
            {
                UnitStatusList.Add(new UnitStatus());
            }
        }

        public virtual void InvokeUnitProduce(SoldierUnitBase gameunitbase, PlayerBase playerbase, UnitStatus status)
        {
            onUnitProduce?.Invoke(gameunitbase, playerbase, status);
        }

        private void ProduceUnit()  
        {
            if (CurUnitCount >= maxBattleUnitCount)
                return;
            for (var i = 0; i < unitPrefabList.Count; i++)
            {
                ((IProduceable)unitPrefabList[i]).Produce(unitPrefabList[i], this, UnitStatusList[i]);
            }
        }

        #endregion

        #region 派遣

        private static float _dispatchOffset = .05f;

        public void DispatchUnits(SoldierUnitBase unit, int count, Road road,UnitStatus status)
        {
            Transform parent = road switch
                               {
                                   Road.Top     => this.topPos,
                                   Road.Mid    => this.midPos,
                                   Road.Bottom => this.botPos,
                                   _           => throw new ArgumentOutOfRangeException(nameof(road), road, null)
                               };
            for (int i = 0; i < count; i++)
            {
                float   angle    = Random.Range(0f, 2 *Mathf.PI);
                float   r        = _dispatchOffset * Random.Range(0f, 1f);
                Vector3 exactPos = parent.position + new Vector3(r * Mathf.Cos(angle), 0, r * Mathf.Sin(angle));
                SoldierUnitBase unitInstance = Instantiate(unit, exactPos, Quaternion.Euler(0, 0, 0), parent);
                unitInstance.UnitTeam = this.playerTeam;
                unitInstance.UnitRoad = road;
                status.curUnitCount++;
                // status.freeUnitCount--;
                // status.totalUnitCount++;
                this.CurUnitCount++;
            }
        }
        
        public void DispatchUnits(int index, int count,Road road)
        {
            DispatchUnits(unitPrefabList[index],count,road,UnitStatusList[index]);
        }

        #endregion

        #endregion

        #region 执行逻辑

        private void Awake()
        {
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

        private void ChangeUnitMaterialColor(GameObject unitObj)
        {
            Renderer renderer = unitObj.GetComponent<Renderer>();
            Material material = renderer.material;
        }

        #endregion

    }
}
