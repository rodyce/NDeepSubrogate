using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spring.Stereotype;

namespace ContextualMocks.Tests.DummyServices
{
    [Service]
    public class ConcreteClassService
    {
        public virtual int AddMod(int n1, int n2, int modulo)
        {
            return (n1 + n2) % modulo;
        }
    }
}
