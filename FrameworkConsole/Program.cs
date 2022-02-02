using QuickJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkConsole
{
    class Program
    {
        // demo of net48 JSON

        static void Main(string[] args)
        {
            {
                var f1 = new JSONFormatter();
                f1.Array();
                f1.Object();
                f1.V("one", 1);
                f1.V("two", 2);
                var x1 = f1.CurrentText;
                System.Diagnostics.Debug.WriteLine($"Json is {x1}");
                f1.Object("three");
                f1.V("threeone", "A");
                f1.V("threetwo", "B");
                f1.Close();
                f1.V("four", 4);
                string sa1 = f1.Get();
            }


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
