using System.Collections;
using Gameplay.GameUnit.ThrowingObject;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.GameUnit.SoldierUnit.CombatUnit.RangedAttackUnit
{
    public interface IRangeAttackable : IAttackable
    {
        public float              ThrowingSpeed    { get; }
        public float              Accuracy         { get; }
        public int                ThrowingCount    { get; }
        public float              ThrowingInterval { get; }
        public ThrowingObjectBase ThrowingObject   { get; }

        public UnityEvent<IRangeAttackable, IDefenable> ThrowingEvent    { get; set; }

        public IEnumerator ThrowObjects(IDefenable attackTarget);
        

        

    }
}