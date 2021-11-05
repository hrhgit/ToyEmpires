using System;

namespace Gameplay.Buff
{
    [Serializable]
    public class FloatBuffableValue : BuffableValue<float>
    {

        public FloatBuffableValue(float value)
        {
            this.Value = value;
        }
        
        public FloatBuffableValue()
        {
            this.Value = 0f;
        }
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
            _magnification *= magnification;
        }
        
        

    }
}