using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace OSStubsGenerator
{
    public class RubyMethod
    {
        public string MethodName { get; set; }
        public List<string> Parameters { get; set; }
        public bool Overloads { get; set; }
        public bool StaticMethod { get; set; }
        private const string depth = "\t\t\t";

        private RubyMethod(string name, bool overloads, bool staticMethod = false, List<String> parameters = null)
        {
            this.MethodName = name;
            this.Overloads = overloads;
            this.StaticMethod = staticMethod;
            this.Parameters = parameters ?? new List<String>();
        }

        public static RubyMethod CreateRubyMethod(MethodInfo method)
        {
            try
            {
                if (method.Name != "Dispose" && method.IsPublic && !method.IsVirtual)
                {
                    var parameters = method.GetParameters().Select(p => char.ToLower(p.ParameterType.Name[0]).ToString() + p.ParameterType.Name.Substring(1)).ToList();
                    //method.GetParameters().Where(p => p.)

                    if (method.IsStatic)
                        return new RubyMethod(method.Name, false, true, parameters);
                    else
                        return new RubyMethod(method.Name, false, false, parameters);
                }
            }
            catch (Exception)
            {
                Debug.WriteLine(method.DeclaringType.Name + ":" + method.Name);
            }

            return null;
        }

        public static RubyMethod CreateOverloadedRubyMethod(MethodInfo method)
        {
            try
            {
                if (method.IsStatic && method.IsPublic && !method.IsVirtual)
                    return new RubyMethod(method.Name, true, true);
                else
                    return new RubyMethod(method.Name, true);
            }
            catch (Exception)
            {
                Debug.WriteLine(method.DeclaringType.Name + ":" + method.Name);
            }

            return null;
        }

        public static RubyMethod CreateRubyConstructor(ConstructorInfo[] ctors)
        {
            if (ctors.Length > 1 || ctors.Length == 0)
                return new RubyMethod("initialize", true, false);
            else
            {
                var parameters = ctors[0].GetParameters().Select(p => char.ToLower(p.ParameterType.Name[0]).ToString() + p.ParameterType.Name.Substring(1)).ToList();
                return new RubyMethod("initialize", false, false, parameters);
            }
        }

        public string write()
        {
            var str = depth + "def ";

            if (StaticMethod)
                str += "self.";

            if (Overloads)
            {
                return str += MethodName + "(*args)\n" + depth + "end";
            }
            else if (Parameters.Count == 0)
            {
                return str += MethodName + "\n" + depth + "end";
            }
            else
            {
                str += MethodName + "(";
                var count = 1;
                foreach (var parameter in Parameters)
                {
                    str += parameter;
                    if (count < Parameters.Count)
                    {
                        str += ", ";
                    }
                    count++;
                }

                str += ")\n" + depth + "end";
                return str;
            }
        }
    }
}