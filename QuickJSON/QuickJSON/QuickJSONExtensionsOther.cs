/*
 * Copyright © 2020-2025 Robbyxp1 @ github.com
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

namespace QuickJSON
{
    /// <summary>
    /// Extension class helpers for JTokens include renames, parse string, write and read from file
    /// </summary>
    public static class JTokenExtensionsOther
    {
        /// <summary> Clone a JObject and replace the fields in a JObject with different names given a pattern</summary>
        /// <param name="jobject">JSON Object</param>
        /// <param name="pattern">Property name pattern to search for. Properties with this name or starting with this pattern name are altered</param>
        /// <param name="replace">Replace the part of the name found using pattern with this.  The rest of the name is kept</param>
        /// <param name="startswith">If false, replace any part of the name which has pattern in it with replace string.
        /// If true, only replace names which start with pattern with replace string</param>
        /// <returns>Return a new JObject with these new names.</returns>
        public static JObject RenameObjectFields(this JToken jobject, string pattern, string replace, bool startswith = false)
        {
            JObject o = jobject.Object();

            if (o != null)
            {
                JObject ret = new JObject();

                foreach (var kvp in o)
                {
                    string name = kvp.Key;
                    if (startswith)
                    {
                        if (name.StartsWith(pattern))
                            name = replace + name.Substring(pattern.Length);
                    }
                    else
                    {
                        name = name.Replace(pattern, replace);
                    }

                    ret[name] = kvp.Value;
                }

                return ret;
            }
            else
                return null;
        }

        /// <summary> Clone object and remove all underscores </summary>
        public static JObject RenameObjectFieldsUnderscores(this JToken jo)
        {
            return jo.RenameObjectFields("_", "");
        }

        /// <summary> Clone object and remove all names starting with prefix</summary>
        public static JObject RemoveObjectFieldsKeyPrefix(this JToken jo, string prefix)
        {
            return jo.RenameObjectFields(prefix, "", true);
        }

        /// <summary> Static parse of string. See JToken.Parse </summary>
        public static JToken JSONParse(this string text, JToken.ParseOptions flags = JToken.ParseOptions.None)
        {
            if (text != null)
                return JToken.Parse(text, flags);
            else
                return null;
        }

        /// <summary> Static parse of string for a JObject. See JObject.Parse </summary>
        public static JObject JSONParseObject(this string text, JToken.ParseOptions flags = JToken.ParseOptions.None)
        {
            if (text != null)
                return JObject.Parse(text, flags);
            else
                return null;
        }

        /// <summary> Static parse of string for a JArray. See JArray.Parse </summary>
        public static JArray JSONParseArray(this string text, JToken.ParseOptions flags = JToken.ParseOptions.None)
        {
            if (text != null)
                return JArray.Parse(text, flags);
            else
                return null;
        }

        /// <summary> Read a file and turn it into JSON. Null if can't read file or not json </summary>
        public static JToken ReadJSONFile(this string filepath, JToken.ParseOptions flags = JToken.ParseOptions.None)
        {
            try
            {
                if (System.IO.File.Exists(filepath))
                {
                    string ftext = System.IO.File.ReadAllText(filepath);
                    return JToken.Parse(ftext, flags);
                }
            }
            catch
            {
            }

            return null;
        }
        /// <summary> Read a file and turn it into JSON. Null if can't read file or not json </summary>
        public static bool WriteJSONFile(this JToken jo, string path, bool verbose = false)
        {
            try
            {
                string text = jo.ToString(verbose);
                System.IO.File.WriteAllText(path, text);
                return true;
            }
            catch
            {
            }

            return false;
        }
        /// <summary> Read a JSON string, convert it, then turn it back to a string (default verbose)</summary>
        public static JToken ReadJSONAndConvertBack(this string jsontext, JToken.ParseOptions flags = JToken.ParseOptions.None, bool verbose = true)
        {
            JToken tk = JToken.Parse(jsontext, flags);
            return tk?.ToString(verbose);
        }
    }
}



