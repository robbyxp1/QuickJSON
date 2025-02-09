using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickJSON;

namespace FrameworkSingleFile
{
    internal class Program
    {
        static void Main(string[] args)
        {
            {
                var o1 = new JObject();
                o1["one"] = 1;
                o1["two"] = 1;
                string sa1 = o1.ToString(true);
                System.Diagnostics.Debug.WriteLine($"Output : {sa1}");
                Console.WriteLine($"Output : {sa1}");
            }

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
                Console.WriteLine($"Output : {sa1}");
            }

         }
    }
}
