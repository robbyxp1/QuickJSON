## QuickJSON
QuickJSON - a c# JSON encoder/decoder - Small, simple, quick

Aim is to be
1) Quicker than the JSON decoder commonly used in C#
2) Be simple to understand - no spagetti of classes
3) Be small with only a few files - so you can copy the code into your codebase and not have to have yet another dependent DLL to go along with your deployment

Download the .sln and you can build the test hardness to verify use.

Check out the [Wiki](../../wiki) for help.  The full class list is [here](../../wiki/QuickJSON-Class-List)

## JToken

This is the base class in QuickJSONToken.cs.  A Token is a value, or an JSON Array or JSON Object.

If its a value (HasValue), it has members which can convert the token to strings, numbers, longs etc.

The file starts with plenty of token classifier members (.IsLong, .IsString). It then has JToken constructors for many different types, as well as a CreateToken(Object o) general token construction.

A set of conversions to types follow, so you can write (string)token to convert to string, etc.  An exception will be thrown if the JToken is not of the right type. Strings are not automatically parsed for numbers note.

A Clone() function provides a means to make an exact copy of the JToken Tree.

A set of virtual functions allow a JToken which is an object or an array to be indexed or enumerated directly without the need to convert it explicity to a JObject or JArray.

## JArray
If its an Array (IsArray will be true) you can index through the JToken items of the array using array[n] syntax.  You can also use a foreach loop on it.  TryGetValue, Add, AddRange, RemoveAt, RemoveRange, Clear, First, FirstOrDefault, Last, LastOrDefaultis provided.  Find with a predicate is provided.  Conversion to string, int, long or double lists is provided (may except).

## JObject
If its an Object (IsObject will be true) you can look up a object property array["name"] syntax to return a JToken.  You can also use a foreach loop on it to return key/value pairs.  You can find the list of property names using ProperyNames(). First, FirstOrDefault, Last, LastOrDefault is provided.  Contains and TryGetValue allow you to test to see if a property is present.  Add, Remove, Clear is provided.

## Gets
The functions in QuickJSONGets allow you to safely convert JToken items to various types, all while not excepting. This is useful for reading external data which may have missing fields, or clashing types. You can specify a default value to use if the JToken does not read correctly.  An example is token.Str("default value").

It also contains static string extensions for JSONParse, and rename functions for JTokens.  A .Object() and .Array() exists which converts the JToken to the respective types, or returns null.

## Parsing

The JToken Parse functions allow you to turn a JSON string representation into a QuickJSON JToken structure of all the elements in the data.  Many different Parse functions exist in QuickJsonParse.cs file, which either throw an exception, or return a null.

## ToString

The ToString functions (QuickJSONToString.cs) allows conversion of the JToken structure to a string representation

## ToObject

The ToObject functions (QuickJSONToObject.cs) allow conversion of a JToken structure to a c# class.  The custom attribute JsonNameAttribute allows you to specify a list of names that is acceptable for each member variable if you need name redirection

## FromObject

The FromObject functions (QuickJSONFromObject.cs) allow conversion of a object to a JToken structure.  The custom attribute JsonNameAttribute can be used as above. The custom attribute JsonIgnore allows you to specify this member should not go into the JSON.

## DeepEquals

Allows comparision of the token values of one JToken tree with another.

## ObjectFilter

Returns a filtered version of a JToken object tree given a filter tree.

## Token Reading

Instead of reading a string all at one into a JToken tree, the token reader presents to the caller one JSON token at a time. The reader gets the full logical structure of the JSON one token at a time. See the test harness for an example of using this.

## Dates

Dates are stored in JToken as strings. When an c# object (FromObject) is converted, its written to the JToken as a string in zulu format. When written to an object (ToObject) the JToken date string is converted by InvariantCulture, assuming UTC.

## Optional Files

The files QuickJSONDeepEquals, ..FromObject, ..ToObject, ..TokenReader, .. Gets, ..ObjectFilter, StringParserTextReader, JSONFluentFormatter are all optional for a simple string read and write JSON converter.

## Fluent Formatter

A simple Fluent Formatter, JSONFormatter is also present. This can construct JSON strings quickly and easily using a fluent format.

## Tests

The NUnit test DLL has plenty of examples of how to use the above functions.  See JSONTests.cs. Note it expects a folder called c:\code which is available for writing.





