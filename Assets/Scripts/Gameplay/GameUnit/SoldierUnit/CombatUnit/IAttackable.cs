
namespace Gameplay.GameUnit.SoldierUnit.CombatUnit
{
    public interface IAttackable
    {
        public int   Attack         { get; }
        public float AttackRange    { get; }
        public float AttackInterval { get; }
        
        
        public float FindEnemyRange { get; }

        public event AttackEventHandler AttackEvent;


        public bool        DoAttack(IDefenable attackTarget);
        public IDefenable FindAEnemy();
    }
}