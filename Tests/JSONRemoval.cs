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

    public class JSONTestsRemoval
    {

        [Test]
        public void JSONRemove()
        {
            JObject obj = new JObject()
            {
                ["Factions"] = new JArray
                        {
                            new JObject
                            {
                                ["Faction"] = "1",
                                ["MyReputation"] = "Good",
                                ["Otherstuff"] = "Good",
                            },
                            new JObject
                            {
                                ["Faction"] = "2",
                                ["MyReputation"] = "Good",
                                ["Otherstuff"] = "Good",
                            },
                        }
            };

            System.Diagnostics.Debug.WriteLine("" + obj.ToString().QuoteString());

            string expectedjson = "{\"Factions\":[{\"Faction\":\"1\",\"MyReputation\":\"Good\",\"Otherstuff\":\"Good\"},{\"Faction\":\"2\",\"MyReputation\":\"Good\",\"Otherstuff\":\"Good\"}]}";

            Check.That(obj.ToString()).IsEqualTo(expectedjson);

            JArray factions = obj["Factions"] as JArray;

            if (factions != null)
            {
                foreach (JObject faction in factions)
                {
                    faction.Remove("MyReputation");
                }
            }

            System.Diagnostics.Debug.WriteLine(obj.ToString().QuoteString());

            string expectedjson2 = "{\"Factions\":[{\"Faction\":\"1\",\"Otherstuff\":\"Good\"},{\"Faction\":\"2\",\"Otherstuff\":\"Good\"}]}";

            Check.That(obj.ToString()).IsEqualTo(expectedjson2);


            {
                JObject obj2 = new JObject()
                {
                    ["Factions"] = "f1",
                    ["Json"] = "j1",
                    ["Facoids"] = "f2",
                    ["Freds"] = "j1",
                };

                Check.That(obj2.Contains("Factions")).IsTrue();
                Check.That(obj2.Contains("Json")).IsTrue();
                Check.That(obj2.Contains("Facoids")).IsTrue();
                Check.That(obj2.Contains("Freds")).IsTrue();

                obj2.RemoveWildcard("Fac*");

                Check.That(obj2.Contains("Factions")).IsFalse();
                Check.That(obj2.Contains("Json")).IsTrue();
                Check.That(obj2.Contains("Facoids")).IsFalse();
                Check.That(obj2.Contains("Freds")).IsTrue();

            }

        }

    }
}