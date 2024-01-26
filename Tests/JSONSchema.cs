/*
 * Copyright © 2023 Robbyxp1 @ github.com
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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace JSONTests
{
    [TestFixture(TestOf = typeof(JToken))]

    public class JSONSchemaTesting
    {
        [Test]
        public void JSONSchemaTest1()
        {
            if (true)
            {
                string schema = @"{
  ""type"": ""object"",
  ""properties"": {
    ""number"": { ""type"": ""number"" },
    ""street_name"": { ""type"": ""string"" },
    ""street_type"": { ""enum"": [""Street"", ""Avenue"", ""Boulevard""] }
  },
  ""additionalProperties"": { ""type"": ""string"" }
}
";

                {
                    string data = @"{
    ""number"": 20,
    ""street_name"" : ""cossham"",
    ""street_type"" : ""Avenue""
}";
                    string err =JSONSchema.Validate(schema, data);
                    Check.That(err).IsEmpty();
                }

                {
                    string data = @"{
    ""number"": ""stringerror"",
    ""street_name"" : ""cossham"",
    ""street_type"" : ""Avenue""
}";
                    string err =JSONSchema.Validate(schema, data);
                    Check.That(err).IsNotEmpty();
                }
                {
                    string data = @"{
    ""number"": 42,
    ""street_name"" : ""cossham"",
    ""street_type"" : ""AvenueX""
}";
                    string err =JSONSchema.Validate(schema, data);
                    Check.That(err).IsNotEmpty();
                }
                {
                    string data = @"{
    ""number"": 42,
    ""street_name"" : ""cossham"",
    ""street_type"" : ""Avenue"",
    ""add1"" : ""text1""

}";
                    string err =JSONSchema.Validate(schema, data);
                    Check.That(err).IsEmpty();
                }
                {
                    string data = @"{
    ""number"": 42,
    ""street_name"" : ""cossham"",
    ""street_type"" : ""Avenue"",
    ""add1"" : 20

}";
                    string err =JSONSchema.Validate(schema, data);
                    Check.That(err).IsNotEmpty();
                }
            }

            if (true)
            {                                                           // check for additiona properties not allowed
                string schema = @"{
  ""type"": ""object"",
  ""properties"": {
    ""number"": { ""type"": ""number"" },
    ""street_name"": { ""type"": ""string"" },
    ""street_type"": { ""enum"": [""Street"", ""Avenue"", ""Boulevard""] }
  },
  ""additionalProperties"": false
}
";

                {
                    string data = @"{
    ""number"": 20,
    ""street_name"" : ""cossham"",
    ""street_type"" : ""Avenue""
}";
                    string err =JSONSchema.Validate(schema, data);
                    Check.That(err).IsEmpty();
                }
                {
                    string data = @"{
    ""number"": 20,
    ""street_name"" : ""cossham"",
    ""street_type"" : ""Avenue"",
    ""add1"" : 20
}";
                    string err =JSONSchema.Validate(schema, data);
                    Check.That(err).IsNotEmpty();
                }


            }

            if (true)
            {                                                           // check for additiona properties not allowed
                string schema = @"{
  ""type"": ""object"",
  ""properties"": {
    ""number"": { ""type"": ""number"" },
    ""street_name"": { ""type"": ""string"" },
    ""street_type"": { ""enum"": [""Street"", ""Avenue"", ""Boulevard""] }
  },
  ""additionalProperties"": false,
  ""required"" : [ ""number"",""street_name"" ] 
}
";

                {
                    string data = @"{
    ""number"": 20,
    ""street_name"" : ""cossham"",
    ""street_type"" : ""Avenue""
}";
                    string err =JSONSchema.Validate(schema, data);
                    Check.That(err).IsEmpty();
                }
                {
                    string data = @"{
    ""number"": 20,
    ""street_name"" : ""cossham""
}";
                    string err =JSONSchema.Validate(schema, data);
                    Check.That(err).IsEmpty();
                }
                {
                    string data = @"{
    ""number"": 20
}";
                    string err =JSONSchema.Validate(schema, data);
                    Check.That(err).IsNotEmpty();
                }


            }

            {                                                           // check for 2-3 properties
                string schema = @"{
  ""type"": ""object"",
  ""properties"": {
    ""number"": { ""type"": ""number"" },
    ""street_name"": { ""type"": ""string"" },
    ""street_type"": { ""enum"": [""Street"", ""Avenue"", ""Boulevard""] },
    ""country"": { ""enum"": [""UK"", ""USA""] }
  },
  ""additionalProperties"": false,
  ""minProperties"": 2,
  ""maxProperties"": 3,
  ""required"" : [ ""number"" ] 
    
}
";

                {
                    string data = @"{
    ""number"": 20
}";
                    string err =JSONSchema.Validate(schema, data);
                    Check.That(err).Contains("too few");
                }
                {
                    string data = @"{
    ""number"": 20,
    ""street_name"" : ""cossham"",
    ""street_type"" : ""Avenue""
}";
                    string err =JSONSchema.Validate(schema, data);
                    Check.That(err).IsEmpty();
                }
                {
                    string data = @"{
    ""number"": 20,
    ""street_name"" : ""cossham""
}";
                    string err =JSONSchema.Validate(schema, data);
                    Check.That(err).IsEmpty();
                }
                {
                    string data = @"{
    ""number"": 20,
    ""street_name"" : ""cossham"",
    ""street_type"" : ""Avenue"",
    ""country"" : ""USA""
}";
                    string err =JSONSchema.Validate(schema, data);
                    Check.That(err).Contains("too many");
                }
            }

            // check maxitems/minitems
            // check string maxlength/minlength
            // check max value/min value for ints and numbers

        }


        [Test]
        public void JSONSchemaTest2()
        {
            {
                JObject schema = JObject.Parse(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.Simpleref));

                string err = JSONSchema.Validate(schema);
                Check.That(err).IsEmpty();

                string data;
                JToken jodata;

                data = @"{ ""message"": { ""NewTraitsDiscovered"" : 20 } }";
                jodata = JToken.Parse(data);
                err = JSONSchema.Validate(schema, jodata);
                Check.That(err).IsEmpty();


                data = @"{ ""message"": { ""IsNewEntry"" : 20 } }";
                jodata = JToken.Parse(data);
                err = JSONSchema.Validate(schema, jodata);
                Check.That(err).Contains("integer not allowed");



            }
        }

        [Test]
        public void JSONSchemaTest3()
        {
            string err = JSONSchema.Validate(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.approachsettlement_v1_0));
            Check.That(err).IsEmpty();

            // need to implement $ref https://json-schema.org/understanding-json-schema/structuring#dollarref

            err = JSONSchema.Validate(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.commodity_v3_0));
            Check.That(err).IsEmpty();
            err = JSONSchema.Validate(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.blackmarket_v1_0));
            Check.That(err).IsEmpty();
            err = JSONSchema.Validate(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.codexentry_v1_0));
            Check.That(err).IsEmpty();
            err = JSONSchema.Validate(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.fcmaterials_capi_v1_0));
            Check.That(err).IsEmpty();
            err = JSONSchema.Validate(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.fcmaterials_journal_v1_0));
            Check.That(err).IsEmpty();
            err = JSONSchema.Validate(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.fssallbodiesfound_v1_0));
            Check.That(err).IsEmpty();
            err = JSONSchema.Validate(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.fssbodysignals_v1_0));
            Check.That(err).IsEmpty();
            err = JSONSchema.Validate(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.fssdiscoveryscan_v1_0));
            Check.That(err).IsEmpty();
            err = JSONSchema.Validate(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.journal_v1_0));
            Check.That(err).IsEmpty();
            err = JSONSchema.Validate(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.navbeaconscan_v1_0));
            Check.That(err).IsEmpty();
            err = JSONSchema.Validate(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.navroute_v1_0));
            Check.That(err).IsEmpty();
            err = JSONSchema.Validate(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.outfitting_v2_0));
            Check.That(err).IsEmpty();
            err = JSONSchema.Validate(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.scanbarycentre_v1_0));
            Check.That(err).IsEmpty();
            err = JSONSchema.Validate(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.shipyard_v2_0));
            Check.That(err).IsEmpty();

        }


        [Test]
        public void JSONSchemaTestApproachSettlement()
        {
            JObject schema = JObject.Parse(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.approachsettlement_v1_0));
            string input = System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.ApproachSettlement_1_t09_00);
            ValidateSchemaFile(schema, input);
        }


        [Test]
        public void JSONSchemaCommodity()
        {
            JObject schema = JObject.Parse(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.commodity_v3_0));
            string input = System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.Commodity_3_t11_00);
           // ValidateSchemaFile(schema, input);
            string[] inlines = System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.Commodity_3_t11_00).Split('\n');

            {
                var eddndata = EDDNMessage(inlines[0], inlines[1]);
                eddndata["message"].Object().Remove("stationName");
                string errs = ValidateSchemaData(schema, eddndata);
                Check.That(errs).Contains("stationName is missing");                        //  Object stationName is missing in message object Object
            }
            {
                var eddndata = EDDNMessage(inlines[0], inlines[1]);
                eddndata["message"].Object()["commodities"][1]["meanPrice"] = -20;
                string errs = ValidateSchemaData(schema, eddndata);
                Check.That(errs).Contains("[1].meanPrice is too small");                        //  Object stationName is missing in message object Object
            }
        }

        public void ValidateSchemaFile(JObject schema, string input)
        {
            string[] lines = input.Split('\n');

            string header = null;
            foreach (var line in lines)
            {
                if (line.Length > 0)
                {
                    if (line.Contains("gamebuild"))
                        header = line;
                    else
                    {
                        JObject eddnmessage = EDDNMessage(header, line);
                        string errs = ValidateSchemaData(schema, eddnmessage);
                        Check.That(errs).IsEmpty();
                    }
                }
            }
        }
        public string ValidateSchemaData(JObject schema, JObject eddnmessage)
        {
            var schemav = new JSONSchema(schema);
            string errs = schemav.Validate(eddnmessage);
            if (errs.Length > 0)
            {
                System.Diagnostics.Debug.WriteLine($"Message failed: {eddnmessage.ToString(true)}");
            }
            return errs;
        }

        public JObject EDDNMessage(string line1, string line2)
        {
            JObject eddnmessage = new JObject();
            eddnmessage.Add("$schemaRef", "Schema");
            eddnmessage.Add("header", JObject.Parse(line1));
            eddnmessage.Add("message", JObject.Parse(line2));
            return eddnmessage;
        }

    }


}


