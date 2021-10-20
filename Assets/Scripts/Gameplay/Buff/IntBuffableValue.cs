using System;

namespace Gameplay.Buff
{
    [Serializable]
    public class IntBuffableValue : BuffableValue<int>
    {

        public IntBuffableValue(int value)
        {
            this.Value = value;
        }
        public IntBuffableValue()
        {
            this.Value = 0;
        }
        public override int Value
        {
            get => (int)(_value * _magnification) + _additionalValue;
            set => _value = value;
        }

        public override void AddAdditionalValue(int additionalValue)
        {
            _additionalValue += additionalValue;
        }

        public override void AddMagnification(float magnification)
        {
            _magnification += magnification;
        }

    }
}