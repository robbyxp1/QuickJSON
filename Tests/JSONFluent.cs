﻿/*
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
    public class JSONFluentTests
    {
        [Test]
        public void JSONFluent()
        {
            {
                var f1 = new JSONFormatter();
                f1.Object();
                f1.V("a", 1);
                f1.V("b", 2);
                string sa1 = f1.Get();
                Check.That(sa1).Equals(@"{""a"":1,""b"":2}");
            }
            {
                var f1 = new JSONFormatter();
                f1.Array();
                f1.V(1);
                f1.V(2);
                string sa1 = f1.Get();
                Check.That(sa1).Equals(@"[1,2]");
            }

            {
                var f1 = new JSONFormatter();
                f1.Array();
                f1.Object();
                f1.V("one", 1);
                f1.V("two", 2);
                f1.Object("three");
                f1.V("threeone", "A");
                f1.V("threetwo", "B");
                f1.Close();
                f1.V("four", 4);
                string sa1 = f1.Get();
                string t = @"[{""one"":1,""two"":2,""three"":{""threeone"":""A"",""threetwo"":""B""},""four"":4}]";
                Check.That(sa1).Equals(t);
            }

            {
                string filename = Path.GetTempFileName();
                System.Diagnostics.Debug.WriteLine($"Write to {filename}");
                using (FileStream filestream = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    using (JSONFormatterStreamer fluent = new JSONFormatterStreamer(filestream))
                    {
                        fluent.Array();

                        for (int i = 0; i < 2000; i++)
                        {
                            fluent.Object().V("one", i).V("two", "two").V("three", true).Close().LF();
                        }

                        fluent.Close();
                    }
                }
            }
            {
                using (MemoryStream memstream = new MemoryStream())
                {
                    using (JSONFormatterStreamer fluent = new JSONFormatterStreamer(memstream))
                    {
                        fluent.Array();

                        for (int i = 0; i < 2000; i++)
                        {
                            fluent.Object().V("one", i).V("two", "two").V("three", true).Close().LF();
                        }

                        fluent.Close();
                    }

                    memstream.Seek(0, SeekOrigin.Begin);

                    var enc = new System.Text.UTF8Encoding();
                    using( TextReader tr = new StreamReader(memstream,enc))
                    {
                        foreach(var t in JToken.ParseToken(tr))
                        {
                            //System.Diagnostics.Debug.WriteLine($"Token {t.ToString()}");
                        }
                    }
                }
            }
        }

    }
} 