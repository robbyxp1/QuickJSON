using System;
using QuickJSON;

namespace NetConsole
{
    class Program
    {
        // demonstrate .net 5 QuickJSON
        static void Main(string[] args)
        {
            {
                var f1 = new JSONFormatter();
                f1.Object();
                f1.V("a", 1);
                f1.V("b", 2);
                string sa1 = f1.Get();
                Console.WriteLine(sa1);
            }
            {
                var f1 = new JSONFormatter();
                f1.Array();
                f1.V(1);
                f1.V(2);
                string sa1 = f1.Get();
                Console.WriteLine(sa1);
            }

        }
    }
}
