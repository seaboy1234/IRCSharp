//    Project:     IRC# Server 
//    File:        StringExtensions.cs
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

namespace IRCSharp.Server
{
    public static class StringExtensions
    {
        public static bool PatternMatch(this string pattern, string subject)
        {
            if (pattern == subject)
            {
                return true;
            }
            if (pattern == "*" || subject == "*")
            {
                return true;
            }
            if (pattern.Length == 0)
            {
                return subject.Length == 0;
            }
            else if (subject.Length == 0)
            {
                return false;
            }

            if (pattern[0] == '*')
            {
                for (int i = 0; i < subject.Length; i++)
                {
                    if (pattern.Substring(1).PatternMatch(subject.Substring(i)))
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (subject[0] == '*')
            {
                for (int i = 0; i < pattern.Length; i++)
                {
                    if (pattern.Substring(i).PatternMatch(subject.Substring(1)))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return pattern[0] == subject[0] && pattern.Substring(1).PatternMatch(subject.Substring(1));
            }
        }
    }
}
