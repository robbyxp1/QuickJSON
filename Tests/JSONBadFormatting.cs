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

    public class JSONTestsBadFormatting
    {
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



    }
}