namespace Gameplay.Buff
{
    public interface IBuffable<out T> where T : BuffContainerBase
    {
       public T    BuffContainer { get; }
       public bool SetNumericalValueBuff(BuffNumericalValueType buffType, bool isAdditionalValue, float value);
    }
}