﻿/*
 * Copyright © 2022 Robbyxp1 @ github.com
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
using System.Text;

namespace QuickJSON
{
    public partial class JToken 
    {
        /// <summary> Convert to string using string builder</summary>
        /// <param name="str">Stringbuilder to append to</param>
        /// <param name="token">Token to convert</param>
        /// <param name="prepad">Pad before token is outputted</param>
        /// <param name="postpad">Pad after token is outputted</param>
        /// <param name="oapad">Pad before objects or arrays are outputted</param>
        /// <param name="stringliterals">true to output strings and key names without escaping or quoting</param>
        public static void ToStringBuilder(StringBuilder str, JToken token, string prepad, string postpad, string oapad, bool stringliterals)
        {
            if (token.TokenType == TType.String)
            {
                if (stringliterals)       // used if your extracting the value of the data as a string, and not turning it back to json.
                    str.Append(prepad).Append((string)token.Value).Append(postpad);
                else
                    str.Append(prepad).Append('"').AppendEscapeControlCharsFull((string)token.Value).Append('"').Append(postpad);
            }
            else if (token.TokenType == TType.Double)
            {
                string sd = ((double)token.Value).ToStringInvariant("R");       // round trip it - use 'R' since minvalue won't work very well. See https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#RFormatString
                if (!(sd.Contains("E") || sd.Contains(".")))                // needs something to indicate its a double, and if it does not have a dot or E, it needs a .0
                    sd += ".0";
                str.Append(prepad).Append(sd).Append(postpad);
            }
            else if (token.TokenType == TType.Long)
                str.Append(prepad).Append(((long)token.Value).ToStringInvariant()).Append(postpad);
            else if (token.TokenType == TType.ULong)
                str.Append(prepad).Append(((ulong)token.Value).ToStringInvariant()).Append(postpad);
#if JSONBIGINT
            else if (token.TokenType == TType.BigInt)
                str.Append(prepad).Append(((System.Numerics.BigInteger)token.Value).ToString(System.Globalization.CultureInfo.InvariantCulture)).Append(postpad);
#endif
            else if (token.TokenType == TType.Boolean)
                str.Append(prepad).Append(((bool)token.Value).ToString().ToLower()).Append(postpad);
            else if (token.TokenType == TType.Null)
                str.Append(prepad).Append("null").Append(postpad);
            else if (token.TokenType == TType.Array)
            {
                str.Append(prepad).Append('[').Append(postpad);

                string arrpad = prepad + oapad;
                JArray ja = token as JArray;
                for (int i = 0; i < ja.Count; i++)
                {
                    bool notlast = i < ja.Count - 1;
                    ToStringBuilder(str, ja[i], arrpad, postpad, oapad, stringliterals);
                    if (notlast)
                    {
                        str.Remove(str.Length - postpad.Length, postpad.Length);    // remove the postpad
                        str.Append(',').Append(postpad);
                    }
                }
                str.Append(prepad).Append(']').Append(postpad);
            }
            else if (token.TokenType == TType.Object)
            {
                str.Append(prepad).Append('{').Append(postpad);

                string objpad = prepad + oapad;
                int i = 0;
                JObject jo = ((JObject)token);
                foreach (var e in jo)
                {
                    bool notlast = i++ < jo.Count - 1;
                    if (e.Value is JObject || e.Value is JArray)
                    {
                        if (stringliterals)
                            str.Append(objpad).Append(e.Key).Append(':').Append(postpad);
                        else
                            str.Append(objpad).Append('"').AppendEscapeControlCharsFull(e.Key).Append("\":").Append(postpad);

                        ToStringBuilder(str,e.Value, objpad, postpad, oapad, stringliterals);

                        if (notlast)
                        {
                            str.Remove(str.Length - postpad.Length, postpad.Length);    // remove the postpad
                            str.Append(',').Append(postpad);
                        }
                    }
                    else
                    {
                        if (stringliterals)
                            str.Append(objpad).Append(e.Key).Append(':');
                        else
                            str.Append(objpad).Append('"').AppendEscapeControlCharsFull(e.Key).Append("\":");

                        ToStringBuilder(str, e.Value, "", "", oapad, stringliterals);
                        if (notlast)
                            str.Append(',');
                        str.Append(postpad);
                    }
                }

                str.Append(prepad).Append('}').Append(postpad);
            }
            else if (token.TokenType == TType.Error)
                str.Append("ERROR:" + (string)token.Value);
            else
                str.Append("?unknown");
        }
    }
}



