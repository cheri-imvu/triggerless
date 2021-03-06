using Newtonsoft.Json;
using System;

namespace Triggerless.Tests
{
    static class ObjectHelper
    {
        public static void Dump<T>(this T x)
        {
            string json = JsonConvert.SerializeObject(x, Formatting.Indented);
            Console.WriteLine(json);
        }
    }
}
