using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OSStubsGenerator
{
    /*
     * TODO: Get the constructor information from the type
     * If there are multiple constructors then def initialize(*args)
     * else def initialize(params)
     */

    public class RubyClass
    {
        public string ClassName { get; set; }
        public List<RubyMethod> Methods { get; set; }
        public string BaseName { get; set; }
        public string ModuleName { get; set; }

        private Dictionary<string, int> _methodCount;
        private List<MethodInfo> _methods;
        private ConstructorInfo[] _constructors;

        private string Depth = "\t";

        public RubyClass(Type type, string moduleName)
        {
            ModuleName = moduleName;
            ClassName = type.Name;
            _methodCount = new Dictionary<string, int>();

            _methods = type.GetMethods().Where(m => m.DeclaringType == type && m.IsPublic).ToList();
            Methods = new List<RubyMethod>();
            _constructors = type.GetConstructors();

            Methods.Add(RubyMethod.CreateRubyConstructor(_constructors));
            CountMethods();
            CreateMethods();

            if (type.BaseType.Name != "Object")
            {
                BaseName = type.BaseType.Name;
            }
        }

        private void CountMethods()
        {
            foreach (var methodName in _methods.Select(m => m.Name))
            {
                if (_methodCount.Keys.Contains(methodName))
                    _methodCount[methodName] += 1;
                else
                    _methodCount[methodName] = 1;
            }
        }

        private void CreateMethods()
        {
            foreach (var method in _methods.Where(m => _methodCount[m.Name] == 1))
            {
                var rubyMethod = RubyMethod.CreateRubyMethod(method);
                if (rubyMethod != null)
                    Methods.Add(rubyMethod);
            }

            foreach (var method in _methods.Where(m => _methodCount[m.Name] > 1 && !Methods.Select(rm => rm.MethodName ?? null).Contains(m.Name)))
            {
                var rubyMethod = RubyMethod.CreateOverloadedRubyMethod(method);
                if (rubyMethod != null)
                    Methods.Add(rubyMethod);
            }
        }

        public string write()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("module OpenStudio\n");
            if (ModuleName != null)
            {
                Depth += '\t';
                stringBuilder.Append(String.Format("\tmodule {0}\n", ModuleName));
            }
            
            stringBuilder.Append(Depth + "class " + this.ClassName);

            if (BaseName != null)
                stringBuilder.Append(" < " + this.BaseName);

            stringBuilder.Append("\n");

            foreach (var method in this.Methods)
            {
                if (method != null)
                    stringBuilder.Append(method.write() + "\n");
            }

            stringBuilder.Append(Depth + "end\n");
            stringBuilder.Append("\tend\n");
            stringBuilder.Append("end");

            return stringBuilder.ToString();
        }
    }
}