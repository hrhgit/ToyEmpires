using System.Collections;
using Gameplay.GameUnit.SoldierUnit.CombatUnit;
using Gameplay.GameUnit.SoldierUnit.CombatUnit.RangedAttackUnit;
using Rendering;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.GameUnit.FortificationUnit
{
    public class FortificationUnitBase : GameUnitBase, IDefenable
    {
        [SerializeField] private int  _maxHp;
        [SerializeField] private int  _defence;
        private                  bool _isDeath;
        private                  int  _curHp;

        public int  MaxHp   => _maxHp;
        public int  CurHp
        {
            get => _curHp;
            private set => _curHp = value;
        }

        public bool IsDeath => _isDeath;
        public int  Defence => _defence;

        public event AttackEventHandler BeAttackedEvent;
        public UnityEvent<IDefenable>   DeathEvent { get; } = new UnityEvent<IDefenable>();

        public virtual void BeAttacked(IAttackable attacker)
        {
            
        }


        protected virtual void Awake()
        {
            
        }

        protected void InitHP()
        {
            this.CurHp = this.MaxHp;
        }
        
        protected void ChangeUnitMaterialColor()
        {
            this.transform.Find("Sprite").GetComponent<SpriteRendererPlus>().ChangeColor();
            DeathEvent.AddListener((u =>
                                    {
                                        Resources.UnloadUnusedAssets();
                                    }));
        }


        protected override void Start()
        {
            base.Start();
            InitHP();
            // ChangeUnitMaterialColor();

        }
    }
}
