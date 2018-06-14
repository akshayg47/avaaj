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

        public MethodUnderTest MethodUnderTest { get; set; }

        public List<string> NameSpacesToBeIncluded { get; set; }
    }
}
