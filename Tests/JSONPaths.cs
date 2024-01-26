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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.DataVisualization.Charting;
using Tests;

namespace JSONTests
{
    [TestFixture(TestOf = typeof(JToken))]
    public class JSONPaths
    {

        [Test]
        public void JSONPathsTest()
        {
            {
                JObject token = new JObject
                {
                    ["timestamp"] = "T0",
                    ["event"] = new JArray() { 10, 20, new JObject() { ["fred"] = 10, ["jim"] = 20 } },
                    ["StarSystem"] = null,              // this was a miss!
                    ["SystemAddress"] = new JObject()
                    {
                        ["One"] = 1,
                        ["Two"] = 2,
                    }
                };

                JToken objrvalue0 = token["event"][2]["fred"];

                JToken obj = token.GetToken(".event[2].fred");

                Check.That(obj == objrvalue0).IsTrue();

                obj = token.GetToken(".timestamp");

                Check.That(obj == token["timestamp"]).IsTrue();

                obj = token.GetTokenSchemaPath("SystemAddress/One");

                Check.That(obj == token["SystemAddress"]["One"]).IsTrue();
            }

            {
                JArray token = new JArray
                {
                    new JObject
                    {
                        ["timestamp"] = "T0",
                        ["event"] = new JArray() { 10, 20, new JObject() { ["fred"] = 10, ["jim"] = 20 } },
                        ["StarSystem"] = null,              // this was a miss!
                        ["SystemAddress"] = true,
                    },
                    10,
                    20,
                };

                JToken objrvalue0 = token[0]["event"][2]["fred"];

                JToken obj = token.GetToken("[0].event[2].fred");

                Check.That(obj == objrvalue0).IsTrue();

                obj = token.GetToken("[0].timestamp");

                Check.That(obj == token[0]["timestamp"]).IsTrue();

                //string ps = objrvalue0.GetPath();
                //Check.That(ps).Equals("$[0].RValue");

                //JToken objrvalue1 = token[1]["RValue"];
                //ps = objrvalue1.GetPath();
                //Check.That(ps).Equals("$[1].RValue");

            }
        }


    }
}