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
        public class ArrayTest1
        {
            public int[] array1;

            [JsonCustomArrayLength(new string[] { "S1" }, new int[] { 10 })]
            public int[] array2;
        }

        [Test]
        public void JSONToObjectCustomFormat()
        {
            string jmd = @"{ ""array1"": [10,20,30], ""array2"":[40,50,60,70] }";

            JToken decode = JToken.Parse(jmd);
            Check.That(decode).IsNotNull();
            string json1 = decode.ToString(true);
            System.Diagnostics.Debug.WriteLine(json1);

            ArrayTest1 m = decode.ToObject<ArrayTest1>() as ArrayTest1;
            Check.That(m.array1.Length == 3);
            Check.That(m.array2.Length == 4);

            ArrayTest1 n = decode.ToObject<ArrayTest1>(setname:"S1") as ArrayTest1;
            Check.That(n.array1.Length == 3);
            Check.That(n.array2.Length == 10);


        }
    }
}
