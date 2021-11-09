namespace Gameplay.Buff
{
    //TODO 未完善
    public enum BuffNumericalValueType
    {
        //防御
        Defence,
        MaxHP,
        
        //移动
        Speed,

        //生产
        Productivity,
        CostFood,
        CostGold,
        CostWood,
        CostTime,
        MaxReserveCount,
        MaxLoadWood,
        MaxLoadFood,
        MaxLoadGold,
        
        //战斗
        Attack,
        AttackRange,
        AttackInterval,
        
        //维护
        MaintenanceTime,
        
        //远程
        ThrowingCount,
        ThrowingSpeed,
        ThrowingInterval,
        Accuracy,

        
    }

    public enum BuffModifiedType
    {
        Magnification,
        AdditionalValue
    }
}