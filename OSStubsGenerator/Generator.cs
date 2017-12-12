using OpenStudio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OSStubsGenerator
{
    internal class StubsGenerator
    {
        public List<RubyClass> RubyClasses { get; set; }
        public List<Type> Types { get; set; }
        public ModuleContainer ModuleContainer { get; set; }

        public StubsGenerator(ModuleContainer moduleContainer)
        {
            ModuleContainer = moduleContainer;
            Types = new List<Type>();

            // There only seems to be one namespace for the C# bindings so the assmebly can be grabbed from any Type
            Assembly assembly = typeof(Model).Assembly;
            RubyClasses = new List<RubyClass>();

            // Generate and add classes for each module
            foreach (ModuleInfo moduleInfo in ModuleContainer.modules)
	        {
		        Types = GetTypesInNamespace(typeof(Model).Assembly, "OpenStudio", moduleInfo);

                foreach (var type in Types)
                {
                    RubyClasses.Add(new RubyClass(type, moduleInfo.moduleName));
                }
	        }


        }

        private List<Type> GetTypesInNamespace(Assembly assembly, string nameSpace, ModuleInfo moduleInfo)
        {
            var classes = moduleInfo.classes;
            return assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).Where(s => classes.Contains(s.Name)).ToList();
        }

        public void CreateStubs()
        {
            var require_string = new StringBuilder();
            foreach (var rubyClass in RubyClasses)
            {
                System.IO.Directory.CreateDirectory("Stubs");
                require_string.Append(String.Format("require 'Classes/{0}'\n", rubyClass.ClassName));
                RubyClassToText(rubyClass);
            }

            System.IO.File.WriteAllText("Stubs/openstudio.rb", require_string.ToString());
        }

        private static void RubyClassToText(RubyClass rubyClass)
        {
            System.IO.Directory.CreateDirectory("Stubs/Classes");
            var text = rubyClass.write();
            System.IO.File.WriteAllText("Stubs/Classes/" + rubyClass.ClassName + ".rb", text);
        }
    }
}