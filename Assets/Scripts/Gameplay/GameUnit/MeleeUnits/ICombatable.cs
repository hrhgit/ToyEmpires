namespace Gameplay.GameUnit.MeleeUnits
{
    public interface ICombatable
    {
        public int Attack         { get; }
        public int AttackRange    { get; }
        public int AttackInterval { get; }
        
        public int Defence { get; }

        public event AttackEventHandler AttackEvent;
        public event AttackEventHandler BeAttackedEvent;

        public void DoAttack(GameUnitBase   attackTarget);
        public void BeAttacked(ICombatable attacker);
    }
}