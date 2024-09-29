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
    public partial class JSONToObjectTests
    {
        public class OtherTypes
        {
            public long? one;
            public uint two;
            public uint? three;
            public uint? four;
            public ulong? five;
            public ulong six;
            public double? seven;
            public bool? eight;
            public float? nine;
            public DateTime ten;
            public DateTime? eleven;
        }


        public enum TestEnum { one, two, three };
        public class FromObjectTest
        {
            public TestEnum t1;
        }

        public class Material
        {
            public string Name { get; set; }        //FDNAME
            public string Name_Localised { get; set; }
            public string FriendlyName { get; set; }        //friendly
            public int Count { get; set; }

            public void Normalise()
            {
            }
        }

        public class Materials
        {
            public int Count;
            public string Name;
            public string Name_Localised;
            public string FriendlyName;
            public string Category;
            public System.Drawing.Bitmap fred;

            [JsonName("QValue")]     // checking for json name override for fields and properties
            public int? qint;
            [JsonName("RValue")]
            public int rint { get; set; }

            [JsonIgnore]
            public int PropGet { get; }

            public int PropGetSet { get; set; }
        }


        public class ProgressInformation
        {
            public string Engineer { get; set; }
            public long EngineerID { get; set; }
            public int? Rank { get; set; }       // only when unlocked
            public string Progress { get; set; }
            public int? RankProgress { get; set; }  // newish 3.x only when unlocked
        }

        public class SimpleTest
        {
            public string one;
            public string two;
            public int three;
            public bool four;
        }
        public class IgnoreTest
        {
            [JsonIgnore()]
            public string one;
            public string two;
            public int three;
            public bool four;
        }

        public class Unlocked
        {
            public string Name;
            public string Name_Localised;
        }

        public class Commodities
        {
            public string Name;
            public string Name_Localised;
            public string FriendlyName;
            public int Count;
        }

        public class MaterialIncorrect
        {
            public double Name { get; set; }        //FDNAME
            public string Name_Localised { get; set; }
            public string FriendlyName { get; set; }        //friendly
            public int Count { get; set; }

            public void Normalise()
            {
            }
        }




        [Test]
        public void JSONToObjectRedirectedNames()
        {
            {
                string jmd = @"
{
  ""timestamp"": ""2018 - 04 - 24T21: 25:46Z"",
  ""event"": ""TechnologyBroker"",
  ""BrokerType"": ""guardian"",
  ""MarketID"": 3223529472,
  ""ItemsUnlocked"": [
    {
      ""Name"": ""Int_GuardianPowerplant_Size2"",
      ""Name_Localised"": ""Guardian Power Plant""
    },
    {
      ""Name"": ""Int_GuardianPowerplant_Size3"",
      ""Name_Localised"": ""$Int_GuardianPowerplant_Size2_Name;""
    },
    {
      ""Name"": ""Int_GuardianPowerplant_Size4"",
      ""Name_Localised"": ""$Int_GuardianPowerplant_Size2_Name;""
    },
    {
      ""Name"": ""Int_GuardianPowerplant_Size5"",
      ""Name_Localised"": ""$Int_GuardianPowerplant_Size2_Name;""
    },
    {
      ""Name"": ""Int_GuardianPowerplant_Size6"",
      ""Name_Localised"": ""$Int_GuardianPowerplant_Size2_Name;""
    },
    {
      ""Name"": ""Int_GuardianPowerplant_Size7"",
      ""Name_Localised"": ""$Int_GuardianPowerplant_Size2_Name;""
    },
    {
      ""Name"": ""Int_GuardianPowerplant_Size8"",
      ""Name_Localised"": ""$Int_GuardianPowerplant_Size2_Name;""
    }
  ],
  ""Commodities"": [
    {
      ""Name"": ""powergridassembly"",
      ""Name_Localised"": ""Energy Grid Assembly"",
      ""Count"": 10
    }
  ],
  ""Materials"": [
   {
      ""Name"": ""heatresistantceramics"",
      ""Name_Localised"": ""Heat Resistant Ceramics"",
      ""Count"": 30,
      ""Category"": ""Manufactured"",
      ""QValue"": 20,
      ""RValue"": 2000
    },
    {
      ""Name"": ""guardian_moduleblueprint"",
      ""Name_Localised"": ""Guardian Module Blueprint Segment"",
      ""Count"": 4,
      ""Category"": ""Encoded""
    },
    {
      ""Name"": ""guardian_powerconduit"",
      ""Name_Localised"": ""Guardian Power Conduit"",
      ""Count"": 36,
      ""Category"": ""Manufactured""
    },
    {
      ""Name"": ""ancienttechnologicaldata"",
      ""Name_Localised"": ""Pattern Epsilon Obelisk Data"",
      ""Count"": 42,
      ""Category"": ""Encoded""
    }
   ]
}";


                JToken decode = JToken.Parse(jmd);
                Check.That(decode).IsNotNull();
                string json1 = decode.ToString(true);
                System.Diagnostics.Debug.WriteLine(json1);

                var MaterialList = decode["Materials"].ToObject<Materials[]>();
                Check.That(MaterialList).IsNotNull();
                Check.That(MaterialList.Length).IsEqualTo(4);
                Check.That(MaterialList[0].qint).IsEqualTo(20);
                Check.That(MaterialList[0].rint).IsEqualTo(2000);

                var ItemsUnlocked1 = decode["WrongNameItemsUnlocked"].ToObject(typeof(Unlocked[]), false);
                Check.That(ItemsUnlocked1).IsNull();
                var ItemsUnlocked = decode["ItemsUnlocked"].ToObject(typeof(Unlocked[]), false);
                Check.That(ItemsUnlocked).IsNotNull();
                var CommodityList = decode["Commodities"].ToObject<Commodities[]>();
                Check.That(CommodityList).IsNotNull();


            }
        }

        [Test]
        public void JSONToObjectIgnoreTest()
        {
            string json = "{ \"one\":\"one\", \"two\":\"two\" , \"three\":30, \"four\":true }";
            JToken decode = JToken.Parse(json);

            IgnoreTest decoded = decode.ToObject<IgnoreTest>(true);
            Check.That(decoded).IsNotNull();
            Check.That(decoded.one == null);
            Check.That(decoded.two == "two");
            Check.That(decoded.three == 30);
            Check.That(decoded.four == true);
        }

        [Test]
        public void JSONToObject()
        {
            {
                string matlist = @"{ ""Raw"":{ ""t1"":""three"" } }";
                JToken matlistj = JToken.Parse(matlist);
                var Raw = matlistj["Raw"]?.ToObject(typeof(FromObjectTest), false);
                Check.That(((FromObjectTest)Raw).t1).Equals(TestEnum.three);

            }
            {
                string json = @"{
                              ""GD"":true,
                              ""SDD"":true,
                              ""TPD"":true,
                              ""TPSD"":""2014 - 12 - 16T00: 00:00Z"",
                              ""TPSDE"":false,
                              ""TPED"":""2023 - 07 - 07T16: 34:45.608Z"",
                              ""TPEDE"":false,
                              ""GALOD"":true,
                              ""GALOBJLIST"":"" +,+,+,+,+,+,+,+,+,+,+,+,+,+,+,+,+,+,+,+,+,+,"",
                              ""ERe"":true,
                              ""ERoe"":true,
                              ""ERse"":true,
                              ""ERte"":true,
                              ""ELe"":false,
                              ""ELoe"":true,
                              ""ELse"":true,
                              ""ELte"":true,
                              ""POSCAMERA"":""306.4375,59.21875,11198.63,306.4375,63.09071,11195.46,0""
                                }";

                Dictionary<string, Object> dict;

                JToken tk = JToken.Parse(json);
                dict = tk.ToObject<Dictionary<string, Object>>();
                Check.That(dict.Count == tk.Count);
            }

            {
                Dictionary<string, int> s1 = new Dictionary<string, int>() { ["one"] = 1, ["two"] = 2, ["three"] = 3 };
                JToken t1 = JToken.FromObject(s1);
                Check.That(t1).IsNotNull();

                var retobj = t1.ToObject<Dictionary<string, int>>();
                Check.That(retobj).IsNotNull();
                Check.That(retobj.Count).IsEqualTo(3);
                Check.That(retobj.ContainsKey("one")).IsTrue();
                Check.That(retobj.ContainsKey("two")).IsTrue();
                Check.That(retobj.ContainsKey("three")).IsTrue();
                Check.That(retobj["one"]).IsEqualTo(1);
                Check.That(retobj["two"]).IsEqualTo(2);
                Check.That(retobj["three"]).IsEqualTo(3);

                var retobj2 = t1.ToObject<string>();        // check it bombs out ok
                Check.That(retobj2).IsNull();
                var retobj3 = t1.ToObject<int[]>();        // check it bombs out ok
                Check.That(retobj3).IsNull();

            }
            {
                List<string> s1 = new List<string>() { "one", "two", "three" };
                JToken t1 = JToken.FromObject(s1);
                Check.That(t1).IsNotNull();

                var retobj = t1.ToObject<List<string>>();
                Check.That(retobj).IsNotNull();
                Check.That(retobj.Count).IsEqualTo(3);
                Check.That(retobj[0]).IsEqualTo("one");
                Check.That(retobj[1]).IsEqualTo("two");
                Check.That(retobj[2]).IsEqualTo("three");

            }
            {
                HashSet<string> s1 = new HashSet<string>() { "one", "two", "three" };
                JToken t1 = JToken.FromObject(s1);
                Check.That(t1).IsNotNull();

                var retobj = t1.ToObject<List<string>>();
                Check.That(retobj).IsNotNull();
                Check.That(retobj.Count).IsEqualTo(3);
                Check.That(retobj.Contains("one")).IsTrue();
                Check.That(retobj.Contains("two")).IsTrue();
                Check.That(retobj.Contains("three")).IsTrue();

            }





            {
                // check the ToObject Datetime is invariant and handles zulu

                var curcul = CultureInfo.CurrentCulture;
                CultureInfo.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("de-de");

                JToken t1 = JToken.Parse(@"{ ""one"":2929, ""two"":29, ""three"":32, ""four"":null, ""six"":606, ""seven"":1.1, ""eight"":true, ""nine"":9.9, ""ten"":""2020-02-01T00:00:00Z"",""eleven"":""2020-02-02T00:00:00Z"" }");
                OtherTypes o1 = t1.ToObject<OtherTypes>();
                Check.That(o1).IsNotNull();
                Check.That(o1.one).Equals(2929);
                Check.That(o1.two).Equals(29);
                Check.That(o1.three).Equals(32);
                Check.That(o1.four).IsNull();
                Check.That(o1.five).Equals(null);
                Check.That(o1.six).Equals(606);
                Check.That(o1.seven).Equals(1.1);
                Check.That(o1.eight).IsEqualTo(true);
                Check.That(o1.nine).Equals(9.9f);
                Check.That(o1.ten).Equals(new DateTime(2020, 2, 1));
                Check.That(o1.eleven).Equals(new DateTime(2020, 2, 2));

                CultureInfo.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture = curcul;

            }
            {
                JToken t1 = JToken.Parse(@"{ ""one"":2929, ""two"":29, ""three"":32, ""four"":null, ""five"":505 , ""six"":606, ""seven"":1.1, ""eight"":true, ""nine"":9.9 }");
                OtherTypes o1 = t1.ToObject<OtherTypes>();
                Check.That(o1).IsNotNull();
                Check.That(o1.one).Equals(2929);
                Check.That(o1.two).Equals(29);
                Check.That(o1.three).Equals(32);
                Check.That(o1.four).IsNull();
                Check.That(o1.five).Equals(505);
                Check.That(o1.six).Equals(606);
                Check.That(o1.seven).Equals(1.1);
                Check.That(o1.eight).IsEqualTo(true);
                Check.That(o1.nine).Equals(9.9f);
                Check.That(o1.ten).Equals(new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc));
                Check.That(o1.eleven).Equals(null);
            }


            {
                string mats = @"{ ""timestamp"":""2020-04-23T19:18:18Z"", ""event"":""Materials"", ""Raw"":[ { ""Name"":""carbon"", ""Count"":77 }, { ""Name"":""sulphur"", ""Count"":81 }, { ""Name"":""tin"", ""Count"":46 }, { ""Name"":""chromium"", ""Count"":32 }, { ""Name"":""nickel"", ""Count"":83 }, { ""Name"":""zinc"", ""Count"":59 }, { ""Name"":""iron"", ""Count"":48 }, { ""Name"":""phosphorus"", ""Count"":28 }, { ""Name"":""manganese"", ""Count"":60 }, { ""Name"":""niobium"", ""Count"":26 }, { ""Name"":""molybdenum"", ""Count"":25 }, { ""Name"":""antimony"", ""Count"":27 }, { ""Name"":""mercury"", ""Count"":5 }, { ""Name"":""yttrium"", ""Count"":50 }, { ""Name"":""selenium"", ""Count"":23 }, { ""Name"":""zirconium"", ""Count"":16 }, { ""Name"":""cadmium"", ""Count"":65 }, { ""Name"":""germanium"", ""Count"":26 }, { ""Name"":""tellurium"", ""Count"":39 }, { ""Name"":""vanadium"", ""Count"":49 }, { ""Name"":""arsenic"", ""Count"":14 }, { ""Name"":""technetium"", ""Count"":9 }, { ""Name"":""polonium"", ""Count"":21 }, { ""Name"":""tungsten"", ""Count"":54 } ], ""Manufactured"":[ { ""Name"":""focuscrystals"", ""Name_Localised"":""Focus Crystals"", ""Count"":9 }, { ""Name"":""refinedfocuscrystals"", ""Name_Localised"":""Refined Focus Crystals"", ""Count"":20 }, { ""Name"":""shieldingsensors"", ""Name_Localised"":""Shielding Sensors"", ""Count"":11 }, { ""Name"":""wornshieldemitters"", ""Name_Localised"":""Worn Shield Emitters"", ""Count"":29 }, { ""Name"":""shieldemitters"", ""Name_Localised"":""Shield Emitters"", ""Count"":44 }, { ""Name"":""heatdispersionplate"", ""Name_Localised"":""Heat Dispersion Plate"", ""Count"":29 }, { ""Name"":""fedproprietarycomposites"", ""Name_Localised"":""Proprietary Composites"", ""Count"":3 }, { ""Name"":""fedcorecomposites"", ""Name_Localised"":""Core Dynamics Composites"", ""Count"":2 }, { ""Name"":""compoundshielding"", ""Name_Localised"":""Compound Shielding"", ""Count"":25 }, { ""Name"":""salvagedalloys"", ""Name_Localised"":""Salvaged Alloys"", ""Count"":24 }, { ""Name"":""heatconductionwiring"", ""Name_Localised"":""Heat Conduction Wiring"", ""Count"":33 }, { ""Name"":""gridresistors"", ""Name_Localised"":""Grid Resistors"", ""Count"":24 }, { ""Name"":""hybridcapacitors"", ""Name_Localised"":""Hybrid Capacitors"", ""Count"":22 }, { ""Name"":""mechanicalequipment"", ""Name_Localised"":""Mechanical Equipment"", ""Count"":41 }, { ""Name"":""mechanicalscrap"", ""Name_Localised"":""Mechanical Scrap"", ""Count"":35 }, { ""Name"":""polymercapacitors"", ""Name_Localised"":""Polymer Capacitors"", ""Count"":4 }, { ""Name"":""phasealloys"", ""Name_Localised"":""Phase Alloys"", ""Count"":8 }, { ""Name"":""uncutfocuscrystals"", ""Name_Localised"":""Flawed Focus Crystals"", ""Count"":17 }, { ""Name"":""highdensitycomposites"", ""Name_Localised"":""High Density Composites"", ""Count"":36 }, { ""Name"":""mechanicalcomponents"", ""Name_Localised"":""Mechanical Components"", ""Count"":26 }, { ""Name"":""chemicalprocessors"", ""Name_Localised"":""Chemical Processors"", ""Count"":28 }, { ""Name"":""conductivecomponents"", ""Name_Localised"":""Conductive Components"", ""Count"":27 }, { ""Name"":""biotechconductors"", ""Name_Localised"":""Biotech Conductors"", ""Count"":8 }, { ""Name"":""galvanisingalloys"", ""Name_Localised"":""Galvanising Alloys"", ""Count"":27 }, { ""Name"":""heatexchangers"", ""Name_Localised"":""Heat Exchangers"", ""Count"":17 }, { ""Name"":""conductivepolymers"", ""Name_Localised"":""Conductive Polymers"", ""Count"":19 }, { ""Name"":""configurablecomponents"", ""Name_Localised"":""Configurable Components"", ""Count"":13 }, { ""Name"":""heatvanes"", ""Name_Localised"":""Heat Vanes"", ""Count"":18 }, { ""Name"":""chemicalmanipulators"", ""Name_Localised"":""Chemical Manipulators"", ""Count"":26 }, { ""Name"":""heatresistantceramics"", ""Name_Localised"":""Heat Resistant Ceramics"", ""Count"":2 }, { ""Name"":""protoheatradiators"", ""Name_Localised"":""Proto Heat Radiators"", ""Count"":54 }, { ""Name"":""crystalshards"", ""Name_Localised"":""Crystal Shards"", ""Count"":10 }, { ""Name"":""exquisitefocuscrystals"", ""Name_Localised"":""Exquisite Focus Crystals"", ""Count"":16 }, { ""Name"":""unknownenergysource"", ""Name_Localised"":""Sensor Fragment"", ""Count"":11 }, { ""Name"":""protolightalloys"", ""Name_Localised"":""Proto Light Alloys"", ""Count"":1 }, { ""Name"":""thermicalloys"", ""Name_Localised"":""Thermic Alloys"", ""Count"":2 }, { ""Name"":""conductiveceramics"", ""Name_Localised"":""Conductive Ceramics"", ""Count"":18 }, { ""Name"":""chemicaldistillery"", ""Name_Localised"":""Chemical Distillery"", ""Count"":6 }, { ""Name"":""chemicalstorageunits"", ""Name_Localised"":""Chemical Storage Units"", ""Count"":3 } ], ""Encoded"":[ { ""Name"":""shielddensityreports"", ""Name_Localised"":""Untypical Shield Scans "", ""Count"":112 }, { ""Name"":""emissiondata"", ""Name_Localised"":""Unexpected Emission Data"", ""Count"":39 }, { ""Name"":""shieldcyclerecordings"", ""Name_Localised"":""Distorted Shield Cycle Recordings"", ""Count"":208 }, { ""Name"":""scrambledemissiondata"", ""Name_Localised"":""Exceptional Scrambled Emission Data"", ""Count"":29 }, { ""Name"":""decodedemissiondata"", ""Name_Localised"":""Decoded Emission Data"", ""Count"":38 }, { ""Name"":""classifiedscandata"", ""Name_Localised"":""Classified Scan Fragment"", ""Count"":7 }, { ""Name"":""consumerfirmware"", ""Name_Localised"":""Modified Consumer Firmware"", ""Count"":20 }, { ""Name"":""industrialfirmware"", ""Name_Localised"":""Cracked Industrial Firmware"", ""Count"":10 }, { ""Name"":""encryptedfiles"", ""Name_Localised"":""Unusual Encrypted Files"", ""Count"":3 }, { ""Name"":""scanarchives"", ""Name_Localised"":""Unidentified Scan Archives"", ""Count"":106 }, { ""Name"":""legacyfirmware"", ""Name_Localised"":""Specialised Legacy Firmware"", ""Count"":33 }, { ""Name"":""disruptedwakeechoes"", ""Name_Localised"":""Atypical Disrupted Wake Echoes"", ""Count"":67 }, { ""Name"":""hyperspacetrajectories"", ""Name_Localised"":""Eccentric Hyperspace Trajectories"", ""Count"":33 }, { ""Name"":""wakesolutions"", ""Name_Localised"":""Strange Wake Solutions"", ""Count"":25 }, { ""Name"":""encodedscandata"", ""Name_Localised"":""Divergent Scan Data"", ""Count"":16 }, { ""Name"":""archivedemissiondata"", ""Name_Localised"":""Irregular Emission Data"", ""Count"":5 }, { ""Name"":""encryptioncodes"", ""Name_Localised"":""Tagged Encryption Codes"", ""Count"":3 }, { ""Name"":""scandatabanks"", ""Name_Localised"":""Classified Scan Databanks"", ""Count"":110 }, { ""Name"":""shieldfrequencydata"", ""Name_Localised"":""Peculiar Shield Frequency Data"", ""Count"":18 }, { ""Name"":""unknownshipsignature"", ""Name_Localised"":""Thargoid Ship Signature"", ""Count"":3 }, { ""Name"":""unknownwakedata"", ""Name_Localised"":""Thargoid Wake Data"", ""Count"":3 }, { ""Name"":""embeddedfirmware"", ""Name_Localised"":""Modified Embedded Firmware"", ""Count"":3 }, { ""Name"":""securityfirmware"", ""Name_Localised"":""Security Firmware Patch"", ""Count"":5 }, { ""Name"":""shieldpatternanalysis"", ""Name_Localised"":""Aberrant Shield Pattern Analysis"", ""Count"":66 }, { ""Name"":""shieldsoakanalysis"", ""Name_Localised"":""Inconsistent Shield Soak Analysis"", ""Count"":117 }, { ""Name"":""fsdtelemetry"", ""Name_Localised"":""Anomalous FSD Telemetry"", ""Count"":24 }, { ""Name"":""bulkscandata"", ""Name_Localised"":""Anomalous Bulk Scan Data"", ""Count"":147 }, { ""Name"":""compactemissionsdata"", ""Name_Localised"":""Abnormal Compact Emissions Data"", ""Count"":9 }, { ""Name"":""dataminedwake"", ""Name_Localised"":""Datamined Wake Exceptions"", ""Count"":7 } ] }";

                JObject jo = JObject.Parse(mats);

                var matsraw = jo["Raw"].ToObject<Material>();        // check it can handle incorrect type
                Check.That(matsraw).IsNull();
                var matsraw2 = jo["Raw"].ToObject<Material[]>();        // check it can handle nullable types
                Check.That(matsraw2).IsNotNull();

                Stopwatch sw = new Stopwatch();
                sw.Start();

                long tick = sw.ElapsedTicks;

                for (int i = 0; i < 3000; i++)
                {
                    var matsraw3r = jo["Raw"].ToObjectQ<Material[]>();        // check it can handle nullable types
                    Check.That(matsraw3r).IsNotNull();
                    var matsraw3m = jo["Manufactured"].ToObjectQ<Material[]>();        // check it can handle nullable types
                    Check.That(matsraw3m).IsNotNull();
                    var matsraw3e = jo["Encoded"].ToObjectQ<Material[]>();        // check it can handle nullable types
                    Check.That(matsraw3e).IsNotNull();
                }

                // 1794ms ToObject, 745 ToObjectQ, 526 with change

                long time = sw.ElapsedTicks - tick;
                double timems = (double)time / Stopwatch.Frequency * 1000;
                System.Diagnostics.Trace.WriteLine("Time is " + timems);
                //File.WriteAllText(@"c:\code\time.txt", "Time is " + timems);
            }

            {
                string ts = @"{""IsEmpty"":false,""Width"":10.1999998092651,""Height"":12.1999998092651,""Empty"":{""IsEmpty"":true,""Width"":0.0,""Height"":0.0}}";
                JToken j = JToken.Parse(ts);
                SizeF sf = j.ToObject<SizeF>(true);
            }


            {
                string mats = @"{ ""Materials"":{ ""iron"":19.741276, ""sulphur"":17.713514 } }";

                JObject jo = JObject.Parse(mats);

                var matsdict = jo["Materials"].ToObject<Dictionary<string, double?>>();        // check it can handle nullable types
                Check.That(matsdict).IsNotNull();
                Check.That(matsdict["iron"].HasValue && matsdict["iron"].Value == 19.741276);

                var matsdict2 = jo["Materials"].ToObject<Dictionary<string, double>>();        // and normal
                Check.That(matsdict2).IsNotNull();
                Check.That(matsdict2["iron"] == 19.741276);

                string mats3 = @"{ ""Materials"":{ ""iron"":20, ""sulphur"":17.713514 } }";
                JObject jo3 = JObject.Parse(mats3);
                var matsdict3 = jo3["Materials"].ToObject<Dictionary<string, double>>();        // and normal
                Check.That(matsdict3).IsNotNull();
                Check.That(matsdict3["iron"] == 20);

                string mats4 = @"{ ""Materials"":{ ""iron"":null, ""sulphur"":17.713514 } }";
                JObject jo4 = JObject.Parse(mats4);
                var matsdict4 = jo4["Materials"].ToObject<Dictionary<string, double?>>();        // and normal
                Check.That(matsdict4).IsNotNull();
                Check.That(matsdict4["iron"] == null);

                string mats5 = @"{ ""Materials"":{ ""iron"":""present"", ""sulphur"":null } }";
                JObject jo5 = JObject.Parse(mats5);
                var matsdict5 = jo5["Materials"].ToObject<Dictionary<string, string>>();        // and normal
                Check.That(matsdict4).IsNotNull();
                Check.That(matsdict4["iron"] == null);
            }


            {
                string englist = @"{ ""timestamp"":""2020 - 08 - 03T12: 07:15Z"",""event"":""EngineerProgress"",""Engineers"":[{""Engineer"":""Etienne\rDorn"",""EngineerID"":2929,""Progress"":""Invited"",""Rank"":null},{""Engineer"":""Zacariah Nemo"",""EngineerID"":300050,""Progress"":""Known""},{""Engineer"":""Tiana Fortune"",""EngineerID"":300270,""Progress"":""Invited""},{""Engineer"":""Chloe Sedesi"",""EngineerID"":300300,""Progress"":""Invited""},{""Engineer"":""Marco Qwent"",""EngineerID"":300200,""Progress"":""Unlocked"",""RankProgress"":55,""Rank"":3},{""Engineer"":""Petra Olmanova"",""EngineerID"":300130,""Progress"":""Invited""},{""Engineer"":""Hera Tani"",""EngineerID"":300090,""Progress"":""Unlocked"",""RankProgress"":59,""Rank"":3},{""Engineer"":""Tod 'The Blaster' McQuinn"",""EngineerID"":300260,""Progress"":""Unlocked"",""RankProgress"":0,""Rank"":5},{""Engineer"":""Marsha Hicks"",""EngineerID"":300150,""Progress"":""Invited""},{""Engineer"":""Selene Jean"",""EngineerID"":300210,""Progress"":""Unlocked"",""RankProgress"":0,""Rank"":5},{""Engineer"":""Lei Cheung"",""EngineerID"":300120,""Progress"":""Unlocked"",""RankProgress"":0,""Rank"":5},{""Engineer"":""Juri Ishmaak"",""EngineerID"":300250,""Progress"":""Unlocked"",""RankProgress"":0,""Rank"":5},{""Engineer"":""Felicity Farseer"",""EngineerID"":300100,""Progress"":""Unlocked"",""RankProgress"":0,""Rank"":5},{""Engineer"":""Broo Tarquin"",""EngineerID"":300030,""Progress"":""Unlocked"",""RankProgress"":0,""Rank"":5},{""Engineer"":""Professor Palin"",""EngineerID"":300220,""Progress"":""Unlocked"",""RankProgress"":0,""Rank"":5},{""Engineer"":""Colonel Bris Dekker"",""EngineerID"":300140,""Progress"":""Invited""},{""Engineer"":""Elvira Martuuk"",""EngineerID"":300160,""Progress"":""Unlocked"",""RankProgress"":0,""Rank"":5},{""Engineer"":""Lori Jameson"",""EngineerID"":300230,""Progress"":""Invited""},{""Engineer"":""The Dweller"",""EngineerID"":300180,""Progress"":""Unlocked"",""RankProgress"":0,""Rank"":5},{""Engineer"":""Liz Ryder"",""EngineerID"":300080,""Progress"":""Unlocked"",""RankProgress"":81,""Rank"":3},{""Engineer"":""Didi Vatermann"",""EngineerID"":300000,""Progress"":""Invited""},{""Engineer"":""The Sarge"",""EngineerID"":300040,""Progress"":""Invited""},{""Engineer"":""Mel Brandon"",""EngineerID"":300280,""Progress"":""Known""},{""Engineer"":""Ram Tah"",""EngineerID"":300110,""Progress"":""Invited""},{""Engineer"":""Bill Turner"",""EngineerID"":300010,""Progress"":""Invited""}]}";
                JToken englistj = JToken.Parse(englist);

                var pinfo = englistj["Engineers"]?.ToObject<ProgressInformation[]>();
                Check.That(pinfo).IsNotNull();
                Check.That(pinfo.Count()).Equals(25);
                StringBuilder str = new StringBuilder();
                JToken.ToStringBuilder(str, englistj, "", "", "", false);
                string strstr = str.ToString();
                string compare = "{\"timestamp\":\"2020 - 08 - 03T12: 07:15Z\",\"event\":\"EngineerProgress\",\"Engineers\":[{\"Engineer\":\"Etienne\\rDorn\",\"EngineerID\":2929,\"Progress\":\"Invited\",\"Rank\":null},{\"Engineer\":\"Zacariah Nemo\",\"EngineerID\":300050,\"Progress\":\"Known\"},{\"Engineer\":\"Tiana Fortune\",\"EngineerID\":300270,\"Progress\":\"Invited\"},{\"Engineer\":\"Chloe Sedesi\",\"EngineerID\":300300,\"Progress\":\"Invited\"},{\"Engineer\":\"Marco Qwent\",\"EngineerID\":300200,\"Progress\":\"Unlocked\",\"RankProgress\":55,\"Rank\":3},{\"Engineer\":\"Petra Olmanova\",\"EngineerID\":300130,\"Progress\":\"Invited\"},{\"Engineer\":\"Hera Tani\",\"EngineerID\":300090,\"Progress\":\"Unlocked\",\"RankProgress\":59,\"Rank\":3},{\"Engineer\":\"Tod 'The Blaster' McQuinn\",\"EngineerID\":300260,\"Progress\":\"Unlocked\",\"RankProgress\":0,\"Rank\":5},{\"Engineer\":\"Marsha Hicks\",\"EngineerID\":300150,\"Progress\":\"Invited\"},{\"Engineer\":\"Selene Jean\",\"EngineerID\":300210,\"Progress\":\"Unlocked\",\"RankProgress\":0,\"Rank\":5},{\"Engineer\":\"Lei Cheung\",\"EngineerID\":300120,\"Progress\":\"Unlocked\",\"RankProgress\":0,\"Rank\":5},{\"Engineer\":\"Juri Ishmaak\",\"EngineerID\":300250,\"Progress\":\"Unlocked\",\"RankProgress\":0,\"Rank\":5},{\"Engineer\":\"Felicity Farseer\",\"EngineerID\":300100,\"Progress\":\"Unlocked\",\"RankProgress\":0,\"Rank\":5},{\"Engineer\":\"Broo Tarquin\",\"EngineerID\":300030,\"Progress\":\"Unlocked\",\"RankProgress\":0,\"Rank\":5},{\"Engineer\":\"Professor Palin\",\"EngineerID\":300220,\"Progress\":\"Unlocked\",\"RankProgress\":0,\"Rank\":5},{\"Engineer\":\"Colonel Bris Dekker\",\"EngineerID\":300140,\"Progress\":\"Invited\"},{\"Engineer\":\"Elvira Martuuk\",\"EngineerID\":300160,\"Progress\":\"Unlocked\",\"RankProgress\":0,\"Rank\":5},{\"Engineer\":\"Lori Jameson\",\"EngineerID\":300230,\"Progress\":\"Invited\"},{\"Engineer\":\"The Dweller\",\"EngineerID\":300180,\"Progress\":\"Unlocked\",\"RankProgress\":0,\"Rank\":5},{\"Engineer\":\"Liz Ryder\",\"EngineerID\":300080,\"Progress\":\"Unlocked\",\"RankProgress\":81,\"Rank\":3},{\"Engineer\":\"Didi Vatermann\",\"EngineerID\":300000,\"Progress\":\"Invited\"},{\"Engineer\":\"The Sarge\",\"EngineerID\":300040,\"Progress\":\"Invited\"},{\"Engineer\":\"Mel Brandon\",\"EngineerID\":300280,\"Progress\":\"Known\"},{\"Engineer\":\"Ram Tah\",\"EngineerID\":300110,\"Progress\":\"Invited\"},{\"Engineer\":\"Bill Turner\",\"EngineerID\":300010,\"Progress\":\"Invited\"}]}";
                Check.That(strstr).Equals(compare);
            }

            {
                string json = "[ \"one\",\"two\",\"three\" ] ";
                JToken decode = JToken.Parse(json);

                var decoded = decode.ToObject(typeof(string[]), false);
                if (decoded is JTokenExtensions.ToObjectError)
                    System.Diagnostics.Debug.WriteLine("Err " + ((JTokenExtensions.ToObjectError)decoded).ErrorString);


                var decoded2 = decode.ToObject(typeof(string), false);
                Check.That(decoded2).IsInstanceOfType(typeof(JTokenExtensions.ToObjectError));
                if (decoded2 is JTokenExtensions.ToObjectError)
                    System.Diagnostics.Debug.WriteLine("Err " + ((JTokenExtensions.ToObjectError)decoded2).ErrorString);
            }

            {
                string json = "{ \"one\":\"one\", \"two\":\"two\" , \"three\":30, \"four\":true }";
                JToken decode = JToken.Parse(json);

                var decoded = decode.ToObject(typeof(SimpleTest), false);
                if (decoded is JTokenExtensions.ToObjectError)
                    System.Diagnostics.Debug.WriteLine("Err " + ((JTokenExtensions.ToObjectError)decoded).ErrorString);
            }

            {
                string listp2 = @"{ ""Materials"":[ ""iron"" , ""nickel"" ]}";
                JToken evt3 = JObject.Parse(listp2);
                var liste = evt3["Materials"].ToObject<List<string>>();  // name in fd logs is lower case
                Check.That(liste).IsNotNull();
                Check.That(liste.Count).IsEqualTo(2);

                string dicp2 = @"{ ""Materials"":{ ""iron"":22.1, ""nickel"":16.7, ""sulphur"":15.6, ""carbon"":13.2, ""chromium"":9.9, ""phosphorus"":8.4 }}";
                JToken evt2 = JObject.Parse(dicp2);
                var Materials2 = evt2["Materials"].ToObject<Dictionary<string, double>>();  // name in fd logs is lower case
                Check.That(Materials2).IsNotNull();
                Check.That(Materials2.Count).IsEqualTo(6);

                var Materials3fail = evt2["Materials"].ToObject<Dictionary<string, string>>();  // name in fd logs is lower case
                Check.That(Materials3fail).IsNull();



                string dicpair = @"{ ""Materials"":[ { ""Name"":""iron"", ""Percent"":19.741276 }, { ""Name"":""sulphur"", ""Percent"":17.713514 }, { ""Name"":""nickel"", ""Percent"":14.931473 }, { ""Name"":""carbon"", ""Percent"":14.895230 }, { ""Name"":""phosphorus"", ""Percent"":9.536182 } ] }";
                JToken evt = JObject.Parse(dicpair);
                JToken mats = evt["Materials"];

                if (mats != null)
                {
                    var Materials = new Dictionary<string, double>();
                    foreach (JObject jo in mats)                                        // name in fd logs is lower case
                    {
                        string name = jo["Name"].Str();

                        Materials[name.ToLowerInvariant()] = jo["Percent"].Double();
                    }
                }

                string matlist = @"{ ""Raw"":[ { ""Name"":""iron"", ""Count"":10 }, { ""Name"":""sulphur"", ""Count"":17 } ] }";
                JToken matlistj = JToken.Parse(matlist);
                var Raw = matlistj["Raw"]?.ToObject<Material[]>();
                Check.That(Raw).IsNotNull();
                Check.That(Raw.Count()).Equals(2);
            }

            {
                string matlist = @"{ ""Raw"":[ { ""Name"":""iron"", ""Count"":10 }, { ""Name"":""sulphur"", ""Count"":17 } ] }";
                JToken matlistj = JToken.Parse(matlist);
                var Raw = matlistj["Raw"]?.ToObject(typeof(MaterialIncorrect), false);
                Check.That(((JTokenExtensions.ToObjectError)Raw).ErrorString.Contains("Not Array"));

            }

            {
                string matlist = @"{ ""Raw"":{ ""t1"":""iron"" } }";
                JToken matlistj = JToken.Parse(matlist);
                var Raw = matlistj["Raw"]?.ToObject(typeof(FromObjectTest), false);
                Check.That(((JTokenExtensions.ToObjectError)Raw).ErrorString.Contains("Unrecognised"));

            }

            {
                string matlist = @"{ ""Raw"":{ ""t1"":10 } }";
                JToken matlistj = JToken.Parse(matlist);
                var Raw = matlistj["Raw"]?.ToObject(typeof(FromObjectTest), false);
                Check.That(((JTokenExtensions.ToObjectError)Raw).ErrorString.Contains("Enum Token is not string"));

            }


            {
                string matlist = @"{ ""Raw"":{ ""t1"":""$three;"" } }";
                JToken matlistj = JToken.Parse(matlist);
                var Raw = matlistj["Raw"]?.ToObject(typeof(FromObjectTest), false, process:(type,text)=> {
                    if (type == typeof(TestEnum))
                        return Enum.Parse(typeof(TestEnum),text.Substring(1, text.Length - 2),true);
                    else
                        return "CRAP";
                });
                Check.That(((FromObjectTest)Raw).t1).Equals(TestEnum.three);

            }

        }

    }
}