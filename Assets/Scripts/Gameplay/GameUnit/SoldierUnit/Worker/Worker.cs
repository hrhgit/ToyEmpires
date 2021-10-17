using System;
using System.Collections;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.GameUnit.SoldierUnit.Worker
{
    public delegate void WorkerFunc(Worker worker);
    public class Worker : SoldierUnitBase,IProduceable
    {
        // 基本能力值
        public int[]   maxLoad        = new int[3];
        public float[] workCostTime   = new float[3];
        public float[] unloadCostTime = new float[3];
        
        public event WorkerFunc WorkerLoadDoneFunc;
        public event WorkerFunc WorkerBackHomeFunc;
        public event WorkerFunc WorkerArriveWorkPosFunc;
        public event WorkerFunc WorkerWorkDoneFunc;
        

        //基本属性
        public int          CurLoad        { get; private set; }

        [HideInInspector] public ResourceType workResourceType;
        private                  PlayerBase   _player;
        private                  Transform      _homePos;
        private                  Transform      _workPos;
        private                  int          _curLoad;
        private                  bool         _isReturning = false;
        private                  bool         _isWorking   = false;
        private                  bool         _isLoading   = false;
        
        [SerializeField] private int _costTime;
        [SerializeField] private int _costFood;
        [SerializeField] private int _costWood;
        [SerializeField] private int _costGold;
        [SerializeField] private int _costPopulation = 1;
        [SerializeField] private int _maxReserveCount;

        public int CostTime => _costTime;

        public int CostFood => _costFood;

        public int CostWood => _costWood;

        public int CostGold       => _costGold;
        public int CostPopulation => _costPopulation;

        public int MaxReserveCount => _maxReserveCount;


        public Worker(ResourceType workResourceType)
        {
            this.workResourceType = workResourceType;
        }

        protected override void Start()
        {
            BaseInit();
            InitHP();
            this._player = UnitTeam switch
                           {
                               Team.Blue => BattleGameManager.BattleGameManagerInstance.bluePlayer,
                               Team.Red  => BattleGameManager.BattleGameManagerInstance.redPlayer,
                               _         => throw new ArgumentOutOfRangeException()
                           };
            workResourceType = UnitRoad switch
                               {
                                   Road.Top     => ResourceType.Food,
                                   Road.Mid    => ResourceType.Gold,
                                   Road.Bottom => ResourceType.Wood,
                                   _           => throw new ArgumentOutOfRangeException()
                               };
            switch (workResourceType)
            {
                case ResourceType.Food:
                    this._workPos = _player.foodWorkPos;
                    this._homePos = _player.topPos;
                    break;
                case ResourceType.Gold:
                    this._workPos = _player.goldWorkPos;
                    this._homePos = _player.midPos;
                    break;
                case ResourceType.Wood:
                    this._workPos = _player.woodWorkPos;
                    this._homePos = _player.botPos;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            this.ChangeUnitMaterialColor();
            this.UnitMover.targetReachedEvent.AddListener(WorkerReachTarget);
            this.ToWork();
            
        }

        private void WorkerReachTarget()
        {
            // 返回到家
            if (_isReturning)
            {
                this._isLoading = true;
                WorkerBackHomeFunc?.Invoke(this);
                StartCoroutine(OnUnloadDone());
            }
            // 到达工作地点
            else
            {
                this._isWorking = true;
                WorkerArriveWorkPosFunc?.Invoke(this);
                StartCoroutine(OnWorkDone());
            }
        }

        private IEnumerator OnUnloadDone()
        {
            yield return new WaitForSeconds(unloadCostTime[(int)this.workResourceType]);
            this._player.AddResource(this.workResourceType,this._curLoad);
            this._curLoad         = 0;
            this._isLoading       = false;
            this._isReturning     = false;
            this.UnitMover.Target = _workPos;
            this.WorkerLoadDoneFunc?.Invoke(this);
        }

        private IEnumerator OnWorkDone()
        {
            yield return new WaitForSeconds(workCostTime[(int)this.workResourceType]);
            this._curLoad         = this.maxLoad[(int)this.workResourceType];
            this._isWorking       = false;
            this._isReturning     = true;
            this.UnitMover.Target = _homePos;
            this.WorkerWorkDoneFunc?.Invoke(this);
        }
        

        private void ToWork()
        {
            _isReturning          = false;
            this.UnitMover.Target = _workPos;
        }

        public void Produce(SoldierUnitBase unit, PlayerBase player, UnitStatus status)
        {
            if (status.totalUnitCount >= player.maxWorkerCount)
                return;
            if(status.unitProduceProcess < 1)
            {
                status.unitProduceProcess += Time.fixedDeltaTime / (((IProduceable)player.workerPrefab).CostTime);
                player.onWorkerProduce.Invoke();
            }
            else
            {
                status.unitProduceProcess = 0;
                status.totalUnitCount++;
                status.freeUnitCount++;
            }

        }

    }
}
