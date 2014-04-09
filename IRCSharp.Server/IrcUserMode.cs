//    Project:     IRC# Server 
//    File:        IrcUserMode.cs
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
using System.Threading.Tasks;

namespace IRCSharp.Server
{
    public class IrcUserMode
    {
        /// <summary>
        /// Gets or sets whether this user is broadcast to anyone.
        /// </summary>
        public bool IsInvisible { get; set; }

        /// <summary>
        /// Gets or sets whether this user is away.
        /// 
        /// MUST NOT be set using the MODE command.
        /// </summary>
        public bool IsAway { get; set; }

        /// <summary>
        /// Gets or sets whether this user is a restricted conection.
        /// </summary>
        public bool IsRestricted { get; set; }

        /// <summary>
        /// Gets or sets whether this user is an IRCOperator.
        /// 
        /// MUST NOT be set using MODE command.
        /// </summary>
        public bool IsOperator { get; set; }

        /// <summary>
        /// Gets or sets whether this user is a Local IRCOperator.
        /// 
        /// MUST NOT be set using MODE command.
        /// </summary>
        public bool IsLocalOperator { get; set; }

        /// <summary>
        /// Gets or sets whether this user is registered.
        /// </summary>
        public bool IsRegistered { get; set; }

        /// <summary>
        /// Gets or sets whether this user should receive WALLOPS messages.
        /// </summary>
        public bool ReceiveWallops { get; set; }

        /// <summary>
        /// Gets or sets whether this user should reseive server notices.
        /// </summary>
        public bool ReceiveServerNotices { get; set; }

        [Flags]
        public enum Modes
        {
            None = 0,

            /// <summary>
            /// Mode +w
            /// </summary>
            Wallops = 2,

            /// <summary>
            /// Mode +r
            /// </summary>
            Restricted = 4,

            /// <summary>
            /// Mode +i
            /// </summary>
            Invisible = 8,
        }

        public void Parse(byte rawMode)
        {
            Modes mode = (Modes)rawMode;
            IsInvisible = FlagsHelper.IsSet(mode, Modes.Invisible);
            IsRestricted = FlagsHelper.IsSet(mode, Modes.Restricted);
            ReceiveWallops = FlagsHelper.IsSet(mode, Modes.Wallops);
        }

        public void Parse(string mode)
        {
            if (string.IsNullOrEmpty(mode))
            {
                return;
            }
            bool setTrue = mode[0] == '+';
            foreach (char ch in mode)
            {
                if (ch == '+')
                {
                    setTrue = true;
                }
                else if (ch == '-')
                {
                    setTrue = false;
                }
                else
                {
                    switch (ch)
                    {
                        case 'a':
                            if (!setTrue)
                            {
                                IsAway = false;
                            }
                            break;
                        case 'i':
                            IsInvisible = setTrue;
                            break;
                        case 'w':
                            ReceiveWallops = setTrue;
                            break;
                        case 'r':
                            IsRestricted = setTrue;
                            break;
                        case 'o':
                            if (!setTrue)
                            {
                                IsOperator = false;
                            }
                            break;
                        case 'O':
                            if (!setTrue)
                            {
                                IsLocalOperator = false;
                            }
                            break;
                        case 's':
                            ReceiveServerNotices = setTrue;
                            break;
                    }
                }
            }
        }
        public override string ToString()
        {
            string mode = "";
            if (IsAway)
            {
                mode += "a";
            }
            if (IsInvisible)
            {
                mode += "i";
            }
            if (IsLocalOperator)
            {
                mode += "O";
            }
            if (IsOperator)
            {
                mode += "o";
            }
            if (IsRestricted)
            {
                mode += "r";
            }
            if (ReceiveWallops)
            {
                mode += "w";
            }
            if (ReceiveServerNotices)
            {
                mode += "s";
            }
            if (IsRegistered)
            {
                mode += "R";
            }
            if (!string.IsNullOrEmpty(mode))
            {
                return "+" + mode;
            }
            return "";
        }
    }
}
