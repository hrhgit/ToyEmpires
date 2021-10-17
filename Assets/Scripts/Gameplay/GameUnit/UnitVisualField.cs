using System;
using Gameplay.GameUnit.SoldierUnit;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.GameUnit
{
    public class UnitVisualField : MonoBehaviour
    {
        public SphereCollider VisualFieldSphere { get; private set; }

        public UnityEvent<UnitVisualField>           initEvent;
        public UnityEvent<UnitVisualField>           fixedUpdateEvent;
        public UnityEvent<UnitVisualField,Collider>  colliderHitEvent;
        public UnityEvent<UnitVisualField, Collider> colliderExitEvent;

        private void Awake()
        {
            VisualFieldSphere = this.GetComponent<SphereCollider>();

        }

        private void Start()
        {
            initEvent.Invoke(this);
        }

        private void FixedUpdate()
        {
            fixedUpdateEvent.Invoke(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            colliderHitEvent.Invoke(this, other);
        }
        
        private void OnTriggerExit(Collider other)
        {
            colliderExitEvent.Invoke(this, other);
        }
        


    }
}
