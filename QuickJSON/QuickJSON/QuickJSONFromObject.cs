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

using QuickJSON.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        /// <param name="customconvert">Use this custom converter on class members when they are marked with [JsonCustomFormat]n</param>
        /// <returns>Null if can't convert (error detected) or JToken tree</returns>
        public static JToken FromObject(Object obj, bool ignoreunserialisable, Type[] ignored = null, int maxrecursiondepth = 256, 
            System.Reflection.BindingFlags membersearchflags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static,
            bool ignoreobjectpropertyifnull = false, string setname = null, Func<Object, JToken> customconvert = null)
        {
            FromObjectConverter cnv = new FromObjectConverter(ignoreunserialisable, ignored, maxrecursiondepth, membersearchflags, ignoreobjectpropertyifnull, setname, customconvert);
            var r = cnv.Execute(obj, 0, null, null);
            System.Diagnostics.Debug.Assert(cnv.ObjectCount == 0);
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
        /// <param name="customconvert">Use this custom converter on class members when they are marked with [JsonCustomFormat]n</param>
        /// <returns>JToken error type if can't convert (check with IsInError, value has error reason) or JToken tree</returns>
        public static JToken FromObjectWithError(Object obj, bool ignoreunserialisable, Type[] ignored = null, int maxrecursiondepth = 256, 
            System.Reflection.BindingFlags membersearchflags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static,
            bool ignoreobjectpropertyifnull = false, string setname = null, Func<Object, JToken> customconvert = null)
        {
            FromObjectConverter cnv = new FromObjectConverter(ignoreunserialisable, ignored, maxrecursiondepth, membersearchflags, ignoreobjectpropertyifnull, setname, customconvert);
            var r = cnv.Execute(obj, 0, null, null);
            System.Diagnostics.Debug.Assert(cnv.ObjectCount == 0);
            return r;
        }
        /// <summary>
        /// Holds the attribute settings found - used in FromObject
        /// </summary>
        [System.Diagnostics.DebuggerDisplay("{MemberInfo.Name}->{Name} cf:{CustomFormat} im:{IgnoreMemberIfNull}")]
        public class MemberAttributeSettings
        {
            /// <summary>
            /// Member info
            /// </summary>
            public MemberInfo MemberInfo;
            /// <summary>
            /// name to call the output
            /// </summary>
            public string Name { get; set; } 
            /// <summary>
            /// if custom format is active
            /// </summary>
            public bool CustomFormat { get; set; }
            /// <summary>
            /// ignore member if null
            /// </summary>
            public bool IgnoreMemberIfNull { get; set; }     
            /// <summary>
            /// Only include these members
            /// </summary>
            public HashSet<string> IncludeSet { get; set; }  
            /// <summary>
            /// if set, sets the memberignore variable for the object below
            /// </summary>
            public HashSet<string> IgnoreSet { get; set; }   
        }

        /// <summary> Clear FromObject convert cache </summary>
        public static void ClearFromObjectCache()
        {
            FromObjectConverter.CacheOfTypesFromMembers.Clear();
        }

        /// <summary>
        /// Get the list of included attributes to output using FromObject for a particular class, given setname and member search flags
        /// Completely ignored objects are not included
        /// </summary>
        /// <param name="tt">class type</param>
        /// <param name="setname">name of set, or null</param>
        /// <param name="membersearchflags">search flags to apply</param>
        /// <returns>Dictionary keyed by member name of all non ignored attributes</returns>
        static public Dictionary<string, JToken.MemberAttributeSettings> GetMemberAttributeSettings(Type tt, string setname = null, System.Reflection.BindingFlags membersearchflags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static)
        {
            var ret = FromObjectConverter.GetOrCreate(tt, setname, membersearchflags);
            return ret;
        }
    }

    /// Class to perform object conversion, added due to amount now of configuration data held over the iterations
    internal class FromObjectConverter
    {
        public static Dictionary<Tuple<string, Type, BindingFlags>, Dictionary<string, JToken.MemberAttributeSettings>> CacheOfTypesFromMembers 
                    = new Dictionary<Tuple<string, Type, BindingFlags>, Dictionary<string, JToken.MemberAttributeSettings>>();

        private bool ignoreunserialisable;
        private Type[] ignoredtypes;
        private int maxrecursiondepth;
        private BindingFlags membersearchflags;
        private bool ignoreobjectpropertyifnull;
        private string setname;
        Func<Object, JToken> customconvert;

        private Stack<Object> objectlist;   // holds the objects converted to check for recursion

        public int ObjectCount => objectlist.Count();

        public FromObjectConverter(bool ignoreunserialisable, Type[] ignoredtypes, int maxrecursiondepth,
                                    BindingFlags membersearchflags, bool ignoreobjectpropertyifnull, string setname,
                                    Func<Object, JToken> customconvert)
        {
            this.ignoreunserialisable = ignoreunserialisable;
            this.ignoredtypes = ignoredtypes;
            this.maxrecursiondepth = maxrecursiondepth;
            this.membersearchflags = membersearchflags;
            this.ignoreobjectpropertyifnull = ignoreobjectpropertyifnull;
            this.setname = setname;
            this.customconvert = customconvert;
            this.objectlist = new Stack<object>();
        }

        public JToken Execute(Object o, int lvl, HashSet<string> memberignore, HashSet<string> memberinclude)
        {
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
                        return new JToken(JToken.TType.Error, "Self Reference in IDictionary");
                    }

                    JToken inner = Execute(kvp.Value, lvl + 1, null, null);
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
                return r ?? new JToken(JToken.TType.Error, "Unserializable " + tt.Name);
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
                        return new JToken(JToken.TType.Error, "Self Reference in IEnumerable");
                    }

                    JToken inner = Execute(oa, lvl + 1, null, null);

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
            else if (tt.IsClass ||                                     // if class
                      (tt.IsValueType && !tt.IsPrimitive && !tt.IsEnum && tt != typeof(DateTime))     // if value type, not primitive, not enum, its a structure. Not datetime (handled in CreateToken)
                    )
            {
                JObject outobj = new JObject();

                objectlist.Push(o);

                // get cached result of settings for this class given setname/membersearchflags
                Dictionary<string, JToken.MemberAttributeSettings> namestosettings = GetOrCreate(tt,setname,membersearchflags);

                foreach (var entry in namestosettings)  // for all active settings
                {
                    JToken.MemberAttributeSettings actrl = entry.Value;
                    var mi = actrl.MemberInfo;
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

                    if ((memberinclude != null && !memberinclude.Contains(attrname)) ||
                            (memberignore != null && memberignore.Contains(attrname)))
                    {
                        continue;
                    }

                    // is it an ignored type?

                    if (ignoredtypes != null && Array.IndexOf(ignoredtypes, innertype) >= 0)
                        continue;

                    //System.Diagnostics.Debug.WriteLine($"Member {mi.Name} {mi.MemberType} name {actrl.Name} ignoreall {actrl.IgnoreAll}");

                    // get the value

                    Object innervalue = null;
                    try
                    {
                        if (mi.MemberType == System.Reflection.MemberTypes.Property)
                        {
                            var pi = (System.Reflection.PropertyInfo)mi;
                            if (pi.GetIndexParameters().Length == 0)       // reject any indexer properties! new Dec 22
                            {
                                innervalue = pi.GetValue(o);
                            }
                        }
                        else
                            innervalue = ((System.Reflection.FieldInfo)mi).GetValue(o);
                    }
                    catch (Exception ex)
                    {
                        objectlist.Pop();
                        return new JToken(JToken.TType.Error, $"QuickJSON FromObject Unable to Get value of {mi.Name} due to {ex}");
                    }

                    if (innervalue != null)
                    {
                        if (objectlist.Contains(innervalue))        // self reference
                        {
                            if (ignoreunserialisable)               // if ignoring this, just continue with the next one
                                continue;

                            objectlist.Pop();
                            return new JToken(JToken.TType.Error, "Self Reference by " + tt.Name + ":" + mi.Name);
                        }

                        var token = actrl.CustomFormat ? customconvert(innervalue) :
                                    Execute(innervalue, lvl + 1, actrl.IgnoreSet, actrl.IncludeSet);

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
                        bool ignoreifnull = ignoreobjectpropertyifnull || actrl.IgnoreMemberIfNull;

                        if (ignoreifnull == false)
                            outobj[actrl.Name] = JToken.Null();        // its null so its a JNull
                    }
                }

                objectlist.Pop();
                return outobj;
            }
            else
            {
                var r = JToken.CreateToken(o, false);        // return token or null indicating unserializable
                return r ?? new JToken(JToken.TType.Error, "Unserializable " + tt.Name);
            }
        }

        /// <summary>
        /// Compute for a type/setname/member flags the FromObject control list, given the class and its JsonAttributes
        /// Cache the result for FromObject
        /// Useful if you need to see how FromObject will treat a class and what objects it will emit with what names
        /// </summary>
        /// <param name="classtype">Class type to analyse</param>
        /// <param name="setname">null, or particular set</param>
        /// <param name="membersearchflags">Members to consider</param>
        /// <returns>Dictionary of class member names vs JToken.MemberAttributeSettings</returns>
        public static Dictionary<string, JToken.MemberAttributeSettings> GetOrCreate(Type classtype, string setname, BindingFlags membersearchflags)
        {
            lock (CacheOfTypesFromMembers)        // thread lock
            {
                // try and find previously computed info

                var key = new Tuple<string, Type, BindingFlags>(setname, classtype,membersearchflags);

                if (!CacheOfTypesFromMembers.TryGetValue(key, out Dictionary<string, JToken.MemberAttributeSettings> namestosettings))
                {
                    // System.Diagnostics.Debug.WriteLine($"Created cached control {key}");

                    namestosettings = new Dictionary<string, JToken.MemberAttributeSettings>();

                    CacheOfTypesFromMembers[key] = namestosettings;

                    var allmembers = classtype.GetMembers(membersearchflags);

                    // we go thru all members and pick up the control settings for each one
                    foreach (var mi in allmembers)
                    {
                        // only props/fields

                        if (mi.MemberType == System.Reflection.MemberTypes.Property || mi.MemberType == System.Reflection.MemberTypes.Field)
                        {
                            var ac = new JToken.MemberAttributeSettings() { Name = mi.Name, MemberInfo = mi };
                            bool ignore = false;

                            // check on json ignore
                            var calist = mi.GetCustomAttributes(typeof(JsonIgnoreAttribute), false);
                            if (calist.Length == 1)
                            {
                                JsonIgnoreAttribute jia = calist[0] as JsonIgnoreAttribute;

                                if (jia.Setting == null)       // null, means all sets
                                {
                                    ignore = true;
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
                                                ignore = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (!ignore)    // if we ignore it, just don't store
                            {
                                // check on name
                                var renamelist = mi.GetCustomAttributes(typeof(JsonNameAttribute), false);
                                if (renamelist.Length == 1)
                                {
                                    JsonNameAttribute na = renamelist[0] as JsonNameAttribute;
                                    if (na.Sets == null)        // null, means all sets
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
                                    if (iin.Sets == null)       // null, means all sets
                                    {
                                        ac.IgnoreMemberIfNull = true;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < iin.Sets.Length; i++)  // find set, if not found, not applicable. Else its ignore if null
                                        {
                                            if (iin.Sets[i].EqualsI(setname))
                                            {
                                                ac.IgnoreMemberIfNull = true;
                                                break;
                                            }
                                        }
                                    }
                                }

                                var customoutput = mi.GetCustomAttributes(typeof(JsonCustomFormat), false);
                                if (customoutput.Length == 1)
                                {
                                    JsonCustomFormat jcf = customoutput[0] as JsonCustomFormat;
                                    if (jcf.Sets == null || jcf.Sets.Contains(setname, StringComparer.InvariantCulture))    // if matches
                                    {
                                        ac.CustomFormat = true;
                                    }
                                }

                                namestosettings[mi.Name] = ac;      // and store, we will process this
                                //System.Diagnostics.Debug.WriteLine($"{tt.Name} {mi.Name} included");
                            }
                            else
                            {
                                //System.Diagnostics.Debug.WriteLine($"{tt.Name} {mi.Name} ignored");
                            }
                        }
                    }
                }
                else
                {
                    // System.Diagnostics.Debug.WriteLine($"Lookup cached control {key}");
                }

                return namestosettings;
            }
        }
    }
}



