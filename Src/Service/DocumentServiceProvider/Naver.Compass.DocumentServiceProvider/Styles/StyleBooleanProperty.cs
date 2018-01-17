using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    internal class StyleBooleanProperty : StyleProperty
    {
        public StyleBooleanProperty(string name, bool value)
            : base(name)
        {
            _booleanValue = value;
        }

        public bool BooleanValue
        {
            get { return _booleanValue; }
            set { _booleanValue = value; }
        }

        public override string Value
        {
            get
            {
                return _booleanValue.ToString();
            }

            set
            {
                Boolean.TryParse(value, out _booleanValue);
            }
        }

        public override bool GetBooleanValue(bool defaultValue)
        {
            return _booleanValue;
        }

        internal override bool SetStringValue(string value)
        {
            return Boolean.TryParse(value, out _booleanValue);
        }

        internal override StyleProperty Clone()
        {
            return new StyleBooleanProperty(_name, _booleanValue);
        }

        private bool _booleanValue;
    }
}
