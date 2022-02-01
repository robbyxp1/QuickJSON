using QuickJSON.FluentFormatter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //string s = (new QuickJSONFormatter()).Object().V("a", 1).V("b", 2).Get();
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
