//    Project:     IRC# Server 
//    File:        IrcChannelUserMode.cs
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
    public class IrcChannelUserMode
    {
        private bool _isCreator;
        private bool _isAdmin;
        private bool _isOperator;
        private bool _isHalfOp;
        private bool _isVoice;
        private bool _isInvited;

        /// <summary>
        /// Gets or sets this user's Creator status.
        /// 
        /// Mode: +q
        /// Prefix: ~nick
        /// </summary>
        public bool IsCreator
        {
            get { return _isCreator; }
            set { _isCreator = value; }
        }

        /// <summary>
        /// Gets or sets this user's ChAdmin status.
        /// 
        /// Mode: +a
        /// Prefix: !nick
        /// </summary>
        public bool IsAdmin 
        { 
            get { return _isAdmin || IsCreator; }
            set { _isAdmin = value; }
        }

        /// <summary>
        /// Gets or sets this user's ChOp status.
        /// 
        /// Mode: +o
        /// Prefix: @nick
        /// </summary>
        public bool IsOperator
        {
            get { return _isOperator || IsAdmin; }
            set { _isOperator = value; }
        }

        /// <summary>
        /// Gets or sets this user's HalfOp status.
        /// 
        /// Mode: +h
        /// Prefix: %nick
        /// </summary>
        public bool IsHalfOp
        {
            get { return _isHalfOp || IsOperator; }
            set { _isHalfOp = value; }
        }

        /// <summary>
        /// Gets or sets this user's voice status.
        /// 
        /// Mode: +v
        /// Prefix: +
        /// </summary>
        public bool IsVoiced
        {
            get { return _isVoice || IsHalfOp; }
            set { _isVoice = value; }
        }

        /// <summary>
        /// Gets or sets this user's invite status.
        /// 
        /// Command: INVITE nick
        /// </summary>
        public bool IsInvited
        {
            get { return _isInvited || _isVoice; }
            set { _isInvited = value; }
        }

        /// <summary>
        /// Gets this user's prefix for this channel.
        /// </summary>
        public string NickPrefix
        {
            get
            {
                if (IsCreator)
                {
                    return "~";
                }
                if (IsAdmin)
                {
                    return "!";
                }
                if (IsOperator)
                {
                    return "@";
                }
                if (IsHalfOp)
                {
                    return "%";
                }
                if (IsVoiced)
                {
                    return "+";
                }
                return "";
            }
        }
    }
}
