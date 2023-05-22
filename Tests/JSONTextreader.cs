/*
* Copyright © 2018 EDDiscovery development team
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
* 
* EDDiscovery is not affiliated with Frontier Developments plc.
*/

using NFluent;
using NUnit.Framework;
using QuickJSON;
using QuickJSON.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Tests;

namespace JSONTests
{
    [TestFixture(TestOf = typeof(JToken))]
    public class JSONTextreader
    {
        [Test]
        public void JSONtextReader()
        {
            string propertyv =
@"    [
  {
    ""Count"":0,
    ""Name"":""0"",
    ""Name_Localised"":""L0"",
    ""FriendlyName"":null,
    ""Category"":null,
    ""QValue"":null,
    ""PropGetSet"":1
  },
  {
    ""Count"":0,
    ""Name"":""1"",
    ""Name_Localised"":""This is a long string to try and make the thing break"",
    ""FriendlyName"":null,
    ""Category"":null,
    ""QValue"":20
  }
]
";

            string jsongithub = @"
        {
          ""url"": ""https://api.github.com/repos/EDDiscovery/EDDiscovery/releases/25769192"",
          ""assets_url"": ""https://api.github.com/repos/EDDiscovery/EDDiscovery/releases/25769192/assets"",
          ""upload_url"": ""https://uploads.github.com/repos/EDDiscovery/EDDiscovery/releases/25769192/assets{?name,label}"",
          ""html_url"": ""https://github.com/EDDiscovery/EDDiscovery/releases/tag/Release_11.4.0"",
          ""id"": 25769192,
          ""node_id"": ""MDc6UmVsZWFzZTI1NzY5MTky"",
          ""tag_name"": ""Release_11.4.0"",
          ""target_commitish"": ""master"",
          ""name"": ""EDDiscovery Release 11.4.0 Material Trader, Scan improvements, lots of others"",
          ""draft"": false,
          ""author"": {
            ""login"": ""robbyxp1"",
            ""id"": 6573992,
            ""node_id"": ""MDQ6VXNlcjY1NzM5OTI="",
            ""avatar_url"": ""https://avatars1.githubusercontent.com/u/6573992?v=4"",
            ""gravatar_id"": """",
            ""url"": ""https://api.github.com/users/robbyxp1"",
            ""html_url"": ""https://github.com/robbyxp1"",
            ""followers_url"": ""https://api.github.com/users/robbyxp1/followers"",
            ""following_url"": ""https://api.github.com/users/robbyxp1/following{/other_user}"",
            ""gists_url"": ""https://api.github.com/users/robbyxp1/gists{/gist_id}"",
            ""starred_url"": ""https://api.github.com/users/robbyxp1/starred{/owner}{/repo}"",
            ""subscriptions_url"": ""https://api.github.com/users/robbyxp1/subscriptions"",
            ""organizations_url"": ""https://api.github.com/users/robbyxp1/orgs"",
            ""repos_url"": ""https://api.github.com/users/robbyxp1/repos"",
            ""events_url"": ""https://api.github.com/users/robbyxp1/events{/privacy}"",
            ""received_events_url"": ""https://api.github.com/users/robbyxp1/received_events"",
            ""type"": ""User"",
            ""site_admin"": false
          },
          ""prerelease"": true,
          ""created_at"": ""2020-04-24T12:32:30Z"",
          ""published_at"": ""2020-04-24T12:37:33Z"",
          ""assets"": [
            {
              ""url"": ""https://api.github.com/repos/EDDiscovery/EDDiscovery/releases/assets/20114552"",
              ""id"": 20114552,
              ""node_id"": ""MDEyOlJlbGVhc2VBc3NldDIwMTE0NTUy"",
              ""name"": ""EDDiscovery.Portable.zip"",
              ""label"": null,
              ""uploader"": {
                ""login"": ""robbyxp1"",
                ""id"": 6573992,
                ""node_id"": ""MDQ6VXNlcjY1NzM5OTI="",
                ""avatar_url"": ""https://avatars1.githubusercontent.com/u/6573992?v=4"",
                ""gravatar_id"": """",
                ""url"": ""https://api.github.com/users/robbyxp1"",
                ""html_url"": ""https://github.com/robbyxp1"",
                ""followers_url"": ""https://api.github.com/users/robbyxp1/followers"",
                ""following_url"": ""https://api.github.com/users/robbyxp1/following{/other_user}"",
                ""gists_url"": ""https://api.github.com/users/robbyxp1/gists{/gist_id}"",
                ""starred_url"": ""https://api.github.com/users/robbyxp1/starred{/owner}{/repo}"",
                ""subscriptions_url"": ""https://api.github.com/users/robbyxp1/subscriptions"",
                ""organizations_url"": ""https://api.github.com/users/robbyxp1/orgs"",
                ""repos_url"": ""https://api.github.com/users/robbyxp1/repos"",
                ""events_url"": ""https://api.github.com/users/robbyxp1/events{/privacy}"",
                ""received_events_url"": ""https://api.github.com/users/robbyxp1/received_events"",
                ""type"": ""User"",
                ""site_admin"": false
              },
              ""content_type"": ""application/x-zip-compressed"",
              ""state"": ""uploaded"",
              ""size"": 11140542,
              ""download_count"": 24,
              ""created_at"": ""2020-04-24T12:35:04Z"",
              ""updated_at"": ""2020-04-24T12:35:13Z"",
              ""browser_download_url"": ""https://github.com/EDDiscovery/EDDiscovery/releases/download/Release_11.4.0/EDDiscovery.Portable.zip""
            },
            {
              ""url"": ""https://api.github.com/repos/EDDiscovery/EDDiscovery/releases/assets/20114548"",
              ""id"": 20114548,
              ""node_id"": ""MDEyOlJlbGVhc2VBc3NldDIwMTE0NTQ4"",
              ""name"": ""EDDiscovery_11.4.0.exe"",
              ""label"": null,
              ""uploader"": {
                ""login"": ""robbyxp1"",
                ""id"": 6573992,
                ""node_id"": ""MDQ6VXNlcjY1NzM5OTI="",
                ""avatar_url"": ""https://avatars1.githubusercontent.com/u/6573992?v=4"",
                ""gravatar_id"": """",
                ""url"": ""https://api.github.com/users/robbyxp1"",
                ""html_url"": ""https://github.com/robbyxp1"",
                ""followers_url"": ""https://api.github.com/users/robbyxp1/followers"",
                ""following_url"": ""https://api.github.com/users/robbyxp1/following{/other_user}"",
                ""gists_url"": ""https://api.github.com/users/robbyxp1/gists{/gist_id}"",
                ""starred_url"": ""https://api.github.com/users/robbyxp1/starred{/owner}{/repo}"",
                ""subscriptions_url"": ""https://api.github.com/users/robbyxp1/subscriptions"",
                ""organizations_url"": ""https://api.github.com/users/robbyxp1/orgs"",
                ""repos_url"": ""https://api.github.com/users/robbyxp1/repos"",
                ""events_url"": ""https://api.github.com/users/robbyxp1/events{/privacy}"",
                ""received_events_url"": ""https://api.github.com/users/robbyxp1/received_events"",
                ""type"": ""User"",
                ""site_admin"": false
              },
              ""content_type"": ""application/x-msdownload"",
              ""state"": ""uploaded"",
              ""size"": 15672578,
              ""download_count"": 55,
              ""created_at"": ""2020-04-24T12:34:52Z"",
              ""updated_at"": ""2020-04-24T12:35:02Z"",
              ""browser_download_url"": ""https://github.com/EDDiscovery/EDDiscovery/releases/download/Release_11.4.0/EDDiscovery_11.4.0.exe""
            }
          ],
          ""tarball_url"": ""https://api.github.com/repos/EDDiscovery/EDDiscovery/tarball/Release_11.4.0"",
          ""zipball_url"": ""https://api.github.com/repos/EDDiscovery/EDDiscovery/zipball/Release_11.4.0"",
          ""body"": ""This is a major overhaul of the Scan panel, addition of Material Trader panels, and general overhaul of lots of the program.\r\n\r\n*** \r\nMajor features\r\n\r\n* Scan panel gets more visuals and a new menu system to select output. Many more options added including distance, star class, planet class, highlighting planets in hab zone. Layout has been optimised.  Since the menu system was reworked all previous selections of display type will need to be reset - use the drop down menu to select them.  The default is everything on.\r\n* UI won't stall when looking up data from EDSM - previous it would stop until EDSM responded. Now just the panel which is asking will stop updating. Rest of the system carries on.\r\n* Material Trader panel added - plan you material trades in advance to see the best outcome before you commit to flying to a trader.\r\n* Surveyor Panel gets many more options for display - show all planets/stars etc and gets more information\r\n* Travel grid, Ships/Loadout, Material Commodities, Engineering, Synthesis get word wrap option to word wrap columns instead of truncating them. Double click on the row now works better expanding/contracting the text.\r\n* Ships/Loadout gets a All modules selection to list all your modules across all ships - useful for engineering\r\n* Synthesis, Engineering and Shopping list panels\r\n\r\nOther Improvements\r\n\r\n* All materials are now classified by Material Group Type\r\n* Improved loading speed when multiple tabbed panels are present - does not sit and use processing time now like it could do\r\n* EDSM Data pick up includes surface gravity\r\n* Journal Missions entry gets faction effects printed\r\n* Can force sell a ship if for some reason your journal has lost the sell ship event\r\n* Various Forms get a close X icon\r\n* Fuel/Reservoir updates are much better, Ships/loadouts auto change with them, and they don't bombard the system with micro changes\r\n* Star Distance panel - fix issue when setting the Max value which could cause it not to look up any stars again\r\n* Workaround for GDI error when saving bitmap\r\n* Bounty Event report correct ship name\r\n* New Y resizer on top of EDD form so you can resize there\r\n* Removed old surface scanner engineering recipes\r\n* Excel output of scan data now works out the value correctly dependent on if you mapped the body\r\n* Can force EDD to use TLS2 \r\n* Asteroid Prospected prints out mats in normal info, Mining refined gets total and type as per MaterialCollected\r\n\r\n***\r\n\r\n|  | EDDiscovery <version>.exe |\r\n|---------|------------------------------------------------------------------|\r\n| SHA-256 | 01D84BF967FE5CDFF2DDC782F0D68FCB4B80F3881EE1F883941454DF9FBB8823 | \r\n\r\n|  |  EDDiscovery.Portable.zip |\r\n|---------|------------------------------------------------------------------|\r\n| SHA-256 | 1D365A30B51280B4676410694C3D1E9F21DF525403E53B735245FD6C7B584DCA |\r\n\r\n![image](https://user-images.githubusercontent.com/6573992/80213091-8d931400-8630-11ea-9f3c-f56d43f7edd8.png)\r\n\r\n\r\n\r\n\r\n""
        }";

            {
                using (StringReader sr = new StringReader(propertyv))         // read directly from file..
                {
                    foreach (var t in JToken.ParseToken(sr, JToken.ParseOptions.None))
                    {
                        if (t.IsProperty)
                            System.Diagnostics.Debug.WriteLine("Property " + t.Name + " " + t.TokenType + " `" + t.Value + "`");
                        else
                            System.Diagnostics.Debug.WriteLine("Token " + t.TokenType + " " + t.Value);
                    }

                }

            }

            {

                using (StringReader sr = new StringReader(jsongithub))         // read directly from file..
                {
                    foreach (var t in JToken.ParseToken(sr, JToken.ParseOptions.None))
                    {
                        if (t.IsProperty)
                            System.Diagnostics.Debug.WriteLine("Property " + t.Name + " " + t.TokenType + " `" + t.Value + "`");
                        else
                            System.Diagnostics.Debug.WriteLine("Token " + t.TokenType + " " + t.Value);
                    }

                }
            }
            {
                using (StringReader sr = new StringReader(jsongithub))         // read directly from file..
                {
                    var enumerator = JToken.ParseToken(sr, JToken.ParseOptions.None).GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        var t = enumerator.Current;
                        if (t.IsProperty)
                            System.Diagnostics.Debug.WriteLine("Property " + t.Name + " " + t.TokenType + " `" + t.Value + "`");
                        else
                            System.Diagnostics.Debug.WriteLine("Token " + t.TokenType + " " + t.Value);
                    }
                }
            }

            {
                string propertyq =
@"    [
  {
    ""Count"":0,
    ""Name"":""0"",
    ""Name_Localised"":""L0"",
    ""FriendlyName"":null,
    ""ArrayValue"":[1,2,3],
    ""Category"":null,
    ""QValue"":null,
    ""PropGetSet"":1
  },
  {
    ""Count"":0,
    ""Name"":""1"",
    ""Name_Localised"":""This is a long string to try and make the thing break"",
    ""FriendlyName"":null,
    ""Category"":null,
    ""QValue"":20
  }
]
";

                using (StringReader sr = new StringReader(propertyq))         // read directly from file..
                {
                    var enumerator = JToken.ParseToken(sr, JToken.ParseOptions.None, 128).GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        var t = enumerator.Current;
                        if (t.IsObject)
                        {
                            JObject to = t as JObject;
                            bool res = JToken.LoadTokens(enumerator);
                            Check.That(res).IsTrue();
                            Check.That(to["Category"]).IsNotNull();
                        }
                    }
                }
            }


            string filename = @"c:\code\edsm\edsmsystems.10000.json";
            if (File.Exists(filename))
            {
                using (FileStream originalFileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(originalFileStream))
                    {
                        foreach (var t in JToken.ParseToken(sr, JToken.ParseOptions.None, 2999))
                        {
                            if (t.IsProperty)
                                System.Diagnostics.Debug.WriteLine("Property " + t.Name + " " + t.TokenType + " `" + t.Value + "`");
                            else
                                System.Diagnostics.Debug.WriteLine("Token " + t.TokenType + " " + t.Value);
                        }
                    }
                }
            }

        }

        [Test]
        public void JSONStarFile()
        {
            string filename = @"c:\code\examples\edsm\systems_1month.json";
            if (File.Exists(filename))
            {
                using (FileStream originalFileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(originalFileStream))
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        var en = JToken.ParseToken(sr, JToken.ParseOptions.None, 512000).GetEnumerator();

                        while (en.MoveNext())
                        {
                            var t = en.Current;
                        }

                        System.Diagnostics.Debug.WriteLine($"Time is {sw.ElapsedMilliseconds}");

                    }
                }
            }
        }

        [Test]
        public void JSONTextReaderIntTest()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            for (long entry = 0; entry < 5000000; entry++)
                sb.Append($"\"count{entry}\":{entry * 23}, ");
            sb.Append("}");

            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 0; i < 1; i++)
            {
                //System.Diagnostics.Trace.WriteLine($"Loop {i}");
                using (StringReader sr = new StringReader(sb.ToString()))         // read directly from file..
                {
                    var sq = new StringParserQuickTextReader(sr, 16384);
                    char[] buf = new char[16384];

                    int entry = 0;

                    char ch;
                    while ((ch = sq.GetChar()) > char.MinValue)
                    {
                        if (ch == '{')
                        {
                            while ((ch = sq.GetNextNonSpaceChar(false)) == '"')
                            {
                                var cc = sq.NextQuotedString('"', buf);

                                if (cc > 0 && sq.IsCharMoveOn(':'))
                                {
                                    cc = sq.NextCharBlock(buf, (x) => char.IsDigit(x) || x == '-');
                                    if (cc > 0)
                                    {
                                        var svalue = new string(buf, 0, cc);
                                        if (long.TryParse(svalue, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out long lv))
                                            System.Diagnostics.Debug.Assert(lv == entry * 23);
                                        else
                                            System.Diagnostics.Debug.Assert(false);

                                        entry++;
                                        //string s = new string(buf, 0, cc); System.Diagnostics.Debug.WriteLine($"value {s}");

                                        sq.IsCharMoveOn(',');
                                    }
                                    else
                                        System.Diagnostics.Debug.Assert(false);
                                }
                                else
                                    System.Diagnostics.Debug.Assert(false);
                            }

                            System.Diagnostics.Debug.Assert(ch == '}');

                        }
                    }
                }
            }

            // with 5e6 seen 2350ms
            System.Diagnostics.Trace.WriteLine($"Time {sw.ElapsedMilliseconds}");
        }

        [Test]
        public void JSONTextReaderJValueTest()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            for (long entry = 0; entry < 5000000; entry++)
                sb.Append($"\"count{entry}\":{entry * 23}, ");
            sb.Append("}");

            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 0; i < 1; i++)
            {
                //System.Diagnostics.Trace.WriteLine($"Loop {i}");
                using (StringReader sr = new StringReader(sb.ToString()))         // read directly from file..
                {
                    var sq = new StringParserQuickTextReader(sr, 16384);
                    char[] buf = new char[16384];

                    int entry = 0;

                    char ch;
                    while ((ch = sq.GetChar()) > char.MinValue)
                    {
                        if (ch == '{')
                        {
                            while ((ch = sq.GetNextNonSpaceChar(false)) == '"')
                            {
                                var cc = sq.NextQuotedString('"', buf);

                                if (cc > 0 && sq.IsCharMoveOn(':'))
                                {
                                    JToken tk = sq.JNextNumber(false);
                                    Check.That(tk != null && tk.IsLong && (long)tk == entry * 23).IsTrue();
                                    entry++;
                                    sq.IsCharMoveOn(',');
                                }
                                else
                                    System.Diagnostics.Debug.Assert(false);
                            }

                            System.Diagnostics.Debug.Assert(ch == '}');

                        }
                    }
                }
            }

            System.Diagnostics.Trace.WriteLine($"Time {sw.ElapsedMilliseconds}");
        }

        [Test]
        public void JSONTextReaderCharBlock()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            for (long entry = 0; entry < 100; entry++)
                sb.Append($"\"count\":{entry * 23}, ");
            sb.Append("}");

            var chkcount = QuickJSON.Utils.Extensions.Checksum("count");

            for (int i = 0; i < 1; i++)
            {
                //System.Diagnostics.Trace.WriteLine($"Loop {i}");
                using (StringReader sr = new StringReader(sb.ToString()))         // read directly from file..
                {
                    var sq = new StringParserQuickTextReader(sr, 16384);
                    char[] buf = new char[16384];

                    int entry = 0;

                    char ch;
                    while ((ch = sq.GetChar()) > char.MinValue)
                    {
                        if (ch == '{')
                        {
                            while ((ch = sq.GetNextNonSpaceChar(false)) == '"')
                            {
                                var checksum = sq.ChecksumCharBlock((xa) => xa != '"');

                                if (checksum == chkcount && sq.IsCharMoveOn('"') && sq.IsCharMoveOn(':'))
                                {
                                    JToken tk = sq.JNextNumber(false);
                                    Check.That(tk != null && tk.IsLong && (long)tk == entry * 23).IsTrue();
                                    entry++;
                                    sq.IsCharMoveOn(',');
                                }
                                else
                                    System.Diagnostics.Debug.Assert(false);
                            }

                            System.Diagnostics.Debug.Assert(ch == '}');

                        }
                    }
                }
            }
        }

        [Test]
        public void JSONStringReaderCharBlock()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            for (long entry = 0; entry < 100; entry++)
                sb.Append($"\"count\":{entry * 23}, ");
            sb.Append("}");

            var chkcount = QuickJSON.Utils.Extensions.Checksum("count");

            for (int i = 0; i < 1; i++)
            {
                {
                    var sq = new StringParserQuick(sb.ToString());
                    int entry = 0;

                    char ch;
                    while ((ch = sq.GetChar()) > char.MinValue)
                    {
                        if (ch == '{')
                        {
                            while ((ch = sq.GetNextNonSpaceChar(false)) == '"')
                            {
                                var checksum = sq.ChecksumCharBlock((xa) => xa != '"');

                                if (checksum == chkcount && sq.IsCharMoveOn('"') && sq.IsCharMoveOn(':'))
                                {
                                    JToken tk = sq.JNextNumber(false);
                                    Check.That(tk != null && tk.IsLong && (long)tk == entry * 23).IsTrue();
                                    entry++;
                                    sq.IsCharMoveOn(',');
                                }
                                else
                                    System.Diagnostics.Debug.Assert(false);
                            }

                            System.Diagnostics.Debug.Assert(ch == '}');

                        }
                    }
                }
            }
        }

    }
} 