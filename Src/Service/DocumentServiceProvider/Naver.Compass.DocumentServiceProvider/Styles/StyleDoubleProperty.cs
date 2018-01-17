using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    internal class StyleDoubleProperty : StyleProperty
    {
        public StyleDoubleProperty(string name, double value)
            : base(name)
        {
            _doubleValue = value;
        }

        public double DoubleValue
        {
            get { return _doubleValue; }
            set { _doubleValue = value; }
        }

        public override string Value
        {
            get
            {
                return _doubleValue.ToString();
            }

            set
            {
                Double.TryParse(value, out _doubleValue);
            }
        }

        public override double GetDoubleValue(double defaultValue)
        {
            return _doubleValue;
        }

        internal override bool SetStringValue(string value)
        {
            return Double.TryParse(value, out _doubleValue);
        }

        internal override StyleProperty Clone()
        {
            return new StyleDoubleProperty(_name, _doubleValue);
        }

        private double _doubleValue;
    }
}
