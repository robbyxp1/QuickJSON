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
    public class JSONTestBasic
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

                string json234 = "  [ 2,3,4 ] ";
                JToken decoded1a = JToken.Parse(json234);
                Check.That(decoded1a).IsNotNull();
                Check.That(decoded1a.ToString()).Equals("[2,3,4]");

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

                // new in aug 24 - direct adds thru JToken
                JToken jo2 = new JObject();
                jo2.Add("Now", DateTime.UtcNow);       // use implicit conversion
                jo2.Add("ten", 10.23);

                JToken jo2t = jo2;      
                jo2t.Add("Twenty", 20);                 // go thru jtoken for new (aug 24) syntax sugar
                Check.That(jo2.Count).Equals(3);

                DateTime[] starts = new DateTime[] { new DateTime(2015, 1, 1), new DateTime(2020, 1, 2) };  // use newer enumerator adds of basic type
                JArray j0 = new JArray();
                j0.AddRange(starts);
                var now = DateTime.UtcNow;
                j0.Add(now);
                Check.That(j0.Count).Equals(3);

                List<DateTime> extractdatetimes = j0.DateTime();
                Check.That(extractdatetimes.Count).Equals(3);
                Check.That(extractdatetimes[0]).Equals(starts[0]);
                Check.That(extractdatetimes[1]).Equals(starts[1]);      // can't check 2 because precision of passing it thru JSON means ticks are not exactly the same as UtcNow



                JArray ja = new JArray(20.2, 30.3, 40.4);
                Check.That(ja).IsNotNull();
                Check.That(ja.Count).Equals(3);

                JToken jat = ja;
                jat.AddRange(new double[] { 50, 60, 70 });       // test aug 24 add range via token
                Check.That(ja.Count).Equals(6);


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

    }
}