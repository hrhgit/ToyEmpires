using Gameplay.GameUnit.SoldierUnit;
using Gameplay.Player;

namespace Gameplay.GameUnit
{
    public interface IProduceable
    {
        void       Produce(SoldierUnitBase unit, PlayerBase player, UnitStatus status);
        public int CostTime        { get; }
        public int CostFood        { get; }
        public int CostWood        { get; }
        public int CostGold        { get; }
        public int MaxReserveCount { get; }
    }
}