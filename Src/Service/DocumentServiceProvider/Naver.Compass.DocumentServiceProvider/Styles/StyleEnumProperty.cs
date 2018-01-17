using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    internal class StyleEnumProperty<T> : StyleProperty
        where T : struct, IConvertible
    {
        public StyleEnumProperty(string name, T value)
            : base(name)
        {
            _enumValue = value;
        }

        public T EnumValue
        {
            get { return _enumValue; }
            set { _enumValue = value; }
        }

        public override string Value
        {
            get
            {
                return _enumValue.ToString();
            }

            set
            {
                Enum.TryParse<T>(_value, out _enumValue);
            }
        }

        internal override bool SetStringValue(string value)
        {
            return Enum.TryParse<T>(value, out _enumValue);
        }

        internal override StyleProperty Clone()
        {
            return new StyleEnumProperty<T>(_name, _enumValue);
        }

        private T _enumValue;
    }
}
