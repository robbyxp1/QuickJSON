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
using System.Net.Mime;

namespace QuickJSON
{
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
        /// <param name="setname">Define set of JSON attributes to apply, null for default</param>
        /// <returns>Null if can't convert (error detected) or JToken tree</returns>
        public static JToken FromObject(Object obj, bool ignoreunserialisable, Type[] ignored = null, int maxrecursiondepth = 256, 
            System.Reflection.BindingFlags membersearchflags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static,
            bool ignoreobjectpropertyifnull = false, string setname = null)
        {
            Stack<Object> objectlist = new Stack<object>();
            var r = FromObjectInt(obj, ignoreunserialisable, ignored, objectlist,0,maxrecursiondepth, membersearchflags,null,null,ignoreobjectpropertyifnull, setname);
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
        /// <param name="setname">Define set of JSON attributes to apply, null for default</param>
        /// <returns>JToken error type if can't convert (check with IsInError, value has error reason) or JToken tree</returns>
        public static JToken FromObjectWithError(Object obj, bool ignoreunserialisable, Type[] ignored = null, int maxrecursiondepth = 256, 
            System.Reflection.BindingFlags membersearchflags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static,
            bool ignoreobjectpropertyifnull = false, string setname = null)
        {
            Stack<Object> objectlist = new Stack<object>();
            var r = FromObjectInt(obj, ignoreunserialisable, ignored, objectlist,0,maxrecursiondepth, membersearchflags,null,null,ignoreobjectpropertyifnull,setname);
            System.Diagnostics.Debug.Assert(objectlist.Count == 0);
            return r;
        }

        class AttrControl
        {
            public string Name;             // name to call it
            public HashSet<string> IncludeSet;
            public HashSet<string> IgnoreSet;
            public bool IgnoreAll;
            public bool IgnoreIfNull;
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
        /// <param name="setname">Define set of JSON attributes to apply</param>
        /// <returns></returns>
        private static JToken FromObjectInt(Object o, bool ignoreunserialisable, 
                        Type[] ignoredtypes, Stack<Object> objectlist, 
                        int lvl, int maxrecursiondepth, 
                        System.Reflection.BindingFlags membersearchflags,
                        HashSet<string> memberignore, HashSet<string> memberinclude,
                        bool ignoreobjectpropertyifnull,
                        string setname
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

                    JToken inner = FromObjectInt(kvp.Value, ignoreunserialisable, ignoredtypes, objectlist, lvl + 1, maxrecursiondepth, membersearchflags, null,null, ignoreobjectpropertyifnull,setname);
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

                    JToken inner = FromObjectInt(oa, ignoreunserialisable, ignoredtypes, objectlist, lvl + 1, maxrecursiondepth, membersearchflags, null, null, ignoreobjectpropertyifnull, setname);

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

                Dictionary<string, AttrControl> namestosettings = null;

                lock (CacheOfTypesFromMembers)        // thread lock
                {
                    // try and find previously computed info

                    var key = new Tuple<string, Type>(setname, tt);

                    if (!CacheOfTypesFromMembers.TryGetValue(key, out namestosettings))
                    {
                       // System.Diagnostics.Debug.WriteLine($"Created cached control {key}");

                        namestosettings = new Dictionary<string, AttrControl>();
                        CacheOfTypesFromMembers[key] = namestosettings;

                        // we go thru all members and pick up the control settings for each one
                        foreach (var mi in allmembers)
                        {
                            // only props/fields

                            if (mi.MemberType == System.Reflection.MemberTypes.Property || mi.MemberType == System.Reflection.MemberTypes.Field)
                            {
                                string attrname = mi.Name;
                                var ac = new AttrControl() { Name = attrname }; 
                                namestosettings[attrname] = ac;

                                // check on json ignore
                                var calist = mi.GetCustomAttributes(typeof(JsonIgnoreAttribute), false);
                                if ( calist.Length == 1)
                                { 
                                    JsonIgnoreAttribute jia = calist[0] as JsonIgnoreAttribute;

                                    if (jia.Setting == null)       // null, means just ignore all time
                                    {
                                        ac.IgnoreAll = true;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < jia.Setting.Length; i++)
                                        {
                                            if (jia.Setting[i].Set.EqualsI(setname))  // else we match on set name
                                            {
                                                if (jia.Setting[i].Ignore != null)
                                                    ac.IgnoreSet = jia.Setting[i].Ignore.ToHashSet();
                                                else if (jia.Setting[i].IncludeOnly != null)
                                                    ac.IncludeSet = jia.Setting[i].IncludeOnly.ToHashSet();
                                                else
                                                    ac.IgnoreAll = true;
                                                break;
                                            }
                                        }
                                    }
                                }

                                // check on name
                                var renamelist = mi.GetCustomAttributes(typeof(JsonNameAttribute), false);
                                if ( renamelist.Length == 1)
                                { 
                                    JsonNameAttribute na = renamelist[0] as JsonNameAttribute;          // get name attribute
                                    if (na.Sets == null)                                                // no sets, use entry 0 as the name
                                    {
                                        ac.Name = na.Names[0];
                                    }
                                    else
                                    {
                                        for (int i = 0; i < na.Sets.Length; i++)        // find set, if not found, no change, else first one gives the name
                                        {
                                            if (na.Sets[i].EqualsI(setname))
                                            {
                                                ac.Name = na.Names[i];
                                                break;
                                            }
                                        }
                                    }
                                }

                                // check on null attribute output flag
                                var ignorenulllist = mi.GetCustomAttributes(typeof(JsonIgnoreIfNullAttribute), false);
                                if (ignorenulllist.Length == 1)
                                {
                                    JsonIgnoreIfNullAttribute iin = ignorenulllist[0] as JsonIgnoreIfNullAttribute;
                                    if (iin.Sets == null)
                                    {
                                        ac.IgnoreIfNull = true;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < iin.Sets.Length; i++)  // find set, if not found, not applicable. Else its ignore if null
                                        {
                                            if (iin.Sets[i].EqualsI(setname))
                                            {
                                                ac.IgnoreIfNull = true;
                                                break;
                                            }
                                        }
                                    }
                                }

                                System.Diagnostics.Debug.WriteLine($"FromObjectType `{setname}`:{tt.Name} attr {attrname} -> {ac.Name} : ignoreall {ac.IgnoreAll} ignoreifnull {ac.IgnoreIfNull} incl {ac.IncludeSet != null} ignore {ac.IgnoreSet != null}");
                            }
                        }
                    }
                    else
                    {
                       // System.Diagnostics.Debug.WriteLine($"Lookup cached control {key}");
                    }

                }

                foreach (var mi in allmembers)
                {
                    // pick up type 

                    Type innertype;

                    if (mi.MemberType == System.Reflection.MemberTypes.Property)        // only properties  and fields are allowed thru
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

                    // check name is not barred

                    string attrname = mi.Name;

                    // check if these are set to see if the attr name has been removed by either include list or ignore list
                    // passed into this level to knock out members in here

                    if ( (memberinclude != null && !memberinclude.Contains(attrname)) ||
                            ( memberignore != null && memberignore.Contains(attrname)))
                    {
                        continue;
                    }


                    // is it ignored?

                    if (ignoredtypes != null && Array.IndexOf(ignoredtypes, innertype) >= 0)
                        continue;

                    // pick up control for this attribute
                    AttrControl actrl = namestosettings[attrname];

                    System.Diagnostics.Debug.WriteLine($"Member {mi.Name} {mi.MemberType} name {actrl.Name} ignoreall {actrl.IgnoreAll}");

                    if (actrl.IgnoreAll)        // ignore all,stop
                        continue;

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

                        var token = FromObjectInt(innervalue, ignoreunserialisable, ignoredtypes, objectlist, lvl + 1, maxrecursiondepth, membersearchflags, actrl.IgnoreSet, actrl.IncludeSet, ignoreobjectpropertyifnull, setname );

                        if (token.IsInError)
                        {
                            if (!ignoreunserialisable)
                            {
                                objectlist.Pop();
                                return token;
                            }
                        }
                        else
                            outobj[actrl.Name] = token;
                    }
                    else
                    {
                        // see if null exclude is active
                        bool ignoreifnull = ignoreobjectpropertyifnull || actrl.IgnoreIfNull;

                        if ( ignoreifnull == false)         
                            outobj[actrl.Name] = JToken.Null();        // its null so its a JNull
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

        static Dictionary<Tuple<string, Type>, Dictionary<string, AttrControl>> CacheOfTypesFromMembers = new Dictionary<Tuple<string, Type>, Dictionary<string, AttrControl>>();

        /// <summary> Clear ToObject convert cache </summary>
        public static void ClearFromObjectCache()
        {
            CacheOfTypesFromMembers = new Dictionary<Tuple<string, Type>, Dictionary<string, AttrControl>>();
        }
    }
}



