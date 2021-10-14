# QuickJSON
QuickJSON - a c# JSON encoder/decoder - Small, simple, quick

Aim is to be 
1) Quicker than the JSON decoder commonly used in C# 
2) Be simple to understand - no spagetti of classes
3) Be small with only a few files - so you can copy the code into your codebase and not have to have yet another dependent DLL to go along with your deployment

Download the .sln and you can build the test hardness to verify use

# JToken

This is the base class.  A Token is a value, or an JSON Array or JSON Object.

If its a value (HasValue), it has members which can convert the token to strings, numbers, longs etc.

# JArray
If its an Array (IsArray will be true) you can index through the JToken items of the array using array[n] syntax.  You can also use a foreach loop on it.

# JObject
If its an Object (IsObject will be true) you can look up a object property array["name"] syntax to return a JToken.  You can also use a foreach loop on it to return key/value pairs.

# Gets
The functions in QuickJSONGets allow you to safely convert JToken items to various types, all while not excepting. This is useful for reading external data which may have missing fields, or clashing types. You can specify a default value to use if the JToken does not read correctly.

# Parsing

The Parse functions allow you to turn a JSON string representation into a QuickJSON JToken structure of all the elements in the data.  Many different Parse functions exist in QuickJsonParse.cs file, which either throw an exception, or return a null. 

It also contains static string extensiopns for JSONParse, and rename functions for JTokens.  A .Object() and .Array() exists which converts the JToken to the respective types, or returns null.

# ToString

The ToString functions (QuickJSONToString.cs) allows conversion of the JToken structure to a string representation

# ToObject

The ToObject functions (QuickJSONToObject.cs) allow conversion of a JToken structure to a c# class.  The custom attribute JsonNameAttribute allows you to specify a list of names that is acceptable for each member variable if you need name redirection

# FromObject

The FromObject functions (QuickJSONFromObject.cs) allow conversion of a object to a JToken structure.  The custom attribute JsonNameAttribute can be used as above (only the first name is used). The custom attribute JsonIgnore allows you to specify this member should not go into the JSON.

# DeepEquals

Allows comparision of the token values of one JToken tree with another.

# ObjectFilter

Returns a filtered version of a JToken object tree given a filter tree.

# Token Reading

Instead of reading a string all at one into a JToken tree, the token reader presents to the caller one JSON token at a time. The reader gets the full logical structure of the JSON one token at a time. See the test harness for an example of using this.

# Optional Files

The files QuickJSONDeepEquals, ..FromObject, ..ToObject, ..TokenReader, .. Gets, ..ObjectFilter, StringParserTextReader are all optional for a simple string read and write JSON converter.




