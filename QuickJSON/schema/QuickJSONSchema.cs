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
    /// Except format (accepted not checked)
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
        /// <param name="jpath">Path of node</param>
        /// <param name="curschema">schema at this point</param>
        /// <param name="input">input, may be null, at this point</param>
        /// <param name="additionalpropertiesfeed">any additional properties inherited from above</param>
        /// <param name="allowarraycheck">allow an array check, used for certain array properties</param>
        private bool Parse(string jpath, JObject schema, JToken input, JObject additionalpropertiesfeed = null, bool allowarraycheck = false)
        {
            JObject parameters = schema;        // we clone parameters if we need to add stuff to it

            while (parameters.Contains("$ref"))        // in case of recursive refs..
            {
                string path = parameters["$ref"].Str();
                if (path.StartsWith("#/"))
                {
                    parameters = parameters.Clone().Object();       // we will aggregate them
                    JToken tk = rootschema.GetTokenSchemaPath(path.Substring(2));
                    if (tk.Object() == null)
                    {
                        return Error($"Schema $ref {path} does not exist or is not an object");
                    }
                    parameters.Remove("$ref");      // remove this ref 
                    parameters.Merge(tk.Object());  // and merge in next set of info..
                }
                else
                {
                    return Error($"Schema $ref {path} does not start with #/");
                }
            }
                
            if ( additionalpropertiesfeed != null )             // pushed properties
            {
                parameters = parameters.Clone().Object();       // we will aggregate them
                parameters.Merge(additionalpropertiesfeed);
            }

            int of = parameters.ContainsIndexOf(out JToken ofobj, "anyOf", "oneOf", "allOf");   // find one of these, and return it
            if ( of >= 0)
            {
                string cmdname = ofobj.Name;        // this is its name

                if (ofobj.IsArray)
                {
                    string curerrors = Errors;
                    int validatedcount = 0;

                    for( int index = 0; index < ofobj.Count; index++)
                    {
                        if (ofobj[index].IsObject)
                        {
                            if (Parse(jpath + $".{cmdname}[{index}]", ofobj[index].Object(), input))       // one parsed!
                            {
                                validatedcount++;

                                if (of == 0)        // any of, stop here, we are good
                                {
                                    Errors = curerrors;     // reset 
                                    return true;
                                }
                            }
                            else
                            {  }
                        }
                        else
                            return Error($"{cmdname} element {index} is not a schema");
                    }

                    if ( of == 1 && validatedcount == 1)        // if oneof, and one validation
                    {
                        Errors = curerrors;     // reset 
                        return true;
                    }

                    if ( of == 2 && validatedcount == ofobj.Count())    // if allof, and all validated
                    {
                        Errors = curerrors;     // reset 
                        return true;
                    }

                    return Error($"{cmdname} failed validation at {jpath}");
                }
                else
                    return Error($"{cmdname} is not an array at {jpath}");
            }

            JToken not = parameters["not"];
            if (not != null)
            {
                if (not.IsObject)
                {
                    if (input == null)          // we can't check the condition
                        return true;

                    string curerrors = Errors;
                    if (Parse(jpath + ".not", not.Object(), input))       // parse it, if success, its a fail!
                    {
                        return Error($"not condition at {jpath} failed");
                    }

                    Errors = curerrors;     // reset
                    return true;
                }
                else
                {
                    return Error($"not is not a schema at {jpath}");
                }
            }

            if (parameters.ContainsAnyOf("if", "then", "else"))
                return Error($"Unimplemented if/then/else");

            string[] baseproperties = new string[] { "title", "description", "default", "examples", "depreciated",  "readOnly", "writeOnly","$comment",
                                                    "$schema", "id", "definitions",
                                                     "type", "enum", "const", 
                                                };

            //System.Diagnostics.Debug.WriteLine($"At {parameters.Name} :  {parameters.ToString()}\r\n    given {input?.ToString()} ");

            if (parameters.Contains("enum"))       // V6.1.2
            {
                //System.Diagnostics.Debug.WriteLine($"At {schemaname} enum");

                var unknown = parameters.UnknownProperties(baseproperties, new string[] { "enum" });
                if (unknown.Count() > 0)
                {
                    Warnings += $"\nSchema at {jpath} has unknown unsupported properties '{string.Join(",", unknown)}";
                }

                JArray enums = parameters["enum"].Array();

                if (enums != null)
                {
                    if (input == null)  // no input, good
                        return true;

                    bool good = false;
                    foreach (JToken chk in enums)
                    {
                        if (chk.ValueEquals(input))
                        {
                            good = true;
                            break;
                        }
                    }

                    if (good)
                        return true;
                    else
                        return Error($"input does not match any enum at {jpath}");
                }
                else
                    return Error($"enum is not an array at {jpath}");
            }

            if (parameters.Contains("const"))       // V6.1.3
            {
                //System.Diagnostics.Debug.WriteLine($"At {schemaname} const");

                var unknown = parameters.UnknownProperties(baseproperties, new string[] { "const" });
                if (unknown.Count() > 0)
                {
                    Warnings += $"\nSchema {jpath} has unknown unsupported properties '{string.Join(",", unknown)}";
                }

                JToken enumv = parameters["const"];

                if (input == null)      // no input, okay
                    return true;

                if (enumv.ValueEquals(input))
                    return true;
                else
                    return Error($"input does not match any const at {input.TokenType}");
            }

            // work out types listed, either an array or a single string
            string[] types = null;
            if (parameters.Contains("type"))
                types = parameters["type"].IsArray ? parameters["type"].Select(x => x.Str()).ToArray() : parameters["type"].IsString ? new string[] { parameters["type"].Str() } : null;

            if (types == null)                          // if there is no type, then presume its an object, and its just a shell to introduce another layer
                types = new string[] { "object" };

            string ptype = null;

            // work out input type.. if present
            string inputtype = input != null ? GetSchemaType(input) : null;

            if (inputtype != null)      // we have input
            {
                if (inputtype == "integer" && types.Contains("number") && !types.Contains("integer"))       // integer, and only number in there, deal as number
                    inputtype = "number";

                if (types.Contains(inputtype))       // if we have a matching input to types
                {
                    ptype = inputtype;              // select it as the type to check
                }
                else
                {
                    return Error($"Input of type {inputtype} is not allowed at {jpath} for {string.Join(",", types)}");
                }
            }
            else
            {
                if (types.Length > 1)   // multiple types, cannot process without data, good
                    return true;

                ptype = types[0];       // single type, pick 
            }

            //System.Diagnostics.Debug.WriteLine($"At {schemaname} type {ptype}");

            if (ptype == "number")       // 6.2
            {
                var unknown = parameters.UnknownProperties(baseproperties, new string[] { "multipleOf", "maximum", "exclusiveMaximum", "minimum", "exclusiveMinimum" });
                if (unknown.Count() > 0)
                {
                    Warnings += $"\nSchema {jpath} has unknown unsupported properties '{string.Join(",", unknown)}'";
                }

                double? multipleOf = parameters["multipleOf"].DoubleNull();       // v6.2.1
                if (multipleOf <= 0)
                    return Error($"MultipleOf at {jpath} is less or equal to zero");

                double? maximum = parameters["maximum"].DoubleNull();   // v6.2.2
                double? exclusiveMaximum = parameters["exclusiveMaximum"].DoubleNull(); //v6.2.3
                double? minimum = parameters["minimum"].DoubleNull();   // v6.2.4
                double? exclusiveMinimum = parameters["exclusiveMinimum"].DoubleNull(); // v6.2.5

                if (input == null)
                { }
                else if (input.IsNull)
                {
                    if (multipleOf.HasValue || maximum.HasValue || exclusiveMaximum.HasValue || minimum.HasValue || exclusiveMinimum.HasValue)
                        return Error($"Value is null at {jpath} but range applied to number");
                }
                else if (input.IsNumber)
                {
                    double v = input.Double();
                    if (v > maximum || v >= exclusiveMaximum)
                        return Error($"Value {jpath} is too large");
                    else if (v < minimum || v <= exclusiveMinimum)
                        return Error($"Value {jpath} is too small");
                    else if (multipleOf != null && v % multipleOf != 0)
                        return Error($"MultipleOf {jpath} failed");
                }
                else if (input.IsArray && allowarraycheck)        // due to checking an array due to items
                {
                    string curerrors = Errors;

                    foreach (JToken x in input.Array())
                    {
                        if (!IsOfType(x, ptype))
                            return Error($"Array check value at {jpath} is not {ptype}");
                        else
                        {
                            double v = x.Double();
                            if (v > maximum || v >= exclusiveMaximum)
                                Errors += $"Value {jpath} is too large";
                            else if (v < minimum || v <= exclusiveMinimum)
                                Errors += $"Value {jpath} is too small";
                            else if (multipleOf != null && v % multipleOf != 0)
                                Errors += $"MultipleOf {jpath} failed";
                        }
                    }

                    if (Errors != curerrors)
                        return false;
                }
                else
                    return Error($"Mismatched type for {jpath} Wanted type {ptype} token type {input.TokenType}");       // should not get here, but double check
            }
            else if (ptype == "integer")       // V6.2
            {
                var unknown = parameters.UnknownProperties(baseproperties, new string[] { "multipleOf", "maximum", "exclusiveMaximum", "minimum", "exclusiveMinimum" });
                if (unknown.Count() > 0)
                {
                    Warnings += $"\nSchema {jpath} has unknown unsupported properties '{string.Join(",", unknown)}'";
                }

                // if any element here is a bigint, must treat all as a bigint

                bool bigint = (input?.IsBigInt ?? false) || (parameters["maximum"]?.IsBigInt ?? false) || (parameters["exclusiveMaximum"]?.IsBigInt ?? false) ||
                    (parameters["minimum"]?.IsBigInt ?? false) || (parameters["exclusiveMinimum"]?.IsBigInt ?? false) || (parameters["multipleOf"]?.IsBigInt ?? false);

                if (bigint)                         // big int!
                {
#if JSONBIGINT
                    System.Numerics.BigInteger? multipleOf = parameters["multipleOf"].BigIntegerNull();
                    if (multipleOf <= 0)
                        return Error($"MultipleOf at {jpath} is less or equal to zero");

                    System.Numerics.BigInteger? maximum = parameters["maximum"].BigIntegerNull();
                    System.Numerics.BigInteger? exclusiveMaximum = parameters["exclusiveMaximum"].BigIntegerNull(); 
                    System.Numerics.BigInteger? minimum = parameters["minimum"].BigIntegerNull();
                    System.Numerics.BigInteger? exclusiveMinimum = parameters["exclusiveMinimum"].BigIntegerNull();

                    if (input == null)
                    { }
                    else if (input.IsNull)
                    {
                        if (multipleOf.HasValue || maximum.HasValue || exclusiveMaximum.HasValue || minimum.HasValue || exclusiveMinimum.HasValue)
                            return Error($"Value is null at {jpath} but range applied to integer");
                    }
                    else if (input.IsInt)
                    {
                        System.Numerics.BigInteger v = input.BigInteger(0);
                        if (v > maximum || v >= exclusiveMaximum)
                            return Error($"Value {jpath} is too large");
                        else if (v < minimum || v <= exclusiveMinimum)
                            return Error($"Value {jpath} is too small");
                        else if (multipleOf != null && v % multipleOf != 0)
                            return Error($"MultipleOf {jpath} failed");
                    }
                    else if (input.IsArray && allowarraycheck)        // due to checking an array due to items
                    {
                        string curerrors = Errors;
                        foreach (var x in input.Array())
                        {
                            if (!IsOfType(x, ptype))
                                return Error($"\nArray check value at {jpath} is not {ptype}");
                            else
                            {
                                System.Numerics.BigInteger v = x.BigInteger(0);
                                if (v > maximum || v >= exclusiveMaximum)
                                    Errors += $"Value {jpath} is too large";
                                else if (v < minimum || v <= exclusiveMinimum)
                                    Errors += $"Value {jpath} is too small";
                                else if (multipleOf != null && v % multipleOf != 0)
                                    Errors += $"MultipleOf {jpath} failed";
                            }
                        }
                        if (Errors != curerrors)
                            return false;
                    }
                    else
                        return Error($"Mismatched type for {jpath} Wanted type {ptype} token type {input.TokenType}");
#else
                    return Error("BIGINT not supported");
#endif
                }
                else
                {                                               // otherwise long
                    long? multipleOf = parameters["multipleOf"].LongNull();      // tbd
                    if (multipleOf <= 0)
                        return Error($"MultipleOf at {jpath} is less or equal to zero");

                    long? maximum = parameters["maximum"].LongNull();
                    long? exclusiveMaximum = parameters["exclusiveMaximum"].LongNull();
                    long? minimum = parameters["minimum"].LongNull();
                    long? exclusiveMinimum = parameters["exclusiveMinimum"].LongNull();

                    if (input == null)
                    { }
                    else if (input.IsNull)
                    {
                        if (multipleOf.HasValue || maximum.HasValue || exclusiveMaximum.HasValue || minimum.HasValue || exclusiveMinimum.HasValue)
                            return Error($"Value is null at {jpath} but range applied to integer");
                    }
                    else if (input.IsInt)
                    {
                        long v = input.Long();
                        if (v > maximum || v >= exclusiveMaximum)
                            return Error($"Value {jpath} is too large");
                        else if (v < minimum || v <= exclusiveMinimum)
                            return Error($"Value {jpath} is too small");
                        else if (multipleOf != null && v % multipleOf != 0)
                            return Error($"MultipleOf {jpath} failed");
                    }
                    else if (input.IsArray && allowarraycheck)        // due to checking an array due to items
                    {
                        string curerrors = Errors;
                        foreach (var x in input.Array())
                        {
                            if (!IsOfType(x, ptype))
                                return Error($"\nArray check value at {jpath} is not {ptype}");
                            else
                            {
                                long v = x.Long();
                                if (v > maximum || v >= exclusiveMaximum)
                                    Errors += $"Value {jpath} is too large";
                                else if (v < minimum || v <= exclusiveMinimum)
                                    Errors += $"Value {jpath} is too small";
                                else if (multipleOf != null && v % multipleOf != 0)
                                    Errors += $"MultipleOf {jpath} failed";
                            }
                        }
                        if (Errors != curerrors)
                            return false;
                    }
                    else
                        return Error($"Mismatched type for {jpath} Wanted type {ptype} token type {input.TokenType}");
                }
            }
            else if (ptype == "string")                 // V6.3
            {
                var unknown = parameters.UnknownProperties(baseproperties, new string[] { "maxLength", "minLength", "pattern", "format" });
                if (unknown.Count() > 0)
                {
                    Warnings += $"\nSchema {jpath} has unknown unsupported properties '{string.Join(",", unknown)}'";
                }

                int? maxLength = parameters["maxLength"].IntNull();     // v6.3.1
                int? minLength = parameters["minLength"].IntNull();     // v6.3.2

                string pattern = parameters["pattern"].StrNull();         // v6.3.3

                string format = parameters["format"].StrNull();     // not in 2020-12. 
                if (format != null)
                    Warnings += $"\nJSON Schema Unsupported format ignored at {jpath}";

                if (input != null)
                {
                    if (input.IsNull)
                    {
                        if (maxLength.HasValue || minLength.HasValue || pattern != null)
                            return Error($"Value is null at {jpath} but range applied to string");
                    }
                    else if (input.IsString)
                    {
                        string v = input.Str();
                        if (v.Length > maxLength || v.Length < minLength)
                            return Error($"String Length is out of range at {jpath}");
                        else if ( pattern != null )
                        {
                            Regex reg = new Regex(pattern);
                            if ( !reg.IsMatch(v))
                                return Error($"String does not match regex {pattern} at {jpath}");
                        }
                    }
                    else if (input.IsArray && allowarraycheck)        // due to checking an array due to items
                    {
                        foreach (var x in input.Array())
                        {
                            if (!IsOfType(x, ptype))
                                return Error($"Array check value at {jpath} is not {ptype}");
                            else
                            {
                                string v = x.Str();
                                if (v.Length > maxLength || v.Length < minLength)
                                    return Error($"String Length is out of range in array at {jpath}");
                            }
                        }
                    }
                    else
                        return Error($"Mismatched type for {jpath} Wanted type {ptype} token type {input.TokenType}");
                }
            }
            else if (ptype == "array")              // v6.4
            {
                var unknown = parameters.UnknownProperties(baseproperties, new string[] { "maxItems", "minItems", "uniqueItems", "maxContains", "minContains", "contains", "items" });
                if (unknown.Count() > 0)
                {
                    Warnings += $"\nSchema {jpath} has unknown unsupported properties '{string.Join(",", unknown)}";
                }

                int? maxItems = parameters["maxItems"].IntNull();           // v6.4.1
                int? minItems = parameters["minItems"].IntNull();           // v6.4.2
                bool? uniqueItems = parameters["uniqueItems"].BoolNull();   // v6.4.3

                JObject contains = parameters["contains"].Object();         // c10.3.1.3
                if (parameters.Contains("contains") && contains == null)
                    return Error($"contains is not an schema at {jpath}");

                int? maxContains = parameters["maxContains"].IntNull();     // v6.4.4
                int? minContains = parameters["minContains"].IntNull();     // v6.4.5

                JArray prefixitems = parameters["prefixItems"].Array();     // c10.3.1.1        either or
                if (parameters.Contains("prefixItems") && prefixitems == null)
                    return Error($"prefixItems is not an array at {jpath}");

                JObject items = parameters["items"].Object();               // c10.3.1.2

                if (parameters.Contains("items") && items == null)
                    return Error($"items is not an array at {jpath}");

                JArray ja = input.Array();

                if (input != null)
                {
                    if (input.IsNull)
                    {
                        if (maxItems.HasValue || minItems.HasValue || uniqueItems.HasValue)
                            return Error($"Value is null at {jpath} but range applied to array");
                    }

                    if (ja == null)
                    {
                        return Error($"Mismatched type at {jpath} Wanted type {ptype} token type {input.TokenType}");
                    }

                    if (ja.Count > maxItems)
                        return Error($"Array has too many elements at {jpath}");
                    else if (ja.Count < minItems)
                        return Error($"Array has too few elements at {jpath}");

                    if (uniqueItems == true)
                    {
                        foreach (var item in ja)
                        {
                            foreach (var item2 in ja)
                            {
                                if (item != item2 && item.ValueEquals(item2)) // if not the same item, but they compare the same
                                {
                                    return Error($"Duplicate items in array at {jpath}");
                                }
                            }
                        }
                    }

                    string curerrors = Errors;

                    int index = 0;

                    if (prefixitems != null)    // see 10.3.1.2 paragraph 2 do prefix items, then items
                    {
                        while (index < ja.Count && index < prefixitems.Count)       // for all items up to count, and if it has a prefix item, process it
                        {
                            JObject subschema = prefixitems[index].Object();

                            Parse(jpath + $"[{index}]", subschema, ja[index]);   // test with subschema
                            index++;
                        }
                    }

                    if (items != null) // see 10.3.1.2 paragraph 4 may be omitted
                    {
                        while (index < ja.Count)       // for all items up to count, and if it has a prefix item, process it
                        {
                            Parse(jpath + $"[{index}]", items, ja[index]);       // test with items schema
                            index++;
                        }
                    }

                    if (curerrors != Errors)        // we have errors, fail
                        return false;

                    if ( contains != null )     // run this schema over the data
                    {
                        if (minContains != 0)     // 0 means valid, don't test 10.3.1.3 P2
                        {
                            index = 0;
                            int validated = 0;
                            while (index < ja.Count)
                            {
                                validated += Parse(jpath + $"[{index}]", contains, ja[index]) ? 1 : 0;      // if validated with no error, count it
                                index++;
                            }

                            Errors = curerrors;     // reset the error value

                            if (minContains.HasValue)
                            {
                                if (validated < minContains)        // if we have a min contains, and we have less than it, error
                                    return Error($"contains failed minimum at {jpath}");
                            }
                            else if ( validated == 0 )
                                return Error($"contains found no matches in array at {jpath}");

                            if (maxContains.HasValue && validated > maxContains)        // if we have a max contains, and we have more than it, error
                                return Error($"contains failed maximum at {jpath}");
                        }
                    }

                }
            }
            else if (ptype == "object")                                            // 6.5
            {
                var unknown = parameters.UnknownProperties(baseproperties, new string[] { "maxProperties", "minProperties", "required", "additionalProperties", "patternProperties", "properties" });
                if (unknown.Count() > 0)
                {
                    Warnings += $"\nSchema {jpath} has unknown unsupported properties '{string.Join(",", unknown)}";
                }

                int? maxProperties = parameters["maxProperties"].IntNull();     // v6.5.1
                int? minProperties = parameters["minProperties"].IntNull();     // v6.5.2
                string[] required = parameters.Contains("required") ? parameters["required"].ToObjectQ<string[]>() : new string[] { };  // v6.5.3

                JObject properties = parameters["properties"].Object();
                if (parameters.Contains("properties") && properties == null)
                    return Error($"properties is not an object at {jpath}");

                JObject patternProperties = parameters["patternProperties"].Object();                  // c10.3.2.2
                if (parameters.Contains("patternProperties") && patternProperties == null)
                    return Error($"patternProperties is not an object at {jpath}");

                JToken additionalProperties = parameters["additionalProperties"];       // c10.3.2.3 can be a bool or a schema

                JObject propertyNames = parameters["propertyNames"].Object();           // c10.3.2.4
                if (propertyNames == null && parameters.Contains("propertyNames"))      // check if not object
                    return Error($"\nPropertynames is not an object in object at {jpath}");

                JObject inputobj = input.Object();

                if (input != null)
                {
                    if (input.IsNull)
                    {
                        if (maxProperties.HasValue || minProperties.HasValue || required.Length > 0)
                            return Error($"Value is null but range applied to object at {jpath}");
                    }

                    if (inputobj == null)
                    {
                        return Error($"Mismatched type in object at {jpath} Wanted type {ptype} token type {input.TokenType}");
                    }

                    if (inputobj.Count < minProperties)
                        return Error($"Object at {jpath} has too few properties");

                    if (inputobj.Count > maxProperties)
                        return Error($"Object at {jpath} has too many properties");

                    foreach (string x in required)
                    {
                        if (!inputobj.Contains(x))
                            return Error($"Property {x} is missing in object at {jpath}");
                    }
                }

                List<string> propertiesprocessed = new List<string>();
                string curerrors = Errors;

                if (patternProperties != null)
                {
                    foreach (var kvp in patternProperties)
                    {
                        Regex ex = new Regex(kvp.Key);

                        if (inputobj != null)
                        {
                            foreach (var ikvp in inputobj)
                            {
                                if (ex.IsMatch(ikvp.Key))      // if name matches
                                {
                                    propertiesprocessed.Add(ikvp.Key);  // mark processed

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
                } 

                if ( properties != null )
                {
                    foreach (var ikvp in properties)     // for all properties..
                    {
                        if (ikvp.Value.IsObject)         // check
                        {
                            propertiesprocessed.Add(ikvp.Key);      // mark processed

                            if (inputobj != null)
                            {
                                if (inputobj.Contains(ikvp.Key))         // if input has the key, parse it. Else ignore, required properties picks up missing
                                {
                                    Parse(jpath + "." + ikvp.Key, ikvp.Value.Object(), inputobj[ikvp.Key], propertyNames);      // check item
                                }
                            }
                            else
                                Parse(jpath + "." + ikvp.Key, ikvp.Value.Object(), null, propertyNames);      // check syntax recurse
                        }
                        else
                            return Error($"Property entry is not an object at {jpath}");
                    }
                }

                if (additionalProperties != null )
                {
                    if (additionalProperties.IsBool)             // if bool
                    {
                        if (additionalProperties.Bool() == false)
                        {
                            if (inputobj != null && inputobj.Count > propertiesprocessed.Count)
                            {
                                return Error($"Properties are present in object at {jpath} but do not match patternProperties or properties");
                            }
                        }
                    }
                    else  if (additionalProperties.IsObject)     // schema to check properties against
                    {
                        if (inputobj != null)
                        {
                            foreach (var kvp in inputobj)
                            {
                                if (!propertiesprocessed.Contains(kvp.Key))     // if not previously processed
                                {
                                    Parse(jpath + "." + kvp.Key, additionalProperties.Object(), kvp.Value);      // check item
                                }
                            }
                        }
                    }
                    else
                        return Error($"additionalProperties is not a schema at {jpath}");
                }

                if (curerrors != Errors)        // if we had errors, fail here
                    return false;
            }
            else if (ptype == "boolean")        // no validation items
            {
                var unknown = parameters.UnknownProperties(baseproperties);
                if (unknown.Count() > 0)
                {
                    Warnings += $"\nSchema {jpath} has unknown unsupported properties '{string.Join(",", unknown)}";
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
                                return Error($"Array check value at {jpath} is not {ptype}");
                        }
                    }
                    else
                        return Error($"Mismatched type at {jpath} Wanted type {ptype} token type {input.TokenType}");
                }
            }
            else if (ptype == "null")   // no validation items
            {
                var unknown = parameters.UnknownProperties(baseproperties);
                if (unknown.Count() > 0)
                {
                    Warnings += $"\nSchema {jpath} has unknown unsupported properties '{string.Join(",", unknown)}";
                }

                if (input != null)
                {
                    if (!input.IsNull)
                        return Error($"Mismatched type at {jpath} Wanted type null");
                }
            }
            else
            {
                return Error($"Unknown schema type {ptype}");
            }

            return true;
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


        private bool Error(string err)
        {
            if (Errors.Length > 0)
                Errors += "\n" + err;
            else
                Errors = err;

            return false;
        }


        private JObject rootschema;
    }
}



