/*
 * Copyright 2024-2024 Robbyxp1 @ github.com
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

using QuickJSON.Utils;
using System;
using System.IO;
using System.Text;

namespace QuickJSON
{
    public partial class JSONFormatter
    {
        /// <summary>
        /// Convert a JToken to fluent code
        /// </summary>
        /// <param name="tk">Token tree</param>
        /// <param name="lf">if to index the code with new lines</param>
        /// <returns></returns>
        public static string ToFluent(JToken tk, bool lf = false)
        {
            StringBuilder sb = new StringBuilder();
            ToFluent(tk, sb, lf);
            return sb.ToString();
        }

        /// <summary>
        /// Convert a JToken to fluent code
        /// </summary>
        /// <param name="tk">Token tree</param>
        /// <param name="code">build into stringbuilder</param>
        /// <param name="lf">if to index the code with new lines</param>
        /// <param name="propertyname">Pass the property name to this level. Null on start</param>
        public static void ToFluent(JToken tk, StringBuilder code, bool lf = false, string propertyname = null)
        {
            if (tk.IsObject)
            {
                if (lf)
                    NewLine(code);

                if (propertyname!= null)
                {
                    code.Append(".Object(");
                    code.Append(propertyname.AlwaysQuoteString());
                    code.Append(')');
                }
                else
                    code.Append(".Object()");

                foreach (var kvp in tk.Object())
                {
                    ToFluent(kvp.Value, code, lf, kvp.Key);
                }

                code.Append(".Close()");
                if (lf)
                    NewLine(code);
            }
            else if (tk.IsArray)
            {
                if (lf)
                    NewLine(code);

                if (propertyname != null)
                {
                    code.Append(".Array(");
                    code.Append(propertyname.AlwaysQuoteString());
                    code.Append(')');
                }
                else
                    code.Append(".Array()");

                foreach (var v in tk.Array())
                {
                    ToFluent(v, code, lf);
                }

                code.Append(".Close()");

                if (lf)
                    NewLine(code);
            }
            else
            {
                string vstring = tk.ToString();

                if (propertyname!=null)
                {
                    code.Append(".V(");
                    code.Append(propertyname.AlwaysQuoteString());
                    code.Append(',');
                    code.Append(vstring);
                    code.Append(')');
                }
                else
                {
                    code.Append(".V(");
                    code.Append(vstring);
                    code.Append(')');
                }
            }
        }

        private static void NewLine(StringBuilder code)
        {
            if (code.Length > 0 && code[code.Length - 1] != '\n')
                code.Append(Environment.NewLine);
        }

    }
}
