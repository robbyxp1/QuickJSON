/*
 * Copyright © 2016-2018 EDDiscovery development team
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
 *
 * EDDiscovery is not affiliated with Frontier Developments plc.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using QuickJSON.Utils;

namespace QuickJSON.FluentFormatter
{
    /// <summary>
    /// Quick formatter using Fluent syntax, quick and easy way to make a JSON string
    /// </summary>

    public class JSONFormatter        
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
            json = "";
            stack = new List<StackEntry>();
            precomma = false;
        }

        /// <summary> Add a property called name with string data</summary>
        /// <exception cref="QuickJSON.FluentFormatter.JSONFormatter.FormatterException">Thrown if adding to an array or start with a property name
        /// </exception>
        public JSONFormatter V(string name, string data)
        {
            Prefix(name != null);
            if (name != null)
                json += "\"" + name + "\":";
            json += "\"" + data + "\"";
            return this;
        }

        /// <summary> Add an array element with string data</summary>
        /// <exception cref="QuickJSON.FluentFormatter.JSONFormatter.FormatterException">Thrown if adding to an object or start because of no property name
        /// </exception>
        public JSONFormatter V(string data)
        {
            return V(null, data);
        }

        /// <summary> Add a property called name with a double value</summary>
        /// <exception cref="QuickJSON.FluentFormatter.JSONFormatter.FormatterException">Thrown if adding to an array or start with a property name
        /// </exception>
        public JSONFormatter V(string name, double value)
        {
            Prefix(name != null);
            if (name != null)
                json += "\"" + name + "\":";
            json += value.ToString("R");
            return this;
        }

        /// <summary> Add an array element with a double value</summary>
        /// <exception cref="QuickJSON.FluentFormatter.JSONFormatter.FormatterException">Thrown if adding to an object or start because of no property name
        /// </exception>
        public JSONFormatter V(double value)
        {
            return V(null, value);
        }

        /// <summary> Add a property called name with int value</summary>
        /// <exception cref="QuickJSON.FluentFormatter.JSONFormatter.FormatterException">Thrown if adding to an array or start with a property name
        /// </exception>
        public JSONFormatter V(string name, int value)
        {
            Prefix(name != null);
            if (name != null)
                json += "\"" + name + "\":";
            json += value.ToStringInvariant();
            return this;
        }

        /// <summary> Add an array element with int value</summary>
        /// <exception cref="QuickJSON.FluentFormatter.JSONFormatter.FormatterException">Thrown if adding to an object or start because of no property name
        /// </exception>
        public JSONFormatter V(int value)
        {
            return V(null, value);
        }

        /// <summary> Add a property called name with long value</summary>
        /// <exception cref="QuickJSON.FluentFormatter.JSONFormatter.FormatterException">Thrown if adding to an array or start with a property name
        /// </exception>
        public JSONFormatter V(string name, long value)
        {
            Prefix(name != null);
            if (name != null)
                json += "\"" + name + "\":";
            json += value.ToStringInvariant();
            return this;
        }

        /// <summary> Add an array element with long value</summary>
        /// <exception cref="QuickJSON.FluentFormatter.JSONFormatter.FormatterException">Thrown if adding to an object or start because of no property name
        /// </exception>
        public JSONFormatter V(long value)
        {
            return V(null, value);
        }

        /// <summary> Add a property called name with bool value</summary>
        /// <exception cref="QuickJSON.FluentFormatter.JSONFormatter.FormatterException">Thrown if adding to an array or start with a property name
        /// </exception>
        public JSONFormatter V(string name, bool value)
        {
            Prefix(name != null);
            if (name != null)
                json += "\"" + name + "\":";
            json += (value ? "true" : "false");
            return this;
        }

        /// <summary> Add an array element with bool value</summary>
        /// <exception cref="QuickJSON.FluentFormatter.JSONFormatter.FormatterException">Thrown if adding to an object or start because of no property name
        /// </exception>
        public JSONFormatter V(bool value)
        {
            return V(null, value);
        }

        /// <summary> Add a property called name with a DateTime value formatted in Zulu time</summary>
        /// <exception cref="QuickJSON.FluentFormatter.JSONFormatter.FormatterException">Thrown if adding to an array or start with a property name
        /// </exception>
        public JSONFormatter V(string name, DateTime value)
        {
            Prefix(name != null);
            if (name != null)
                json += "\"" + name + "\":";
            json += "\"" + value.ToString("yyyy-MM-ddTHH:mm:ssZ") + "\"";
            return this;
        }

        /// <summary> Add an array element with a DateTime value formatted in Zulu time</summary>
        /// <exception cref="QuickJSON.FluentFormatter.JSONFormatter.FormatterException">Thrown if adding to an object or start because of no property name
        /// </exception>
        public JSONFormatter V(DateTime value)
        {
            return V(null, value);
        }

        /// <summary> Start an array, either unnamed (at start or in array) or with property name</summary>
        /// <exception cref="QuickJSON.FluentFormatter.JSONFormatter.FormatterException">Thrown if adding to an array or start with a property name
        /// </exception>
        public JSONFormatter Array(string name = null)        
        {
            Prefix(name != null);
            if ( name != null)
                json += "\"" + name + "\": [ ";
            else
                json += "[ ";

            stack.Add(new StackEntry(StackType.Array, precomma));
            precomma = false;
            return this;
        }

        /// <summary> Start an object, either unnamed (at start or in array) or property name </summary>
        /// <exception cref="QuickJSON.FluentFormatter.JSONFormatter.FormatterException">Thrown if adding to an array or start with a property name
        /// </exception>
        public JSONFormatter Object(string name = null)                  // call, add elements, call close
        {
            Prefix(name!= null);
            if ( name != null )
                json += "\"" + name + "\":{ ";

            json += "{ ";
            var se = new StackEntry(StackType.Object, precomma);
            stack.Add(se);
            System.Diagnostics.Debug.Assert(stack.Count > 0);
            precomma = false;
            return this;
        }

        /// <summary> Close the current array, object. Can close multiple levels if depth>0 </summary>
        public JSONFormatter Close( int depth = 1 )    // close one of more Arrays/Objects
        {
            while (depth-- > 0 && stack.Count > 0 )
            {
                StackEntry e = stack.Last();

                if (e.stacktype == StackType.Array)
                    json += " ]";
                else
                    json += " }";

                precomma = e.precomma;
                stack.RemoveAt(stack.Count - 1);
            }

            return this;
        }

        /// <summary> Format the JSON output with a LF at this point </summary>
        public JSONFormatter LF()
        {
            json += Environment.NewLine;
            return this;
        }

        /// <summary> Close the JSON stream and return the JSON string representation</summary>
        public string Get()                                 
        {
            Close(int.MaxValue);
            return json.Trim();
        }

        /// <summary> Close the JSON stream and return the JSON string representation</summary>
        public override string ToString()
        {
            return Get();
        }

        private enum StackType { Array, Object };
        private class StackEntry
        {
            public bool precomma;
            public StackType stacktype;
            public StackEntry(StackType a, bool b)
            { precomma = b; stacktype = a; }
        }

        private string json;

        private List<StackEntry> stack;
        private bool precomma;          // starts false, ever value sets it true.

        private void Prefix(bool named)
        {
            if ( named )
            {
                if (stack.Count == 0)
                    throw new FormatterException("Can't start JSON with a property name");
                else if (stack.Last().stacktype == StackType.Array)
                    throw new FormatterException("Property names not allowed in arrays");
            }
            else
            {
                if (stack.Count >0 && stack.Last().stacktype == StackType.Object)
                {
                    throw new FormatterException("Property name is required in an object");
                }
            }

            if (precomma)
                json += ", ";

            precomma = true;
        }

    }
}
