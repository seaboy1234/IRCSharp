//    Project:     IRC# Server 
//    File:        IrcNumericResponce.cs
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

namespace IRCSharp
{
    /// <summary>
    /// Represents a Numeric Responce.
    /// <example>:irc.example.org 001 john :Welcome to the IRC Network.  Your userhost is john!JohnDoe@host.example.com.</example>
    /// </summary>
    public class IrcNumericResponce
    {
        /// <summary>
        /// Gets or sets the Id of this responce.
        /// </summary>
        public IrcNumericResponceId NumericId { get; set; }

        /// <summary>
        /// Gets or sets this message's Host (from)
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the first parameter of this message (generally the user's nick or a command name)
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Gets or sets any extra parameters before the message.
        /// </summary>
        public string Extra { get; set; }

        /// <summary>
        /// Gets or sets the human-readable message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Parses a responce.
        /// </summary>
        /// <param name="line"></param>
        public void Parse(string line)
        {
            IrcMessage message = new IrcMessage();
            message.Parse(line);
            Host = message.Prefix.Trim();
            NumericId = (IrcNumericResponceId)(int.Parse(message.Command));
            Extra = string.Join(" ", message.Params, 0, message.Params.Length - 1).Trim();
            Message = " :" + message.Params.Last().Trim();
        }

        public override string ToString()
        {
            IrcMessage message = new IrcMessage();
            List<string> strings = new List<string>();
            if (!string.IsNullOrEmpty(To))
            {
                strings.Add(To);
            }
            if (!string.IsNullOrEmpty(Extra))
            {
                strings.AddRange(Extra.Split(' '));
            }
            if (!string.IsNullOrEmpty(Message))
            {
                strings.Add(Message);
            }

            message.Prefix = Host;
            message.Command = ((int)NumericId).ToString("D3");
            message.Params = strings.ToArray();

            return message.ToString();
        }
    }
}
