using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;

namespace Naver.Compass.Common.CommonBase
{
    public class NumericOnlyValidationRule : ValidationRule
    {
        public NumericOnlyValidationRule()
        {
            Max = Int32.MaxValue;
            Min = Int32.MinValue;
        }

        public int Max { get; set; }

        public int Min { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int result = 0;
            if(!Int32.TryParse(value.ToString(), out result))
            {
                return new ValidationResult(false, "Value is not a numeric.");
            }

            if (result < Min || result > Max)
            {
                return new ValidationResult(false, "Not in the range " + Min + " to " + Max + ".");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}
