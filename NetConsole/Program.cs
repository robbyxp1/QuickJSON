using System;
using QuickJSON;
using QuickJSON.FluentFormatter;

namespace NetConsole
{
    class Program
    {
        // demonstrate .net QuickJSON
        // the tests are build in 4.8 so demo those

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            JObject jo = new JObject { ["one"] = 1, ["two"] = 2 };
            Console.WriteLine(jo.ToString(true));


            string s = (new JSONFormatter()).Object().V("a", 1).V("b", 2).Get();
            var f1 = (new JSONFormatter());
            f1.Object();
            f1.V("a", 1);
            f1.V(2);
            string sa1 = f1.Get();
        }
    }
}
