using System;
using Gameplay.GameUnit.SoldierUnit.CombatUnit.RangedAttackUnit;
using UnityEngine;

namespace Gameplay.GameUnit.ThrowingObject
{
    public class ThrowingObjectBase :GameUnitBase
    {
        public event ThrowingObjectHandler ShotSuccessedEvent;
        public event ThrowingObjectHandler ShotFailedEvent;
        public IRangeAttackable            shooter;
        public IDefenable                 target;

        public bool IsUsed
        {
            get => _isIsUsed;
            private set
            {
                _isIsUsed = value;
                if (_isIsUsed)
                {
                    _rigidbody.velocity                   = Vector3.zero;
                    _rigidbody.isKinematic                = true;
                    this.GetComponent<Collider>().enabled = false;
                    Destroy(this.gameObject, _isMissed ? 1f : 0.1f);
                }
            }
        }

        private Rigidbody _rigidbody;
        private bool      _isIsUsed = false;
        private bool      _isMissed = false;


        private void Awake()
        {
            _rigidbody = this.GetComponent<Rigidbody>();
        }

        protected override void Start()
        {
            BaseInit();
            this.gameObject.layer = UnitTeam switch
                                    {
                                        Team.Blue => LayerMask.NameToLayer("BlueMisc"),
                                        Team.Red  => LayerMask.NameToLayer("RedMisc"),
                                        _         => throw new ArgumentOutOfRangeException()
                                    };
        }

        private void Update()
        {
            if(!IsUsed)
                this.transform.LookAt(this.transform.position + _rigidbody.velocity);
        }

        private void OnCollisionEnter(Collision other)
        {
            try
            {
                if (other.collider.isTrigger || other.transform == ((GameUnitBase)shooter).transform) return;

            }
            catch (MissingReferenceException e)
            {
                Miss();
            }
            try
            {
                GameObject   gameObj = other.gameObject;
                GameUnitBase unit    = gameObj.GetComponent<GameUnitBase>();
                if (unit.UnitTeam != ((GameUnitBase)this.shooter).UnitTeam)
                {
                    try
                    {
                        IDefenable defenable = other.gameObject.GetComponent<GameUnitBase>() as IDefenable;
                        Hit(defenable);
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                    
                }
                
            }
            catch (Exception e)
            {
                Miss();
            }

        }

        private void Hit(IDefenable u)
        {
            _isMissed = false;
            IsUsed    = true;
            ShotSuccessedEvent?.Invoke(shooter, u);
        }

        private void Miss()
        {
            _isMissed = true;
            IsUsed    = true;
        }
        
    }
}