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
    /// Extension class helpers for converting a JToken safely to a c# class.
    /// Null may be passed as the JToken and it will return the default given in the converter
    /// ie. token["propertyname"].Str("default") works safely even if propertyname does not exist.
    /// </summary>
    public static class JTokenExtensionsGet
    {
        /// <summary>Index into JObject or JArray safely</summary>
        public static JToken I(this JToken token, object id)           // safe [] allowing previous to be null
        {
            return token != null ? token[id] : null;
        }

        /// <summary> Is token null or JToken Null</summary>
        public static bool IsNull(this JToken token)
        {
            return token == null || token.IsNull;
        }

        /// <summary> Return a string looking for this list of property names. Return def if not found or not string </summary>
        public static string MultiStr(this JObject token, string[] propertynameslist, string def = "")  
        {
            JToken t = token?.Contains(propertynameslist);
            return t != null && t.IsString ? (string)t.Value : def;
        }

        /// <summary> Return a string looking for this list of property names. Return def if not found or not string 
        /// This one you must supply def but you can use the params array syntax with </summary>
        public static string MultiStr(this JObject token, string def, params string[] propertynameslist)
        {
            JToken t = token?.Contains(propertynameslist);
            return t != null && t.IsString ? (string)t.Value : def;
        }

        /// <summary> Return a string from the token. Return def if null or not a string</summary>
        public static string Str(this JToken token, string def = "")       // if not string, or null, return def.
        {
            return token != null ? ((string)token ?? def) : def;
        }

        /// <summary> Return a string from the token. Return null if not string</summary>
        public static string StrNull(this JToken token)
        {
            return token != null ? (string)token : null;
        }

        /// <summary> Return the enumeration of T from a JToken Long representation of the value of the enumeration. Return def if token is not present, null or not a long</summary>
        /// <typeparam name="T">Enumeration type to use</typeparam>
        public static T Enum<T>(this JToken token, T def)      
        {
            if (token != null && token.IsLong)
            {
                var i = (int)(long)token.Value;
                return (T)System.Enum.ToObject(typeof(T), i);
            }
            else
                return def;
        }

        /// <summary> Return the enumeration of T from a JToken string representation of the value of the enumeration. Return def if token is not present, null or not a string.</summary>
        /// <typeparam name="T">Enumeration type to use</typeparam>
        public static T EnumStr<T>(this JToken token, T def, bool ignorecase = true) where T:struct    
        {
            if (token != null && token.IsString)
            {
                string s = (string)token.Value;
                if (System.Enum.TryParse(s, ignorecase,out T result) )
                {
                    return result;
                }
            }

            return def;
        }

        /// <summary> Return an integer. Return def if token is not present, not a number or null</summary>
        public static int Int(this JToken token, int def = 0)
        {
            if (token != null)
                return (int?)token ?? def;
            else
                return def;
        }

        /// <summary> Return an integer. Return null if token is not present, not a number or null</summary>
        public static int? IntNull(this JToken token)
        {
            return token != null ? (int?)token : null;
        }

        /// <summary>
        /// Try and get an integer
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="value">integer return</param>
        /// <returns>true if token is not null and is an value which can convert to an integer </returns>
        public static bool TryGetInt(this JToken token, out int value)
        {
            if (token != null)
            {
                int? res = (int?)token;
                if (res.HasValue)
                {
                    value = res.Value;
                    return true;
                }
            }
            value = 0;
            return false;
        }

        /// <summary> Return an unsigned integer. Return def if token is not present, not a number or null</summary>
        public static uint UInt(this JToken token, uint def = 0)
        {
            if (token != null)
                return (uint?)token ?? def;
            else
                return def;
        }

        /// <summary> Return an unsigned integer. Return null if token is not present, not a number or null</summary>
        public static uint? UIntNull(this JToken token)
        {
            return token != null ? (uint?)token : null;
        }

        /// <summary>
        /// Try and get an integer
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="value">integer return</param>
        /// <returns>true if token is not null and is an value which can convert to an integer </returns>
        public static bool TryGetUInt(this JToken token, out uint value)
        {
            if (token != null)
            {
                uint? res = (uint?)token;
                if (res.HasValue)
                {
                    value = res.Value;
                    return true;
                }
            }
            value = 0;
            return false;
        }


        /// <summary> Return a long. Return def if token is not present, not a number or null</summary>
        public static long Long(this JToken token, long def = 0)
        {
            if (token != null)
                return (long?)token ?? def;
            else
                return def;
        }

        /// <summary> Return a long. Return null if token is not present, not a number or null</summary>
        public static long? LongNull(this JToken token)
        {
            return token != null ? (long?)token : null;
        }

        /// <summary>
        /// Try and get an integer
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="value">integer return</param>
        /// <returns>true if token is not null and is an value which can convert to an integer </returns>
        public static bool TryGetLong(this JToken token, out long value)
        {
            if (token != null)
            {
                long? res = (long?)token;
                if (res.HasValue)
                {
                    value = res.Value;
                    return true;
                }
            }
            value = 0;
            return false;
        }

        /// <summary> Return an unsigned long. Return def if token is not present, not a number or null</summary>
        public static ulong ULong(this JToken token, ulong def = 0)
        {
            if (token != null)
                return (ulong?)token ?? def;
            else
                return def;
        }

        /// <summary> Return an unsigned long. Return null if token is not present, not a number or null</summary>
        public static ulong? ULongNull(this JToken token)
        {
            return token != null ? (ulong?)token : null;
        }

        /// <summary>
        /// Try and get an integer
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="value">integer return</param>
        /// <returns>true if token is not null and is an value which can convert to an integer </returns>
        public static bool TryGetULong(this JToken token, out ulong value)
        {
            if (token != null)
            {
                ulong? res = (ulong?)token;
                if (res.HasValue)
                {
                    value = res.Value;
                    return true;
                }
            }
            value = 0;
            return false;
        }

        /// <summary> Return a double. Return def if token is not present, not a number or null</summary>
        public static double Double(this JToken token, double def = 0)
        {
            if (token != null)
                return (double?)token ?? def;
            else
                return def;
        }

        /// <summary> Return a float with scaling. Return def if token is not present, not a number or null.</summary>
        public static double Double(this JToken token, double scale, double def)
        {
            if (token != null)
            {
                double? v = (double?)token;
                if (v != null)
                    return v.Value * scale;
            }
            return def;
        }


        /// <summary> Return a double. Return null if token is not present, not a number or null</summary>
        public static double? DoubleNull(this JToken token)
        {
            return token != null ? (double?)token : null;
        }

        /// <summary> Return a double with scaling. Return null if token is not present, not a number or null</summary>
        public static double? DoubleNull(this JToken token, double scale)
        {
            if ( token != null )
            {
                double? v = (double?)token;
                if (v != null)
                    return v.Value * scale;
            }
            return null;
        }

        /// <summary>
        /// Try and get an double
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="value">double return</param>
        /// <returns>true if token is not null and is an value which can convert to an double </returns>
        public static bool TryGetDouble(this JToken token, out double value)
        {
            if (token != null)
            {
                double? res = (double?)token;
                if (res.HasValue)
                {
                    value = res.Value;
                    return true;
                }
            }
            value = 0;
            return false;
        }

        /// <summary> Return a float. Return def if token is not present, not a number or null</summary>
        public static float Float(this JToken token, float def = 0)
        {
            if (token != null)
                return (float?)token ?? def;
            else
                return def;
        }

        /// <summary> Return a float with scaling. Return def if token is not present, not a number or null.</summary>
        public static float Float(this JToken token, float scale, float def)
        {
            if (token != null)
            {
                float? v = (float?)token;
                if (v != null)
                    return v.Value * scale;
            }

            return def;
        }

        /// <summary> Return a float. Return null if token is not present, not a number or null</summary>
        public static float? FloatNull(this JToken token)
        {
            return token != null ? (float?)token : null;
        }

        /// <summary> Return a float with scale. Return null if token is not present, not a number or null</summary>
        public static float? FloatNull(this JToken token, float scale)
        {
            if (token != null)
            {
                float? v = (float?)token;
                if (v != null)
                    return v.Value * scale;
            }

            return null;
        }

        /// <summary>
        /// Try and get a float
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="value">float return</param>
        /// <returns>true if token is not null and is an value which can convert to an float</returns>
        public static bool TryGetFloat(this JToken token, out float value)
        {
            if (token != null)
            {
                float? res = (float?)token;
                if (res.HasValue)
                {
                    value = res.Value;
                    return true;
                }
            }
            value = 0;
            return false;
        }


#if JSONBIGINT
        /// <summary> Return a Big Integer. Return def if token is not present, not a number or null</summary>
        public static System.Numerics.BigInteger BigInteger(this JToken token, System.Numerics.BigInteger def)
        {
            if (token == null)
                return def;
            else if (token.TokenType == JToken.TType.ULong)
                return (ulong)token.Value;
            else if (token.IsLong )
                return (long)token.Value;
            else if (token.TokenType == JToken.TType.BigInt)
                return (System.Numerics.BigInteger)token.Value;
            else
                return def;
        }
        /// <summary> Return a Big Integer. Return null if token is not present, not a number or null</summary>
        public static System.Numerics.BigInteger? BigIntegerNull(this JToken token)
        {
            if (token == null)
                return null;
            else if (token.TokenType == JToken.TType.ULong)
                return (ulong)token.Value;
            else if (token.IsLong )
                return (long)token.Value;
            else if (token.TokenType == JToken.TType.BigInt)
                return (System.Numerics.BigInteger)token.Value;
            else
                return null;
        }
#endif

        /// <summary> Return a bool. Return def if token is not present, not a number or null</summary>
        public static bool Bool(this JToken token, bool def = false)
        {
            if ( token != null )
                return (bool?)token ?? def;
            else
                return def;
        }

        /// <summary> Return an integer. Return null if token is not present, not a number or null</summary>
        public static bool? BoolNull(this JToken token)
        {
            return token != null ? (bool?)token : null;
        }

        /// <summary>
        /// Try and get a bool
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="value">bool return</param>
        /// <returns>true if token is not null and is an value which can convert to an bool</returns>
        public static bool TryGetBool(this JToken token, out bool value)
        {
            if (token != null)
            {
                bool? res = (bool?)token;
                if (res.HasValue)
                {
                    value = res.Value;
                    return true;
                }
            }
            value = false;
            return false;
        }


        /// <summary> Return a nullable DateTime with the parse of the string JToken. </summary>
        /// <param name="token">Token to convert. Must be a string. May be null.</param>
        /// <param name="cultureinfo">Culture</param>
        /// <param name="datetimestyle">Convert style</param>
        /// <returns>DateTime, or null if can't convert</returns>
        public static DateTime? DateTime(this JToken token, System.Globalization.CultureInfo cultureinfo, System.Globalization.DateTimeStyles datetimestyle = System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal)
        {
            if (token != null && token.IsString && System.DateTime.TryParse((string)token.Value, cultureinfo, datetimestyle, out DateTime ret))
                return ret;
            else
                return null;
        }

        /// <summary> Return a DateTime with the parse of the string JToken. </summary>
        /// <param name="token">Token to convert. Must be a string. May be null.</param>
        /// <param name="def">Default time if conversion fails</param>
        /// <param name="cultureinfo">Culture</param>
        /// <param name="datetimestyle">Convert style</param>
        /// <returns>DateTime</returns>
        public static DateTime DateTime(this JToken token, DateTime def, System.Globalization.CultureInfo cultureinfo, System.Globalization.DateTimeStyles datetimestyle = System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal)
        {
            if (token != null && token.IsString && System.DateTime.TryParse((string)token.Value, cultureinfo, datetimestyle, out DateTime ret))
                return ret;
            else
                return def;
        }

        /// <summary> Return a DateTime presuming UTC with the parse of the string JToken. </summary>
        /// <param name="token">Token to convert. Must be a string. May be null.</param>
        /// <returns>DateTime with Kind=UTC, or DateTime.MinValue (with Kind=UTC) if conversion fails</returns>
        public static DateTime DateTimeUTC(this JToken token)
        {
            if (token != null && token.IsString && System.DateTime.TryParse((string)token.Value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal, out DateTime ret))
                return ret;
            else
                return new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);        //Minvalue in utc mode
        }
        /// <summary> Return a DateTime presuming UTC with the parse of the string JToken. </summary>
        /// <param name="token">Token to convert. Must be a string. May be null.</param>
        /// <param name="def">Default time if conversion fails. Converted to UTC</param>
        /// <returns>DateTime with Kind=UTC, or def (with Kind=UTC) if conversion fails</returns>
        public static DateTime DateTimeUTC(this JToken token, DateTime def)
        {
            if (token != null && token.IsString && System.DateTime.TryParse((string)token.Value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal, out DateTime ret))
                return ret;
            else
                return def.ToUniversalTime();
        }

        /// <summary>Convert a string token to an enumeration of type T.  Return def if conversion fails </summary>
        /// <typeparam name="T">Enumeration type to convert</typeparam>
        /// <param name="token">Token to convert. Must be a string. May be null.</param>
        /// <param name="def">Default enumeration value</param>
        /// <param name="preprocess">Function to call to preprocess the string before conversion. May be null</param>
        /// <returns>Enumeration, or def if converstion fails.</returns>
        public static T Enumeration<T>(this JToken token, T def, Func<string, string> preprocess = null)
        {
            if (token != null && token.IsString)
            {
                try
                {
                    string v = (string)token.Value;
                    if (preprocess != null)
                        v = preprocess(v);
                    return (T)System.Enum.Parse(typeof(T), v, true);
                }
                catch
                {
                }
            }

            return def;
        }

        /// <summary> Convert token to JArray</summary>
        public static JArray Array(this JToken token)       // null if not
        {
            return token as JArray;
        }

        /// <summary> Convert token to JObject</summary>
        public static JObject Object(this JToken token)     // null if not
        {
            return token as JObject;
        }
    }
}



