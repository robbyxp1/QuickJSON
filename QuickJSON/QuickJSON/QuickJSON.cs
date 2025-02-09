﻿/*
 * Copyright © 2020-2024 Robbyxp1 @ github.com
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
using System.Collections;
using System.Collections.Generic;

namespace QuickJSON
{
    /// <summary>
    /// This namespace contains the Quick JSON parser
    /// </summary>
    internal static class NamespaceDoc { } // just for documentation purposes

    /// <summary>
    /// JToken is the base type of all JSON Tokens.  JObject and JArray are derived from this
    /// Provides Parsers and Decoders for all JSON properties
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("L{Level}:{Name}:{TokenType}= {ToString()}")]
    public partial class JToken : IEnumerable<JToken>, IEnumerable
    {
        /// <summary> Token Type</summary>
        public enum TType {
            /// <summary> JSON Null </summary>
            Null,
            /// <summary> JSON boolean value </summary>
            Boolean,
            /// <summary> JSON string </summary>
            String,
            /// <summary> JSON Number, real </summary>
            Double,
            /// <summary> JSON Number, long (64 bits)</summary>
            Long,
            /// <summary> JSON Number, unsigned long (64 bits)</summary>
            ULong,
            /// <summary> JSON Number, BigInt </summary>
            BigInt,
            /// <summary> JSON Object </summary>
            Object,
            /// <summary> JSON Array </summary>
            Array,
            /// <summary> For token reading only, an EndObject '}' token </summary>
            EndObject,
            /// <summary> For token reading only, an EndArray ']' token </summary>
            EndArray,
            /// <summary> In FromObject, an error has occurred. Value holds error string </summary>
            Error
        }

        /// <summary> The JToken type </summary>
        public TType TokenType { get; set; }
        /// <summary> Value of the token, if it has one </summary>
        public Object Value { get; set; }

        /// <summary> Name of token found during parsing if its a property of an JSON Object, or Null if not a property.
        /// Only set during Parse and ParseToken.  Not set on an compiler initialisation.
        /// On Parse, if the property name is empty it will be called !!!EmptyNameN!!! N is 0..
        /// If it is a repeat of a previous name, it will be called name[N] where N = 1..
        /// JObject [] must have unique names for all objects
        /// On ParseToken, this will be the name in the text, irrespective or empty or repeat.
        /// </summary>
        public string Name { get; set; }
        /// <summary> Normally null, set to the original name in Parse only if the name is empty or a repeat</summary>
        public string OriginalName { get; set; }
        /// <summary> The parsed name, either Name or OriginalName (if the property name was empty or a repeat), set on Parse or ParseToken only</summary>
        public string ParsedName { get { return OriginalName ?? Name; } }

        /// <summary> If the parsed name is empty or a repeat, it will be given a synthetic name. </summary>
        static public bool IsKeyNameSynthetic(string name) { return (name.StartsWith("!!!EmptyName") && name.EndsWith("!!!")) || (name.StartsWith("!!!Repeat-") && name.EndsWith("]!!!")); }

        /// <summary> Heirachy level, 0 onwards. Set in Parse and ParseToken only</summary>
        public int Level { get; set; }

        /// <summary> Is the token a property of an Object. Only set during Parse or ParseToken. 
        /// Compiler initialiser will not have this set</summary>
        public bool IsProperty { get { return Name != null; } }      

        /// <summary> Does the object have a Value. True for bool/string/number</summary>
        public bool HasValue { get { return Value != null;  } }
        /// <summary> Is the token a string </summary>
        public bool IsString { get { return TokenType == TType.String; } }
        /// <summary> Is the token a Integer Number</summary>
        public bool IsInt { get { return TokenType == TType.Long || TokenType == TType.ULong || TokenType == TType.BigInt; } }
        /// <summary> Is the token a Long</summary>
        public bool IsLong { get { return TokenType == TType.Long; } }
        /// <summary> Is the token a BigInt</summary>
        public bool IsBigInt { get { return TokenType == TType.BigInt; } }
        /// <summary> Is the token a Unsigned Long</summary>
        public bool IsULong { get { return TokenType == TType.ULong; } }
        /// <summary> Is the token a Real Number</summary>
        public bool IsDouble { get { return TokenType == TType.Double; } }
        /// <summary> Is the token a Real or Integer Number</summary>
        public bool IsNumber { get { return TokenType == TType.Double || TokenType == TType.Long || TokenType == TType.ULong || TokenType == TType.BigInt; } }
        /// <summary> Is the token a Boolean</summary>
        public bool IsBool { get { return TokenType == TType.Boolean; } }
        /// <summary> Is the token a JSON Array </summary>
        public bool IsArray { get { return TokenType == TType.Array; } }
        /// <summary> Is the token a JSON Object</summary>
        public bool IsObject { get { return TokenType == TType.Object; } }
        /// <summary> Is the token a Null</summary>
        public bool IsNull { get { return TokenType == TType.Null; } }
        /// <summary> Is the token a End Object marker</summary>
        public bool IsEndObject { get { return TokenType == TType.EndObject; } }    // only seen for TokenReader
        /// <summary> Is the token a End Array marker</summary>
        public bool IsEndArray { get { return TokenType == TType.EndArray; } }      // only seen for TokenReader
        /// <summary> Is the token an error token</summary>
        public bool IsInError { get { return TokenType == TType.Error; } }          // only seen for FromObject when asking for error return

        /// <summary>Return JSON Schema name of the object </summary>
        /// <returns>Schema type name or null for not json type</returns>
        public string GetSchemaTypeName()                                       
        {
            if (IsInt)
                return "integer";
            else if (IsDouble)
                return "number";
            else if (IsString)
                return "string";
            else if (IsBool)
                return "boolean";
            else if (IsArray)
                return "array";
            else if (IsObject)
                return "object";
            else if (IsNull)
                return "null";
            else
                System.Diagnostics.Debug.WriteLine($"{TokenType} is not a valid schema type");
            return null;
        }

        #region Construction

        /// <summary> Construct a JSON Null token (default)</summary>
        public JToken()
        {
            TokenType = TType.Null;
        }

        /// <summary> Construct a copy of another JToken</summary>
        public JToken(JToken other)
        {
            TokenType = other.TokenType;
            Value = other.Value;
            Name = other.Name;
        }

        /// <summary> Create a token </summary>
        /// <param name="tokentype">Token type to make</param>
        /// <param name="value">Optional, value of token</param>
        /// <param name="level">Set the level of the token in the JSON heirarchy</param>
        public JToken(TType tokentype, Object value = null ,int level = 0)
        {
            TokenType = tokentype; Value = value; Level = level;
        }

        /// <summary> Implicit conversion of a string to a JSON token of the type String </summary>
        public static implicit operator JToken(string v)        
        {
            if (v == null)
                return new JToken(TType.Null);
            else
                return new JToken(TType.String, v);
        }
        /// <summary> Implicit conversion of a bool to a JSON token of the type Bool </summary>
        public static implicit operator JToken(bool v)      // same order as https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types
        {
            return new JToken(TType.Boolean, v);
        }
        /// <summary> Implicit conversion of a byte to a JSON token of the type Long</summary>
        public static implicit operator JToken(byte v)
        {
            return new JToken(TType.Long, (long)v);
        }
        /// <summary> Implicit conversion of a sbyte to a JSON token of the type Long </summary>
        public static implicit operator JToken(sbyte v)
        {
            return new JToken(TType.Long, (long)v);
        }
        /// <summary> Implicit conversion of a character to a JSON token of the type String </summary>
        public static implicit operator JToken(char v)
        {
            return new JToken(TType.String, v.ToString());
        }
        /// <summary> Implicit conversion of a decimal to a JSON token of the type Long</summary>
        public static implicit operator JToken(decimal v)
        {
            return new JToken(TType.Long, (long)v);
        }
        /// <summary> Implicit conversion of a double to a JSON token of the type Double </summary>
        public static implicit operator JToken(double v)
        {
            return new JToken(TType.Double, v);
        }
        /// <summary> Implicit conversion of a float to a JSON token of the type Double </summary>
        public static implicit operator JToken(float v)
        {
            return new JToken(TType.Double, (double)v);
        }
        /// <summary> Implicit conversion of a int to a JSON token of the type Long </summary>
        public static implicit operator JToken(int v)
        {
            return new JToken(TType.Long, (long)v);
        }
        /// <summary> Implicit conversion of a unsigned int to a JSON token of the type Long </summary>
        public static implicit operator JToken(uint v)
        {
            return new JToken(TType.Long, (long)v);
        }
        /// <summary> Implicit conversion of a long to a JSON token of the type Long </summary>
        public static implicit operator JToken(long v)
        {
            return new JToken(TType.Long, v);
        }
        /// <summary> Implicit conversion of a ulong to a JSON token of the type Unsigned Long </summary>
        public static implicit operator JToken(ulong v)
        {
            return new JToken(TType.ULong, v);
        }
        /// <summary> Implicit conversion of a short to a JSON token of the type Long </summary>
        public static implicit operator JToken(short v)
        {
            return new JToken(TType.Long, (long)v);
        }
        /// <summary> Implicit conversion of a unsigned short to a JSON token of the type Long </summary>
        public static implicit operator JToken(ushort v)
        {
            return new JToken(TType.Long, (long)v);
        }
        /// <summary> Implicit conversion of a DateTime to a JSON token of the type String. 
        /// In the zulu format yyyy-mm-ddThh-mm-ssZ or yyyy-mm-ddThh-mm-ss.fffZ
        /// </summary>
        public static implicit operator JToken(DateTime v)      
        {
            return new JToken(TType.String, v.ToStringZulu());
        }

        /// <summary> Creata a token from an object 
        /// * Will convert null, string
        /// * Will convert bool,byte,sbyte,decimal,double,float,int,uint,long,ulong,short,ushort, DateTime and their ? types as per the implicit rules
        /// * Will convert a Enum type to a JSON string
        /// * Will clone a JArray or JObject
        /// </summary>
        /// <param name="obj">Object to make token from</param>
        /// <param name="except">True to except on error, else return null </param>
        public static JToken CreateToken(Object obj, bool except = true)
        {
            if (obj == null)
                return Null();
            else if (obj is string)
                return (string)obj;
            else if (obj is bool || obj is bool?)
                return (bool)obj;
            else if (obj is byte || obj is byte?)
                return (byte)obj;
            else if (obj is sbyte || obj is sbyte?)
                return (sbyte)obj;
            else if (obj is char || obj is char?)
                return (char)obj;
            else if (obj is decimal || obj is decimal?)
                return (decimal)obj;
            else if (obj is double || obj is double?)
                return (double)obj;
            else if (obj is float || obj is float?)
                return (float)obj;
            else if (obj is int || obj is int?)
                return (int)obj;
            else if (obj is uint || obj is uint?)
                return (uint)obj;
            else if (obj is long || obj is long?)
                return (long)obj;
            else if (obj is ulong || obj is ulong?)
                return (ulong)obj;
            else if (obj is short || obj is short?)
                return (short)obj;
            else if (obj is ushort || obj is ushort?)
                return (ushort)obj;
            else if (obj is JArray)
            {
                var ja = obj as JArray;
                return ja.Clone();
            }
            else if (obj is JObject)
            {
                var jo = obj as JObject;
                return jo.Clone();
            }
            else if (obj is Enum)
            {
                return obj.ToString();
            }
            else if (obj is DateTime)
            {
                return ((DateTime)obj).ToStringZulu();        // obeys current culture and calandar
            }
            else if (except)
                throw new NotImplementedException();
            else
            {
                System.Diagnostics.Trace.WriteLine("QuickJSON Failed to serialise type " + obj.GetType().Name);
                return null;
            }
        }

        /// <summary> Creates a Null JToken</summary>
        public static JToken Null()
        {
            return new JToken(TType.Null);
        }

        /// <summary> Explicit conversion of a JSON Token Null or String to a string. Return null if JToken is not a Null or String </summary>
        public static explicit operator string(JToken tk)
        {
            if (tk.TokenType == TType.Null || tk.TokenType != TType.String)
                return null;
            else
                return (string)tk.Value;
        }

        /// <summary> Explicit conversion of a JSON Token Long or Double to an int?. Return null if JToken is not a Long or Double </summary>
        public static explicit operator int? (JToken tk)     
        {                                                   
            if (tk.TokenType == TType.Long)                  // it won't be a ulong/bigint since that would be too big for an int
                return (int)(long)tk.Value;
            else if (tk.TokenType == TType.Double)           // doubles get trunced.. as per previous system
                return (int)(double)tk.Value;
            else
                return null;
        }

        /// <summary> Explicit conversion of a JSON Token Long or Double to an int.</summary>
        /// <exception cref="System.InvalidOperationException">If JToken is not a Long or Double
        /// </exception>
        public static explicit operator int(JToken tk)
        {
            if (tk.TokenType == TType.Long)
                return (int)(long)tk.Value;
            else if (tk.TokenType == TType.Double)
                return (int)(double)tk.Value;
            else
                throw new InvalidOperationException();
        }

        /// <summary> Explicit conversion of a JSON Token Long or Double to an uint?. Return null if JToken is not a Long or Double or negative</summary>
        public static explicit operator uint? (JToken tk)    
        {
            if (tk.TokenType == TType.Long && (long)tk.Value >= 0)        // it won't be a ulong/bigint since that would be too big for an uint
                return (uint)(long)tk.Value;
            else if (tk.TokenType == TType.Double && (double)tk.Value >= 0)   // doubles get trunced
                return (uint)(double)tk.Value;
            else
                return null;
        }

        /// <summary> Explicit conversion of a JSON Token Long or Double to an unsigned int. </summary>
        /// <exception cref="System.InvalidOperationException">If JToken is not a Long or Double or negative.
        /// </exception>
        public static explicit operator uint(JToken tk)
        {
            if (tk.TokenType == TType.Long && (long)tk.Value >= 0)
                return (uint)(long)tk.Value;
            else if (tk.TokenType == TType.Double && (double)tk.Value >= 0)
                return (uint)(double)tk.Value;
            else
                throw new InvalidOperationException();
        }

        /// <summary> Explicit conversion of a JSON Token Long or Double to a long?. Return null if JToken is not a Long or Double</summary>
        public static explicit operator long? (JToken tk)    
        {
            if (tk.TokenType == TType.Long)              
                return (long)tk.Value;
            else if (tk.TokenType == TType.Double)       
                return (long)(double)tk.Value;
            else
                return null;
        }

        /// <summary> Explicit conversion of a JSON Token Long or Double to a long.</summary>
        /// <exception cref="System.InvalidOperationException">If JToken is not a Long or Double
        /// </exception>
        public static explicit operator long(JToken tk)  
        {
            if (tk.TokenType == TType.Long)              // it won't be a ulong/bigint since that would be too big for an long
                return (long)tk.Value;
            else if (tk.TokenType == TType.Double)       // doubles get trunced
                return (long)(double)tk.Value;
            else
                throw new InvalidOperationException();
        }

        /// <summary> Explicit conversion of a JSON Token Unsigned Long, Long or Double to an unsigned long?. Return null if JToken is not a Long or Double or negative</summary>
        public static explicit operator ulong? (JToken tk) 
        {
            if (tk.TokenType == TType.ULong)             // it won't be a bigint since that would be too big for an ulong
                return (ulong)tk.Value;
            else if (tk.TokenType == TType.Long && (long)tk.Value >= 0)
                return (ulong)(long)tk.Value;
            else if (tk.TokenType == TType.Double && (double)tk.Value >= 0)       // doubles get trunced
                return (ulong)(double)tk.Value;
            else
                return null;
        }

        /// <summary> Explicit conversion of a JSON Token Unsigned Long, Long or Double to an unsigned long.</summary>
        /// <exception cref="System.InvalidOperationException">If JToken is not of the right type or negative
        /// </exception>
        public static explicit operator ulong(JToken tk)
        {
            if (tk.TokenType == TType.ULong)
                return (ulong)tk.Value;
            else if (tk.TokenType == TType.Long && (long)tk.Value >= 0)
                return (ulong)(long)tk.Value;
            else if (tk.TokenType == TType.Double && (double)tk.Value >= 0)
                return (ulong)(double)tk.Value;
            else
                throw new InvalidOperationException();
        }

        /// <summary> Explicit conversion of a JSON Token Long, Unsigned Long, BigInt or Double to a double?. Return null if JToken is not of the right type</summary>
        public static explicit operator double? (JToken tk)
        {
            if (tk.TokenType == TType.Long)                      // any of these types could be converted to double
                return (double)(long)tk.Value;
            else if (tk.TokenType == TType.ULong)
                return (double)(ulong)tk.Value;
#if JSONBIGINT
            else if (tk.TokenType == TType.BigInt)
                return (double)(System.Numerics.BigInteger)tk.Value;
#endif
            else if (tk.TokenType == TType.Double)
                return (double)tk.Value;
            else
                return null;
        }

        /// <summary> Explicit conversion of a JSON Token Unsigned Long, Long, BigInt or Double to a double. </summary>
        /// <exception cref="System.InvalidOperationException">If JToken is not of the right type
        /// </exception>
        public static explicit operator double(JToken tk)
        {
            if (tk.TokenType == TType.Long)                      
                return (double)(long)tk.Value;
            else if (tk.TokenType == TType.ULong)
                return (double)(ulong)tk.Value;
#if JSONBIGINT
            else if (tk.TokenType == TType.BigInt)
                return (double)(System.Numerics.BigInteger)tk.Value;
#endif
            else if (tk.TokenType == TType.Double)
                return (double)tk.Value;
            else
                throw new InvalidOperationException();
        }

        /// <summary> Explicit conversion of a JSON Token Long, Unsigned Long, BigInt or Double to a float?. Return null if JToken is not of the right type</summary>
        public static explicit operator float? (JToken tk)
        {
            if (tk.TokenType == TType.Long)                  // any of these types could be converted to double
                return (float)(long)tk.Value;
            else if (tk.TokenType == TType.ULong)
                return (float)(ulong)tk.Value;
#if JSONBIGINT
            else if (tk.TokenType == TType.BigInt)
                return (float)(System.Numerics.BigInteger)tk.Value;
#endif
            else if (tk.TokenType == TType.Double)
                return (float)(double)tk.Value;
            else
                return null;
        }

        /// <summary> Explicit conversion of a JSON Token Unsigned Long, Long, BigInt or Double to an float. </summary>
        /// <exception cref="System.InvalidOperationException">If JToken is not of the right type
        /// </exception>
        public static explicit operator float(JToken tk)
        {
            if (tk.TokenType == TType.Long)
                return (float)(long)tk.Value;
            else if (tk.TokenType == TType.ULong)
                return (float)(ulong)tk.Value;
#if JSONBIGINT
            else if (tk.TokenType == TType.BigInt)
                return (float)(System.Numerics.BigInteger)tk.Value;
#endif
            else if (tk.TokenType == TType.Double)
                return (float)(double)tk.Value;
            else
                throw new InvalidOperationException();
        }

        /// <summary> Explicit conversion of a JSON Token Boolean or Long to a bool. 
        /// Both true/false and integers (0=false, otherwise true) are acceptable
        /// Return null if JToken is not of the right type</summary>
        public static explicit operator bool? (JToken tk)
        {
            if (tk.TokenType == TType.Boolean)
                return (bool)tk.Value;
            else if (tk.TokenType == TType.Long)       // accept LONG 1/0 as boolean
                return (long)tk.Value != 0;
            else
                return null;
        }

        /// <summary> Explicit conversion of a JSON Token Boolean or Long to a bool.</summary>
        /// <exception cref="System.InvalidOperationException">If JToken is not of the right type
        /// </exception>
        public static explicit operator bool(JToken tk)
        {
            if (tk.TokenType == TType.Boolean)
                return (bool)tk.Value;
            else if (tk.TokenType == TType.Long)      
                return (long)tk.Value != 0;
            else
                throw new InvalidOperationException();
        }

        /// <summary> Explicit conversion of a JSON Token String to a date, assuming UTC. Return MinValue if JToken is not of the right type or date conversion fails</summary>
        public static explicit operator DateTime(JToken t)
        {
            if (t.IsString && System.DateTime.TryParse((string)t.Value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal, out DateTime ret))
                return ret;
            else
                return new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);        //Minvalue in utc mode
        }

        /// <summary> Return a copy of this JToken </summary>
        public JToken Clone()   // make a copy of the token
        {
            switch (TokenType)
            {
                case TType.Array:
                    {
                        JArray copy = new JArray();
                        foreach (JToken t in this)
                        {
                            copy.Add(t.Clone());
                        }

                        return copy;
                    }
                case TType.Object:
                    {
                        JObject copy = new JObject();
                        foreach (var kvp in (JObject)this)
                        {
                            copy[kvp.Key] = kvp.Value.Clone();
                        }
                        return copy;
                    }
                default:
                    return new JToken(this);
            }
        }

        #endregion

        #region Equality

        /// <summary> Is this token equal to another tokens value. Only for object types of string, int, uint, long, ulong, bool </summary>
        public bool ValueEquals(Object value)               
        {
            if (value is string)
                return ((string)this) != null && ((string)this).Equals((string)value);
            else if (value is int)
                return ((int?)this) != null && ((int)this).Equals((int)value);
            else if (value is uint)
                return ((uint?)this) != null && ((uint)this).Equals((uint)value);
            else if (value is ulong)
                return ((ulong?)this) != null && ((ulong)this).Equals((ulong)value);
            else if (value is long)
                return ((long?)this) != null && ((long)this).Equals((long)value);
            else if (value is bool)
                return ((bool?)this) != null && ((bool)this).Equals((bool)value);
            else
                return false;
        }

        /// <summary> Is this token equal to another tokens. For types incl double </summary>
        public bool ValueEquals(JToken other)
        {
            if( Value is string)
            {
                if (other.IsString)
                    return ((string)Value).Equals((string)other);
            }
            else if (Value is long)
            {
                if (other.IsLong || other.IsDouble)
                    return ((long)Value).Equals((long)other);
            }
            else if (Value is ulong)
            {
                if (other.IsULong)
                    return ((ulong)Value).Equals((ulong)other);
            }
            else if (Value is double)
            {
                if (other.IsNumber)
                    return ((double)Value).Equals((double)other);
            }
            else if (Value is bool)
            {
                if (other.IsBool)
                    return ((bool)Value).Equals((bool)other);
            }
            else if (TokenType == TType.Null)
            {
                if (other.TokenType == TType.Null)
                    return true;
            }

            return false;
        }

        #endregion

        #region Paths

        /// <summary>
        /// Get the token at the end of the path using schema format
        /// objectname/objectname only.  Do not include #/ as we do not have an absolute path.
        /// </summary>
        /// <param name="path">Path to the token</param>
        /// <returns></returns>
        public JToken GetTokenSchemaPath(string path)
        {
            JToken token = this;

            while ( path.Length>0)
            {
                if (!token.IsObject)
                    return null;

                int indexofslash = path.IndexOf("/");
                string name = indexofslash == -1 ? path : path.Substring(0, indexofslash);
                path = indexofslash == -1 ? "" : path.Substring(indexofslash + 1);

                if (token.Object().Contains(name))
                    token = token.Object()[name];
                else
                    return null;
            }

            return token;
        }

        /// <summary>
        /// Get the token at the end of the path using JSONPath format.  
        /// . [] format only. Do not include $ as we do not have an absolute path.
        /// https://support.smartbear.com/alertsite/docs/monitors/api/endpoint/jsonpath.html
        /// </summary>
        /// <param name="path">Path to the token</param>
        /// <returns></returns>
        public JToken GetToken(string path)
        {
            JToken token = this;

            while (path.Length > 0)
            {
                if (token.IsArray)
                {
                    if (path[0] == '[')
                    {
                        int indexofbracket = path.IndexOf("]");
                        if (indexofbracket == -1)
                            return null;
                        int? index = path.Substring(1, indexofbracket - 1).InvariantParseIntNull();
                        if (index == null)
                            return null;
                        if (index >= token.Count || index < 0)
                            return null;
                        token = token[index];
                        path = path.Substring(indexofbracket + 1);
                    }
                    else
                        return null;
                }
                else if (token.IsObject)
                {
                    if (path[0] == '.')
                    {
                        path = path.Substring(1);
                        int indexofdot = path.IndexOfAny(new char[] { '[', '.' });
                        string name = indexofdot != -1 ? path.Substring(0, indexofdot) : path;
                        path = indexofdot != -1 ? path.Substring(indexofdot) : "";

                        if (token.Object().Contains(name))
                            token = token[name];
                        else
                            return null;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }

            return token;
        }

        #endregion
        #region Operators and functions

        /// <summary> Access JToken in JArray or JObject by indexer. For JArray its an integer index (0+) and for JObject its the property string key name.
        /// Returns JToken found by indexer, or null if noRobet present, indexer out of range (JArray) or indexer is not the right type
        /// </summary>
        /// <exception cref="System.NotImplementedException">Thrown if used on an non indexed object
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">If indexer is out of range on set (JArray)
        /// </exception>
        /// <exception cref="System.InvalidCastException">If indexer is not of right type for object on set
        /// </exception>
        public virtual JToken this[object key] { get { return null; } set { throw new NotImplementedException(); } }

        /// <summary> Does the JObject contain property name</summary>
        public virtual bool Contains(string name) { return false; }

        /// <summary> Add to a JArray a JToken thru this class</summary>
        /// <exception cref="System.NotImplementedException">Thrown if used on an non indexed object
        /// </exception>
        public virtual void Add(JToken value) { throw new NotImplementedException(); }

        /// <summary> Add to a JArray a value of type T thru this class. T must be convertable to a JToken - see JToken Implicit conversions</summary>
        /// <exception cref="System.NotImplementedException">Thrown if used on an non indexed object
        /// </exception>
        public virtual void Add<T>(T value) { throw new NotImplementedException(); }

        /// <summary> Add a range of JTokens to a JArray thru this class.</summary>
        /// <exception cref="System.NotImplementedException">Thrown if used on an non indexed object
        /// </exception>
        public virtual void AddRange(IEnumerable<JToken> o) { throw new NotImplementedException(); }

        /// <summary> Add a range of items of type T to a JArray thru this class. T must be convertable to a JToken - see JToken Implicit conversions</summary>
        /// <exception cref="System.NotImplementedException">Thrown if used on an non indexed object
        /// </exception>
        public virtual void AddRange<T>(IEnumerable<T> values) { throw new NotImplementedException(); }

        /// <summary> Add a JToken with this property name thru this class.  Will overwrite any existing property</summary>
        /// <exception cref="System.NotImplementedException">Thrown if used on an non indexed object
        /// </exception>
        public virtual void Add(string key, JToken value) { throw new NotImplementedException(); }

        /// <summary> Add value of type T with this property name.  Will overwrite any existing property. T must be convertable to a JToken - see JToken Implicit conversions </summary>
        /// <exception cref="System.NotImplementedException">Thrown if used on an non indexed object
        /// </exception>
        public virtual void Add<T>(string key, T value) { throw new NotImplementedException(); }

        /// <summary> Get the first JToken </summary>
        /// <exception cref="System.NotImplementedException">Thrown if used on an non indexed object
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">If no items are present
        /// </exception>
        public virtual JToken First() { throw new NotImplementedException(); }

        /// <summary> Get the last JToken </summary>
        /// <exception cref="System.NotImplementedException">Thrown if used on an non indexed object
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">If no items are present
        /// </exception>
        public virtual JToken Last() { throw new NotImplementedException(); }

        /// <summary> Get the first JToken or null if no elements are in the list</summary>
        /// <exception cref="System.NotImplementedException">Thrown if used on an non indexed object
        /// </exception>
        public virtual JToken FirstOrDefault() { throw new NotImplementedException(); }

        /// <summary> Get the last JToken or null if no elements are in the list</summary>
        /// <exception cref="System.NotImplementedException">Thrown if used on an non indexed object
        /// </exception>
        public virtual JToken LastOrDefault() { throw new NotImplementedException(); }

        /// <summary> Get an Enumerator for the JToken</summary>
        /// <exception cref="System.NotImplementedException">Thrown if used on an non indexed object</exception>
        public IEnumerator<JToken> GetEnumerator()
        {
            return GetSubClassTokenEnumerator();
        }

        internal virtual IEnumerator<JToken> GetSubClassTokenEnumerator() { throw new NotImplementedException(); }

        /// <summary> Get a IEnumerator for the JToken </summary>
        /// <exception cref="System.NotImplementedException">Thrown if used on an non indexed object
        /// </exception>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetSubClassEnumerator();
        }

        internal virtual IEnumerator GetSubClassEnumerator() { throw new NotImplementedException(); }

        /// <summary> Get number of JArray or JObject items (of 0 for JToken) </summary>
        public virtual int Count { get { return 0; } }

        /// <summary> Clear JArray or JObject of items</summary>
        /// <exception cref="System.NotImplementedException">Thrown if used on an non indexed object
        /// </exception>
        public virtual void Clear() { throw new NotImplementedException(); }

        #endregion

        #region Debug Output Switch

        /// <summary> Set to enable trace output on failures which are ordered to be masked during operation</summary>
        public static bool TraceOutput { get; set; } = false;          
        #endregion

    }
}



