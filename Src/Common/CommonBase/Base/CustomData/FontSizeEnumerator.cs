using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Common.CommonBase
{
    public class FontSizeEnumerator
    {
        private static List<double> _Sizelist = null;
        public static List<double> GetSizeList()
        {
            if (_Sizelist == null)
           {
               _Sizelist = new List<double>();
               _Sizelist.Add(8);
               _Sizelist.Add(9);
               _Sizelist.Add(10);
               _Sizelist.Add(11);
               _Sizelist.Add(12);
               _Sizelist.Add(13);
               _Sizelist.Add(14);
               _Sizelist.Add(16);
               _Sizelist.Add(18);
               _Sizelist.Add(20);
               _Sizelist.Add(22);
               _Sizelist.Add(24);
               _Sizelist.Add(28);
               _Sizelist.Add(32);
               _Sizelist.Add(36);
               _Sizelist.Add(40);
               _Sizelist.Add(44);
               _Sizelist.Add(48);
               _Sizelist.Add(54);
               _Sizelist.Add(60);
               _Sizelist.Add(66);
               _Sizelist.Add(72);
               _Sizelist.Add(80);
               _Sizelist.Add(88);
               _Sizelist.Add(96);
           }
            return _Sizelist;
        }

        public static double GetNext(double value)
        {
            List<double> collect = GetSizeList();

           if (value >= collect[collect.Count - 1])
            {
                value = value + 10;

                return (value >= 1000) ? 1000 : value;
            }
            else
            {
                
                foreach (double data in collect)
                {
                    if (value < data)
                    {
                        return data;
                    }
                }
                return collect[collect.Count - 1];
                    
            }
        }

        public static double GetPrevious(double value)
        {

            List<double> collect = GetSizeList();

           if (value > collect[collect.Count - 1])
            {
                value = value - 10;

                return (value <= collect[collect.Count - 1]) ? collect[collect.Count - 1] : value;
                
            }
            else
            {
                int index = collect.Count - 1;
                double data = 0.0001;
                for (; index >= 0; index--)
                {
                    data = collect[index];
                    if (value > data)
                    {
                        return data;
                    }
                }

                return collect[0];
            }
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
