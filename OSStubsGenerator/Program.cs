using System;
using Newtonsoft.Json;
using System.IO;

namespace OSStubsGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var serializer = new JsonSerializer();
            var modules = serializer.Deserialize<ModuleContainer>(new JsonTextReader(new StreamReader("../../../resources/classes.json")));

            var stubsGenerator = new StubsGenerator(modules);
            stubsGenerator.CreateStubs();
        }
    }
}