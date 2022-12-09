/*
 * Copyright © 2020-2022 Robbyxp1 @ github.com
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

namespace QuickJSON
{
    public partial class JToken
    {
        /// <summary> Convert to string default settings </summary>
        /// <returns>JSON string representation</returns>
        public override string ToString()
        {
            return ToString(this, "", "", "", false);
        }
        /// <summary> Convert to string with strings themselves being unquoted or escaped.
        /// Useful for data extraction purposes</summary>
        /// <returns>JSON string representation</returns>
        public string ToStringLiteral()
        {
            return ToString(this, "", "", "", true);
        }

        /// <summary> Convert to string </summary>
        /// <param name="verbose">If verbose, pad the structure out</param>
        /// <param name="oapad">Pad before objects or arrays are outputted (only for verbose=true) mode</param>
        /// <returns>JSON string representation</returns>
        public string ToString(bool verbose = false, string oapad = "  ")
        {
            return verbose ? ToString(this, "", "\r\n", oapad, false) : ToString(this, "", "", "", false);
        }

        /// <summary> Convert to string with ability to control the array/output pad</summary>
        /// <param name="oapad">Pad before objects or arrays are outputted</param>
        /// <returns>JSON string representation</returns>
        public string ToString(string oapad)            // not verbose, but prefix in from of obj/array is configuration
        {
            return ToString(this, "", "", oapad, false);
        }

        /// <summary> Convert to string </summary>
        /// <param name="token">Token to convert</param>
        /// <param name="prepad">Pad before token is outputted</param>
        /// <param name="postpad">Pad after token is outputted</param>
        /// <param name="oapad">Pad before objects or arrays are outputted</param>
        /// <param name="stringliterals">true to output strings or keys without escaping or quoting</param>
        /// <returns>JSON string representation</returns>
        public static string ToString(JToken token, string prepad, string postpad, string oapad, bool stringliterals)
        {
            var sb = new System.Text.StringBuilder();
            ToStringBuilder(sb, token, prepad, postpad, oapad, stringliterals);
            return sb.ToString();
        }
    }
}



