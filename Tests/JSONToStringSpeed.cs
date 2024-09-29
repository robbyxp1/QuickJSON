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

    public class JSONTestsToStringSpeed
    {

        [Test]
        public void JSONToStringSpeed()
        {

            {
                List<Tuple<string, double, double, double>> d1 = new List<Tuple<string, double, double, double>>();
                for (int i = 0; i < 10000; i++)
                    d1.Add(new Tuple<string, double, double, double>("str" + i, i * 1, i * 2, i * 4));

                JToken jo = JToken.FromObject(d1, true);

                Stopwatch sw = new Stopwatch(); sw.Start();

                StringBuilder str = new StringBuilder();

                JToken.ToStringBuilder(str, jo, "", "", "", false);

                string res = str.ToString();

                var time = sw.ElapsedMilliseconds;
                Check.That(time).IsStrictlyLessThan(100);
                // measured at 6.7/10000 for traditional tostring
                // much faster with string builder
            }

        }


    }
}