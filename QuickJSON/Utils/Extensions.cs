/*
* Copyright © 2021-2024 robbyxp1 @ github.com
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
*/

using System;
using System.Linq;
using System.Reflection;


namespace QuickJSON.Utils
{
    /// <summary>
    /// Extension class helpers for JSON system.
    /// </summary>

    public static class Extensions
    {

        /// <summary> Date helper function </summary>
        public static string ToStringZulu(this DateTime dt)     // zulu warrior format web style
        {
            if (dt.Millisecond != 0)
                return dt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            else
                return dt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");

        }

        /// <summary> Number helper function </summary>
        public static string ToStringInvariant(this int v)
        {
            return v.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        /// <summary> Number helper function </summary>
        public static string ToStringInvariant(this int v, string format)
        {
            return v.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
        }
        /// <summary> Number helper function </summary>
        public static string ToStringInvariant(this uint v)
        {
            return v.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        /// <summary> Number helper function </summary>
        public static string ToStringInvariant(this uint v, string format)
        {
            return v.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
        }
        /// <summary> Number helper function </summary>
        public static string ToStringInvariant(this long v)
        {
            return v.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        /// <summary> Number helper function </summary>
        public static string ToStringInvariant(this long v, string format)
        {
            return v.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
        }
        /// <summary> Number helper function </summary>
        public static string ToStringInvariant(this ulong v)
        {
            return v.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        /// <summary> Number helper function </summary>
        public static string ToStringInvariant(this ulong v, string format)
        {
            return v.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
        }
        /// <summary> Number helper function - return 1 or 0 </summary>
        public static string ToStringIntValue(this bool v)
        {
            return v ? "1" : "0";
        }
        /// <summary> Number helper function </summary>
        public static string ToStringInvariant(this bool? v)
        {
            return (v.HasValue) ? (v.Value ? "1" : "0") : "";
        }
        /// <summary> Number helper function </summary>
        public static string ToStringInvariant(this double v, string format)
        {
            return v.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
        }
        /// <summary> Number helper function </summary>
        public static string ToStringInvariant(this double v)
        {
            return v.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        /// <summary> Number helper function </summary>
        public static string ToStringInvariant(this float v, string format)
        {
            return v.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
        }
        /// <summary> Number helper function </summary>
        public static string ToStringInvariant(this float v)
        {
            return v.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        /// <summary> Number helper function </summary>
        public static string ToStringInvariant(this double? v, string format)
        {
            return (v.HasValue) ? v.Value.ToString(format, System.Globalization.CultureInfo.InvariantCulture) : "";
        }
        /// <summary> Number helper function </summary>
        public static string ToStringInvariant(this float? v, string format)
        {
            return (v.HasValue) ? v.Value.ToString(format, System.Globalization.CultureInfo.InvariantCulture) : "";
        }
        /// <summary> Number helper function </summary>
        public static string ToStringInvariant(this int? v)
        {
            return (v.HasValue) ? v.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : "";
        }
        /// <summary> Number helper function </summary>
        public static string ToStringInvariant(this int? v, string format)
        {
            return (v.HasValue) ? v.Value.ToString(format, System.Globalization.CultureInfo.InvariantCulture) : "";
        }
        /// <summary> Number helper function </summary>
        public static string ToStringInvariant(this long? v)
        {
            return (v.HasValue) ? v.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : "";
        }
        /// <summary> Number helper function </summary>
        public static string ToStringInvariant(this long? v, string format)
        {
            return (v.HasValue) ? v.Value.ToString(format, System.Globalization.CultureInfo.InvariantCulture) : "";
        }


        /// <summary> Approximate equal </summary>
        public static bool ApproxEquals(this double left, double right, double epsilon = 2.2204460492503131E-16)       // fron newtonsoft JSON, et al, calculate relative epsilon and compare
        {
            if (left == right)
            {
                return true;
            }

            double tolerance = ((Math.Abs(left) + Math.Abs(right)) + 10.0) * epsilon;       // given an arbitary epsilon, scale to magnitude of values
            double difference = left - right;
            //System.Diagnostics.Debug.WriteLine("Approx equal {0} {1}", tolerance, difference);
            return (-tolerance < difference && tolerance > difference);
        }

        /// <summary> Change value to type </summary>
        public static Object ChangeTo(this Type type, Object value)     // this extends ChangeType to handle nullables.
        {
            Type underlyingtype = Nullable.GetUnderlyingType(type);     // test if its a nullable type (double?)
            if (underlyingtype != null)
            {
                if (value == null)
                    return null;
                else
                    return Convert.ChangeType(value, underlyingtype);
            }
            else
            {
                return Convert.ChangeType(value, type);       // convert to element type, which should work since we checked compatibility
            }
        }

        /// <summary> Set value in member </summary>
        public static bool SetValue(this MemberInfo mi, Object instance, Object value)   // given a member of fields/property, set value in instance
        {
            if (mi.MemberType == System.Reflection.MemberTypes.Field)
            {
                var fi = (System.Reflection.FieldInfo)mi;
                fi.SetValue(instance, value);
                return true;
            }
            else if (mi.MemberType == System.Reflection.MemberTypes.Property)
            {
                var pi = (System.Reflection.PropertyInfo)mi;
                if (pi.SetMethod != null)
                {
                    pi.SetValue(instance, value);
                    return true;
                }
                else
                    return false;
            }
            else
                throw new NotSupportedException();
        }

        /// <summary> Get Type of field property </summary>
        static public Type FieldPropertyType(this MemberInfo mi)        // from member info for properties/fields return type
        {
            if (mi.MemberType == System.Reflection.MemberTypes.Property)
                return ((System.Reflection.PropertyInfo)mi).PropertyType;
            else if (mi.MemberType == System.Reflection.MemberTypes.Field)
                return ((System.Reflection.FieldInfo)mi).FieldType;
            else
                return null;
        }

        /// <summary> Escape control characters </summary>
        public static string EscapeControlCharsFull(this string obj)        // unicode points not escaped out
        {
            string s = obj.Replace(@"\", @"\\");        // \->\\
            s = s.Replace("\r", @"\r");     // CR -> \r
            s = s.Replace("\"", "\\\"");     // " -> \"
            s = s.Replace("\t", @"\t");     // TAB - > \t
            s = s.Replace("\b", @"\b");     // BACKSPACE - > \b
            s = s.Replace("\f", @"\f");     // FORMFEED -> \f
            s = s.Replace("\n", @"\n");     // LF -> \n
            return s;
        }

        /// <summary> Append and escape control characters </summary>
        public static System.Text.StringBuilder AppendEscapeControlCharsFull(this System.Text.StringBuilder str, string s)
        {
            foreach (var c in s)
            {
                if (c == '\\')
                    str.Append(@"\\");
                else if (c == '\r')
                    str.Append(@"\r");
                else if (c == '"')
                    str.Append("\\\"");
                else if (c == '\t')
                    str.Append(@"\t");
                else if (c == '\b')
                    str.Append(@"\b");
                else if (c == '\f')
                    str.Append(@"\f");
                else if (c == '\n')
                    str.Append(@"\n");
                else
                    str.Append(c);
            }
            return str;
        }

        /// <summary> Convert character to hex value </summary>
        static public int? ToHex(this char c)
        {
            if (char.IsDigit(c))
                return c - '0';
            else if ("ABCDEF".Contains(c))
                return c - 'A' + 10;
            else if ("abcdef".Contains(c))
                return c - 'a' + 10;
            else
                return null;
        }

        /// <summary> Change wildcard to Reg Ex Wildcard </summary>
        public static string RegExWildCardToRegular(this string value)
        {
            if (value.Contains("*") || value.Contains("?"))
                return "^" + System.Text.RegularExpressions.Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
            else
                return "^" + value + ".*$";
        }

        /// <summary> Perform wild card Reg Ex </summary>
        public static bool WildCardMatch(this string value, string match, bool caseinsensitive = false)
        {
            match = match.RegExWildCardToRegular();
            return System.Text.RegularExpressions.Regex.IsMatch(value, match, caseinsensitive ? System.Text.RegularExpressions.RegexOptions.IgnoreCase : System.Text.RegularExpressions.RegexOptions.None);
        }

        /// <summary> Calculate a String parser CheckSumCharBlock value </summary>
        public static uint Checksum(this string s)
        {
            uint checksum = 0;

            foreach (var ch in s)
            {
                checksum += 7 + (uint)ch * 23;
            }
            return Math.Max(1, checksum);
        }


        /// <summary>
        /// Parse integer from string invariant culture, return null if not parsed
        /// </summary>
        /// <param name="s">string</param>
        /// <returns>null of integer</returns>
        static public int? InvariantParseIntNull(this string s)     // s can be null
        {
            int i;
            if (s != null && int.TryParse(s, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out i))
                return i;
            else
                return null;

        }
    }
}

