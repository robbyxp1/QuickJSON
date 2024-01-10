/*sPa
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONTests
{
    [TestFixture(TestOf = typeof(JToken))]

    public class JSONSchema
    {
        [Test]
        public void JSONSchemaTest2()
        {
            string err = ParseSchema(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.approachsettlement_v1_0));
            Check.That(err).IsEmpty();

            // need to implement $ref https://json-schema.org/understanding-json-schema/structuring#dollarref

            err = ParseSchema(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.blackmarket_v1_0));
            Check.That(err).IsEmpty();
            err = ParseSchema(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.codexentry_v1_0));
            Check.That(err).IsEmpty();
            err = ParseSchema(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.commodity_v3_0));
            Check.That(err).IsEmpty();
            err = ParseSchema(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.fcmaterials_capi_v1_0));
            Check.That(err).IsEmpty();
            err = ParseSchema(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.fcmaterials_journal_v1_0));
            Check.That(err).IsEmpty();
            err = ParseSchema(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.fssallbodiesfound_v1_0));
            Check.That(err).IsEmpty();
            err = ParseSchema(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.fssbodysignals_v1_0));
            Check.That(err).IsEmpty();
            err = ParseSchema(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.fssdiscoveryscan_v1_0));
            Check.That(err).IsEmpty();
            err = ParseSchema(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.journal_v1_0));
            Check.That(err).IsEmpty();
            err = ParseSchema(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.navbeaconscan_v1_0));
            Check.That(err).IsEmpty();
            err = ParseSchema(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.navroute_v1_0));
            Check.That(err).IsEmpty();
            err = ParseSchema(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.outfitting_v2_0));
            Check.That(err).IsEmpty();
            err = ParseSchema(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.scanbarycentre_v1_0));
            Check.That(err).IsEmpty();
            err = ParseSchema(System.Text.Encoding.UTF8.GetString(Tests.Properties.Resources.shipyard_v2_0));
            Check.That(err).IsEmpty();

        }

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
                    string err = ParseSchema(schema, data);
                    Check.That(err).IsEmpty();
                }

                {
                    string data = @"{
    ""number"": ""stringerror"",
    ""street_name"" : ""cossham"",
    ""street_type"" : ""Avenue""
}";
                    string err = ParseSchema(schema, data);
                    Check.That(err).IsNotEmpty();
                }
                {
                    string data = @"{
    ""number"": 42,
    ""street_name"" : ""cossham"",
    ""street_type"" : ""AvenueX""
}";
                    string err = ParseSchema(schema, data);
                    Check.That(err).IsNotEmpty();
                }
                {
                    string data = @"{
    ""number"": 42,
    ""street_name"" : ""cossham"",
    ""street_type"" : ""Avenue"",
    ""add1"" : ""text1""

}";
                    string err = ParseSchema(schema, data);
                    Check.That(err).IsEmpty();
                }
                {
                    string data = @"{
    ""number"": 42,
    ""street_name"" : ""cossham"",
    ""street_type"" : ""Avenue"",
    ""add1"" : 20

}";
                    string err = ParseSchema(schema, data);
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
                    string err = ParseSchema(schema, data);
                    Check.That(err).IsEmpty();
                }
                {
                    string data = @"{
    ""number"": 20,
    ""street_name"" : ""cossham"",
    ""street_type"" : ""Avenue"",
    ""add1"" : 20
}";
                    string err = ParseSchema(schema, data);
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
                    string err = ParseSchema(schema, data);
                    Check.That(err).IsEmpty();
                }
                {
                    string data = @"{
    ""number"": 20,
    ""street_name"" : ""cossham""
}";
                    string err = ParseSchema(schema, data);
                    Check.That(err).IsEmpty();
                }
                {
                    string data = @"{
    ""number"": 20
}";
                    string err = ParseSchema(schema, data);
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
                    string err = ParseSchema(schema, data);
                    Check.That(err).Contains("too few");
                }
                {
                    string data = @"{
    ""number"": 20,
    ""street_name"" : ""cossham"",
    ""street_type"" : ""Avenue""
}";
                    string err = ParseSchema(schema, data);
                    Check.That(err).IsEmpty();
                }
                {
                    string data = @"{
    ""number"": 20,
    ""street_name"" : ""cossham""
}";
                    string err = ParseSchema(schema, data);
                    Check.That(err).IsEmpty();
                }
                {
                    string data = @"{
    ""number"": 20,
    ""street_name"" : ""cossham"",
    ""street_type"" : ""Avenue"",
    ""country"" : ""USA""
}";
                    string err = ParseSchema(schema, data);
                    Check.That(err).Contains("too many");
                }
            }

            // check maxitems/minitems
            // check string maxlength/minlength
            // check max value/min value for ints and numbers

        }

        string ParseSchema(JObject jschema, JToken input)
        {
            string errors = "";
            ParseSchema(jschema, input, ref errors, false);
            return errors;
        }

        string ParseSchema(JObject jschema, string inputstring, JToken.ParseOptions parseoptions = JToken.ParseOptions.CheckEOL)
        {
            string errors = "";
            JToken input = JToken.Parse(inputstring, out string error, parseoptions);
            if (input != null)
                ParseSchema(jschema, input, ref errors, false);
            else
                errors = $"JSON does not parse : {error}";
            return errors;
        }

        string ParseSchema(string jschemastring, string inputstring, JToken.ParseOptions parseoptions = JToken.ParseOptions.CheckEOL)
        {
            string errors = "";
            JObject jschema = JObject.Parse(jschemastring, out string serror, JToken.ParseOptions.CheckEOL);
            if (jschema != null)
            {
                JToken input = JToken.Parse(inputstring, out string ierror, parseoptions);
                if (input != null)
                    ParseSchema(jschema, input, ref errors, false);
                else
                    errors = $"JSON input does not parse : {ierror}";
            }
            else
                errors = $"JSON schema does not parse : {serror}";

            return errors;
        }
        string ParseSchema(string jschemastring)
        {
            string errors = "";
            JObject jschema = JObject.Parse(jschemastring, out string serror, JToken.ParseOptions.CheckEOL);
            if (jschema != null)
            {
                ParseSchema(jschema, null, ref errors, false);
            }
            else
                errors = $"JSON schema does not parse : {serror}";

            return errors;
        }

        void ParseSchema(JObject jschema, JToken input, ref string errors, bool allowarraycheck = false)
        {
            if ( !jschema.IsObject)
            {
                errors += $"\nSchema point is not an object";
                return;
            }

            JObject schema = jschema.Object();
            string schemaname = schema.Name ?? "Unnamed Object";

            string[] baseproperties = new string[] { "title", "description", "default", "examples", "depreciated",  "readOnly", "writeOnly","$comment",
                                                    "$schema", "id", "definitions",
                                                     "type", "enum",
                                                     "renamed", // in a script, not sure where this comes from
                                                };

            if ( schema.Contains("enum"))
            {
                var unknown = schema.UnknownProperties(baseproperties, new string[] { "enum" });
                if (unknown.Count() > 0)
                {
                    errors += $"\nSchema has unknown unsupported properties '{string.Join(",", unknown)} {schemaname}";
                    return;
                }

                JArray enums = schema["enum"].Array();

                if (enums != null)
                {
                    if (input == null)
                        return;

                    bool good = false;
                    foreach (JToken chk in enums)
                    {
                        if (chk.ValueEquals(input))
                        {
                            good = true;
                            break;
                        }
                    }
                    if (!good)
                        errors += $"\ninput does not match any enum {schema.Name} {input.TokenType}";
                }
                else
                    errors += $"\nenum is not an array {schemaname}";

                return;
            }

            string ptype = schema["type"].Str();               // 6.1.1

            System.Diagnostics.Debug.WriteLine($"At {schemaname} type {ptype}");

            if (ptype == "number")       // 6.2
            {
                var unknown = schema.UnknownProperties(baseproperties, new string[] { "multipleOf", "maximum", "exclusiveMaximum", "minimum", "exclusiveMinimum" });
                if (unknown.Count() > 0)
                {
                    errors += $"\nSchema has unknown unsupported properties '{string.Join(",", unknown)}' {schemaname}";
                    return;
                }

                double? multipleOf = schema["multipleOf"].DoubleNull();        // tbd
                double? maximum = schema["maximum"].DoubleNull();
                double? exclusiveMaximum = schema["exclusiveMaximum"].DoubleNull();
                double? minimum = schema["minimum"].DoubleNull();
                double? exclusiveMinimum = schema["exclusiveMinimum"].DoubleNull();

                System.Diagnostics.Debug.Assert(multipleOf == null, "Unimplemented");

                if (input != null)
                {
                    if (input.IsNull)
                    {
                        if (multipleOf.HasValue || maximum.HasValue || exclusiveMaximum.HasValue || minimum.HasValue || exclusiveMinimum.HasValue)
                            errors += $"\nValue is null but range applied to integer {schemaname} {ptype} {input.TokenType}";
                    }
                    else if (input.IsNumber)
                    {
                        double v = input.Double();
                        if (v > maximum || v >= exclusiveMaximum)
                            errors += $"\nValue is too large {schemaname} {ptype} {input.TokenType}";
                        else if (v < minimum || v <= exclusiveMinimum)
                            errors += $"\nValue is too small {schemaname} {ptype} {input.TokenType}";
                    }
                    else if (input.IsArray && allowarraycheck)        // due to checking an array due to items
                    {
                        foreach (JToken x in input.Array())
                        {
                            if (!IsOfType(x, ptype))
                                errors += $"\nArray check value is not {ptype} {schemaname} {input.TokenType}";
                            else
                            {
                                double v = x.Double();
                                if (v > maximum || v >= exclusiveMaximum)
                                    errors += $"\nValue is too large {schemaname} {ptype} {input.TokenType}";
                                else if (v < minimum || v <= exclusiveMinimum)
                                    errors += $"\nValue is too small {schemaname} {ptype} {input.TokenType}";
                            }
                        }
                    }
                    else
                        errors += $"\nMismatched type for {schemaname} Wanted type {ptype} token type {input.TokenType}";
                }
            }
            else if (ptype == "integer")       // 6.2
            {
                var unknown = schema.UnknownProperties(baseproperties, new string[] { "multipleOf", "maximum", "exclusiveMaximum", "minimum", "exclusiveMinimum" });
                if (unknown.Count() > 0)
                {
                    errors += $"\nSchema has unknown unsupported properties '{string.Join(",", unknown)}' {schemaname}";
                    return;
                }

                long? multipleOf = schema["multipleOf"].LongNull();      // tbd
                long? maximum = schema["maximum"].LongNull();
                long? exclusiveMaximum = schema["exclusiveMaximum"].LongNull();
                long? minimum = schema["minimum"].LongNull();
                long? exclusiveMinimum = schema["exclusiveMinimum"].LongNull();

                System.Diagnostics.Debug.Assert(multipleOf == null, "Unimplemented");

                if (input != null)
                {
                    if (input.IsNull)
                    {
                        if (multipleOf.HasValue || maximum.HasValue || exclusiveMaximum.HasValue || minimum.HasValue || exclusiveMinimum.HasValue)
                            errors += $"\nValue is null but range applied to integer {schemaname} {ptype} {input.TokenType}";
                    }
                    else if (input.IsInt)
                    {
                        long v = input.Long();
                        if (v > maximum || v >= exclusiveMaximum)
                            errors += $"\nValue is too large {schemaname} {ptype} {input.TokenType}";
                        else if (v < minimum || v <= exclusiveMinimum)
                            errors += $"\nValue is too small {schemaname} {ptype} {input.TokenType}";
                    }
                    else if (input.IsArray && allowarraycheck)        // due to checking an array due to items
                    {
                        foreach (var x in input.Array())
                        {
                            if (!IsOfType(x, ptype))
                                errors += $"\nArray check value is not {ptype} {schemaname} {input.TokenType}";
                            else
                            {
                                long v = x.Long();
                                if (v > maximum || v >= exclusiveMaximum)
                                    errors += $"\nValue is too large {schemaname} {ptype} {input.TokenType}";
                                else if (v < minimum || v <= exclusiveMinimum)
                                    errors += $"\nValue is too small {schemaname} {ptype} {input.TokenType}";
                            }
                        }
                    }
                    else
                        errors += $"\nMismatched type for {schemaname} Wanted type {ptype} token type {input.TokenType}";
                }
            }
            else if (ptype == "string")                                                  // 6.3
            {
                var unknown = schema.UnknownProperties(baseproperties, new string[] { "maxLength", "minLength", "pattern", "format" });
                if (unknown.Count() > 0)
                {
                    errors += $"\nSchema has unknown unsupported properties '{string.Join(",", unknown)}' {schemaname}";
                    return;
                }

                int? maxLength = schema["maxLength"].IntNull();
                int? minLength = schema["minLength"].IntNull();
                string pattern = schema["pattern"].StrNull(); // check not implemented tbd
                if (pattern != null)
                    System.Diagnostics.Debug.WriteLine($"JSON Schema Unsupported pattern");
                string format = schema["format"].StrNull();     // check not implemented tbd
                if (format != null)
                    System.Diagnostics.Debug.WriteLine($"JSON Schema Unsupported format");

                if (input != null)
                {
                    if (input.IsNull)
                    {
                        if (maxLength.HasValue || minLength.HasValue || pattern != null)
                            errors += $"\nValue is null but range applied to string {schemaname} {ptype} {input.TokenType}";
                    }
                    else if (input.IsString)
                    {
                        string v = input.Str();
                        if (v.Length > maxLength || v.Length < minLength)
                            errors += $"\nString Length is out of range {schemaname} {ptype} {input.TokenType}";
                    }
                    else if (input.IsArray && allowarraycheck)        // due to checking an array due to items
                    {
                        foreach (var x in input.Array())
                        {
                            if (!IsOfType(x, ptype))
                                errors += $"\nArray check value is not {ptype} {schemaname} {input.TokenType}";
                            else
                            {
                                string v = x.Str();
                                if (v.Length > maxLength || v.Length < minLength)
                                    errors += $"\nString Length is out of range in array {schemaname} {ptype} {input.TokenType}";
                            }
                        }
                    }
                    else
                        errors += $"\nMismatched type for {schemaname} Wanted type {ptype} token type {input.TokenType}";
                }
            }
            else if (ptype == "boolean")
            {
                var unknown = schema.UnknownProperties(baseproperties);
                if (unknown.Count() > 0)
                {
                    errors += $"\nSchema has unknown unsupported properties '{string.Join(",", unknown)}' {schemaname}";
                    return;
                }

                if (input != null)
                {
                    if (input.IsBool || input.IsNull)
                    {
                    }
                    else if (input.IsArray && allowarraycheck)        // due to checking an array due to items
                    {
                        foreach (var x in input.Array())
                        {
                            if (!IsOfType(x, ptype))
                                errors += $"\nArray check value is not {ptype} {schemaname} {input.TokenType}";
                        }
                    }
                    else
                        errors += $"\nMismatched type for {schemaname} Wanted type {ptype} token type {input.TokenType}";
                }
            }
            else if (ptype == "array")              // 6.4
            {
                var unknown = schema.UnknownProperties(baseproperties, new string[] { "maxItems", "minItems", "uniqueItems", "maxContains", "minContains", "contains", "items" });
                if (unknown.Count() > 0)
                {
                    errors += $"\nSchema has unknown unsupported properties '{string.Join(",", unknown)} {schemaname}";
                    return;
                }

                int? maxItems = schema["maxItems"].IntNull();      
                int? minItems = schema["minItems"].IntNull();

                bool? uniqueItems = schema["uniqueItems"].BoolNull();      // tbd
                int? maxContains = schema["maxContains"].IntNull();    // tbd
                int? minContains = schema["minContains"].IntNull();    // tbd

                System.Diagnostics.Debug.Assert(maxContains == null && minContains == null && uniqueItems == null, "Unimplemented");
                System.Diagnostics.Debug.Assert(!schema.Contains("contains"), "Unimplemented");

                JArray ja = input.Array();

                if (input != null)
                {
                    if (input.IsNull)
                    {
                        if (maxItems.HasValue || minItems.HasValue || uniqueItems.HasValue)
                            errors += $"\nValue is null but range applied to array {schemaname} {ptype} {input.TokenType}";
                        return;
                    }

                    if ( ja == null)
                    {
                        errors += $"\nMismatched type for {schemaname} Wanted type {ptype} token type {input.TokenType}";
                        return;
                    }


                    if (ja.Count > maxItems)
                        errors += $"\nArray has too many elements {schemaname} {ptype} {input.TokenType}";
                    else if (ja.Count < minItems)
                        errors += $"\nArray has too few elements {schemaname} {ptype} {input.TokenType}";

                    JArray prefixitems = schema["prefixItems"].Array();

                    if (prefixitems != null)
                    {
                        System.Diagnostics.Debug.Assert(!schema.Contains("items"), "Unimplemented");   // eiter an object or true/false

                        int index = 0;
                        foreach (JToken item in ja)
                        {
                            if (index < prefixitems.Count)      // good to not have enough prefix items, or for ja to be shorter
                            {
                                JObject subschema = prefixitems[index].Object();

                                ParseSchema(subschema, item, ref errors, false);      // check item

                                index++;
                            }
                            else
                                break;
                        }
                    }
                    else
                    {
                        JObject subschema = schema["items"].Object();
                        if (subschema != null)
                        {
                            foreach (JToken item in ja)
                            {
                                ParseSchema(subschema, item, ref errors, true);      // check item, allow array check 
                            }
                        }
                        else
                            errors += $"\nMissing Items/PrefixItems for array {schemaname} {ptype} {input.TokenType}";
                    }
                }
            }
            else if (ptype == "object")                                            // 6.5
            {
                var unknown = schema.UnknownProperties(baseproperties, new string[] { "maxProperties", "minProperties", "required", "additionalProperties", "patternProperties", "properties" });
                if (unknown.Count() > 0)
                {
                    errors += $"\nSchema has unknown unsupported properties '{string.Join(",", unknown)}' {schemaname}";
                    return;
                }

                int? maxProperties = schema["maxProperties"].IntNull();
                int? minProperties = schema["minProperties"].IntNull();
                string[] required = schema.Contains("required") ? schema["required"].ToObjectQ<string[]>() : new string[] { };
                JToken additionalProperties = schema["additionalProperties"];

                JObject patternProperties = schema["patternProperties"].Object();

                if (patternProperties != null)
                    System.Diagnostics.Debug.WriteLine($"JSON Schema Unsupported patternProperties");

                JObject obj = input.Object();

                if ( input != null)
                {
                    if ( input.IsNull )
                    {
                        if (maxProperties.HasValue || minProperties.HasValue || required.Length > 0)
                            errors += $"\nValue is null but range applied to object {schemaname} {ptype} {input.TokenType}";
                        return;
                    }

                    if ( obj == null)
                    {
                        errors += $"\nMismatched type for {schemaname} Wanted type {ptype} token type {input.TokenType}";
                        return;
                    }

                    if (obj.Count < minProperties)
                        errors += $"\nObject has too few properties {schemaname} {ptype} {input.TokenType}";

                    if (obj.Count > maxProperties)
                        errors += $"\nObject has too many properties {schemaname} {ptype} {input.TokenType}";

                    foreach (string x in required)
                    {
                        if (!obj.Contains(x))
                            errors += $"\nObject {x} is missing in {schemaname} {ptype} {input.TokenType}";
                    }
                }

                JObject properties = schema["properties"].Object();

                if (properties != null)
                {
                    foreach (var kvp in properties)
                    {
                        if (kvp.Value.IsObject)
                        {
                            if (obj != null)
                            {
                                if (obj.Contains(kvp.Key))         // if input has the key, parse it. Else ignore, required properties picks up missing
                                {
                                    ParseSchema(kvp.Value.Object(), obj[kvp.Key], ref errors, false);      // check item
                                }
                            }
                            else
                                ParseSchema(kvp.Value.Object(), null, ref errors, false);      // check syntax recurse
                        }
                        else
                            errors += $"\nProperty entry is not an object {schemaname} {ptype} {input.TokenType} {kvp.Key}";
                    }

                    if (input != null)
                    {
                        if (additionalProperties.Bool(true) == false) // if its set to false
                        {
                            int notinproperties = 0;
                            foreach (var kvp in obj)
                            {
                                if (!properties.Contains(kvp.Key))
                                    notinproperties++;
                            }

                            if (notinproperties > 0)
                                errors += $"\n{notinproperties} Properties are present in object but not in property list in {obj.Name ?? "Unnamed Object"}";
                        }
                        else if (additionalProperties != null && additionalProperties.IsObject)     // schema to check properties against
                        {
                            foreach (var kvp in obj)
                            {
                                if (!properties.Contains(kvp.Key))         // ones not in properties, check
                                    ParseSchema(additionalProperties.Object(), kvp.Value, ref errors, false);      // check item
                            }
                        }
                    }
                }
                else
                    errors += $"\nNo properties defined for object {schemaname} {ptype} {input.TokenType}";

            }
            else if (ptype == "null")
            {
                var unknown = schema.UnknownProperties(baseproperties);
                if (unknown.Count() > 0)
                {
                    errors += $"\nSchema has unknown unsupported properties '{string.Join(",", unknown)}' {schemaname}";
                    return;
                }

                if (input != null)
                {
                    if (!input.IsNull)
                        errors += $"\nMismatched type for {schemaname} Wanted type {ptype} token type {input.TokenType}";
                }
            }
            else
            {
                errors += $"\nUnknown schema type {ptype}";
            }


        }


        bool IsOfType(JToken t , string name)
        {
            if (name == "string")
                return t.IsString;
            else if (name == "boolean")
                return t.IsBool;
            else if (name == "number")
                return t.IsNumber;
            else if (name == "integer")
                return t.IsInt;
            else
                return false;
        }


    }
}


