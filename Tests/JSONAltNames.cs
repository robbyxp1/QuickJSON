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

    public class JSONTestsAltNames
    {

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
    }
}