//    Project:     IRC# Server 
//    File:        IRC.cs
//    Copyright:   Copyright (C) 2014 Christian Wilson. All rights reserved.
//    Website:     https://github.com/seaboy1234/IRCSharp
//    Description: An Internet Relay Chat (IRC) Server written in C#.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IRCSharp.Server
{
    public static class IRC
    {
        /// <summary>
        /// Available channel prefixes.
        /// </summary>
        public const string CHANNEL_PREFIXES = "#&+!";

        /// <summary>
        /// Alpha characters.
        /// </summary>
        public static readonly CharRange ALPHA = new CharRange(new byte[,] {
                                                   {0x41, 0x5A},
                                                   {0x61, 0x7A}
                                               });

        /// <summary>
        /// Digits
        /// </summary>
        public static readonly CharRange DIGIT = new CharRange(new byte[,] {
                                                   {0x30, 0x39}
                                               });

        /// <summary>
        /// Special characters.
        /// </summary>
        public static readonly CharRange SPECIAL = new CharRange(new byte[,] {
                                                     {0x5B, 0x60},
                                                     {0x7B, 0x7D},
                                                     {0x2D, 0x2E}
                                                 });

        /// <summary>
        /// No Space, CR, LF, or CL.
        /// </summary>
        public static readonly CharRange NOSPCRLFCL = new CharRange(new byte[,] {
                                                        {0x01, 0x09}, 
                                                        {0x0B, 0x0C}, 
                                                        {0x0E, 0x1F}, 
                                                        {0x21, 0x39}, 
                                                        {0x3B, 0xFF}
                                                    });

        /// <summary>
        /// Allowed characters for a user (nick!USER@host)
        /// </summary>
        public static readonly CharRange USER = new CharRange(new byte[,] {
                                                  {0x01, 0x09},
                                                  {0x0B, 0x0C},
                                                  {0x0E, 0x1F},
                                                  {0x21, 0x3F},
                                                  {0x41, 0xFF}
                                               });

        /// <summary>
        /// Allowed characters for a channel.
        /// </summary>
        public static readonly CharRange CHANNEL = new CharRange(new byte[,] {
                                                     {0x01, 0x06},
                                                     {0x08, 0x09},
                                                     {0x0B, 0x0C},
                                                     {0x0E, 0x1F},
                                                     {0x21, 0x2B},
                                                     {0x2D, 0x39},
                                                     {0x3B, 0xFF}
                                                 });

        /// <summary>
        /// Allowed characters for a Nickname.
        /// </summary>
        public static readonly CharRange NICKNAME = ALPHA + SPECIAL + DIGIT;

        /// <summary>
        /// Checls a string against a CharRange.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="charRange"></param>
        /// <returns>Whether this string is allowed based on the supplied range.</returns>
        public static bool CheckString(string str, CharRange charRange)
        {
            char[] allowed = charRange.All;
            foreach (char ch in str)
            {
                if (!allowed.Contains(ch))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks whether a string is a legel channel name.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static bool IsChannel(string channel)
        {
            if (!CheckString(channel, CHANNEL))
            {
                return false;
            }
            foreach (char ch in CHANNEL_PREFIXES)
            {
                if (channel[0] == ch)
                {
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// Represents a range of allowed characters.
    /// </summary>
    public class CharRange
    {
        /// <summary>
        /// Gets the beginings of the ranges.
        /// </summary>
        public List<char> Starts { get; private set; }

        /// <summary>
        /// Gets the endings of the ranges.
        /// </summary>
        public List<char> Ends { get; private set; }

        /// <summary>
        /// Gets all characters allowed by this range.
        /// </summary>
        public char[] All
        {
            get
            {
                List<char> all = new List<char>();
                for (int i = 0; i < Starts.Count; i++)
                {
                    for (char ch = Starts[i]; ch < Ends[i]; ch++)
                    {
                        if (!all.Contains(ch))
                        {
                            all.Add(ch);
                        }
                    }
                }
                return all.ToArray();
            }
        }

        /// <summary>
        /// Gets or sets the start and end of the range at the specified index.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public KeyValuePair<char, char> this[int i]
        {
            get
            {
                var kvp = new KeyValuePair<char, char>(Starts[i], Ends[i]);
                return kvp;
            }
            set
            {
                if (Starts.Count <= i)
                {
                    Starts[i] = value.Key;
                    Ends[i] = value.Value;
                }
                else
                {
                    Starts.Add(value.Key);
                    Ends.Add(value.Value);
                }
            }
        }

        public CharRange()
        {
            Starts = new List<char>();
            Ends = new List<char>();
        }

        public CharRange(byte[,] range)
            : this()
        {
            for (int i = 0; i < range.GetLength(0); i++)
            {
                Starts.Add((char)range[i, 0]);
                Ends.Add((char)range[i, 1]);
            }
        }

        public CharRange(CharRange range)
            : this()
        {
            Starts.AddRange(range.Starts);
            Ends.AddRange(range.Ends);
        }

        public static implicit operator CharRange(byte[,] bytes)
        {
            return new CharRange(bytes);
        }

        public static implicit operator byte[,](CharRange range)
        {
            byte[,] newRange = new byte[range.Starts.Count, 2];
            for (int i = 0; i < range.Starts.Count; i++)
            {
                newRange[i, 0] = (byte)range.Starts[i];
                newRange[i, 1] = (byte)range.Ends[i];
            }
            return newRange;
        }

        public static CharRange operator +(CharRange range1, CharRange range2)
        {
            CharRange newRange = new CharRange();
            newRange.Starts.AddRange(range1.Starts);
            newRange.Starts.AddRange(range2.Starts);
            newRange.Ends.AddRange(range1.Ends);
            newRange.Ends.AddRange(range2.Ends);
            return newRange;
        }
    }
}
