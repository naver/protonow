using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    internal class StyleProperty : IStyleProperty
    {
        public StyleProperty(string name)
            : this(name, null)
        {                    
        }

        public StyleProperty(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public string Name 
        {
            get { return _name; }
            set
            {
                throw new NotSupportedException("Cannot change the name of style property!");
            }
        }

        public virtual string Value 
        {
            get { return _value; }
            set { _value = value; }
        }

        public virtual bool GetBooleanValue(bool defaultValue)
        {
            bool result;
            if (Boolean.TryParse(Value, out result))
            {
                return result;
            }

            return defaultValue;
        }

        public virtual double GetDoubleValue(double defaultValue)
        {
            double result;
            if (Double.TryParse(Value, out result))
            {
                return result;
            }

            return defaultValue;
        }

        public virtual T GetEnumValue<T>(T defaultValue) where T : struct, IConvertible
        {
            T result;
            if (Enum.TryParse<T>(Value, out result))
            {
                return result;
            }

            return defaultValue;
        }

        public virtual int GetIntegerValue(int defaultValue)
        {
            int result;
            if (Int32.TryParse(Value, out result))
            {
                return result;
            }

            return defaultValue;
        }

        public virtual StyleColor GetStyleColorValue(StyleColor defaultValue)
        {
            // TODO : Convert string to StyleColor
            return defaultValue;
        }

        internal virtual bool SetStringValue(string value)
        {
            _value = value;
            return true;
        }

        internal virtual StyleProperty Clone()
        {
            return new StyleProperty(_name, _value);
        }

        protected string _name;
        protected string _value;
    }
}
