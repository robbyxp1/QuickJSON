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

    public class JSONTestsArray
    {
        [Test]
        public void JSONArray()
        {
            JArray ja = new JArray
                        {
                            "one",
                            "two",
                            "three",
                            new JObject()
                            {
                                ["SystemAllegiance"] = true,
                            }
                        };

            System.Diagnostics.Debug.WriteLine("" + ja.ToString().QuoteString());

            string expectedjson = "[\"one\",\"two\",\"three\",{\"SystemAllegiance\":true}]";
            string str = ja.ToString();
            Check.That(str).IsEqualTo(expectedjson);

            //     string s = ja.Find<JString>(x => x is JString && ((JString)x).Value.Equals("one"))?.Value;

            //    Check.That(s).IsNotNull().Equals("one");

            JObject o = ja.Find<JObject>(x => x is JObject);
            Check.That(o).IsNotNull();

            int i1 = ja[0].Int(-1);
            Check.That(i1 == -1).IsTrue();

            int count = 0;
            foreach (var v1 in ja)
            {
                count++;
            }
            Check.That(count).Equals(4);
        }
    }

}