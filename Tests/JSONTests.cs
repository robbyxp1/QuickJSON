﻿/*
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
    public class JSONTests
    {
        void Dump(string s)
        {
            foreach (var c in s)
            {
                System.Diagnostics.Debug.WriteLine((int)c + ":" + ((int)c > 32 ? c : '?'));
            }
        }
        [Test]
        public void JSONBasic()
        {
            {
                JObject jo = new JObject { ["one"] = "one", ["two"] = "two" };

                Check.That(jo.Contains("one")).IsTrue();

            }
            {
                string jsonemptyobj = "  {   } ";
                JToken decoded1 = JToken.Parse(jsonemptyobj);
                Check.That(decoded1).IsNotNull();
                Check.That(decoded1.ToString()).Equals("{}");

                string jsonemptyarray = "  [   ] ";
                JToken decoded2 = JToken.Parse(jsonemptyarray);
                Check.That(decoded2).IsNotNull();
                Check.That(decoded2.ToString()).Equals("[]");

                string json3 = "  [ {},{}  ] ";
                JToken decoded3 = JToken.Parse(json3);
                Check.That(decoded3).IsNotNull();
                Check.That(decoded3.ToString()).Equals("[{},{}]");

                string json4 = @"  { ""one"":{}, ""two"":{} } ";
                JToken decoded4 = JToken.Parse(json4);
                Check.That(decoded4).IsNotNull();
                Check.That(decoded4.ToString()).Equals(@"{""one"":{},""two"":{}}");

                string json5 = @"{}";
                JToken decoded5 = JToken.Parse(json5);
                Check.That(decoded5).IsNotNull();
                Check.That(decoded5.ToString()).Equals(@"{}");
            }
            {
                //string json = "{ \"timest\\\"amp\":\"2020-06-29T09:53:54Z\", \"bigint\":298182772762562557788377626262773 \"ulong\":18446744073709551615 \"event\":\"FSDJump\t\", \"StarSystem\":\"Shinrarta Dezhra\", \"SystemAddress\":3932277478106, \"StarPos\":[55.71875,17.59375,27.15625], \"SystemAllegiance\":\"PilotsFederation\", \"SystemEconomy\":\"$economy_HighTech;\", \"SystemEconomy_Localised\":\"High Tech\", \"SystemSecondEconomy\":\"$economy_Industrial;\", \"SystemSecondEconomy_Localised\":\"Industrial\", \"SystemGovernment\":\"$government_Democracy;\", \"SystemGovernment_Localised\":\"Democracy\", \"SystemSecurity\":\"$SYSTEM_SECURITY_high;\", \"SystemSecurity_Localised\":\"High Security\", \"Population\":85206935, \"Body\":\"Shinrarta Dezhra\", \"BodyID\":1, \"BodyType\":\"Star\", \"JumpDist\":5.600, \"FuelUsed\":0.387997, \"FuelLevel\":31.612003, \"Factions\":[ { \"Name\":\"LTT 4487 Industry\", \"FactionState\":\"None\", \"Government\":\"Corporate\", \"Influence\":0.288000, \"Allegiance\":\"Federation\", \"Happiness\":\"$Faction_HappinessBand2;\", \"Happiness_Localised\":\"Happy\", \"MyReputation\":0.000000, \"RecoveringStates\":[ { \"State\":\"Drought\", \"Trend\":0 } ] }, { \"Name\":\"Future of Arro Naga\", \"FactionState\":\"Outbreak\", \"Government\":\"Democracy\", \"Influence\":0.139000, \"Allegiance\":\"Federation\", \"Happiness\":\"$Faction_HappinessBand2;\", \"Happiness_Localised\":\"Happy\", \"MyReputation\":0.000000, \"ActiveStates\":[ { \"State\":\"Outbreak\" } ] }, { \"Name\":\"The Dark Wheel\", \"FactionState\":\"CivilUnrest\", \"Government\":\"Democracy\", \"Influence\":0.376000, \"Allegiance\":\"Independent\", \"Happiness\":\"$Faction_HappinessBand2;\", \"Happiness_Localised\":\"Happy\", \"MyReputation\":0.000000, \"PendingStates\":[ { \"State\":\"Expansion\", \"Trend\":0 } ], \"RecoveringStates\":[ { \"State\":\"PublicHoliday\", \"Trend\":0 } ], \"ActiveStates\":[ { \"State\":\"CivilUnrest\" } ] }, { \"Name\":\"Los Chupacabras\", \"FactionState\":\"None\", \"Government\":\"PrisonColony\", \"Influence\":0.197000, \"Allegiance\":\"Independent\", \"Happiness\":\"$Faction_HappinessBand2;\", \"Happiness_Localised\":\"Happy\", \"MyReputation\":0.000000, \"RecoveringStates\":[ { \"State\":\"Outbreak\", \"Trend\":0 } ] } ], \"SystemFaction\":{ \"Name\":\"Pilots' Federation Local Branch\" } }";
                string json = "{ \"timest\\\"am\tp\":\"2020-06-29T09:53:54Z\", \"ulong\":18446744073709551615, \"bigint\":-298182772762562557788377626262773, \"array\":[ 10, 20, 30  ], \"object\":{ \"a\":20, \"b\":30}, \"fred\":20029 }";

                //   string json = "{ \"timestamp\":\"2016-09-27T19:43:21Z\", \"event\":\"Fileheader\", \"part\":1, \"language\":\"English\\\\UK\", \"gameversion\":\"2.2 (Beta 3)\", \"build\":\"r121970/r0 \" }";

                JToken decoded = JToken.Parse(json, out string errortext);
                Check.That(decoded).IsNotNull();
                string outstr = decoded.ToString(true);
                System.Diagnostics.Debug.WriteLine("" + outstr);
                Dump(outstr);

                JToken decoded2 = JToken.Parse(outstr);

                string outstr2 = decoded2.ToString(true);
                System.Diagnostics.Debug.WriteLine("" + outstr2);

                Check.That(outstr).IsEqualTo(outstr2);

                JObject jo = decoded as JObject;
                Check.That(jo).IsNotNull();

                // string j = jo["timest\"am\tp"].Str();
                //  Check.That(j).Equals("2020-06-29T09:53:54Z");

                JArray ja = new JArray(20.2, 30.3, 40.4);
                Check.That(ja).IsNotNull();
                Check.That(ja.Count).Equals(3);

                JArray jb = new JArray("hello", "jim", "sheila");
                Check.That(jb).IsNotNull();
                Check.That(jb.Count).Equals(3);

                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict["fred"] = "one";
                dict["jim"] = "two";
                JObject jod = new JObject(dict);

                Dictionary<string, float> dict2 = new Dictionary<string, float>();
                dict2["fred"] = 20.0f;
                dict2["jim"] = 30f;
                JObject jod2 = new JObject(dict2);
            }
            {
                string json1 = "{ \"timest\\\"am\tp\":\"2020-06-29T09:53:54Z\", \"ulong\"w:18446744073709551615, \"bigint\":-298182772762562557788377626262773, \"array\":[ 10, 20, 30  ], \"object\":{ \"a\":20, \"b\":30}, \"fred\":20029 }";
                JToken jo = JToken.Parse(json1, out string error, JToken.ParseOptions.None);
                Check.That(error).Contains("missing : after");
            }
            {
                string json1 = "{ \"timest\\\"am\tp\":\"2020-06-29T09:53:54Z\", \"ulong\":18446744073709551615, \"bigint\":-298182772762562557788377626262773, \"array\":[ 10, 20, 30  ], \"object\":{ \"a\":20, \"b\":30}, \"fred\":20029 } extra";
                JToken jo = JToken.Parse(json1, out string error, JToken.ParseOptions.CheckEOL);
                Check.That(error).Contains("Extra Chars");
            }

            {
                string json1 = "{ \"timest\\\"am\tp\":\"2020-06-29T09:53:54Z\", \"ulong\":18446744073709551615, \"bigint\":-298182772762562557788377626262773, \"array\":[ 10, 20, 30  ], \"object\":{ \"a\":20, \"b\":30}, \"fred\":20029 } extra";
                try
                {
                    JToken jo = JToken.Parse(json1, out string error, JToken.ParseOptions.CheckEOL | JToken.ParseOptions.ThrowOnError);
                    Check.That(true).IsFalse();
                }
                catch (JToken.JsonException ex)
                {
                    Check.That(ex.Error).Contains("Extra Chars");
                }
            }

            {
                string json2 = @"{ ""data"": {""ver"":2, ""commander"":""Irisa Nyira"", ""fromSoftware"":""EDDiscovery"",  ""fromSoftwareVersion"":""11.7.2.0"", ""p0"": { ""name"": ""Hypo Aeb XF-M c8-0"" },   ""refs"": [ { ""name"": """"Hypua Hypoo MJ-S a72-0"""",  ""dist"": 658.84 } ] }  }";
                JToken jo = JToken.Parse(json2, out string error, JToken.ParseOptions.CheckEOL);
                Check.That(jo == null).IsTrue();
            }

            {
                string quotedstr = "\"quote\"\nHello";
                JObject jo = new JObject();
                jo["str1"] = quotedstr;
                string s = jo.ToString();
                JObject jo1 = JObject.Parse(s);
                Check.That(jo1 != null).IsTrue();
                Check.That(jo1["str1"].Equals(quotedstr));

            }

            {
                JArray ja = new JArray();
                ja.Add(0);
                ja.Add(1);
                var first = ja.Find(a => false);        // check find does not except
                Check.That(first).Equals(null);
            }

            {
                try
                {
                    string js = "{ \"t\":20 }";
                    var jo = JObject.Parse(js, JToken.ParseOptions.ThrowOnError);
                    Check.That(jo).IsNotNull();
                    var ja = JArray.Parse(js);
                    Check.That(ja).IsNull();
                    var jb = JArray.Parse(js, JToken.ParseOptions.ThrowOnError);
                }
                catch (Exception ex)
                {
                    var je = ex as JToken.JsonException;
                    Check.That(je.Error).Equals("Parse is not returning a JArray");

                }
            }

            {
                string js = "{ \"t\":20.2 }";
                var jo = JObject.Parse(js, JToken.ParseOptions.ThrowOnError);
                Check.That(jo).IsNotNull();
                double t1 = jo["t"].Double();
                Check.That(t1).Equals(20.2);
                double? t2 = jo["t2"].DoubleNull();
                Check.That(t2).IsNull();
                double t3 = jo["t2"].Double(30);
                Check.That(t3).Equals(30);
                double t4 = jo["t"].Double(4, 20);
                Check.That(t4).Equals(80.8);
                double t5 = jo["t2"].Double(4, 20);
                Check.That(t5).Equals(20);

            }

            {
                string js = "{ \"t\":20.2 }";
                var jo = JObject.Parse(js, JToken.ParseOptions.ThrowOnError);
                Check.That(jo).IsNotNull();
                float t1 = jo["t"].Float();
                Check.That(t1).Equals(20.2f);
                float? t2 = jo["t2"].FloatNull();
                Check.That(t2).IsNull();
                float t3 = jo["t2"].Float(30);
                Check.That(t3).Equals(30f);
                float t4 = jo["t"].Float(4, 20);
                Check.That(t4).Equals(80.8f);
                float t5 = jo["t2"].Float(4, 20);
                Check.That(t5).Equals(20f);

            }
        }


        [Test]
        public void JSONObject()
        {
            {
                JObject j1 = new JObject();
                j1["One"] = "one";
                j1["Two"] = "two";
                JArray ja = new JArray();
                ja.AddRange(new List<JToken> { "one", "two", 10.23 });
                j1["Array"] = ja;

                System.Diagnostics.Debug.WriteLine("" + j1.ToString().QuoteString());

                string expectedjson = "{\"One\":\"one\",\"Two\":\"two\",\"Array\":[\"one\",\"two\",10.23]}";

                Check.That(j1.ToString()).IsEqualTo(expectedjson);

                StringBuilder str = new StringBuilder();
                JToken.ToStringBuilder(str, j1, ">", "\r\n", "    ", false);
                string res = str.ToString();
                string cmp =
@">{
>    ""One"":""one"",
>    ""Two"":""two"",
>    ""Array"":
>    [
>        ""one"",
>        ""two"",
>        10.23
>    ]
>}
";

                Check.That(res).IsEqualTo(cmp);
            }

            {
                JObject jo = new JObject
                {
                    ["timestamp"] = true,
                    ["event"] = true,
                    ["StarSystem"] = true,
                    ["SystemAddress"] = true,
                };

                System.Diagnostics.Debug.WriteLine("" + jo.ToString().QuoteString());

                string expectedjson = "{\"timestamp\":true,\"event\":true,\"StarSystem\":true,\"SystemAddress\":true}";

                Check.That(jo.ToString()).IsEqualTo(expectedjson);

                int count = 0;
                foreach (KeyValuePair<string, JToken> p in jo)
                {
                    count++;
                }
                Check.That(count).Equals(4);

                JToken basv = jo;

                Check.That(count).Equals(4);

                int count2 = 0;
                foreach (var v1 in basv)
                {
                    count2++;
                }

                Check.That(count).Equals(4);
            }

            {
                JObject jo = new JObject
                {
                    ["timestamp"] = "T0",
                    ["event"] = true,
                    ["StarSystem"] = null,              // this was a miss!
                    ["SystemAddress"] = true,
                };

                Check.That(jo.Count).Equals(4);
                Check.That(jo.Contains("event")).IsTrue();
                Check.That(jo.Contains("SystemAddress")).IsTrue();
                Check.That(jo["StarSystem"].IsNull).IsTrue();
                Check.That(jo["SystemAddress"].IsBool).IsTrue();
                Check.That(jo["timestamp"].IsString).IsTrue();
                Check.That(jo["timestamp"].Str()).Equals("T0");

            }
        }

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

        string jsongithub = @"
        {
          ""url"": ""https://api.github.com/repos/EDDiscovery/EDDiscovery/releases/25769192"",
          ""assets_url"": ""https://api.github.com/repos/EDDiscovery/EDDiscovery/releases/25769192/assets"",
          ""upload_url"": ""https://uploads.github.com/repos/EDDiscovery/EDDiscovery/releases/25769192/assets{?name,label}"",
          ""html_url"": ""https://github.com/EDDiscovery/EDDiscovery/releases/tag/Release_11.4.0"",
          ""id"": 25769192,
          ""node_id"": ""MDc6UmVsZWFzZTI1NzY5MTky"",
          ""tag_name"": ""Release_11.4.0"",
          ""target_commitish"": ""master"",
          ""name"": ""EDDiscovery Release 11.4.0 Material Trader, Scan improvements, lots of others"",
          ""draft"": false,
          ""author"": {
            ""login"": ""robbyxp1"",
            ""id"": 6573992,
            ""node_id"": ""MDQ6VXNlcjY1NzM5OTI="",
            ""avatar_url"": ""https://avatars1.githubusercontent.com/u/6573992?v=4"",
            ""gravatar_id"": """",
            ""url"": ""https://api.github.com/users/robbyxp1"",
            ""html_url"": ""https://github.com/robbyxp1"",
            ""followers_url"": ""https://api.github.com/users/robbyxp1/followers"",
            ""following_url"": ""https://api.github.com/users/robbyxp1/following{/other_user}"",
            ""gists_url"": ""https://api.github.com/users/robbyxp1/gists{/gist_id}"",
            ""starred_url"": ""https://api.github.com/users/robbyxp1/starred{/owner}{/repo}"",
            ""subscriptions_url"": ""https://api.github.com/users/robbyxp1/subscriptions"",
            ""organizations_url"": ""https://api.github.com/users/robbyxp1/orgs"",
            ""repos_url"": ""https://api.github.com/users/robbyxp1/repos"",
            ""events_url"": ""https://api.github.com/users/robbyxp1/events{/privacy}"",
            ""received_events_url"": ""https://api.github.com/users/robbyxp1/received_events"",
            ""type"": ""User"",
            ""site_admin"": false
          },
          ""prerelease"": true,
          ""created_at"": ""2020-04-24T12:32:30Z"",
          ""published_at"": ""2020-04-24T12:37:33Z"",
          ""assets"": [
            {
              ""url"": ""https://api.github.com/repos/EDDiscovery/EDDiscovery/releases/assets/20114552"",
              ""id"": 20114552,
              ""node_id"": ""MDEyOlJlbGVhc2VBc3NldDIwMTE0NTUy"",
              ""name"": ""EDDiscovery.Portable.zip"",
              ""label"": null,
              ""uploader"": {
                ""login"": ""robbyxp1"",
                ""id"": 6573992,
                ""node_id"": ""MDQ6VXNlcjY1NzM5OTI="",
                ""avatar_url"": ""https://avatars1.githubusercontent.com/u/6573992?v=4"",
                ""gravatar_id"": """",
                ""url"": ""https://api.github.com/users/robbyxp1"",
                ""html_url"": ""https://github.com/robbyxp1"",
                ""followers_url"": ""https://api.github.com/users/robbyxp1/followers"",
                ""following_url"": ""https://api.github.com/users/robbyxp1/following{/other_user}"",
                ""gists_url"": ""https://api.github.com/users/robbyxp1/gists{/gist_id}"",
                ""starred_url"": ""https://api.github.com/users/robbyxp1/starred{/owner}{/repo}"",
                ""subscriptions_url"": ""https://api.github.com/users/robbyxp1/subscriptions"",
                ""organizations_url"": ""https://api.github.com/users/robbyxp1/orgs"",
                ""repos_url"": ""https://api.github.com/users/robbyxp1/repos"",
                ""events_url"": ""https://api.github.com/users/robbyxp1/events{/privacy}"",
                ""received_events_url"": ""https://api.github.com/users/robbyxp1/received_events"",
                ""type"": ""User"",
                ""site_admin"": false
              },
              ""content_type"": ""application/x-zip-compressed"",
              ""state"": ""uploaded"",
              ""size"": 11140542,
              ""download_count"": 24,
              ""created_at"": ""2020-04-24T12:35:04Z"",
              ""updated_at"": ""2020-04-24T12:35:13Z"",
              ""browser_download_url"": ""https://github.com/EDDiscovery/EDDiscovery/releases/download/Release_11.4.0/EDDiscovery.Portable.zip""
            },
            {
              ""url"": ""https://api.github.com/repos/EDDiscovery/EDDiscovery/releases/assets/20114548"",
              ""id"": 20114548,
              ""node_id"": ""MDEyOlJlbGVhc2VBc3NldDIwMTE0NTQ4"",
              ""name"": ""EDDiscovery_11.4.0.exe"",
              ""label"": null,
              ""uploader"": {
                ""login"": ""robbyxp1"",
                ""id"": 6573992,
                ""node_id"": ""MDQ6VXNlcjY1NzM5OTI="",
                ""avatar_url"": ""https://avatars1.githubusercontent.com/u/6573992?v=4"",
                ""gravatar_id"": """",
                ""url"": ""https://api.github.com/users/robbyxp1"",
                ""html_url"": ""https://github.com/robbyxp1"",
                ""followers_url"": ""https://api.github.com/users/robbyxp1/followers"",
                ""following_url"": ""https://api.github.com/users/robbyxp1/following{/other_user}"",
                ""gists_url"": ""https://api.github.com/users/robbyxp1/gists{/gist_id}"",
                ""starred_url"": ""https://api.github.com/users/robbyxp1/starred{/owner}{/repo}"",
                ""subscriptions_url"": ""https://api.github.com/users/robbyxp1/subscriptions"",
                ""organizations_url"": ""https://api.github.com/users/robbyxp1/orgs"",
                ""repos_url"": ""https://api.github.com/users/robbyxp1/repos"",
                ""events_url"": ""https://api.github.com/users/robbyxp1/events{/privacy}"",
                ""received_events_url"": ""https://api.github.com/users/robbyxp1/received_events"",
                ""type"": ""User"",
                ""site_admin"": false
              },
              ""content_type"": ""application/x-msdownload"",
              ""state"": ""uploaded"",
              ""size"": 15672578,
              ""download_count"": 55,
              ""created_at"": ""2020-04-24T12:34:52Z"",
              ""updated_at"": ""2020-04-24T12:35:02Z"",
              ""browser_download_url"": ""https://github.com/EDDiscovery/EDDiscovery/releases/download/Release_11.4.0/EDDiscovery_11.4.0.exe""
            }
          ],
          ""tarball_url"": ""https://api.github.com/repos/EDDiscovery/EDDiscovery/tarball/Release_11.4.0"",
          ""zipball_url"": ""https://api.github.com/repos/EDDiscovery/EDDiscovery/zipball/Release_11.4.0"",
          ""body"": ""This is a major overhaul of the Scan panel, addition of Material Trader panels, and general overhaul of lots of the program.\r\n\r\n*** \r\nMajor features\r\n\r\n* Scan panel gets more visuals and a new menu system to select output. Many more options added including distance, star class, planet class, highlighting planets in hab zone. Layout has been optimised.  Since the menu system was reworked all previous selections of display type will need to be reset - use the drop down menu to select them.  The default is everything on.\r\n* UI won't stall when looking up data from EDSM - previous it would stop until EDSM responded. Now just the panel which is asking will stop updating. Rest of the system carries on.\r\n* Material Trader panel added - plan you material trades in advance to see the best outcome before you commit to flying to a trader.\r\n* Surveyor Panel gets many more options for display - show all planets/stars etc and gets more information\r\n* Travel grid, Ships/Loadout, Material Commodities, Engineering, Synthesis get word wrap option to word wrap columns instead of truncating them. Double click on the row now works better expanding/contracting the text.\r\n* Ships/Loadout gets a All modules selection to list all your modules across all ships - useful for engineering\r\n* Synthesis, Engineering and Shopping list panels\r\n\r\nOther Improvements\r\n\r\n* All materials are now classified by Material Group Type\r\n* Improved loading speed when multiple tabbed panels are present - does not sit and use processing time now like it could do\r\n* EDSM Data pick up includes surface gravity\r\n* Journal Missions entry gets faction effects printed\r\n* Can force sell a ship if for some reason your journal has lost the sell ship event\r\n* Various Forms get a close X icon\r\n* Fuel/Reservoir updates are much better, Ships/loadouts auto change with them, and they don't bombard the system with micro changes\r\n* Star Distance panel - fix issue when setting the Max value which could cause it not to look up any stars again\r\n* Workaround for GDI error when saving bitmap\r\n* Bounty Event report correct ship name\r\n* New Y resizer on top of EDD form so you can resize there\r\n* Removed old surface scanner engineering recipes\r\n* Excel output of scan data now works out the value correctly dependent on if you mapped the body\r\n* Can force EDD to use TLS2 \r\n* Asteroid Prospected prints out mats in normal info, Mining refined gets total and type as per MaterialCollected\r\n\r\n***\r\n\r\n|  | EDDiscovery <version>.exe |\r\n|---------|------------------------------------------------------------------|\r\n| SHA-256 | 01D84BF967FE5CDFF2DDC782F0D68FCB4B80F3881EE1F883941454DF9FBB8823 | \r\n\r\n|  |  EDDiscovery.Portable.zip |\r\n|---------|------------------------------------------------------------------|\r\n| SHA-256 | 1D365A30B51280B4676410694C3D1E9F21DF525403E53B735245FD6C7B584DCA |\r\n\r\n![image](https://user-images.githubusercontent.com/6573992/80213091-8d931400-8630-11ea-9f3c-f56d43f7edd8.png)\r\n\r\n\r\n\r\n\r\n""
        }";

        [Test]
        public void JSONGithub()
        {
            JToken decode = JToken.Parse(jsongithub);
            Check.That(decode).IsNotNull();
            string json2 = decode.ToString(true);
            JToken decode2 = JToken.Parse(json2);
            Check.That(decode2).IsNotNull();
            string json3 = decode2.ToString(true);
            Check.That(json2).IsEqualTo(json3);

            var asset = decode["assets"];
            var e1 = asset.FirstOrDefault(func);
            Check.That(e1).IsNotNull();
            Check.That(e1["size"].IsInt).IsTrue();
            Check.That(e1["size1"]?.IsInt ?? true).IsTrue();
            Check.That(e1["size"].Int() == 11140542).IsTrue();
            Check.That(e1["state"].Str() == "uploaded").IsTrue();
        }

        bool func(JToken j)
        {
            if (j["name"].Str().ToLowerInvariant().EndsWith(".zip") && j["name"].Str().ToLowerInvariant().Contains("portable"))
                return true;
            else
                return false;
        }

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

        [Test]
        public void JSONDeepClone()
        {
            JToken decode = JToken.Parse(jsongithub);
            Check.That(decode).IsNotNull();
            JToken copy = decode.Clone();
            Check.That(copy).IsNotNull();
            string json1 = decode.ToString(true);
            string json2 = copy.ToString(true);
            Check.That(json1).Equals(json2);
            System.Diagnostics.Debug.WriteLine(json2);

        }

        [Test]
        public void JSONDeepEquals()
        {
            if (true)
            {
                JToken decode = JToken.Parse(jsongithub);
                Check.That(decode).IsNotNull();
                JToken copy = decode.Clone();
                Check.That(copy).IsNotNull();
                string json1 = decode.ToString(true);
                string json2 = copy.ToString(true);
                Check.That(json1).Equals(json2);
                System.Diagnostics.Debug.WriteLine(json2);

                Check.That(decode.DeepEquals(copy)).IsTrue();
            }

            if (true)
            {
                string json1 = "{\"SystemAllegiance\":true,\"Array\":[10.0,-20.2321212123,-30.232,-30.0],\"String\":\"string\",\"bool\":true}";
                JToken decode1 = JToken.Parse(json1);
                string json1out = decode1.ToString();

                Check.That(json1.Equals(json1out)).IsTrue();

                string json2 = "{\"SystemAllegiance\":true,\"Array\":[10,-20.2321212123,-30.232,-30.0],\"String\":\"string\",\"bool\":1}";
                JToken decode2 = JToken.Parse(json2);

                Check.That(decode1.DeepEquals(decode2)).IsTrue();

                string json3 = "{\"SystemAllegiance\":true,\"Array\":[10,-20.2321212123,-30.232,-30.0],\"String\":\"string\",\"bool\":\"string\"}";
                JToken decode3 = JToken.Parse(json3);

                Check.That(decode1.DeepEquals(decode3)).IsFalse();
            }


            if (true)
            {
                string json1 = @"{""timestamp"":""2016-09-27T19:59:39Z"",""event"":""ShipyardTransfer"",""ShipType"":""FerDeLance"",""ShipID"":15,""System"":""Lembava"",""Distance"":939379235343040512.0,""TransferPrice"":2693097}";
                string json2 = @"{""timestamp"":""2016-09-27T19:59:39Z"",""event"":""ShipyardTransfer"",""ShipType"":""FerDeLance"",""ShipID"":15,""System"":""Lembava"",""Distance"":939379235343040512.0,""TransferPrice"":2693097}";
                JToken decode1 = JToken.Parse(json1);
                JToken decode2 = JToken.Parse(json2);
                Check.That(decode1.DeepEquals(decode2)).IsTrue();
            }
        }

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

        public class AltNameClass
        {
            public string Name;
            [JsonNameAttribute("Category", "Type")]
            public string Category;
            public int Count;
        }

        [Test]
        public void JSONAltNamesToObject()
        {
            {
                string propertyv =
                @"
      {
        ""Count"":0,
        ""Name"":""Name"",
        ""Category"":""Cat""
      }
    ";
                JToken matpro = JToken.Parse(propertyv);
                var AltNameClass = matpro.ToObject<AltNameClass>();
                Check.That(AltNameClass).IsNotNull();
                Check.That(AltNameClass.Name).Equals("Name");
                Check.That(AltNameClass.Count).Equals(0);
                Check.That(AltNameClass.Category).Equals("Cat");


            }
            {
                string propertyv =
                @"
      {
        ""Count"":0,
        ""Name"":""Name"",
        ""Type"":""Cat""
      }
    ";
                JToken matpro = JToken.Parse(propertyv);
                var AltNameClass = matpro.ToObject<AltNameClass>();
                Check.That(AltNameClass).IsNotNull();
                Check.That(AltNameClass.Name).Equals("Name");
                Check.That(AltNameClass.Count).Equals(0);
                Check.That(AltNameClass.Category).Equals("Cat");


            }
            {
                string propertyv =
                @"
      {
        ""Count"":0,
        ""Name"":""Name"",
        ""Fred"":""Cat""
      }
    ";
                JToken matpro = JToken.Parse(propertyv);
                var AltNameClass = matpro.ToObject<AltNameClass>();
                Check.That(AltNameClass).IsNotNull();
                Check.That(AltNameClass.Name).Equals("Name");
                Check.That(AltNameClass.Count).Equals(0);
                Check.That(AltNameClass.Category).IsNull();


            }

        }

        [Test]
        public void JSONBadFormatting()
        {
            {
                string propertyv = @"{ ""timestamp"":""2021-07-13T13:18:55Z"", ""event"":""Status"", ""Flags"":2097152, ""Flags2"":17, ""Oxygen"":1.0," +
                                   @"""Health"":1.0, ""Temperature"":78.0, ""SelectedWeapon"":""$humanoid_fists_name;"", ""SelectedWeapon_Localised"":""Unarmed"", ""Gravity"":0.166399, ""LegalState"":""Clean"", ""Latitude"":3.2, ""Longitude"":6.2, ""Heading"":92.3, ""BodyName"":""Nervi 2g""}";

                JToken matpro = JToken.Parse(propertyv);
                Check.That(matpro).IsNotNull();
            }
            {
                string propertyv = @"{ ""timestamp"":""2021-07-13T13:18:55Z"", ""event"":""Status"", ""Flags"":2097152, ""Flags2"":17, ""Oxygen"":1.0," +
                                   @"""Health"":1.0, ""Temperature"":-nan(ind), ""SelectedWeapon"":""$humanoid_fists_name;"", ""SelectedWeapon_Localised"":""Unarmed"", ""Gravity"":0.166399, ""LegalState"":""Clean"", ""Latitude"":3.2, ""Longitude"":6.2, ""Heading"":92.3, ""BodyName"":""Nervi 2g""}";

                JToken m1 = JToken.Parse(propertyv, out string err, JToken.ParseOptions.AllowTrailingCommas | JToken.ParseOptions.CheckEOL);
                Check.That(m1).IsNull();

                JToken m2 = JToken.Parse(propertyv, out err, JToken.ParseOptions.AllowTrailingCommas | JToken.ParseOptions.CheckEOL | JToken.ParseOptions.IgnoreBadObjectValue);
                Check.That(m2).IsNotNull();
                var e = m2["SelectedWeapon"];
                Check.That(e.Str().Equals("$humanoid_fists_name;")).IsTrue();
            }

            {
                string p1 = @"{ ""array"":[ 20,30,40 ] }";

                JToken m1 = JToken.Parse(p1, out string err, JToken.ParseOptions.AllowTrailingCommas | JToken.ParseOptions.CheckEOL);
                Check.That(m1).IsNotNull();
                Check.That(m1["array"][0].Int()).Equals(20);
                Check.That(m1["array"][1].Int()).Equals(30);
                Check.That(m1["array"][2].Int()).Equals(40);

                string p2 = @"{ ""array"":[ 20,nan     ,40 ] }";

                JToken m2 = JToken.Parse(p2, out err, JToken.ParseOptions.AllowTrailingCommas | JToken.ParseOptions.CheckEOL);
                Check.That(m2).IsNull();

                JToken m3 = JToken.Parse(p2, out err, JToken.ParseOptions.AllowTrailingCommas | JToken.ParseOptions.CheckEOL | JToken.ParseOptions.IgnoreBadArrayValue);
                Check.That(m3).IsNotNull();
                Check.That(m3["array"][0].Int()).Equals(20);
                Check.That(m3["array"][1].IsNull).IsTrue();
                Check.That(m3["array"][2].Int()).Equals(40);

                string p4 = @"{ ""array"":[ 20,30, nana ] }";

                JToken m4 = JToken.Parse(p4, out err, JToken.ParseOptions.AllowTrailingCommas | JToken.ParseOptions.CheckEOL | JToken.ParseOptions.IgnoreBadArrayValue);
                Check.That(m4).IsNotNull();
                Check.That(m4["array"][0].Int()).Equals(20);
                Check.That(m4["array"][1].Int()).Equals(30);
                Check.That(m4["array"][2].IsNull).IsTrue();


            }
        }


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