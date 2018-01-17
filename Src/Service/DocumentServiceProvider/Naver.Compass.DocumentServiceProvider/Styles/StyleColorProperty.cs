using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    internal class StyleColorProperty : StyleProperty
    {
        public StyleColorProperty(string name, StyleColor color)
            : base(name)
        {
            _color = new StyleColor(color);
        }

        public StyleColor ColorValue
        {
            get { return _color; }
            set
            {
                _color = new StyleColor(value);
                _value = null;
            }
        }

        public override string Value
        {
            get 
            {
                if (_value == null)
                {
                    _value = _color.ToString();
                }

                return _value;
            }
            set
            {
                // TODO : Convert string to StyleColor
                //_color = new StyleColor(value);
                _value = value;
            }
        }

        internal override bool SetStringValue(string value)
        {
            // TODO : Convert string to StyleColor
            return false;
        }

        internal override StyleProperty Clone()
        {
            return new StyleColorProperty(_name, _color);
        }

        private StyleColor _color;
    }
}
