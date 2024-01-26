/*
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
using System.Collections.Generic;
using System.Linq;

namespace QuickJSON
{
    /// <summary>
    /// Ignore attribute. Attach to an member of a class to say don't serialise this item or 
    /// to exclude certain members or to only include specific members
    /// Applicable to FromObject and ToObject
    /// </summary>
    public sealed class JsonIgnoreAttribute : Attribute 
    {
        /// <summary> FromObject: If non null, list of object members to ignore completely</summary>
        public string[] Ignore { get; set; }
        /// <summary> FromObject: If non null, list of object members to include only</summary>
        public string[] IncludeOnly { get; set; }

        /// <summary> FromObject and ToObject: Constructor to indicate that this member should be ignored completely. </summary>
        public JsonIgnoreAttribute() { }

        /// <summary> Options for FromObject only. </summary>
        public enum Operation {
            /// <summary> Use ignore to say the list is a set of members to ignore </summary>
            Ignore,
            /// <summary> Use include to say the list is a set of members only to include </summary>
            Include
        };

        /// <summary> FromObject: Constructor to indicate which members to enumerate to JSON.  
        /// ToObject: the setting is ignored and the member processed as normal
        /// If Ignore, name list are the members which should be excluded. All others will be included
        /// If Include, name list are only the members which will be included, all others will be excluded.
        /// </summary>
        public JsonIgnoreAttribute(Operation ignoreorinclude, params string[] names) { if (ignoreorinclude==Operation.Include) IncludeOnly = names; else Ignore = names; }
    }

    /// <summary>
    /// Name attribute. Attach to an member of a class to indicate an alternate name to use in the JSON structure from its c# name.
    /// Applicable to FromObject and ToObject.  ToObject supports multiple names (any name in JSON will match this entry), FromObject only one.
    /// </summary>
    public sealed class JsonNameAttribute : Attribute
    {
        /// <summary> List of names for this attribute </summary>
        public string[] Names { get; set; }
        /// <summary> Constructor with name list </summary>
        public JsonNameAttribute(params string[] names) { Names = names; }

    }
    /// <summary>
    /// Attach to a member to indicate if the value of it is null, don't add it to JSON.
    /// FromObject only
    /// </summary>
    public sealed class JsonIgnoreIfNullAttribute : Attribute
    {
        /// <summary> Constructor </summary>
        public JsonIgnoreIfNullAttribute() {}
    }

    public partial class JToken
    {
        /// <summary> Convert Object to JToken tree 
        /// Beware of using this except for the simpliest classes, use one below and control the ignored/max recursion
        /// </summary>
        /// <param name="obj">Object to convert from</param>
        /// <returns>JToken tree</returns>
        public static JToken FromObject(Object obj)      
        {
            return FromObject(obj, false);
        }

        /// <summary> Convert Object to JToken tree </summary>
        /// <param name="obj">Object to convert from</param>
        /// <param name="ignoreunserialisable">If true, do not stop if an unserialisable member is found. These are self referencing members which would cause an infinite loop</param>
        /// <param name="ignored">List of ignored types not to serialise, may be null</param>
        /// <param name="maxrecursiondepth">Maximum depth to recurse through the objects heirarchy</param>
        /// <param name="membersearchflags">Member search flags, to select what types of members are serialised</param>
        /// <param name="ignoreobjectpropertyifnull">acts as per JSONIgnoreIfNull and does not output JSON object property null</param>
        /// <returns>Null if can't convert (error detected) or JToken tree</returns>
        public static JToken FromObject(Object obj, bool ignoreunserialisable, Type[] ignored = null, int maxrecursiondepth = 256, 
            System.Reflection.BindingFlags membersearchflags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static,
            bool ignoreobjectpropertyifnull = false)
        {
            Stack<Object> objectlist = new Stack<object>();
            var r = FromObjectInt(obj, ignoreunserialisable, ignored, objectlist,0,maxrecursiondepth, membersearchflags,null,null,ignoreobjectpropertyifnull);
            System.Diagnostics.Debug.Assert(objectlist.Count == 0);
            return r.IsInError ? null : r;
        }

        /// <summary> Convert Object to JToken tree </summary>
        /// <param name="obj">Object to convert from</param>
        /// <param name="ignoreunserialisable">If true, do not stop if an unserialisable member is found. These are self referencing members which would cause an infinite loop</param>
        /// <param name="ignored">List of ignored types not to serialise, may be null</param>
        /// <param name="maxrecursiondepth">Maximum depth to recurse through the objects heirarchy</param>
        /// <param name="membersearchflags">Member search flags, to select what types of members are serialised</param>
        /// <param name="ignoreobjectpropertyifnull">acts as per JSONIgnoreIfNull and does not output JSON object property null</param>
        /// <returns>JToken error type if can't convert (check with IsInError, value has error reason) or JToken tree</returns>
        public static JToken FromObjectWithError(Object obj, bool ignoreunserialisable, Type[] ignored = null, int maxrecursiondepth = 256, 
            System.Reflection.BindingFlags membersearchflags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static,
            bool ignoreobjectpropertyifnull = false)
        {
            Stack<Object> objectlist = new Stack<object>();
            var r = FromObjectInt(obj, ignoreunserialisable, ignored, objectlist,0,maxrecursiondepth, membersearchflags,null,null,ignoreobjectpropertyifnull);
            System.Diagnostics.Debug.Assert(objectlist.Count == 0);
            return r;
        }

        /// <summary>
        /// Internal Object converstion
        /// </summary>
        /// <param name="o">object</param>
        /// <param name="ignoreunserialisable"></param>
        /// <param name="ignoredtypes"></param>
        /// <param name="objectlist">Stack of objects already processed</param>
        /// <param name="lvl">recursion level</param>
        /// <param name="maxrecursiondepth">max recurson level</param>
        /// <param name="membersearchflags">which members of a class to include</param>
        /// <param name="memberignore">which members of a class to ignore, null for none</param>
        /// <param name="memberinclude">which members of a class to include only, null for all, else must be in list</param>
        /// <param name="ignoreobjectpropertyifnull">acts as per JSONIgnoreIfNull and does not output JSON object property null</param>
        /// <returns></returns>
        private static JToken FromObjectInt(Object o, bool ignoreunserialisable, 
                        Type[] ignoredtypes, Stack<Object> objectlist, 
                        int lvl, int maxrecursiondepth, 
                        System.Reflection.BindingFlags membersearchflags,
                        HashSet<string> memberignore, HashSet<string> memberinclude,
                        bool ignoreobjectpropertyifnull
                        )
        {
            //System.Diagnostics.Debug.WriteLine(lvl + "From Object on " + o.GetType().Name);

            if (lvl >= maxrecursiondepth)
                return new JToken();        // returns NULL

            Type tt = o.GetType();

            if (typeof(System.Collections.IDictionary).IsAssignableFrom(tt))       // if its a Dictionary<x,y> then expect a set of objects
            {
                System.Collections.IDictionary idict = o as System.Collections.IDictionary;

                JObject outobj = new JObject();

                objectlist.Push(o);

                foreach (dynamic kvp in idict)      // need dynamic since don't know the types of Value or Key
                {
                    if (objectlist.Contains(kvp.Value))   // self reference
                    {
                        if (ignoreunserialisable)       // if ignoring this, just continue with the next one
                            continue;

                        objectlist.Pop();
                        return new JToken(TType.Error, "Self Reference in IDictionary");
                    }

                    JToken inner = FromObjectInt(kvp.Value, ignoreunserialisable, ignoredtypes, objectlist, lvl + 1, maxrecursiondepth, membersearchflags, null,null, ignoreobjectpropertyifnull);
                    if (inner.IsInError)      // used as an error type
                    {
                        objectlist.Pop();
                        return inner;
                    }

                    if (kvp.Key is DateTime)               // handle date time specially, use zulu format
                    {
                        DateTime t = (DateTime)kvp.Key;
                        outobj[t.ToStringZulu()] = inner;
                    }
                    else
                        outobj[kvp.Key.ToString()] = inner;
                }

                objectlist.Pop();
                return outobj;
            }
            else if (o is string)           // strings look like classes, so need to intercept first
            {
                var r = JToken.CreateToken(o, false);        // return token or null indicating unserializable
                return r ?? new JToken(TType.Error, "Unserializable " + tt.Name);
            }
            else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(tt))       // enumerables, arrays, lists, hashsets
            {
                var ilist = o as System.Collections.IEnumerable;

                JArray outarray = new JArray();

                objectlist.Push(o);

                foreach (var oa in ilist)
                {
                    if (objectlist.Contains(oa))        // self reference
                    {
                        objectlist.Pop();
                        return new JToken(TType.Error, "Self Reference in IEnumerable");
                    }

                    JToken inner = FromObjectInt(oa, ignoreunserialisable, ignoredtypes, objectlist, lvl + 1, maxrecursiondepth, membersearchflags, null, null, ignoreobjectpropertyifnull);

                    if (inner.IsInError)      // used as an error type
                    {
                        objectlist.Pop();
                        return inner;
                    }

                    outarray.Add(inner);
                }

                objectlist.Pop();
                return outarray;
            }
            else if ( tt.IsClass ||                                     // if class
                      (tt.IsValueType && !tt.IsPrimitive && !tt.IsEnum && tt != typeof(DateTime))     // if value type, not primitive, not enum, its a structure. Not datetime (handled in CreateToken)
                    )
            {
                JObject outobj = new JObject();

                var allmembers = tt.GetMembers(membersearchflags);

                objectlist.Push(o);

                foreach (var mi in allmembers)
                {
                    string attrname = mi.Name;

                    // check if these are set to see if the attr name has been removed by either include list or ignore list

                    if ( (memberinclude != null && !memberinclude.Contains(attrname)) ||
                            ( memberignore != null && memberignore.Contains(attrname)))
                    {
                        continue;
                    }

                    // pick up type 

                    Type innertype = null;

                    if (mi.MemberType == System.Reflection.MemberTypes.Property)
                    {
                        innertype = ((System.Reflection.PropertyInfo)mi).PropertyType;
                    }
                    else if (mi.MemberType == System.Reflection.MemberTypes.Field)
                    {
                        var fi = (System.Reflection.FieldInfo)mi;
                        innertype = fi.FieldType;
                    }
                    else
                        continue;       // not a prop/field

                    // is it ignored?

                    if (ignoredtypes != null && Array.IndexOf(ignoredtypes, innertype) >= 0)
                        continue;

                    // pick up JSONIgnoreAttribute and see if its members are empty (ignore all) or have an include/ignore list

                    HashSet<string> ignorelist = null;
                    HashSet<string> includeonly = null;

                    var ca = mi.GetCustomAttributes(typeof(JsonIgnoreAttribute), false);
                    if (ca.Length > 0)                                             
                    {
                        JsonIgnoreAttribute jia = ca[0] as JsonIgnoreAttribute;
                        if (jia.Ignore != null)
                            ignorelist = jia.Ignore.ToHashSet();
                        else if (jia.IncludeOnly != null)
                            includeonly = jia.IncludeOnly.ToHashSet();
                        else
                            continue;   // ignore all with no lists
                    }

                    // see if rename is active

                    var rename = mi.GetCustomAttributes(typeof(JsonNameAttribute), false);
                    if (rename.Length == 1)                                         
                    {
                        dynamic attr = rename[0];                                   // dynamic since compiler does not know rename type
                        attrname = attr.Names[0];                                   // only first entry is used for FromObject
                    }

                    //System.Diagnostics.Debug.WriteLine("Member " + mi.Name + " " + mi.MemberType + " attrname " + attrname);

                    // get the value

                    Object innervalue = null;
                    if (mi.MemberType == System.Reflection.MemberTypes.Property)
                    {
                        var pi = (System.Reflection.PropertyInfo)mi;
                        if (pi.GetIndexParameters().Length == 0)       // reject any indexer properties! new Dec 22
                            innervalue = pi.GetValue(o);
                    }
                    else
                        innervalue = ((System.Reflection.FieldInfo)mi).GetValue(o);

                    if (innervalue != null)
                    {
                        if (objectlist.Contains(innervalue))        // self reference
                        {
                            if (ignoreunserialisable)               // if ignoring this, just continue with the next one
                                continue;

                            objectlist.Pop();
                            return new JToken(TType.Error, "Self Reference by " + tt.Name + ":" + mi.Name);
                        }

                        var token = FromObjectInt(innervalue, ignoreunserialisable, ignoredtypes, objectlist, lvl + 1, maxrecursiondepth, membersearchflags, ignorelist, includeonly, ignoreobjectpropertyifnull);     // may return End Object if not serializable

                        if (token.IsInError)
                        {
                            if (!ignoreunserialisable)
                            {
                                objectlist.Pop();
                                return token;
                            }
                        }
                        else
                            outobj[attrname] = token;
                    }
                    else
                    {
                        // see if null exclude is active
                        bool ignoreifnull = ignoreobjectpropertyifnull || mi.GetCustomAttributes(typeof(JsonIgnoreIfNullAttribute), false).Length == 1;

                        if ( ignoreifnull == false)         
                            outobj[attrname] = JToken.Null();        // its null so its a JNull
                    }
                }

                objectlist.Pop();
                return outobj;
            }
            else
            {
                var r = JToken.CreateToken(o, false);        // return token or null indicating unserializable
                                                             //                return r ?? new JToken(TType.Error, "Unserializable " + tt.Name);
                return r ?? new JToken(TType.Error, "Unserializable " + tt.Name);
            }
        }


    }
}



