/*
 * Copyright © 2021-2025 Robbyxp1 @ github.com
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static QuickJSON.JToken;
using static QuickJSON.JTokenExtensions;

namespace QuickJSON
{
    /// <summary>
    /// Class which extends QuickJSON with the ability to create c# objects from JTokens
    /// </summary>
    public static class JTokenExtensions
    {
        /// <summary> Convert the JToken tree to an object of type T. 
        /// Type errors are not ignored</summary>
        /// <typeparam name="T">Convert to this type</typeparam>
        /// <param name="token">JToken to convert from</param>
        /// <returns>New object T containing fields filled by JToken, or default(T) on error</returns>
        public static T ToObjectQ<T>(this JToken token)
        {
            return ToObject<T>(token, false);
        }

        /// <summary> Convert the JToken tree to an object of type T. Protected from all exception sources</summary>
        /// <typeparam name="T">Convert to this type</typeparam>
        /// <param name="token">JToken to convert from</param>
        /// <param name="ignoretypeerrors">Ignore any errors in type between the JToken type and the member type.</param>
        /// <param name="process">For enums and datetime, and if set, process and return enum or DateTime</param>
        /// <param name="setname">Define set of JSON attributes to apply, null for default</param>
        /// <param name="customformat">Class members convert to object using this function if marked with [JsonCustomFormat]</param>
        /// <returns>New object T containing fields filled by JToken, or default(T) on error. </returns>
        public static T ToObject<T>(this JToken token, bool ignoretypeerrors = false,
                                        Func<Type, string, object> process = null,
                                        string setname = null,
                                        Func<Type, object, object> customformat = null)
        {
            try
            {
                ToObjectConverter cnv = new ToObjectConverter(ignoretypeerrors, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static, 
                                                                process, customformat, setname);
                Object ret = cnv.Execute(token, typeof(T), null, false);
                if (ret is ToObjectError)
                {
                    System.Diagnostics.Debug.WriteLine("JSON ToObject error:" + ((ToObjectError)ret).ErrorString + ":" + ((ToObjectError)ret).PropertyName);
                    return default(T);
                }
                else if (ret != null)      // or null
                    return (T)ret;          // must by definition have returned tt.
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception JSON ToObject " + ex.Message + " " + ex.StackTrace);
            }

            return default(T);
        }

        /// <summary> Convert the JToken tree to an object of type. Protected from all exception sources</summary>
        /// <param name="token">JToken to convert from</param>
        /// <param name="converttype">Type to convert to</param>
        /// <param name="ignoretypeerrors">Ignore any errors in type between the JToken type and the member type.</param>
        /// <param name="membersearchflags">Member search flags, to select what types of members are serialised</param>
        /// <param name="initialobject">If null, object is newed. If given, start from this object. Will except if it and the converttype is not compatible.</param>
        /// <param name="process">For enums and datetime, and if set, process and return enum or DateTime.</param>
        /// <param name="setname">Define set of JSON attributes to apply, null for default</param>
        /// <param name="customformat">Class members convert to object using this function if marked with [JsonCustomFormat]</param>
        /// <returns>Object containing fields filled by JToken, or a object of ToObjectError on named error, or null if no tokens</returns>

        public static Object ToObjectProtected(this JToken token, Type converttype, bool ignoretypeerrors,
                                    System.Reflection.BindingFlags membersearchflags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static,
                                    Object initialobject = null,
                                    Func<Type, string, object> process = null,
                                    string setname = null,
                                    Func<Type, object, object> customformat = null
                                    )
        {
            try
            {
                ToObjectConverter cnv = new ToObjectConverter(ignoretypeerrors, membersearchflags, process, customformat, setname);
                return cnv.Execute(token, converttype, initialobject, false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception JSON ToObject " + ex.Message + " " + ex.StackTrace);
            }
            return null;

        }

        /// <summary> Convert the JToken tree to an object of type.  This member will except.</summary>
        /// <param name="token">JToken to convert from</param>
        /// <param name="converttype">Type to convert to</param>
        /// <param name="ignoretypeerrors">Ignore any errors in type between the JToken type and the member type.</param>
        /// <param name="membersearchflags">Member search flags, to select what types of members are serialised.
        /// Use the default plus DeclaredOnly for only members of the top class only </param>
        /// <param name="initialobject">If null, object is newed. If given, start from this object. Will except if it and the converttype is not compatible.</param>
        /// <param name="process">For enums and datetime, and if set, process and return enum or DateTime</param>
        /// <param name="setname">Define set of JSON attributes to apply, null for default</param>
        /// <param name="customformat">Class members convert to object using this function if marked with [JsonCustomFormat]</param>
        /// <returns>Object containing fields filled by JToken, or a object of ToObjectError on named error, or null if no tokens</returns>
        /// <exception cref="System.Exception"> Generic exception</exception>
        /// <exception cref="System.InvalidCastException"> If a type failure occurs.  Other excepections could also occur.
        /// </exception>
        public static Object ToObject(this JToken token, Type converttype, bool ignoretypeerrors,
            System.Reflection.BindingFlags membersearchflags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static,
            Object initialobject = null,
            Func<Type, string, object> process = null,
            string setname = null,
            Func<Type, object, object> customformat = null
            )
        {
            ToObjectConverter cnv = new ToObjectConverter(ignoretypeerrors, membersearchflags, process, customformat, setname);
            return cnv.Execute(token, converttype, initialobject, false);
        }

        /// <summary> Class to hold an conversion error </summary>
        public class ToObjectError
        {
            /// <summary> Error string </summary>
            public string ErrorString;
            /// <summary> Property name causing the error, if applicable. Null otherwise</summary>
            public string PropertyName;
            /// <summary> Constructor </summary>
            public ToObjectError(string s) { ErrorString = s; PropertyName = ""; }
        };

        /// <summary> Clear ToObject convert cache </summary>
        public static void ClearToObjectCache()
        {
            ToObjectConverter.CacheOfTypesToMembers.Clear();
        }
    }

    /// <summary>
    /// Class to perform object conversion, added due to amount now of configuration data held over the iterations
    /// </summary>
    internal class ToObjectConverter
    {
        public class AttrControl
        {
            public MemberInfo MemberInfo;       // null if not to include this json name information into the object, else the member to set.
            public bool CustomFormat;           // if custom format call the customformat should be used to convert the whole object
            public bool CustomFormatArray;      // if custom format array the array elements the customformat call should be used to convert it
        }

        static public Dictionary<Tuple<string, Type, BindingFlags>, Dictionary<string, AttrControl>> CacheOfTypesToMembers =
                    new Dictionary<Tuple<string, Type,BindingFlags>, Dictionary<string, AttrControl>>();

        private bool ignoretypeerrors;
        private BindingFlags membersearchflags;
        private Func<Type, string, object> process;
        private Func<Type, object, object> custcomconvert;
        private string setname;

        public ToObjectConverter(bool ignoretypeerrors, BindingFlags membersearchflags, Func<Type, string, object> process,
                                    Func<Type, object, object> customconvert, string setname)
        {
            this.ignoretypeerrors = ignoretypeerrors;
            this.membersearchflags = membersearchflags;
            this.process = process;
            this.custcomconvert = customconvert;
            this.setname = setname;
        }

        // execute with this token, and to this converrtpe.  Give the initialobject, which can be null to make it
        // forcecustom means for token.IsArray pass it thru to the element converters, and in the element converters, call convertcustom
        public Object Execute(JToken token, Type converttype, Object initialobject, bool forcecustom)
        {
            if (token == null)
            {
                return null;
            }
            else if (token.IsArray)
            {
                JArray jarray = (JArray)token;

                try               // we need to protect this, as createinstance can fail here if a bad type is fed in, and we really want to report it via ObjectError
                {
                    if (converttype.IsArray)        // need to handle arrays because it needs a defined length
                    {
                        dynamic instance = initialobject != null ? initialobject : Activator.CreateInstance(converttype, token.Count);   // dynamic holder for instance of array[]

                        for (int i = 0; i < token.Count; i++)
                        {
                            // get the underlying element, must match array element type

                            Object ret = Execute(token[i], converttype.GetElementType(), null, forcecustom);

                            if (ret != null && ret.GetType() == typeof(ToObjectError))      // arrays must be full, any errors means an error
                            {
                                ((ToObjectError)ret).PropertyName = converttype.Name + "." + i.ToString() + "." + ((ToObjectError)ret).PropertyName;
                                return ret;
                            }
                            else
                            {
                                dynamic d = converttype.GetElementType().ChangeTo(ret);
                                instance[i] = d;
                            }
                        }

                        return instance;
                    }
                    else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(converttype))      // else anything which is enumerable can be handled by add
                    {
                        dynamic instance = initialobject != null ? initialobject : Activator.CreateInstance(converttype);        // create the List
                        var types = converttype.GetGenericArguments();

                        for (int i = 0; i < token.Count; i++)
                        {
                            // get the underlying element, must match types[0] which is list type
                            Object ret = Execute(token[i], types[0], null, forcecustom);

                            if (ret != null && ret.GetType() == typeof(ToObjectError))  // lists must be full, any errors are errors
                            {
                                ((ToObjectError)ret).PropertyName = converttype.Name + "." + i.ToString() + "." + ((ToObjectError)ret).PropertyName;
                                return ret;
                            }
                            else
                            {
                                dynamic d = types[0].ChangeTo(ret);
                                instance.Add(d);
                            }
                        }

                        return instance;
                    }
                    else
                        return new ToObjectError($"JSONToObject: Not array {converttype.Name}");        // catch all ending
                }
                catch (Exception ex)
                {
                    return new ToObjectError($"JSONToObject: Create Error {converttype.Name} {ex.Message}");
                }

            }
            else if (token.TokenType == JToken.TType.Object)                   // objects are best efforts.. fills in as many fields as possible
            {
                if (typeof(System.Collections.IDictionary).IsAssignableFrom(converttype))       // if its a Dictionary<x,y> then expect a set of objects
                {
                    dynamic instance = initialobject != null ? initialobject : Activator.CreateInstance(converttype);        // create the class, so class must has a constructor with no paras
                    var types = converttype.GetGenericArguments();

                    foreach (var kvp in (JObject)token)
                    {
                        // get the value as the dictionary type - it must match type or it get OE

                        Object ret = Execute(kvp.Value, types[1], null, false);

                        if (ret != null && ret.GetType() == typeof(ToObjectError))
                        {
                            ((ToObjectError)ret).PropertyName = converttype.Name + "." + kvp.Key + "." + ((ToObjectError)ret).PropertyName;

                            if (ignoretypeerrors)
                            {
                                System.Diagnostics.Debug.WriteLine("Ignoring Object error:" + ((ToObjectError)ret).ErrorString + ":" + ((ToObjectError)ret).PropertyName);
                            }
                            else
                            {
                                return ret;
                            }
                        }
                        else
                        {
                            dynamic k = types[0].ChangeTo(kvp.Key);             // convert kvp.Key, to the dictionary type. May except if not compatible
                            dynamic d = types[1].ChangeTo(ret);                 // convert value to the data type.
                            instance[k] = d;
                        }
                    }

                    return instance;
                }
                else if (converttype.IsClass ||      // if class
                            (converttype.IsValueType && !converttype.IsPrimitive && !converttype.IsEnum && converttype != typeof(DateTime)))   // or struct, but not datetime (handled below)
                {
                    var instance = initialobject != null ? initialobject : Activator.CreateInstance(converttype);        // create the class, so class must has a constructor with no paras

                    // see if we have already dealt with this converttype

                    Dictionary<string, AttrControl> namestosettings = GetOrCreate(converttype, setname, membersearchflags);

                    foreach (var kvp in (JObject)token)
                    {
                        // if can find information about this field, we process it. Else its an unknown JSON field name, ignore

                        if (namestosettings.TryGetValue(kvp.Key, out AttrControl ac))       
                        {
                            MemberInfo mi = ac.MemberInfo;

                            if (mi != null)                                     // ignore any ones with null as its memberinfo as its an ignored value
                            {
                                Type otype = mi.FieldPropertyType();
                                string name = otype.Name;                       // compare by name quicker than is
                                var tk = kvp.Value;
                                bool success = false;

                                // We pick off the most common types here for speed reasons. Less common types, arrays, objects are dealt with by recursing into ToObject

                                if (name.Equals("Nullable`1"))                          // nullable types
                                {
                                    if (!tk.IsNull)         // if not null, then get underlying type.. and store in name. If null, its a null and below will set it to null
                                        name = otype.GenericTypeArguments[0].Name;    // get 
                                }

                                if (tk.IsArray)
                                {
                                    Object ret = Execute(kvp.Value, otype, null, ac.CustomFormatArray); // arrays can have custom force on, so the elements are custom converted
                                    success = ret == null || ret.GetType() != typeof(ToObjectError);    // is it a good conversion
                                    if (success)      // if good, set 
                                        success = mi.SetValue(instance, ret);
                                }
                                else if ( tk.IsObject)
                                {
                                    // complex, ToObject it, return ret, either null (good) or value or ToObjectError (bad)
                                    Object ret = Execute(kvp.Value, otype, null, false);
                                    success = ret == null || ret.GetType() != typeof(ToObjectError);    // is it a good conversion
                                    if (success)      // if good, set 
                                        success = mi.SetValue(instance, ret);
                                }
                                else if (ac.CustomFormat)               // if this is marked as custom, convert the whole object as custom
                                {
                                    Object p = custcomconvert(otype, tk.Value);

                                    if (p != null && p.GetType() != typeof(ToObjectError))  // if good, and not error, try set
                                        success = mi.SetValue(instance, p);
                                }
                                else if (name.Equals("String"))                              // copies of QuickJSON explicit operators in QuickJSON.cs
                                {
                                    if (tk.IsNull)
                                        success = mi.SetValue(instance, null);
                                    else if (tk.IsString)
                                        success = mi.SetValue(instance, (string)tk.Value);
                                }
                                else if (name.Equals("Int64"))
                                {
                                    if (tk.TokenType == TType.Long)
                                        success = mi.SetValue(instance, tk.Value);
                                    else if (tk.TokenType == TType.Double)
                                        success = mi.SetValue(instance, (long)(double)tk.Value);
                                }
                                else if (name.Equals("Double"))
                                {
                                    if (tk.TokenType == TType.Long)
                                        success = mi.SetValue(instance, (double)(long)tk.Value);
                                    else if (tk.TokenType == TType.ULong)
                                        success = mi.SetValue(instance, (double)(ulong)tk.Value);
#if JSONBIGINT
                                    else if (tk.TokenType == TType.BigInt)
                                        success = mi.SetValue(instance, (double)(System.Numerics.BigInteger)tk.Value);
#endif
                                    else if (tk.TokenType == TType.Double)
                                        success = mi.SetValue(instance, (double)tk.Value);
                                }
                                else if (name.Equals("Int32"))
                                {
                                    if (tk.TokenType == TType.Long)                  // it won't be a ulong/bigint since that would be too big for an int
                                        success = mi.SetValue(instance, (int)(long)tk.Value);
                                    else if (tk.TokenType == TType.Double)           // doubles get trunced.. as per previous system
                                        success = mi.SetValue(instance, (int)(double)tk.Value);
                                }
                                else if (name.Equals("Boolean"))
                                {
                                    if (tk.TokenType == TType.Boolean)
                                        success = mi.SetValue(instance, (bool)tk.Value);
                                    else if (tk.TokenType == TType.Long)
                                        success = mi.SetValue(instance, (long)tk.Value != 0);
                                }
                                else if (name.Equals("Single"))
                                {
                                    if (tk.TokenType == TType.Long)
                                        success = mi.SetValue(instance, (float)(long)tk.Value);
                                    else if (tk.TokenType == TType.ULong)
                                        success = mi.SetValue(instance, (float)(ulong)tk.Value);
#if JSONBIGINT
                                    else if (tk.TokenType == TType.BigInt)
                                        success = mi.SetValue(instance, (float)(System.Numerics.BigInteger)tk.Value);
#endif
                                    else if (tk.TokenType == TType.Double)
                                        success = mi.SetValue(instance, (float)(double)tk.Value);
                                }
                                else if (otype.IsEnum)
                                {
                                    if (tk.IsString)     // if string, possible
                                    {
                                        Object p = null;

                                        if (process != null)    // process can return value
                                        {
                                            p = process(otype, (string)tk.Value);

                                            if (p != null && p.GetType() != typeof(ToObjectError))  // if good, and not error, try set
                                                success = mi.SetValue(instance, p);
                                        }
                                        else
                                        {
                                            try
                                            {
                                                p = Enum.Parse(otype, (string)tk.Value, true); // get object, will except if bad, in which case we just swallow
                                                success = mi.SetValue(instance, p);
                                            }
                                            catch
                                            {   // error, leave success off
                                                if (JToken.TraceOutput)
                                                    System.Diagnostics.Trace.WriteLine("QuickJSON ToObject Ignoring cannot convert string to enum on property " + mi.Name);
                                            }
                                        }
                                    }
                                }
                                else if (name.Equals("UInt32"))
                                {
                                    if (tk.TokenType == TType.Long && (long)tk.Value >= 0)
                                        success = mi.SetValue(instance, (uint)(long)tk.Value);
                                    else if (tk.TokenType == TType.Double && (double)tk.Value >= 0)
                                        success = mi.SetValue(instance, (uint)(double)tk.Value);
                                }
                                else if (name.Equals("UInt64"))
                                {
                                    if (tk.TokenType == TType.ULong)
                                        success = mi.SetValue(instance, (ulong)tk.Value);
                                    else if (tk.TokenType == TType.Long && (long)tk.Value >= 0)
                                        success = mi.SetValue(instance, (ulong)(long)tk.Value);
                                    else if (tk.TokenType == TType.Double && (double)tk.Value >= 0)
                                        success = mi.SetValue(instance, (ulong)(double)tk.Value);
                                }
                                else if (name.Equals("Nullable`1"))                     // name stays nullable`1 if value is null
                                {
                                    success = mi.SetValue(instance, null);
                                }
                                else
                                {
                                    // other complex, ToObject it, prob an enum, return ret, either null (good) or value or ToObjectError (bad)
                                    Object ret = Execute(kvp.Value, otype, null, false);
                                    success = ret == null || ret.GetType() != typeof(ToObjectError);    // is it a good conversion
                                    if (success)      // if good, set 
                                        success = mi.SetValue(instance, ret);
                                }


                                if (!success)
                                {
                                    if (ignoretypeerrors)
                                    {
                                        if (JToken.TraceOutput)
                                            System.Diagnostics.Trace.WriteLine("QuickJSON ToObject Ignoring cannot set value on property " + mi.Name);

                                        success = true;
                                    }
                                    else
                                    {
                                        return new ToObjectError("Cannot set value on property " + mi.Name);
                                    }
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"ToObject ignore {converttype.Name}:{kvp.Key}");
                            }
                        }
                        else
                        {
                            // System.Diagnostics.Debug.WriteLine("JSONToObject: No such member " + kvp.Key + " in " + tt.Name);
                        }
                    }

                    return instance;
                }
                else
                    return new ToObjectError($"JSONToObject: Not class {converttype.Name}");
            }
            else if (converttype.IsEnum)
            {
                if (!token.IsString)
                {
                    if (TraceOutput)
                        System.Diagnostics.Trace.WriteLine($"QuickJSON ToObject Enum Token is not string for {converttype.Name}");

                    return new ToObjectError($"JSONToObject: Enum Token is not string for {converttype.Name}");
                }

                try
                {
                    if (process != null)        // if enum processor, do it
                    {
                        return process(converttype, (string)token.Value);
                    }
                    else if (forcecustom)       // if force custom on it, do it
                    {
                        return custcomconvert(converttype, token.Value);
                    }
                    else
                    {
                        //System.Diagnostics.Trace.WriteLine($"JSON ToObject enum convert {converttype.Name} from {Environment.StackTrace}");
                        return Enum.Parse(converttype, (string)token.Value, true);
                    }
                }
                catch
                {
                    if (TraceOutput)
                        System.Diagnostics.Trace.WriteLine($"QuickJSON ToObject Unrecognised value '{token.Str()}' for enum {converttype.Name}");

                    return new ToObjectError($"JSONToObject: Unrecognised value '{token.Str()}' for enum {converttype.Name}");
                }
            }
            else
            {
                var tk = token;
                string name = converttype.Name;                         // compare by name quicker than is

                if (name.Equals("Nullable`1"))                          // nullable types
                {
                    if (tk.IsNull)
                        return null;

                    name = converttype.GenericTypeArguments[0].Name;    // get underlying type.. and store in name
                }

                // now check type

                if ( forcecustom )                                      // if custom convert is forced, do it
                {
                    return custcomconvert(converttype, token.Value);
                }
                else if (name.Equals("String"))                              // copies of QuickJSON explicit operators in QuickJSON.cs
                {
                    if (tk.IsNull)
                        return null;
                    else if (tk.IsString)
                        return tk.Value;
                }
                else if (name.Equals("Int32"))
                {
                    if (tk.TokenType == TType.Long)                  // it won't be a ulong/bigint since that would be too big for an int
                        return (int)(long)tk.Value;
                    else if (tk.TokenType == TType.Double)           // doubles get trunced.. as per previous system
                        return (int)(double)tk.Value;
                }
                else if (name.Equals("Int64"))
                {
                    if (tk.TokenType == TType.Long)
                        return tk.Value;
                    else if (tk.TokenType == TType.Double)
                        return (long)(double)tk.Value;
                }
                else if (name.Equals("Boolean"))
                {
                    if (tk.TokenType == TType.Boolean)
                        return (bool)tk.Value;
                    else if (tk.TokenType == TType.Long)
                        return (long)tk.Value != 0;
                }
                else if (name.Equals("Double"))
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
                }
                else if (name.Equals("Single"))
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
                }
                else if (name.Equals("UInt32"))
                {
                    if (tk.TokenType == TType.Long && (long)tk.Value >= 0)
                        return (uint)(long)tk.Value;
                    else if (tk.TokenType == TType.Double && (double)tk.Value >= 0)
                        return (uint)(double)tk.Value;
                }
                else if (name.Equals("UInt64"))
                {
                    if (tk.TokenType == TType.ULong)
                        return (ulong)tk.Value;
                    else if (tk.TokenType == TType.Long && (long)tk.Value >= 0)
                        return (ulong)(long)tk.Value;
                    else if (tk.TokenType == TType.Double && (double)tk.Value >= 0)
                        return (ulong)(double)tk.Value;
                }
                else if (name.Equals("DateTime"))
                {
                    if (tk.IsString)       // must be a string
                    {
                        if (process != null)
                            return process(converttype, (string)tk.Value);
                        else
                        {
                            if (DateTime.TryParse((string)tk.Value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal, out DateTime ret))
                            {
                                return ret;
                            }
                        }
                    }
                }
                else if (name.Equals("Object"))                     // catch all, june 7/6/23 fix
                {
                    return tk.Value;
                }

                return new ToObjectError("JSONToObject: Bad Conversion " + tk.TokenType + " to " + converttype.Name);
            }

        }

        static public Dictionary<string, AttrControl> GetOrCreate(Type converttype, string setname, BindingFlags membersearchflags)
        {
            lock (CacheOfTypesToMembers)        // thread lock
            {
                // try and find previously computed info

                var key = new Tuple<string, Type, BindingFlags>(setname, converttype, membersearchflags);

                if (!CacheOfTypesToMembers.TryGetValue(key, out Dictionary<string, AttrControl> namestosettings))
                {
                    // System.Diagnostics.Debug.WriteLine($"Created cached control {key}");

                    namestosettings = new Dictionary<string, AttrControl>();     // name -> MI, or name->Null if ignored

                    CacheOfTypesToMembers[key] = namestosettings;

                    var list = new List<System.Reflection.MemberInfo>();
                    list.AddRange(converttype.GetFields(membersearchflags));        // get field list..
                    list.AddRange(converttype.GetProperties(membersearchflags));        // get property list

                    foreach (var mi in list)
                    {
                        bool includeit = true;

                        // compute if to be ignored
                        var calist = mi.GetCustomAttributes(typeof(JsonIgnoreAttribute), false);
                        if (calist.Length == 1)
                        {
                            JsonIgnoreAttribute jia = calist[0] as JsonIgnoreAttribute;
                            if (jia.Setting == null)       // null, means all sets
                            {
                                includeit = false;
                            }
                            else
                            {
                                for (int i = 0; i < jia.Setting.Length; i++)        // try and find the set, if found
                                {
                                    if (jia.Setting[i].Set.EqualsI(setname))        // found set, as long as its not an ignore/include list..
                                    {
                                        if (jia.Setting[i].Ignore == null && jia.Setting[i].IncludeOnly == null)
                                            includeit = false;
                                        break;
                                    }
                                }
                            }
                        }

                        var ac = new AttrControl();     // member info = null, therefore default is ignore.

                        if (includeit)      // only worth doing this if we are including it
                        {
                            ac.MemberInfo = mi;

                            // Custom format means the element is totally processed externally

                            var customoutput = mi.GetCustomAttributes(typeof(JsonCustomFormat), false);
                            if (customoutput.Length == 1)
                            {
                                JsonCustomFormat jcf = customoutput[0] as JsonCustomFormat;
                                if (jcf.Sets == null || jcf.Sets.Contains(setname, StringComparer.InvariantCulture))    // if matches
                                {
                                    ac.CustomFormat = true;
                                }
                            }

                            // Custom format Array means the element array elements are totally processed externally

                            var customoutputarray = mi.GetCustomAttributes(typeof(JsonCustomFormatArray), false);
                            if (customoutputarray.Length == 1)
                            {
                                JsonCustomFormatArray jcf = customoutputarray[0] as JsonCustomFormatArray;
                                if (jcf.Sets == null || jcf.Sets.Contains(setname, StringComparer.InvariantCulture))    // if matches
                                {
                                    ac.CustomFormatArray = true;
                                }
                            }
                        }

                        var renamelist = mi.GetCustomAttributes(typeof(JsonNameAttribute), false);
                        if (renamelist.Length == 1)
                        {
                            JsonNameAttribute na = renamelist[0] as JsonNameAttribute;
                            if (na.Sets == null)        // null, means all sets
                            {
                                // add all names and all point to ac
                                foreach (var x in na.Names)
                                    namestosettings[x] = ac;
                            }
                            else
                            {
                                for (int i = 0; i < na.Sets.Length; i++)  // find set, if not found, no change, else add name to one accepted for this member
                                {
                                    if (na.Sets[i].EqualsI(setname))
                                    {
                                        // add all names to point to access control 
                                        namestosettings[na.Names[i]] = ac;
                                        //System.Diagnostics.Debug.WriteLine($"ToObjectType NameList {na.Names[i]} -> {mi.Name} {includeit}");
                                    }
                                }
                            }
                        }
                        else
                        {
                            namestosettings[mi.Name] = ac;
                        }
                    }
                }
                else
                {
                    //   System.Diagnostics.Debug.WriteLine($"Lookup cached control {key}");
                }

                return namestosettings;
            }
        }
    }
}



