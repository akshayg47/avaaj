using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avaaj
{
    public class MethodUnderTest
    {
        public string MethodName { get; set; }

        public List<string> ParameterTypes { get; set; }

        public List<MethodEntityToBeArranged> MethodsToBeArranged { get; set; }
    }
}
