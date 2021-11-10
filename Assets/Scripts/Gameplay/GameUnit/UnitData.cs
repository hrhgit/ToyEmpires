namespace Gameplay.GameUnit
{
    public struct UnitData
    {
        public int    unitId;
        public string name;
        public string content;

        public UnitData(int unitId, string name, string content)
        {
            this.unitId  = unitId;
            this.name    = name;
            this.content = content;
        }
    }
    
}