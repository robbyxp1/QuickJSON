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

    public class JSONTestsRoundTrip
    {

        [Test]
        public void JSONRoundtripdouble()
        {
            {
                JObject jo = new JObject()
                {
                    ["a"] = 10,
                    ["b"] = 1029292882892.2,
                    ["c"] = double.MinValue,
                    ["d"] = double.MaxValue,
                    ["e"] = 10E10,
                };

                string s = jo.ToString();
                JObject ji = JObject.Parse(s);

                Check.That(jo["a"].Double()).Equals(ji["a"].Double());
                Check.That(ji["c"].Double()).Equals(double.MinValue);
                Check.That(ji["d"].Double()).Equals(double.MaxValue);
            }
        }



    }
}