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
        public string ReturnType { get; set; }

        private const string depth = "      ";

        private RubyMethod(string name, bool overloads, bool staticMethod = false, List<String> parameters = null, string returnType = null)
        {
            this.MethodName = name;
            this.Overloads = overloads;
            this.StaticMethod = staticMethod;
            this.Parameters = parameters ?? new List<String>();
            this.ReturnType = returnType;
        }

        public static RubyMethod CreateRubyMethod(MethodInfo method)
        {
            try
            {
                if (method.Name != "Dispose" && method.IsPublic && !method.IsVirtual)
                {
                    var parameters = method.GetParameters().Select(p => char.ToLower(p.ParameterType.Name[0]).ToString() + p.ParameterType.Name.Substring(1)).ToList();
                    var returnType = method.ReturnType.Name;

                    if (method.IsStatic)
                        return new RubyMethod(method.Name, false, true, parameters, returnType);
                    else
                        return new RubyMethod(method.Name, false, false, parameters, returnType);
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
            var returnType = method.ReturnType.Name;

            try
            {
                if (method.IsStatic)
                    return new RubyMethod(method.Name, true, true, null, returnType);
                else
                    return new RubyMethod(method.Name, true, false, null, returnType);
            }
            catch (Exception)
            {
                Debug.WriteLine(method.DeclaringType.Name + ":" + method.Name);
            }

            return null;
        }

        public static RubyMethod CreateRubyConstructor(ConstructorInfo[] ctors)
        {
            if (ctors.Length > 1)
                return new RubyMethod("new", true, true);
            else
            {
                var parameters = ctors[0].GetParameters().Select(p => char.ToLower(p.ParameterType.Name[0]).ToString() + p.ParameterType.Name.Substring(1)).ToList();
                return new RubyMethod("new", false, true, parameters);
            }
        }

        public string write()
        {
            var str = "";
            if (ReturnType != "Void" && ReturnType != null)
                str += depth + "# @return [" + ReturnType + "]\n";
            str += depth + "def ";

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