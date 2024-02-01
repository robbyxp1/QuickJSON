/*
 * Copyright 2019-2024 Robbyxp1 @ github.com
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
using System.Text;
using QuickJSON.Utils;

namespace QuickJSON
{
    /// <summary>
    /// Quick formatter using Fluent syntax, quick and easy way to make a JSON string
    /// </summary>

    public partial class JSONFormatter
    {
        /// <summary> QuickJSONFormatter Error exception</summary>
        public class FormatterException : Exception
        {
            /// <summary> Constructor </summary>
            public FormatterException(string error)
            {
                Error = error;
            }

            /// <summary> Error condition</summary>
            public string Error { get; set; }
        }

        /// <summary> Constructor </summary>
        public JSONFormatter()
        {
            json = new StringBuilder(10000);
            stack = new List<StackEntry>();
            precomma = false;
        }

        /// <summary> Add a property called name with string data</summary>
        /// <exception cref="FormatterException">Thrown if adding to an array or start with a property name
        /// </exception>
        public JSONFormatter V(string name, string data)
        {
            Prefix(name != null);
            if (name != null)
            {
                json.Append('"');
                json.Append(name);
                json.Append("\":");
            }
            json.Append('"');
            json.Append(data);
            json.Append('"');
            //System.Diagnostics.Debug.WriteLine($"String: `{json}`");
            return this;
        }

        /// <summary> Add an array element with string data</summary>
        /// <exception cref="FormatterException">Thrown if adding to an object because of no property name or if first element added
        /// </exception>
        public JSONFormatter V(string data)
        {
            return V(null, data);
        }

        /// <summary> Add a property called name with a float value</summary>
        /// <exception cref="FormatterException">Thrown if adding to an array or start with a property name
        /// </exception>
        public JSONFormatter V(string name, float value)
        {
            Prefix(name != null);
            if (name != null)
            {
                json.Append('"');
                json.Append(name);
                json.Append("\":");
            }
            json.Append(value.ToString("R"));
            return this;
        }

        /// <summary> Add an array element with a float value</summary>
        /// <exception cref="FormatterException">Thrown if adding to an object because of no property name or if first element added
        /// </exception>
        public JSONFormatter V(float value)
        {
            return V(null, value);
        }
        /// <summary> Add a property called name with a double value</summary>
        /// <exception cref="FormatterException">Thrown if adding to an array or start with a property name
        /// </exception>
        public JSONFormatter V(string name, double value)
        {
            Prefix(name != null);
            if (name != null)
            {
                json.Append('"');
                json.Append(name);
                json.Append("\":");
            }
            json.Append(value.ToString("R"));
            return this;
        }

        /// <summary> Add an array element with a double value</summary>
        /// <exception cref="FormatterException">Thrown if adding to an object because of no property name or if first element added
        /// </exception>
        public JSONFormatter V(double value)
        {
            return V(null, value);
        }

        /// <summary> Add a property called name with int value</summary>
        /// <exception cref="FormatterException">Thrown if adding to an array with a property name or if first element added
        /// </exception>
        public JSONFormatter V(string name, int value)
        {
            Prefix(name != null);
            if (name != null)
            {
                json.Append('"');
                json.Append(name);
                json.Append("\":");
            }
            json.Append(value.ToStringInvariant());
            //System.Diagnostics.Debug.WriteLine($"Int: `{json}`");
            return this;
        }

        /// <summary> Add an array element with int value</summary>
        /// <exception cref="FormatterException">Thrown if adding to an object because of no property name or if first element added
        /// </exception>
        public JSONFormatter V(int value)
        {
            return V(null, value);
        }

        /// <summary> Add a property called name with long value</summary>
        /// <exception cref="FormatterException">Thrown if adding to an array or start with a property name
        /// </exception>
        public JSONFormatter V(string name, long value)
        {
            Prefix(name != null);
            if (name != null)
            {
                json.Append('"');
                json.Append(name);
                json.Append("\":");
            }
            json.Append(value.ToStringInvariant());
            return this;
        }

        /// <summary> Add an array element with long value</summary>
        /// <exception cref="FormatterException">Thrown if adding to an object because of no property name or if first element added
        /// </exception>
        public JSONFormatter V(long value)
        {
            return V(null, value);
        }

        /// <summary> Add a property called name with bool value</summary>
        /// <exception cref="FormatterException">Thrown if adding to an array or start with a property name
        /// </exception>
        public JSONFormatter V(string name, bool value)
        {
            Prefix(name != null);
            if (name != null)
            {
                json.Append('"');
                json.Append(name);
                json.Append("\":");
            }
            json.Append(value ? "true" : "false");
            return this;
        }

        /// <summary> Add an array element with bool value</summary>
        /// <exception cref="FormatterException">Thrown if adding to an object because of no property name or if first element added
        /// </exception>
        public JSONFormatter V(bool value)
        {
            return V(null, value);
        }

        /// <summary> Add a property called name with a DateTime value formatted in Zulu time</summary>
        /// <exception cref="FormatterException">Thrown if adding to an array or start with a property name
        /// </exception>
        public JSONFormatter V(string name, DateTime value)
        {
            Prefix(name != null);
            if (name != null)
            {
                json.Append('"');
                json.Append(name);
                json.Append("\":");
            }
            json.Append('"');
            json.Append(value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            json.Append('"'); 
            return this;
        }

        /// <summary> Add an array element with a DateTime value formatted in Zulu time</summary>
        /// <exception cref="FormatterException">Thrown if adding to an object because of no property name or if first element added
        /// </exception>
        public JSONFormatter V(DateTime value)
        {
            return V(null, value);
        }

        /// <summary> Start an array, either unnamed (at start or in array) or with property name when in object</summary>
        /// <exception cref="FormatterException">Thrown if adding to an array or start with a property name, or if adding to an object without a property name
        /// </exception>
        public JSONFormatter Array(string name = null)
        {
            Prefix(name != null);
            if (name != null)
            {
                json.Append('"');
                json.Append(name);
                json.Append("\":[");
            }
            else
                json.Append("[");

            stack.Add(new StackEntry(StackType.Array, precomma));
            precomma = false;
            //System.Diagnostics.Debug.WriteLine($"Array: `{json}`");
            return this;
        }

        /// <summary> Start an object, either unnamed (at start or in array) or property name </summary>
        /// <exception cref="FormatterException">Thrown if adding to an array or start with a property name, or if adding to an object without a property name
        /// </exception>
        public JSONFormatter Object(string name = null)
        {
            Prefix(name != null);
            if (name != null)
            {
                json.Append('"');
                json.Append(name);
                json.Append("\":{");
            }
            else
                json.Append("{");

            stack.Add(new StackEntry(StackType.Object, precomma));
            precomma = false;
            //System.Diagnostics.Debug.WriteLine($"Object: `{json}`");
            return this;
        }

        /// <summary> Close the current array, object. Can close multiple levels if depth is greater than one </summary>
        public JSONFormatter Close(int depth = 1)
        {
            while (depth-- > 0 && stack.Count > 0)
            {
                StackEntry e = stack.Last();

                if (lf)
                {
                    json.Append(Environment.NewLine);
                    lf = false;
                }

                if (e.stacktype == StackType.Array)
                    json.Append(']');
                else
                    json.Append('}');

                precomma = e.precomma;
                stack.RemoveAt(stack.Count - 1);
            }

            //System.Diagnostics.Debug.WriteLine($"Close: `{json}`");

            return this;
        }

        /// <summary> Format the JSON output with a LF at this point </summary>
        public JSONFormatter LF()
        {
            lf = true;
            return this;
        }

        /// <summary> Close the JSON stream and return the JSON string representation</summary>
        public string Get()
        {
            Close(int.MaxValue);
            return CurrentText;
        }

        /// <summary> Clear the buffer</summary>
        public void Clear()
        {
            json.Clear();
        }

        /// <summary> For debugging purposes, return current text state. Use Get() to properly close and get the text </summary>
        public string CurrentText { get { return json.ToString(); } }
        /// <summary> How big is the JSON string</summary>
        public int Length { get { return json.Length; } }

        private enum StackType { Array, Object };
        private class StackEntry
        {
            public bool precomma;
            public StackType stacktype;
            public StackEntry(StackType a, bool b)
            { precomma = b; stacktype = a; }
        }

        private StringBuilder json;
        private List<StackEntry> stack;
        private bool precomma;          // starts false, every value sets it true.
        private bool lf;                // want a lf next

        /// <summary>
        /// Internal prefix function
        /// </summary>
        /// <param name="named">is named parameter </param>
        protected virtual void Prefix(bool named)
        {
            if (named)
            {
                if (stack.Count == 0)
                    throw new FormatterException("Can't start JSON with a property name");
                else if (stack.Last().stacktype == StackType.Array)
                    throw new FormatterException("Property names not allowed in arrays");
            }
            else
            {
                if (stack.Count > 0 && stack.Last().stacktype == StackType.Object)
                {
                    throw new FormatterException("Property name is required in an object");
                }
            }

            if (precomma)
                json.Append(',');

            if (lf)
            {
                json.Append(Environment.NewLine);
                lf = false;
            }

            precomma = true;
        }

    }
}
