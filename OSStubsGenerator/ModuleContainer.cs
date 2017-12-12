using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSStubsGenerator
{
    public class ModuleContainer
    {
        public List<ModuleInfo> modules { get; set; }
    }

    public class ModuleInfo
    {
        public string moduleName { get; set; }
        public List<string> classes { get; set; }
    }
}
