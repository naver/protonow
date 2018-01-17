using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    internal class StyleIntegerProperty : StyleProperty
    {
        public StyleIntegerProperty(string name, int value)
            : base(name)
        {
            _intValue = value;
        }

        public int IntegerValue
        {
            get { return _intValue; }
            set { _intValue = value; }
        }

        public override string Value
        {
            get
            {
                return _intValue.ToString();
            }

            set
            {
                Int32.TryParse(value, out _intValue);
            }
        }

        public override int GetIntegerValue(int defaultValue)
        {
            return _intValue;
        }

        internal override bool SetStringValue(string value)
        {
            return Int32.TryParse(value, out _intValue);
        }

        internal override StyleProperty Clone()
        {
            return new StyleIntegerProperty(_name, _intValue);
        }

        private int _intValue;
    }
}
