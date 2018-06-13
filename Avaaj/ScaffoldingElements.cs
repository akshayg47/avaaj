using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avaaj
{
    public class ScaffoldingElements
    {
        public string ContainingClassName { get; set; }

        public string MethodUnderTest { get; set; }

        public List<string> NameSpacesToBeIncluded { get; set; }

        public List<MethodEntityTobeArranged> MethodsTobeArranged { get; set; }
    }
}
