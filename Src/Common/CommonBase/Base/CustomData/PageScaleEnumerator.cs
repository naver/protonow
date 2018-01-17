using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Common.CommonBase
{
    public class PageScaleEnumerator
    {

        private static List<double> _ScaleList = null;

        public static List<double> GetSizeList()
        {
            if (_ScaleList == null)
            {
                _ScaleList = new List<double>();
                _ScaleList.Add(0.1);
                _ScaleList.Add(0.25);
                _ScaleList.Add(0.33);
                _ScaleList.Add(0.5);
                _ScaleList.Add(0.65);
                _ScaleList.Add(0.8);
                _ScaleList.Add(1);
                _ScaleList.Add(1.25);
                _ScaleList.Add(1.5);
                _ScaleList.Add(2);
                _ScaleList.Add(3);
                _ScaleList.Add(4);
            }

            return _ScaleList;
        }

        public static double GetNext(double value)
        {
            List<double> collect = GetSizeList();

            foreach (double data in collect)
            {
                if (value < data)
                {
                    return data;
                }
            }

            return collect[collect.Count - 1];
        }

        public static double GetPrevious(double value)
        {
            List<double> collect = GetSizeList();
           
            int index = collect.Count-1;

            for (; index>=0;index--)
            {
                if (value > collect[index])
                {
                    return collect[index];
                }
            }

            return collect[0];
        }


        public static double GetValue(double value, bool bAdd)
        {
            if (bAdd)
            {
                return GetNext(value);
            }
            else
            {
                return GetPrevious(value);
            }
        }

    }
}
