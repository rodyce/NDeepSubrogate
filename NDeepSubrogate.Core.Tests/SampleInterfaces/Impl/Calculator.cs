using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDeepSubrogate.Core.Tests.SampleInterfaces.Impl
{
    class Calculator : ICalculator
    {
        public double Add(double a, double b)
        {
            return a + b;
        }

        public int ModularAdd(int a, int b, int c)
        {
            return (a + b)%c;
        }

        public double Divide(double a, double b)
        {
            return a/b;
        }
    }
}
