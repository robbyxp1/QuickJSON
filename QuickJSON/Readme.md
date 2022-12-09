This is a JSON encoder decoder, which is designed to be quick and very small footprint.

Measured quicker that the common c# JSON code, and much much smaller.

Designed with very few classes : JToken, JObject, JArray.

Convert to/from C# objects.

Designed with safe extension getter classes (jt["Name"].Str()) to make it safe to read those json files with data sometimes present or not.

Full class help is available on our Wiki : https://github.com/robbyxp1/QuickJSON/wiki/Class-List