
namespace Gameplay.GameUnit.SoldierUnit.CombatUnit
{
    public interface ICombatable
    {
        public int   Attack         { get; }
        public float AttackRange    { get; }
        public float AttackInterval { get; }
        
        
        public float FindEnemyRange { get; }

        public event AttackEventHandler AttackEvent;


        public void        DoAttack(IAttackable attackTarget);
        public IAttackable FindEnemy();
    }
}