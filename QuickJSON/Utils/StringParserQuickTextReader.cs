﻿/*
 * Copyright © 2021-2024 robbyxp1 @ github.com
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
using System.IO;
using System.Linq;

namespace QuickJSON.Utils
{
    /// <summary>
    /// Text Reader Parser Quick for JSON system.  
    /// </summary>

    [System.Diagnostics.DebuggerDisplay("Action {new string(line,pos,line.Length-pos)} : ({new string(line,0,line.Length)})")]
    public class StringParserQuickTextReader : IStringParserQuick
    {
        /// <summary>Constructor </summary>
        /// <param name="textreader">Textreader to read from</param>
        /// <param name="chunksize">Chunk size to take each time from textreader. Make sure its big enough to read the longest number</param>
        public StringParserQuickTextReader(TextReader textreader, int chunksize)
        {
            tr = textreader;
            line = new char[chunksize];
        }
        
        /// <inheritdoc cref="QuickJSON.Utils.IStringParserQuick.Position"/>
        public int Position { get { return pos; } }
        /// <inheritdoc cref="QuickJSON.Utils.IStringParserQuick.Line"/>
        public string Line { get { return new string(line, 0, length); } }
        /// <inheritdoc cref="QuickJSON.Utils.IStringParserQuick.IsEOL"/>
        public bool IsEOL()
        {
            if (pos < length)       // if we have data, its not eol
                return false;
            else
                return !Reload();   // else reload. reload returns true if okay, so its inverted for EOL
        }

        /// <inheritdoc cref="QuickJSON.Utils.IStringParserQuick.GetChar"/>
        public char GetChar()
        {
            if (pos < length)
                return line[pos++];
            else
            {
                Reload();
                return pos < length ? line[pos++] : char.MinValue;
            }
        }

        /// <inheritdoc cref="QuickJSON.Utils.IStringParserQuick.GetNextNonSpaceChar"/>
        public char GetNextNonSpaceChar(bool skipspacesafter = true)
        {
            SkipSpace();

            if (pos < length)
                return line[pos++];
            else
            {
                Reload();
                if (pos < length)
                {
                    char ret = line[pos++];
                    if ( skipspacesafter )
                        SkipSpace();
                    return ret;
                }
                else
                    return char.MinValue;
            }
        }

        /// <inheritdoc cref="QuickJSON.Utils.IStringParserQuick.PeekChar"/>
        public char PeekChar()
        {
            if (pos < length)
                return line[pos];
            else
            {
                Reload();
                return pos < length ? line[pos] : char.MinValue;
            }
        }

        /// <inheritdoc cref="QuickJSON.Utils.IStringParserQuick.SkipSpace"/>
        public void SkipSpace()
        {
            while (pos < length && char.IsWhiteSpace(line[pos]))        // first skip what we have
                pos++;

            while (pos == length)                   // if we reached end, then reload, and skip, and repeat if required
            {
                if (!Reload())
                    return;
                while (pos < length && char.IsWhiteSpace(line[pos]))
                    pos++;
            }
        }

        /// <inheritdoc cref="QuickJSON.Utils.IStringParserQuick.IsStringMoveOn(string)"/>
        public bool IsStringMoveOn(string s)
        {
            if (pos > length - s.Length)            // if not enough to cover s, reload
                Reload();

            if (pos + s.Length > length)            // if not enough for string, not true
                return false;

            for (int i = 0; i < s.Length; i++)
            {
                if (line[pos + i] != s[i])
                    return false;
            }

            pos += s.Length;
            SkipSpace();

            return true;
        }

        /// <inheritdoc cref="QuickJSON.Utils.IStringParserQuick.IsCharMoveOn(char, bool)"/>
        public bool IsCharMoveOn(char t, bool skipspaceafter = true)
        {
            if (pos == length)                          // if at end, reload
                Reload();

            if (pos < length && line[pos] == t)
            {
                pos++;
                if (skipspaceafter)
                    SkipSpace();
                return true;
            }
            else
                return false;
        }

        /// <inheritdoc cref="QuickJSON.Utils.IStringParserQuick.BackUp"/>
        public void BackUp()
        {
            pos--;
        }

        /// <inheritdoc cref="QuickJSON.Utils.IStringParserQuick.NextQuotedString(char, char[], bool, bool)"/>
        public int NextQuotedString(char quote, char[] buffer, bool replaceescape = false, bool skipafter = true)
        {
            int bpos = 0;

            while (true)
            {
                if (pos == line.Length)                 // if out of chars, reload
                    Reload();

                if (pos == line.Length || bpos == buffer.Length)  // if reached end of line, or out of buffer, error
                {
                    return -1;
                }
                else if (line[pos] == quote)        // if reached quote, end of string
                {
                    pos++; //skip end quote

                    if ( skipafter)
                        SkipSpace();

                    return bpos;
                }
                else if (line[pos] == '\\' ) // 2 chars min
                {
                    if (pos >= length - 6)      // this number left for a good go
                    {
                        Reload();
                        if (length < 1)
                            return -1;
                    }

                    pos++;
                    char esc = line[pos++];     // grab escape and move on

                    if (esc == quote)
                    {
                        buffer[bpos++] = esc;      // place in the character
                    }
                    else if (replaceescape)
                    {
                        switch (esc)
                        {
                            case '\\':
                                buffer[bpos++] = '\\';
                                break;
                            case '/':
                                buffer[bpos++] = '/';
                                break;
                            case 'b':
                                buffer[bpos++] = '\b';
                                break;
                            case 'f':
                                buffer[bpos++] = '\f';
                                break;
                            case 'n':
                                buffer[bpos++] = '\n';
                                break;
                            case 'r':
                                buffer[bpos++] = '\r';
                                break;
                            case 't':
                                buffer[bpos++] = '\t';
                                break;
                            case 'u':
                                if (pos < line.Length - 4)
                                {
                                    int? v1 = line[pos++].ToHex();
                                    int? v2 = line[pos++].ToHex();
                                    int? v3 = line[pos++].ToHex();
                                    int? v4 = line[pos++].ToHex();
                                    if (v1 != null && v2 != null && v3 != null && v4 != null)
                                    {
                                        char c = (char)((v1 << 12) | (v2 << 8) | (v3 << 4) | (v4 << 0));
                                        buffer[bpos++] = c;
                                    }
                                }
                                else
                                    return -1;
                                break;
                        }
                    }
                }
                else
                    buffer[bpos++] = line[pos++];
            }
        }

        static char[] decchars = new char[] { '.', 'e', 'E', '+', '-' };

        /// <inheritdoc cref="QuickJSON.Utils.IStringParserQuick.JNextNumber(bool,bool)"/>
        public JToken JNextNumber(bool sign, bool skipafter = true)     
        {
            // must be on a digit

            ulong ulv = 0;
            bool bigint = false;
            int start = pos;
            bool slid = false;

            while (true)
            {
                if (pos == line.Length)     // if at end of loaded text
                {
                    System.Diagnostics.Debug.Assert(slid == false);         // must not slide more than once
                    Reload(start);              // get more data, keeping d  back to start
                    start = 0;                  // start of number now at 0 in buffer
                    slid = true;
                }

                if (pos == line.Length)         // if at end, return number got
                {
                    if (bigint)
                    {
#if JSONBIGINT
                        string part = new string(line, start, pos - start);    // get double string

                        if (skipafter)
                            SkipSpace();

                        if (System.Numerics.BigInteger.TryParse(part, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out System.Numerics.BigInteger bv))
                            return new JToken(JToken.TType.BigInt, sign ? -bv : bv);
                        else
#endif
                            return null;
                    }
                    else if (pos == start)      // no chars read
                        return null;
                    else if (ulv <= long.MaxValue)
                        return new JToken(JToken.TType.Long, sign ? -(long)ulv : (long)ulv);
                    else if (sign)
                        return null;
                    else
                        return new JToken(JToken.TType.ULong, ulv);
                }
                else if (line[pos] < '0' || line[pos] > '9')        // if at end of integer..
                {
                    if (line[pos] == '.' || line[pos] == 'E' || line[pos] == 'e')  // if we have gone into a decimal, collect the string and return
                    {
                        while(true)
                        {
                            if (pos == line.Length)
                            {
                                System.Diagnostics.Debug.Assert(slid == false);         // must not slide more than once
                                Reload(start);              // get more data, keeping text back to start
                                start = 0;                  // start of number now at 0 in buffer
                                slid = true;
                            }

                            if (pos < line.Length && ((line[pos] >= '0' && line[pos] <= '9') || decchars.Contains(line[pos])))
                                pos++;
                            else
                                break;
                        }

                        string part = new string(line, start, pos - start);    // get double string

                        if (skipafter)
                            SkipSpace();

                        if (double.TryParse(part, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double dv))
                            return new JToken(JToken.TType.Double, sign ? -dv : dv);
                        else
                            return null;
                    }
                    else if (bigint)
                    {
#if JSONBIGINT
                        string part = new string(line, start, pos - start);    // get double string

                        if (skipafter)
                            SkipSpace();

                        if (System.Numerics.BigInteger.TryParse(part, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out System.Numerics.BigInteger bv))
                            return new JToken(JToken.TType.BigInt, sign ? -bv : bv);
                        else
#endif
                            return null;
                    }
                    else
                    {
                        if (pos == start)   // this means no chars, caused by a - nothing
                            return null;

                        if (skipafter)
                            SkipSpace();

                        if (ulv <= long.MaxValue)
                            return new JToken(JToken.TType.Long, sign ? -(long)ulv : (long)ulv);
                        else if (sign)
                            return null;
                        else
                            return new JToken(JToken.TType.ULong, ulv);
                    }
                }
                else
                {
                    if (ulv > ulong.MaxValue / 10)  // if going to overflow, bit int. collect all ints
                        bigint = true;

                    ulv = (ulv * 10) + (ulong)(line[pos++] - '0');
                }
            }
        }

        static JToken jendarray = new JToken(JToken.TType.EndArray);

        /// <inheritdoc cref="QuickJSON.Utils.IStringParserQuick.JNextValue(char[], bool)"/>
        public JToken JNextValue(char[] buffer, bool inarray)
        {
            //System.Diagnostics.Debug.WriteLine("Decode at " + p.LineLeft);
            char next = GetChar();
            switch (next)
            {
                case '{':
                    SkipSpace();
                    return new JObject();

                case '[':
                    SkipSpace();
                    return new JArray();

                case '"':
                    int textlen = NextQuotedString(next, buffer, true);
                    return textlen >= 0 ? new JToken(JToken.TType.String, new string(buffer, 0, textlen)) : null;

                case ']':
                    if (inarray)
                    {
                        SkipSpace();
                        return jendarray;
                    }
                    else
                        return null;

                case '0':       // all positive. JSON does not allow a + at the start (integer fraction exponent)
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    BackUp();
                    return JNextNumber(false);
                case '-':
                    return JNextNumber(true);
                case 't':
                    return IsStringMoveOn("rue") ? new JToken(JToken.TType.Boolean, true) : null;
                case 'f':
                    return IsStringMoveOn("alse") ? new JToken(JToken.TType.Boolean, false) : null;
                case 'n':
                    return IsStringMoveOn("ull") ? new JToken(JToken.TType.Null) : null;

                default:
                    return null;
            }
        }

        /// <inheritdoc cref="QuickJSON.Utils.IStringParserQuick.NextCharBlock(char[], Func{char, bool}, bool)"/>
        public int NextCharBlock(char[] buffer, Func<char, bool> test, bool skipafter = true)
        {
            int bpos = 0;

            while (true)
            {
                if (pos == line.Length)     // if out of chars, reload
                    Reload();

                if (pos == line.Length || bpos == buffer.Length)  // if reached end of line, or out of buffer, error
                {
                    return -1;
                }

                if (test(line[pos]))        // if ok, store
                {
                    buffer[bpos++] = line[pos++];
                }
                else
                {
                    if (skipafter)
                        SkipSpace();

                    return bpos;
                }
            }
        }

        /// <inheritdoc cref="QuickJSON.Utils.IStringParserQuick.ChecksumCharBlock(Func{char, bool}, bool)"/>
        public uint ChecksumCharBlock(Func<char, bool> test, bool skipafter = true)
        {
            uint checksum = 0;

            while (true)
            {
                if (pos == line.Length)     // if out of chars, reload
                    Reload();

                if (pos == line.Length)  // if reached end of line, checksum
                {
                    return Math.Max(1, checksum);
                }

                if (test(line[pos]))        // if ok, store
                {
                    checksum += 7 + (uint)line[pos++] * 23;
                }
                else
                {
                    if (skipafter)
                        SkipSpace();

                    return Math.Max(1, checksum);
                }
            }
        }

        private bool Reload(int from = -1)          // from means keep at this position onwards, default is pos.
        {
            if (from == -1)
                from = pos;

            if (from < length)                      // if any left, slide
            {
                Array.Copy(line, from, line, 0, length - from);
                length -= from;
                pos -= from;
            }
            else
            {
                pos = length = 0;
            }

            if (length < line.Length)               // if space left, fill
            {
                length += tr.ReadBlock(line, length, line.Length - length);
            }

            return length > 0;
        }


        private TextReader tr;
        private char[] line;
        private int length = 0;
        private int pos = 0;
    }
}
