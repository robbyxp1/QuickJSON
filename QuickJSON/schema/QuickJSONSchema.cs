/*
 * Copyright © 2024-2024 Robbyxp1 @ github.com
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace QuickJSON
{
    /// <summary>
    /// JSchema is a static class holding experimental schema decoders
    /// Implements most of 2022-12 https://json-schema.org/specification
    /// Except If/then/else
    /// Except allof/oneof
    /// Only not type implemented
    /// Except format (accepted not checked)
    /// Except contains/maxcontains/mincontains
    /// </summary>
    public class JSONSchema
    {
        /// <summary>
        /// Errors encountered
        /// </summary>
        public string Errors { get; private set; }
        /// <summary>
        /// Warnings encountered
        /// </summary>
        public string Warnings { get; private set; }
        /// <summary>
        /// Create a schema entry
        /// </summary>
        /// <param name="rootschema"></param>
        public JSONSchema(JObject rootschema)
        {
            this.rootschema = rootschema;
            Errors = Warnings = "";
        }

        /// <summary>
        /// Parse a schema, with optional input
        /// </summary>
        /// <param name="jschema">schema</param>
        /// <param name="jinput">input</param>
        /// <returns>Empty string no errors, else a list of \n separated errors</returns>
        public string Validate(JToken jinput = null)
        {
            Parse("",rootschema, jinput);
            return Errors;
        }

        /// <summary>
        /// Parse a schema, with input
        /// </summary>
        /// <param name="jschema">schema</param>
        /// <param name="jinput">input</param>
        /// <returns>Empty string no errors, else a list of \n separated errors</returns>
        public static string Validate(JObject jschema, JToken jinput)
        {
            var schema = new JSONSchema(jschema);
            schema.Parse("", jschema, jinput);
            return schema.Errors;
        }

        /// <summary>
        /// Parse a schema, with input
        /// </summary>
        /// <param name="jschema">schema</param>
        /// <param name="inputstring">input JSON data as string</param>
        /// <param name="parseoptions">JSON parse options</param>
        /// <returns>Empty string no errors, else a list of \n separated errors</returns>
        public string Validate(JObject jschema, string inputstring, JToken.ParseOptions parseoptions = JToken.ParseOptions.CheckEOL)
        {
            JToken jinput = JToken.Parse(inputstring, out string error, parseoptions);
            if (jinput != null)
            {
                var schema = new JSONSchema(jschema);
                schema.Parse("", jschema, jinput);
                return schema.Errors;
            }
            else
                return $"JSON does not parse : {error}";
        }

        /// <summary>
        /// Parse a schema, with input
        /// </summary>
        /// <param name="schemastring">schema as string</param>
        /// <param name="jinput">input as JSON</param>
        /// <param name="parseoptions">JSON parse options</param>
        /// <returns>Empty string no errors, else a list of \n separated errors</returns>
        public static string Validate(string schemastring, JToken jinput, JToken.ParseOptions parseoptions = JToken.ParseOptions.CheckEOL)
        {
            JObject jschema = JObject.Parse(schemastring, out string error, parseoptions);
            if (jschema != null)
            {
                var schema = new JSONSchema(jschema);
                schema.Parse("", jschema, jinput);
                return schema.Errors;
            }
            else
                return $"JSON does not parse : {error}";
        }
        /// <summary>
        /// Parse a schema, with input
        /// </summary>
        /// <param name="schemastring">schema as string</param>
        /// <param name="inputstring">input JSON data as string</param>
        /// <param name="parseoptions">JSON parse options</param>
        /// <returns>Empty string no errors, else a list of \n separated errors</returns>
        public static string Validate(string schemastring, string inputstring, JToken.ParseOptions parseoptions = JToken.ParseOptions.CheckEOL)
        {
            JObject jschema = JObject.Parse(schemastring, out string serror, JToken.ParseOptions.CheckEOL);
            if (jschema != null)
            {
                JToken jinput = JToken.Parse(inputstring, out string ierror, parseoptions);
                if (jinput != null)
                {
                    var schema = new JSONSchema(jschema);
                    schema.Parse("", jschema, jinput);
                    return schema.Errors;
                }
                else
                    return $"JSON input does not parse : {ierror}";
            }
            else
                return $"JSON schema does not parse : {serror}";
        }
        /// <summary>
        /// Validate schema
        /// </summary>
        /// <param name="schemastring">schema as string</param>
        /// <returns>Empty string no errors, else a list of \n separated errors</returns>
        public static string Validate(string schemastring)
        {
            JObject jschema = JObject.Parse(schemastring, out string serror, JToken.ParseOptions.CheckEOL);
            if (jschema != null)
            {
                var schema = new JSONSchema(jschema);
                schema.Parse("", jschema, null);
                return schema.Errors;
            }
            else
                return $"JSON schema does not parse : {serror}";
        }

        /// <summary>
        /// Validate schema
        /// </summary>
        /// <param name="jschema">schema</param>
        /// <returns>Empty string no errors, else a list of \n separated errors</returns>
        public static string Validate(JObject jschema)
        {
            var schema = new JSONSchema(jschema);
            schema.Parse("", jschema, null);
            return schema.Errors;
        }

        /// <summary>
        /// Parser
        /// </summary>
        /// <param name="curschema">schema at this point</param>
        /// <param name="input">input, may be null, at this point</param>
        /// <param name="additionalpropertiesfeed">any additional properties inherited from above</param>
        /// <param name="allowarraycheck">allow an array check, used for certain array properties</param>
        private void Parse(string jpath, JObject curschema, JToken input, JObject additionalpropertiesfeed = null, bool allowarraycheck = false)
        {
            if (!curschema.IsObject)
            {
                Errors += $"\nSchema point is not an object";
                return;
            }

            JObject schema = curschema.Object();

            JObject parameters = schema;

            while (parameters.Contains("$ref"))        // in case of recursive refs..
            {
                string path = parameters["$ref"].Str();
                if (path.StartsWith("#/"))
                {
                    parameters = parameters.Clone().Object();       // we will aggregate them
                    JToken tk = rootschema.GetTokenSchemaPath(path.Substring(2));
                    if (tk.Object() == null)
                    {
                        Errors += $"\nSchema $ref {path} does not exist or is not an object";
                        return;
                    }
                    parameters.Remove("$ref");      // remove this ref 
                    parameters.Merge(tk.Object());  // and merge in next set of info..
                }
                else
                {
                    Errors += $"\nSchema $ref {path} does not start with #/";
                    return;
                }
            }
                
            if ( additionalpropertiesfeed != null )             // pushed properties
            {
                parameters = parameters.Clone().Object();       // we will aggregate them
                parameters.Merge(additionalpropertiesfeed);
            }


            if (parameters.Contains("anyOf"))
            {
                JArray ao = parameters["anyOf"].Array();
                bool good = true;
                foreach (JObject a in ao)
                {
                    string interrors = "";
                    Parse(jpath, a, input);
                    if (interrors.Length == 0)      // no errors, passed here
                        good = true;
                }

                if (!good)
                    Errors += $"\nAnyOf failed validation at {schema}";
                return;
            }

            if (parameters.ContainsThese("allOf", "oneOf", "if", "then", "else"))
            {
                Errors += $"Unimplemented parameter type";
                return;
            }

            string[] baseproperties = new string[] { "title", "description", "default", "examples", "depreciated",  "readOnly", "writeOnly","$comment",
                                                    "$schema", "id", "definitions",
                                                     "type", "enum", "const", 
                                                };

            System.Diagnostics.Debug.WriteLine($"At {parameters.Name} :  {parameters.ToString()}\r\n    given {input?.ToString()} ");

            if (parameters.Contains("enum"))       // V6.1.2
            {
                //System.Diagnostics.Debug.WriteLine($"At {schemaname} enum");

                var unknown = parameters.UnknownProperties(baseproperties, new string[] { "enum" });
                if (unknown.Count() > 0)
                {
                    Warnings += $"\nSchema has unknown unsupported properties '{string.Join(",", unknown)} {jpath}";
                }

                JArray enums = parameters["enum"].Array();

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
                        Errors += $"\ninput does not match any enum {parameters.Name} {input.TokenType}";
                }
                else
                    Errors += $"\nenum is not an array {jpath}";

                return;
            }

            if (parameters.Contains("const"))       // V6.1.3
            {
                //System.Diagnostics.Debug.WriteLine($"At {schemaname} const");

                var unknown = parameters.UnknownProperties(baseproperties, new string[] { "const" });
                if (unknown.Count() > 0)
                {
                    Warnings += $"\nSchema has unknown unsupported properties '{string.Join(",", unknown)} {jpath}";
                }

                JToken enumv = parameters["const"];

                if (input == null)      // no input, okay
                    return;

                if (enumv.ValueEquals(input))
                {
                    return;
                }
                else
                {
                    Errors += $"\ninput does not match any const {input.TokenType}";
                    return;
                }
            }

            string[] types = null;

            string inputtype = input != null ? GetSchemaType(input) : null;

            JToken not = parameters["not"];
            if (not != null)
            {
                if (not.IsObject)
                {
                    foreach (var kvp in not.Object())
                    {
                        if (kvp.Key == "type")
                        {
                            if (input == null)          // not type - no input, can't determine what the input type is, so we say okay
                                return;

                            if (kvp.Value.IsArray)
                            {
                                string[] values = kvp.Value.Array().Select(x => x.Str()).ToArray();
                                if (values.Contains(inputtype))
                                {
                                    Errors += $"\nInput type {inputtype} not allowed due to Not Type";
                                    return;
                                }

                                types = new string[] { "number", "integer", "null", "boolean", "object", "array", "string" };       // set types to any now, since we triaged it
                            }
                            else
                            {
                                Errors += $"\nnot type is not an array";
                                return;
                            }
                        }
                        else
                        {
                            Errors += $"\nnot condition not implemented {kvp.Key}";
                            return;
                        }
                    }
                }
                else
                {
                    Errors += $"\nnot is not an object";
                    return;
                }
            }

            if (types == null && parameters.Contains("type"))
                types = parameters["type"].IsArray ? parameters["type"].Select(x => x.Str()).ToArray() : parameters["type"].IsString ? new string[] { parameters["type"].Str() } : null;

            if (types == null)                          // if there is no type, then presume its an object, and its just a shell to introduce another layer
                types = new string[] { "object" };

            string ptype = null;

            if (inputtype != null)      // we have input
            {
                if (inputtype == "integer" && types.Contains("number") && !types.Contains("integer"))       // integer, and only number in there, deal as number
                    inputtype = "number";

                if (types.Contains(inputtype))       // if we have an input type, 
                {
                    ptype = inputtype;
                }
                else
                {
                    Errors += $"\nInput of type {inputtype} is not allowed for {string.Join(",", types)}";
                    return;
                }
            }
            else
            {
                if (types.Length > 1)     // multiple types, cannot process without data, good
                    return;

                ptype = types[0];       // single type, pick 
            }

            //System.Diagnostics.Debug.WriteLine($"At {schemaname} type {ptype}");

            if (ptype == "number")       // 6.2
            {
                var unknown = parameters.UnknownProperties(baseproperties, new string[] { "multipleOf", "maximum", "exclusiveMaximum", "minimum", "exclusiveMinimum" });
                if (unknown.Count() > 0)
                {
                    Warnings += $"\nSchema has unknown unsupported properties '{string.Join(",", unknown)}' {jpath}";
                }

                double? multipleOf = parameters["multipleOf"].DoubleNull();       // v6.2.1
                if (multipleOf <= 0)
                {
                    Errors += $"\nMultipleOf is less or equal to zero";
                    return;
                }

                double? maximum = parameters["maximum"].DoubleNull();   // v6.2.2
                double? exclusiveMaximum = parameters["exclusiveMaximum"].DoubleNull(); //v6.2.3
                double? minimum = parameters["minimum"].DoubleNull();   // v6.2.4
                double? exclusiveMinimum = parameters["exclusiveMinimum"].DoubleNull(); // v6.2.5

                if (input != null)
                {
                    if (input.IsNull)
                    {
                        if (multipleOf.HasValue || maximum.HasValue || exclusiveMaximum.HasValue || minimum.HasValue || exclusiveMinimum.HasValue)
                            Errors += $"\nValue is null but range applied to integer {jpath} {ptype} {input.TokenType}";
                    }
                    else if (input.IsNumber)
                    {
                        double v = input.Double();
                        if (v > maximum || v >= exclusiveMaximum)
                            Errors += $"\nValue {jpath} is too large";
                        else if (v < minimum || v <= exclusiveMinimum)
                            Errors += $"\nValue {jpath} is too small";
                        else if (multipleOf != null && v % multipleOf != 0)
                            Errors += $"\nMultipleOf {jpath} failed";
                    }
                    else if (input.IsArray && allowarraycheck)        // due to checking an array due to items
                    {
                        foreach (JToken x in input.Array())
                        {
                            if (!IsOfType(x, ptype))
                                Errors += $"\nArray check value is not {ptype} {jpath} {input.TokenType}";
                            else
                            {
                                double v = x.Double();
                                if (v > maximum || v >= exclusiveMaximum)
                                    Errors += $"\nValue {jpath} is too large";
                                else if (v < minimum || v <= exclusiveMinimum)
                                    Errors += $"\nValue {jpath} is too small";
                                else if (multipleOf != null && v % multipleOf != 0)
                                    Errors += $"\nMultipleOf {jpath} failed";
                            }
                        }
                    }
                    else
                        Errors += $"\nMismatched type for {jpath} Wanted type {ptype} token type {input.TokenType}";       // should not get here, but double check
                }
            }
            else if (ptype == "integer")       // V6.2
            {
                var unknown = parameters.UnknownProperties(baseproperties, new string[] { "multipleOf", "maximum", "exclusiveMaximum", "minimum", "exclusiveMinimum" });
                if (unknown.Count() > 0)
                {
                    Warnings += $"\nSchema has unknown unsupported properties '{string.Join(",", unknown)}' {jpath}";
                }

                long? multipleOf = parameters["multipleOf"].LongNull();      // tbd
                if (multipleOf <= 0)
                {
                    Errors += $"\nMultipleOf is less or equal to zero";
                    return;
                }

                long? maximum = parameters["maximum"].LongNull();
                long? exclusiveMaximum = parameters["exclusiveMaximum"].LongNull();
                long? minimum = parameters["minimum"].LongNull();
                long? exclusiveMinimum = parameters["exclusiveMinimum"].LongNull();

                System.Diagnostics.Debug.Assert(multipleOf == null, "Unimplemented");

                if (input != null)
                {
                    if (input.IsNull)
                    {
                        if (multipleOf.HasValue || maximum.HasValue || exclusiveMaximum.HasValue || minimum.HasValue || exclusiveMinimum.HasValue)
                            Errors += $"\nValue is null but range applied to integer {jpath}";
                    }
                    else if (input.IsInt)
                    {
                        long v = input.Long();
                        if (v > maximum || v >= exclusiveMaximum)
                            Errors += $"\nValue {jpath} is too large";
                        else if (v < minimum || v <= exclusiveMinimum)
                            Errors += $"\nValue {jpath} is too small";
                        else if (multipleOf != null && v % multipleOf != 0)
                            Errors += $"\nMultipleOf {jpath} failed";
                    }
                    else if (input.IsArray && allowarraycheck)        // due to checking an array due to items
                    {
                        foreach (var x in input.Array())
                        {
                            if (!IsOfType(x, ptype))
                                Errors += $"\nArray check value is not {ptype} {jpath} {input.TokenType}";
                            else
                            {
                                long v = x.Long();
                                if (v > maximum || v >= exclusiveMaximum)
                                    Errors += $"\nValue {jpath} is too large";
                                else if (v < minimum || v <= exclusiveMinimum)
                                    Errors += $"\nValue {jpath} is too small";
                                else if (multipleOf != null && v % multipleOf != 0)
                                    Errors += $"\nMultipleOf {jpath} failed";
                            }
                        }
                    }
                    else
                        Errors += $"\nMismatched type for {jpath} Wanted type {ptype} token type {input.TokenType}";
                }
            }
            else if (ptype == "string")                                                  // V6.3
            {
                var unknown = parameters.UnknownProperties(baseproperties, new string[] { "maxLength", "minLength", "pattern", "format" });
                if (unknown.Count() > 0)
                {
                    Warnings += $"\nSchema has unknown unsupported properties '{string.Join(",", unknown)}' {jpath}";
                }

                int? maxLength = parameters["maxLength"].IntNull();     // v6.3.1
                int? minLength = parameters["minLength"].IntNull();     // v6.3.2
                string regex = parameters["pattern"].StrNull();         // v6.3.3
                if (regex != null)
                    System.Diagnostics.Debug.Assert(false, $"JSON Schema Unsupported pattern");

                string format = parameters["format"].StrNull();     // not in 2020-12. 
                if (format != null)
                    System.Diagnostics.Debug.WriteLine($"JSON Schema Unsupported format ignored");

                if (input != null)
                {
                    if (input.IsNull)
                    {
                        if (maxLength.HasValue || minLength.HasValue || regex != null)
                            Errors += $"\nValue is null but range applied to string {jpath} {ptype} {input.TokenType}";
                    }
                    else if (input.IsString)
                    {
                        string v = input.Str();
                        if (v.Length > maxLength || v.Length < minLength)
                            Errors += $"\nString Length is out of range {jpath} {ptype} {input.TokenType}";
                    }
                    else if (input.IsArray && allowarraycheck)        // due to checking an array due to items
                    {
                        foreach (var x in input.Array())
                        {
                            if (!IsOfType(x, ptype))
                                Errors += $"\nArray check value is not {ptype} {jpath} {input.TokenType}";
                            else
                            {
                                string v = x.Str();
                                if (v.Length > maxLength || v.Length < minLength)
                                    Errors += $"\nString Length is out of range in array {jpath} {ptype} {input.TokenType}";
                            }
                        }
                    }
                    else
                        Errors += $"\nMismatched type for {jpath} Wanted type {ptype} token type {input.TokenType}";
                }
            }
            else if (ptype == "array")              // v6.4
            {
                var unknown = parameters.UnknownProperties(baseproperties, new string[] { "maxItems", "minItems", "uniqueItems", "maxContains", "minContains", "contains", "items" });
                if (unknown.Count() > 0)
                {
                    Warnings += $"\nSchema has unknown unsupported properties '{string.Join(",", unknown)} {jpath}";
                }

                int? maxItems = parameters["maxItems"].IntNull();           // v6.4.1
                int? minItems = parameters["minItems"].IntNull();           // v6.4.2
                bool? uniqueItems = parameters["uniqueItems"].BoolNull();   // v6.4.3
                int? maxContains = parameters["maxContains"].IntNull();     // v6.4.4
                int? minContains = parameters["minContains"].IntNull();     // v6.4.5
                JToken contains = parameters["contains"];                   // c10.3.1.3

                JArray prefixitems = parameters["prefixItems"].Array();     // c10.3.1.1        either or
                JObject items = parameters["items"].Object();               // c10.3.1.2

                System.Diagnostics.Debug.Assert(maxContains == null && minContains == null && contains == null, "Unimplemented");

                JArray ja = input.Array();

                if (input != null)
                {
                    if (input.IsNull)
                    {
                        if (maxItems.HasValue || minItems.HasValue || uniqueItems.HasValue)
                            Errors += $"\nValue is null but range applied to array {jpath} {ptype} {input.TokenType}";
                        return;
                    }

                    if (ja == null)
                    {
                        Errors += $"\nMismatched type for {jpath} Wanted type {ptype} token type {input.TokenType}";
                        return;
                    }

                    if (ja.Count > maxItems)
                        Errors += $"\nArray has too many elements {jpath} {ptype} {input.TokenType}";
                    else if (ja.Count < minItems)
                        Errors += $"\nArray has too few elements {jpath} {ptype} {input.TokenType}";

                    if (uniqueItems == true)
                    {
                        foreach (var item in ja)
                        {
                            foreach (var item2 in ja)
                            {
                                if (item != item2 && item.ValueEquals(item2)) // if not the same item, but they compare the same
                                {
                                    Errors += $"\nDuplicate items in array {jpath} {ptype} {input.TokenType}";
                                    return;
                                }
                            }
                        }
                    }

                    if (prefixitems != null)
                    {
                        System.Diagnostics.Debug.Assert(!parameters.Contains("items"), "Unimplemented");   // eiter an object or true/false

                        int index = 0;
                        foreach (JToken item in ja)
                        {
                            if (index < prefixitems.Count)      // good to not have enough prefix items, or for ja to be shorter
                            {
                                JObject subschema = prefixitems[index].Object();

                                Parse(jpath + $"[{index}]", subschema, item);      // check item

                                index++;
                            }
                            else
                                break;
                        }
                    }
                    else
                    {
                        if (items != null)
                        {
                            int index = 0;
                            foreach (JToken item in ja)
                            {
                                Parse(jpath + $"[{index}]", items, item);      // check item, allow array check 
                                index++;
                            }
                        }
                        else
                            Errors += $"\nMissing Items/PrefixItems for array {jpath} {ptype} {input.TokenType}";
                    }
                }
            }
            else if (ptype == "object")                                            // 6.5
            {
                var unknown = parameters.UnknownProperties(baseproperties, new string[] { "maxProperties", "minProperties", "required", "additionalProperties", "patternProperties", "properties" });
                if (unknown.Count() > 0)
                {
                    Warnings += $"\nSchema has unknown unsupported properties '{string.Join(",", unknown)}' {jpath}";
                }

                int? maxProperties = parameters["maxProperties"].IntNull();     // v6.5.1
                int? minProperties = parameters["minProperties"].IntNull();     // v6.5.2
                string[] required = parameters.Contains("required") ? parameters["required"].ToObjectQ<string[]>() : new string[] { };  // v6.5.3

                JToken patternProperties = parameters["patternProperties"];                  // c10.3.2.2
                JToken additionalProperties = parameters["additionalProperties"];       // c10.3.2.3

                JObject propertyNames = parameters["propertyNames"].Object();           // c10.3.2.4
                if (propertyNames == null && parameters.Contains("propertyNames"))      // check if not object
                {
                    Errors += $"\nPropertynames is not an object in object {jpath}";
                    return;
                }

                JObject properties = parameters["properties"].Object();

                JObject inputobj = input.Object();

                if (input != null)
                {
                    if (input.IsNull)
                    {
                        if (maxProperties.HasValue || minProperties.HasValue || required.Length > 0)
                            Errors += $"\nValue is null but range applied to object {jpath}";
                        return;
                    }

                    if (inputobj == null)
                    {
                        Errors += $"\nMismatched type in object {jpath} Wanted type {ptype} token type {input.TokenType}";
                        return;
                    }

                    if (inputobj.Count < minProperties)
                        Errors += $"\nObject {jpath} has too few properties";

                    if (inputobj.Count > maxProperties)
                        Errors += $"\nObject {jpath} has too many properties";

                    foreach (string x in required)
                    {
                        if (!inputobj.Contains(x))
                            Errors += $"\nProperty {x} is missing in object {jpath}";
                    }
                }

                if (properties != null)
                {
                    foreach (var kvp in properties)     // for all properties..
                    {
                        if (kvp.Value.IsObject)         // check
                        {
                            if (inputobj != null)       
                            {
                                if (inputobj.Contains(kvp.Key))         // if input has the key, parse it. Else ignore, required properties picks up missing
                                {
                                    Parse(jpath + "." + kvp.Key, kvp.Value.Object(),inputobj[kvp.Key], propertyNames);      // check item
                                }
                            }
                            else
                                Parse(jpath + "." + kvp.Key, kvp.Value.Object(),  null, propertyNames);      // check syntax recurse
                        }
                        else
                            Errors += $"\nProperty entry is not an object {jpath} {ptype} {input.TokenType} {kvp.Key}";
                    }

                    if (inputobj != null)       // check for additional properties
                    {
                        if (additionalProperties.Bool(true) == false) // if its set to false
                        {
                            int notinproperties = 0;
                            foreach (var kvp in inputobj)
                            {
                                if (!properties.Contains(kvp.Key))
                                    notinproperties++;
                            }

                            if (notinproperties > 0)
                                Errors += $"\n{notinproperties} Properties are present in object {jpath} but not in property list in {inputobj.Name ?? "Unnamed Object"}";
                        }
                        else if (additionalProperties != null && additionalProperties.IsObject)     // schema to check properties against
                        {
                            foreach (var kvp in inputobj)       // additionalProperties is a schema, match
                            {
                                if (!properties.Contains(kvp.Key))    // if name is not in properties list, its additional ,schema match
                                    Parse(jpath + "." + kvp.Key, additionalProperties.Object(), kvp.Value);      // check item
                            }
                        }
                    }
                }
                else if (patternProperties != null)
                {
                    JObject pp = patternProperties.Object();
                    if (pp != null)
                    {
                        // names of properties, not matched at this part, empty if no data
                        List<string> propertiesnotmatched = inputobj != null ? inputobj.PropertyNames().ToList() : new List<string>();

                        foreach (var kvp in pp)
                        {
                            Regex ex = new Regex(kvp.Key);

                            if (inputobj != null)
                            {
                                foreach (var ikvp in inputobj)
                                {
                                    if (ex.IsMatch(ikvp.Key))      // if name matches
                                    {
                                        propertiesnotmatched.Remove(ikvp.Key);

                                        if (inputobj != null)
                                        {
                                            Parse(jpath + "." + kvp.Key, kvp.Value.Object(), ikvp.Value, propertyNames);      // check item as it matches against this schema
                                        }
                                        else
                                            Parse(jpath + "." + kvp.Key, kvp.Value.Object(), null, propertyNames);      // check null input as it matches against this schema
                                    }
                                }
                            }
                        }

                        if (inputobj != null)       // check for additional properties
                        {
                            if (additionalProperties.Bool(true) == false) // if its set to false
                            {
                                if (propertiesnotmatched.Count != 0)      // if we have dat
                                {
                                    Errors += $"\nProperties are present in object {jpath} but do not match patternProperties";
                                    return;
                                }
                            }
                            else if (additionalProperties != null && additionalProperties.IsObject)     // schema to check properties against
                            {
                                foreach (var name in propertiesnotmatched)
                                {
                                    Parse(jpath + "." + name, additionalProperties.Object(), inputobj[name]);      // check item
                                }
                            }
                        }
                    }
                    else
                    {
                        Errors += $"\npatternProperties is not an object in object {jpath}";
                        return;
                    }
                }
                else
                {
                    Errors += $"\nNo properties defined for object {jpath}";
                    return;
                }

            }
            else if (ptype == "boolean")        // no validation items
            {
                var unknown = parameters.UnknownProperties(baseproperties);
                if (unknown.Count() > 0)
                {
                    Errors += $"\nSchema has unknown unsupported properties '{string.Join(",", unknown)}' {jpath}";
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
                                Errors += $"\nArray check value is not {ptype} {jpath} {input.TokenType}";
                        }
                    }
                    else
                        Errors += $"\nMismatched type for {jpath} Wanted type {ptype} token type {input.TokenType}";
                }
            }
            else if (ptype == "null")   // no validation items
            {
                var unknown = parameters.UnknownProperties(baseproperties);
                if (unknown.Count() > 0)
                {
                    Errors += $"\nSchema has unknown unsupported properties '{string.Join(",", unknown)}' {jpath}";
                    return;
                }

                if (input != null)
                {
                    if (!input.IsNull)
                        Errors += $"\nMismatched type for {jpath} Wanted type {ptype} token type {input.TokenType}";
                }
            }
            else
            {
                Errors += $"\nUnknown schema type {ptype}";
            }
        }

        private static string GetSchemaType(JToken input)
        {
            if (input.IsInt)            // classify object into one of schema types
                return "integer";
            else if (input.IsDouble)
                return "number";
            else if (input.IsString)
                return "string";
            else if (input.IsBool)
                return "boolean";
            else if (input.IsArray)
                return "array";
            else if (input.IsObject)
                return "object";
            else if (input.IsNull)
                return "null";
            else
                System.Diagnostics.Debug.Assert(false, "Bad input type for schema");
            return null;
        }

        private static bool IsOfType(JToken t, string name)
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


        private JObject rootschema;
    }
}



