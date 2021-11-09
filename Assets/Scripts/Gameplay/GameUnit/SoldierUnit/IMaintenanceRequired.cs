using Gameplay.Buff;

namespace Gameplay.GameUnit.SoldierUnit
{
    public interface IMaintenanceRequired
    {
        public int          MaintenanceCostFood { get; }
        public bool         IsWellResourced     { get; }
        public UnitBuffBase NonResourceDeBuff   { get; }
    }
}