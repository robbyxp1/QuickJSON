/*
 * Copyright © 2021-2023 Robbyxp1 @ github.com
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 */

using NFluent;
using NUnit.Framework;
using QuickJSON;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Tests;

namespace JSONTests
{
    [TestFixture(TestOf = typeof(JToken))]

    public class JSONTestsSpeed
    {

        struct FileLines
        {
            public string[] filelines;
        }
        [Test]
        public void JSONSpeed()
        {
            string[] files = Directory.EnumerateFiles(@"C:\Users\RK\Saved Games\Frontier Developments\Elite Dangerous", "Journal.*.log").ToArray();

            List<FileLines> filelines = new List<FileLines>();

            foreach (var f in files)
            {
                // System.Diagnostics.Debug.WriteLine("Check " + f);
                string[] lines = File.ReadAllLines(f);
                filelines.Add(new FileLines { filelines = lines });
            }

            System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
            st.Start();

            foreach (var fl in filelines)
            {
                foreach (var l in fl.filelines)
                {
                    JObject t = JObject.Parse(l, out string error, JToken.ParseOptions.CheckEOL);
                    Check.That(t).IsNotNull();
                    JObject t2 = JObject.Parse(l, out string error2, JToken.ParseOptions.CheckEOL);
                    Check.That(t2).IsNotNull();
                    JObject t3 = JObject.Parse(l, out string error3, JToken.ParseOptions.CheckEOL);
                    Check.That(t3).IsNotNull();
                    JObject t4 = JObject.Parse(l, out string error4, JToken.ParseOptions.CheckEOL);
                    Check.That(t4).IsNotNull();
                    JObject t5 = JObject.Parse(l, out string error5, JToken.ParseOptions.CheckEOL);
                    Check.That(t5).IsNotNull();
                    JObject t6 = JObject.Parse(l, out string error6, JToken.ParseOptions.CheckEOL);
                    Check.That(t6).IsNotNull();
                }

            }

            long time = st.ElapsedMilliseconds;
            System.Diagnostics.Debug.WriteLine("Read journals took " + time);

        }


    }
}