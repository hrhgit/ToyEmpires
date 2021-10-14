namespace Gameplay.GameUnit.SoldierUnit
{
    public interface IMovable
    {
        public GameUnitMover UnitMover { get; }
        public float         MaxSpeed  { get; }
        
    }
}