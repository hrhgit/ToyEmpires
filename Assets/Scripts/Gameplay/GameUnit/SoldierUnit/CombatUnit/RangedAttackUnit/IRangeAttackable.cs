using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.GameUnit.SoldierUnit.CombatUnit.RangedAttackUnit
{
    public interface IRangeAttackable : IAttackable
    {
        public float                                     ThrowingSpeed    { get; }
        public int                                       ThrowingCount    { get; }
        public float                                     ThrowingInterval { get; }
        public UnityEvent<IRangeAttackable, IDefenable> ThrowingEvent    { get; set; }

        public IEnumerator ThrowObjects(IDefenable attackTarget);
        


    }
}