using System;

namespace Gameplay.GameUnit
{
    [Serializable]
    public class UnitStatus
    {
        public int   unitID;
        public int   freeUnitCount      = 0;
        public int   totalUnitCount     = 0;
        public int   curUnitCount       = 0;
        public float unitProduceProcess = 0f;
    }
}