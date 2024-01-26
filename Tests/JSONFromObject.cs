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
    public class JSONFromObjectTest
    {
        class FromTest
        {
            public int v1;
            public string v2;
            public FromTest2 other1;
        }
        class FromTest2
        {
            public int v1;
            public string v2;
            public FromTest other2;
        }
        class FromTest3
        {
            public int v1;
            public string v2;
            public DateTime dt;
        }

        public enum TestEnum { one, two, three };

        public class FromObjectTest
        {
            public TestEnum t1;
        }

        public class ImageEntry
        {
            public bool Enabled { get; set; }
            public string Name { get; set; }                // name given to it, for preselected ones only
            public string ImagePathOrURL { get; set; }      // http:... or c:\ or Resource:<name>

            [JsonIgnoreIfNull]
            public string NotNull { get; set; }

            [JsonIgnore(JsonIgnoreAttribute.Operation.Include, "X", "Y", "Z")]
            public Point3D Centre { get; set; }
            [JsonIgnore(JsonIgnoreAttribute.Operation.Ignore, "IsEmpty")]
            public PointF Size { get; set; }
            [JsonIgnore(JsonIgnoreAttribute.Operation.Include, "X", "Y", "Z")]
            public Point3D RotationDegrees { get; set; }
            public bool RotateToViewer { get; set; }
            public bool RotateElevation { get; set; }
            public float AlphaFadeScalar { get; set; }
            public float AlphaFadePosition { get; set; }

            public ImageEntry()
            {
            }
            public ImageEntry(string name, string path, bool enabled, Point3D centre, PointF size, Point3D rotation,
                                bool rotviewer = false, bool rotelevation = false, float alphafadescaler = 0, float alphafadepos = 1)
            {
                Name = name; ImagePathOrURL = path; Enabled = enabled; Centre = centre; Size = size; RotationDegrees = rotation;
                RotateToViewer = rotviewer; RotateElevation = rotelevation; AlphaFadeScalar = alphafadescaler; AlphaFadePosition = alphafadepos;
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


        [Test]
        public void JSONFromObject()
        {
            if (true) // demonstrate removal of unwanted members by the new (sept 23) ignore or include operation
            {
                List<ImageEntry> ielist = new List<ImageEntry>
                {
                    new ImageEntry("A1","P1",true,new Point3D(1,2,3),new PointF(5,6),new Point3D(7,8,9)),
                    new ImageEntry("A2", "P2", true, new Point3D(1, 2, 3), new PointF(5, 6), new Point3D(7, 8, 9)),
                };

                ielist[1].NotNull = "hello";

                JArray json = JToken.FromObjectWithError(ielist, false, membersearchflags: System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Array();

                Check.That(json[0].Object()["Centre"].Object().Contains("PointF")).IsFalse();
                Check.That(json[0].Object()["Centre"].Object().Contains("X")).IsTrue();
                Check.That(json[0].Object()["Centre"].Object().Contains("Y")).IsTrue();
                Check.That(json[0].Object()["Centre"].Object().Contains("Z")).IsTrue();
                Check.That(json[0].Object()["Size"].Object().Contains("IsEmpty")).IsFalse();
                Check.That(json[0].Object()["Size"].Object().Contains("IsEmpty")).IsFalse();
                Check.That(json[0].Object().Contains("NotNull")).IsFalse();

                Check.That(json[1].Object()["Centre"].Object().Contains("PointF")).IsFalse();
                Check.That(json[1].Object()["Centre"].Object().Contains("X")).IsTrue();
                Check.That(json[1].Object()["Centre"].Object().Contains("Y")).IsTrue();
                Check.That(json[1].Object()["Centre"].Object().Contains("Z")).IsTrue();
                Check.That(json[1].Object()["Size"].Object().Contains("IsEmpty")).IsFalse();
                Check.That(json[1].Object()["Size"].Object().Contains("IsEmpty")).IsFalse();
                Check.That(json[1].Object().Contains("NotNull")).IsTrue();

                var str = json.ToString(true);

                var list = json.ToObject<List<ImageEntry>>();
                Check.That(list).IsNotNull();
                Check.That(list.Count).Equals(2);
                Check.That(list[0].Centre).IsNotNull();
                Check.That(list[0].Centre.X).IsEqualTo(1);
                Check.That(list[0].Centre.Y).IsEqualTo(2);
                Check.That(list[0].Centre.Z).IsEqualTo(3);
                Check.That(list[0].Size.X).IsEqualTo(5);
                Check.That(list[0].Size.Y).IsEqualTo(6);
                Check.That(list[0].RotationDegrees.X).IsEqualTo(7);
                Check.That(list[0].RotationDegrees.Y).IsEqualTo(8);
                Check.That(list[0].RotationDegrees.Z).IsEqualTo(9);
            }

            if (true)
            {
                List<ImageEntry> ielist = new List<ImageEntry>
                {
                    new ImageEntry(null,"P1",true,new Point3D(1,2,3),new PointF(5,6),new Point3D(7,8,9)),
                    new ImageEntry("A2", null, true, new Point3D(1, 2, 3), new PointF(5, 6), new Point3D(7, 8, 9)),
                };

                ielist[1].NotNull = "hello";

                JArray json = JToken.FromObjectWithError(ielist, false, membersearchflags: System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, ignoreobjectpropertyifnull: true).Array();

                Check.That(json[0].Object().Contains("Name")).IsFalse();
                Check.That(json[0].Object().Contains("ImagePathOrURL")).IsTrue();
                Check.That(json[0].Object().Contains("NotNull")).IsFalse();

                Check.That(json[1].Object().Contains("Name")).IsTrue();
                Check.That(json[1].Object().Contains("ImagePathOrURL")).IsFalse();
                Check.That(json[1].Object().Contains("NotNull")).IsTrue();

                var str = json.ToString(true);

            }


            if (true)
            {
                HashSet<string> hash = new HashSet<string>() { "one", "two", "three" };
                JToken t = JToken.FromObject(hash);
                JArray ja = t as JArray;
                Check.That(ja).IsNotNull();
                Check.That(ja.Count).IsEqualTo(3);
            }

            if (true)
            {
                string t1 = "Hello";
                JToken t = JToken.FromObject(t1);
                Check.That(t.IsString).IsTrue();
            }

            {
                SizeF sf = new SizeF(10.2f, 12.2f);

                JToken t = JToken.FromObject(sf, true);
                string ts = @"{""IsEmpty"":false,""Width"":10.199999809265137,""Height"":12.199999809265137,""Empty"":{""IsEmpty"":true,""Width"":0.0,""Height"":0.0}}";
                Check.That(t.ToString()).Equals(ts);                // check ignores self ref and does as much as possible
            }

            {
                var fm = new FromTest() { v1 = 10, v2 = "Hello1" };
                var fm2 = new FromTest2() { v1 = 20, v2 = "Hello2" };
                fm.other1 = fm2;
                fm2.other2 = fm;

                JToken t = JToken.FromObjectWithError(fm, false);
                Check.That(t.IsInError).IsTrue();
                Check.That(((string)t.Value).Contains("Self")).IsTrue();        // check self ref fails
                System.Diagnostics.Debug.WriteLine(t.Value.ToString());

                JToken t2 = JToken.FromObjectWithError(fm, true);
                Check.That(t2.IsObject).IsTrue();
                Check.That(t2["other1"]["v1"].Int()).Equals(20);                // check ignores self ref and does as much as possible
                System.Diagnostics.Debug.WriteLine(t.Value.ToString());
            }

            {
                var dttest = new DateTime(2022, 12, 3, 20, 3, 4, 123, DateTimeKind.Utc);
                var fm = new FromTest3() { v1 = 10, v2 = "Hello1", dt = dttest };
                JObject t = JObject.FromObjectWithError(fm, false).Object();
                Check.That(t["v1"].Long()).IsEqualTo(10);
                Check.That(t["v2"].Str()).IsEqualTo("Hello1");
                string str = t["dt"].Str();
                Check.That(t["dt"].Str()).IsEqualTo("2022-12-03T20:03:04.123Z");

            }

            // prove that fromobject converts a date time to zulu under another culture
            {
                var curcul = CultureInfo.CurrentCulture;
                CultureInfo.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("de-de");


                var dttest = new DateTime(2022, 12, 3, 20, 3, 4, 123, DateTimeKind.Utc);
                var fm = new FromTest3() { v1 = 10, v2 = "Hello1", dt = dttest };
                JObject t = JObject.FromObjectWithError(fm, false).Object();
                Check.That(t["v1"].Long()).IsEqualTo(10);
                Check.That(t["v2"].Str()).IsEqualTo("Hello1");
                string str = t["dt"].Str();
                Check.That(t["dt"].Str()).IsEqualTo("2022-12-03T20:03:04.123Z");

                JToken tj = JToken.CreateToken(dttest);
                Check.That(tj.Str()).IsEqualTo("2022-12-03T20:03:04.123Z");

                CultureInfo.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture = curcul;
            }


            {
                var mats = new Materials[2];
                mats[0] = new Materials();
                mats[0].Name = "0";
                mats[0].Name_Localised = "L0";
                mats[0].fred = new System.Drawing.Bitmap(20, 20);
                mats[1] = new Materials();
                mats[1].Name = "1";
                mats[1].Name_Localised = "L1";
                mats[1].qint = 20;

                JToken t = JToken.FromObject(mats, true, new System.Type[] { typeof(System.Drawing.Bitmap) });
                Check.That(t).IsNotNull();
                string json = t.ToString(true);
                System.Diagnostics.Debug.WriteLine("JSON " + json);

                StringBuilder str = new StringBuilder();
                JToken.ToStringBuilder(str, t, ">", "\r\n", "    ", false);
                string res = str.ToString();
                string cmp =
@">[
>    {
>        ""RValue"":0,
>        ""PropGetSet"":0,
>        ""Count"":0,
>        ""Name"":""0"",
>        ""Name_Localised"":""L0"",
>        ""FriendlyName"":null,
>        ""Category"":null,
>        ""QValue"":null
>    },
>    {
>        ""RValue"":0,
>        ""PropGetSet"":0,
>        ""Count"":0,
>        ""Name"":""1"",
>        ""Name_Localised"":""L1"",
>        ""FriendlyName"":null,
>        ""Category"":null,
>        ""QValue"":20
>    }
>]
";

                Check.That(res).Equals(cmp);


            }

            {
                string mats = @"{ ""Materials"":{ ""iron"":19.741276, ""sulphur"":17.713514 } }";
                JObject jo = JObject.Parse(mats);
                var matsdict = jo["Materials"].ToObject<Dictionary<string, double?>>();        // check it can handle nullable types
                Check.That(matsdict).IsNotNull();
                Check.That(matsdict["iron"].HasValue && matsdict["iron"].Value == 19.741276);
                var json = JToken.FromObject(matsdict);
                Check.That(json).IsNotNull();
                var jsonw = new JObject();
                jsonw["Materials"] = json;
                Check.That(jsonw.DeepEquals(jo)).IsTrue();
            }

            {
                var jo = new JArray();
                jo.Add(10.23);
                jo.Add(20.23);
                var var1 = jo.ToObject<List<double>>();
                var jback = JToken.FromObject(var1);
                Check.That(jback.DeepEquals(jo)).IsTrue();
            }

            {
                var mats2 = new Dictionary<string, double>();
                mats2["Iron"] = 20.2;
                mats2["Steel"] = 10;

                var json = JObject.FromObject(mats2);
                Check.That(json).IsNotNull();
                Check.That(json["Iron"].Double()).Equals(20.2);
                Check.That(json["Steel"].Double()).Equals(10);


            }

            {
                string propertyv =
@"[
  {
    ""Count"":0,
    ""Name"":""0"",
    ""Name_Localised"":""L0"",
    ""FriendlyName"":null,
    ""Category"":null,
    ""QValue"":null,
    ""PropGetSet"":1
  },
  {
    ""Count"":0,
    ""Name"":""1"",
    ""Name_Localised"":""L1"",
    ""FriendlyName"":null,
    ""Category"":null,
    ""QValue"":20
  }
]
";
                JToken matpro = JToken.Parse(propertyv);
                var Materials = matpro.ToObject<Materials[]>();
                Check.That(Materials).IsNotNull();

                JToken t = JToken.FromObject(Materials, true, new System.Type[] { typeof(System.Drawing.Bitmap) });
                string s = t.ToString();
                System.Diagnostics.Debug.WriteLine("JSON is " + s);

                string expout = @"[{""RValue"":0,""PropGetSet"":1,""Count"":0,""Name"":""0"",""Name_Localised"":""L0"",""FriendlyName"":null,""Category"":null,""QValue"":null},{""RValue"":0,""PropGetSet"":0,""Count"":0,""Name"":""1"",""Name_Localised"":""L1"",""FriendlyName"":null,""Category"":null,""QValue"":20}]";
                System.Diagnostics.Debug.WriteLine("exp is " + expout);

                Check.That(s).Equals(expout);
            }

            {
                FromObjectTest s = new FromObjectTest();
                s.t1 = TestEnum.three;

                JToken t = JToken.FromObject(s);

                FromObjectTest r = t.ToObject<FromObjectTest>();
                Check.That(r.t1 == TestEnum.three);
            }
        }


    }
}