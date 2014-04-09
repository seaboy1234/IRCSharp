//    Project:     IRC# Server 
//    File:        IrcMessage.cs
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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IRCSharp
{
    public class IrcMessage
    {
        public string Prefix { get; set; }
        public string Command { get; set; }
        public string[] Params { get; set; }

        public void Read(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                Parse(reader.ReadLine());
            }
        }

        public void Write(Stream stream)
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine(this);
                writer.Flush();
            }
        }

        public void Parse(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return;
            }

            Regex parsingRegex = new Regex(@"^(:(?<prefix>\S+) )?(?<command>\S+)( (?!:)(?<params>.+?))?( :(?<trail>.+))?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
            Match messageMatch = parsingRegex.Match(line);

            if (messageMatch.Success)
            {
                Prefix = messageMatch.Groups["prefix"].Value;
                Command = messageMatch.Groups["command"].Value;
                Params = messageMatch.Groups["params"].Value.Split(' ');
                string trailing = messageMatch.Groups["trail"].Value;

                if (!String.IsNullOrEmpty(trailing))
                {
                    Params = Params.Concat(new string[] { trailing }).ToArray();
                }
            }
        }

        public override string ToString()
        {
            string message = "";
            if (!string.IsNullOrEmpty(Prefix))
            {
                message += string.Format(":{0} ", Prefix.Trim());
            }
            message += string.Format("{0} ", Command.Trim());
            message += string.Join(" ", Params, 0, Params.Length - 1).Trim();
            message += Params.Last().Contains(' ') ? " :" : " ";
            message += Params.Last().Trim();

            message = message.Replace("  ", " ");
            return message;
        }
    }
}
