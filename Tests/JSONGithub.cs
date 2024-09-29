/*
 * Copyright © 2021-2023 Robbyxp1 @ github.com
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

using NFluent;
using NUnit.Framework;
using QuickJSON;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Tests;

namespace JSONTests
{
    [TestFixture(TestOf = typeof(JToken))]

    public class JSONTestsGithub
    {

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

        bool func(JToken j)
        {
            if (j["name"].Str().ToLowerInvariant().EndsWith(".zip") && j["name"].Str().ToLowerInvariant().Contains("portable"))
                return true;
            else
                return false;
        }

        [Test]
        public void JSONGithub()
        {
            JToken decode = JToken.Parse(jsongithub);
            Check.That(decode).IsNotNull();
            string json2 = decode.ToString(true);
            JToken decode2 = JToken.Parse(json2);
            Check.That(decode2).IsNotNull();
            string json3 = decode2.ToString(true);
            Check.That(json2).IsEqualTo(json3);

            var asset = decode["assets"];
            var e1 = asset.FirstOrDefault(func);
            Check.That(e1).IsNotNull();
            Check.That(e1["size"].IsInt).IsTrue();
            Check.That(e1["size1"]?.IsInt ?? true).IsTrue();
            Check.That(e1["size"].Int() == 11140542).IsTrue();
            Check.That(e1["state"].Str() == "uploaded").IsTrue();
        }

    }
}