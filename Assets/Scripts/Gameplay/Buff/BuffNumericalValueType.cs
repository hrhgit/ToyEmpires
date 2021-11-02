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
        CostFood,
        CostGold,
        CostWood,
        CostTime,
        MaxReserveCount,
        
        //战斗
        Attack,
        AttackRange,
        AttackInterval,
        
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