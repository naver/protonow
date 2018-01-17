using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestDocument
{
    abstract class TestCase
    {
        public TestCase(string caseName)
        {
            _caseName = caseName;
        }

        public void Run()
        {
            try
            {
                RunInternal();
            }
            catch(Exception exp)
            {
                Console.WriteLine("Test case " + _caseName + " is failed!!!!!!!!");
                Console.WriteLine("Exception : " + exp.Message);
                return;
            }

            Console.WriteLine("Test case " + _caseName + " is successful. Shuang A~~~~~~~");
        }

        protected abstract void RunInternal();

        public abstract string Description { get; }

        protected string _caseName;
    }
}
