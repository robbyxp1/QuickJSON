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

using System;

namespace QuickJSON
{
    /// <summary>
    /// Ignore attribute. Attach to an member of a class to say don't serialise this item or 
    /// to exclude certain members or to only include specific members
    /// Applicable to FromObject and ToObject
    /// </summary>
    public sealed class JsonIgnoreAttribute : Attribute 
    {
        /// <summary>
        /// Class to hold a JSON ignore setting for a single set
        /// </summary>
        public class SetSetting
        {
            /// <summary> Set name. Attribute sets allow selection of different outputs from the same class </summary>
            public string Set { get; set; }
            /// <summary> FromObject: If non null, list of object members to ignore completely</summary>
            public string[] Ignore { get; set; }
            /// <summary> FromObject: If non null, list of object members to include only</summary>
            public string[] IncludeOnly { get; set; }

            /// <summary> Construct default set setting </summary>
            public SetSetting() { }
            /// <summary> Construct a set setting for a named set </summary>
            public SetSetting(string set ) { Set = set; }
            /// <summary> Construct a set setting for a named set with include/ignore and a list of names</summary>
            public SetSetting(string set, Operation ignoreorinclude, string[] names)
            {
                Set = set;
                if (ignoreorinclude == Operation.Include) IncludeOnly = names; else Ignore = names;
            }
        }

        /// <summary>Settings of the ignore by set. Null if just ignore all</summary>
        public SetSetting[] Setting { get; set; }

        /// <summary> FromObject and ToObject: Constructor to indicate that this member should be ignored completely for all sets </summary>
        public JsonIgnoreAttribute() { Setting = null; }

        /// <summary> FromObject and ToObject: Constructor to indicate that this member should be ignored completely for this list of sets
        /// For other sets the member is not ignored</summary>
        public JsonIgnoreAttribute(params string[] setnames) {
            Setting = new SetSetting[setnames.Length];
            for (int i = 0; i < setnames.Length; i++) 
                Setting[i] = new SetSetting(setnames[i]);
        }

        /// <summary> Options for FromObject only. </summary>
        public enum Operation {
            /// <summary> Use ignore to say the list is a set of members to ignore </summary>
            Ignore,
            /// <summary> Use include to say the list is a set of members only to include </summary>
            Include
        };

        /// <summary> FromObject: Constructor to indicate which members of this class to enumerate to JSON for the default set only
        /// If Ignore, name list are the members which should be excluded. All others will be included
        /// If Include, name list are only the members which will be included, all others will be excluded.
        /// ToObject: the setting is ignored and the member processed as normal
        /// </summary>
        public JsonIgnoreAttribute(Operation ignoreorinclude, params string[] names) {
            Setting = new SetSetting[1] { new SetSetting(null,ignoreorinclude, names) };
        }

        // Note: Can't seem to get SetSettings[] working, so do manually

        /// <summary> FromObject: Constructor to indicate which members of this class to enumerate to JSON, by set and with include/ignore attributes, twice
        /// ToObject: the setting is ignored and the member processed as normal
        /// </summary>
        public JsonIgnoreAttribute(string setname1, Operation ignoreorinclude1, string[] names1,
            string setname2, Operation ignoreorinclude2, string[] names2)
        {
            Setting = new SetSetting[2] { new SetSetting(setname1, ignoreorinclude1, names1),
                new SetSetting(setname2, ignoreorinclude2, names2)
            };
        }

        /// <summary> FromObject: Constructor to indicate which members of this class to enumerate to JSON, by set and with include/ignore attributes, three times
        /// ToObject: the setting is ignored and the member processed as normal
        /// </summary>
        public JsonIgnoreAttribute(string setname1, Operation ignoreorinclude1, string[] names1,
            string setname2, Operation ignoreorinclude2, string[] names2,
            string setname3, Operation ignoreorinclude3, string[] names3)
        {
            Setting = new SetSetting[3] { new SetSetting(setname1, ignoreorinclude1, names1),
                new SetSetting(setname2, ignoreorinclude2, names2), new SetSetting(setname3, ignoreorinclude3, names3)
            };
        }
    }

/// <summary>
/// Name attribute. Attach to an member of a class to indicate an alternate name to use in the JSON structure from its c# name.
/// Applicable to FromObject and ToObject.  
/// ToObject supports multiple names (any name in JSON will match this entry), FromObject only one and uses the first entry if multiple is given
/// </summary>
public sealed class JsonNameAttribute : Attribute
    {
        /// <summary> If non null, this lists the names of the sets associated with the name list.
        /// Must be the same length as Names list
        /// For ToObject, sets can be mentioned more than once to get multiple names accepted.
        /// For FromObject, the first matching set name gives the name of the output JSON variable
        /// Attribute sets allow selection of different inputs and outputs from the same class </summary>
        public string[] Sets { get; set; }
        /// <summary> List of names for this attribute </summary>
        public string[] Names { get; set; }

        /// <summary> Constructor with name list, applies to all sets.  
        /// ToObject, this names a list of names to accept 
        /// FromObject, the first name in the list is used for the output</summary>
        public JsonNameAttribute(params string[] names) { Names = names; Sets = null; }

        /// <summary> Constructor with set name and name list. Bool is just used as a marker. Set names can be repeated. 
        /// sets and names must be the same length</summary>
        public JsonNameAttribute(string[] sets, string[] names) { Names = names; Sets = sets; System.Diagnostics.Trace.Assert(Names.Length == Sets.Length); }

    }
    /// <summary>
    /// Attach to a member to indicate if the value of it is null, don't add it to JSON.
    /// Applicable to FromObject only
    /// </summary>
    public sealed class JsonIgnoreIfNullAttribute : Attribute
    {
        /// <summary> If non null, this belongs to an attribute set name to check.  If null, applies to all sets
        /// Attribute sets allow selection of different outputs from the same class </summary>
        public string[] Sets { get; set; }
        /// <summary> Constructor, applies to all sets </summary>
        public JsonIgnoreIfNullAttribute() { Sets = null; }
        /// <summary> Constructor with a list of sets to apply this to.</summary>
        public JsonIgnoreIfNullAttribute(params string[] setnames) { Sets = setnames; }
    }

    /// <summary>
    /// Attach to a member of a class to indicate a custom output/input call is needed
    /// Applicable to FromObject and ToObject
    /// Must supply a customformat callback to both From and To object if used.
    /// </summary>
    public sealed class JsonCustomFormat : Attribute
    {
        /// <summary> If non null, this belongs to an attribute set name to check.  If null, applies to all sets
        /// Attribute sets allow selection of different outputs from the same class </summary>
        public string[] Sets { get; set; }

        /// <summary> Constructor, applies to all sets </summary>
        public JsonCustomFormat() { Sets = null; }
        /// <summary> Constructor with a list of sets to apply this to.</summary>
        public JsonCustomFormat(params string[] setnames) { Sets = setnames; }
    }

}



