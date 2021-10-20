using System;

namespace Gameplay.Buff
{
    [Serializable]
    public class FloatBuffableValue : BuffableValue<float>
    {

        public override float Value
        {
            get => (_value * _magnification) + _additionalValue;
            set => _value = value;
        }

        public override void AddAdditionalValue(float additionalValue)
        {
            _additionalValue += additionalValue;
        }

        public override void AddMagnification(float magnification)
        {
            _magnification += magnification;
        }
        
        

    }
}