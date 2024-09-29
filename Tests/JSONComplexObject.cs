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

    public class JSONTestsComplexObject
    {
        [Test]
        public void JSONComplexObject()
        {
            JObject AllowedFieldsLocJump = new JObject()
            {
                ["SystemAllegiance"] = true,
                ["Powers"] = new JObject(),
                ["SystemEconomy"] = true,
                ["SystemSecondEconomy"] = true,
                ["SystemFaction"] = new JObject
                {
                    ["Name"] = true,
                    ["FactionState"] = true,
                },
                ["SystemGovernment"] = true,
                ["SystemSecurity"] = true,
                ["Population"] = true,
                ["PowerplayState"] = true,
                ["Factions"] = new JArray
                        {
                            new JObject
                            {
                                ["Name"] = true,
                                ["Allegiance"] = true,
                                ["Government"] = true,
                                ["FactionState"] = true,
                                ["Happiness"] = true,
                                ["Influence"] = true,
                                ["ActiveStates"] = new JArray
                                {
                                    new JObject
                                    {
                                        ["State"] = true
                                    }
                                },
                                ["PendingStates"] = new JArray
                                {
                                    new JObject
                                    {
                                        ["State"] = true,
                                        ["Trend"] = true
                                    }
                                },
                                ["RecoveringStates"] = new JArray
                                {
                                    new JObject
                                    {
                                        ["State"] = true,
                                        ["Trend"] = true
                                    }
                                },
                            }
                        },
                ["Conflicts"] = new JArray
                        {
                            new JObject
                            {
                                ["WarType"] = true,
                                ["Status"] = true,
                                ["Faction1"] = new JObject
                                {
                                    ["Name"] = true,
                                    ["Stake"] = true,
                                    ["WonDays"] = true
                                },
                                ["Faction2"] = new JObject
                                {
                                    ["Name"] = true,
                                    ["Stake"] = true,
                                    ["WonDays"] = true
                                },
                            }
                        }
            };

            System.Diagnostics.Debug.WriteLine("" + AllowedFieldsLocJump.ToString().QuoteString());

            string expectedjson = "{\"SystemAllegiance\":true,\"Powers\":{},\"SystemEconomy\":true,\"SystemSecondEconomy\":true,\"SystemFaction\":{\"Name\":true,\"FactionState\":true},\"SystemGovernment\":true,\"SystemSecurity\":true,\"Population\":true,\"PowerplayState\":true,\"Factions\":[{\"Name\":true,\"Allegiance\":true,\"Government\":true,\"FactionState\":true,\"Happiness\":true,\"Influence\":true,\"ActiveStates\":[{\"State\":true}],\"PendingStates\":[{\"State\":true,\"Trend\":true}],\"RecoveringStates\":[{\"State\":true,\"Trend\":true}]}],\"Conflicts\":[{\"WarType\":true,\"Status\":true,\"Faction1\":{\"Name\":true,\"Stake\":true,\"WonDays\":true},\"Faction2\":{\"Name\":true,\"Stake\":true,\"WonDays\":true}}]}";

            Check.That(AllowedFieldsLocJump.ToString()).IsEqualTo(expectedjson);

            string jsonout = AllowedFieldsLocJump.ToString(true);       // round trip it
            JToken decode = JToken.Parse(jsonout);
            Check.That(decode).IsNotNull();
            Check.That(decode.ToString(true)).IsEqualTo(jsonout);
        }


    }
}