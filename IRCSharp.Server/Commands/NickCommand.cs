//    Project:     IRC# Server 
//    File:        NickCommand.cs
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

namespace IRCSharp.Server.Commands
{
    class NickCommand : IrcCommand
    {
        public override string Name
        {
            get { return "Nick"; }
        }
        public override bool RequireRegistered
        {
            get { return false; }
        }

        public override void Run(IIrcUser client, IrcMessage message)
        {
            if (message.Params.Length != 1)
            {
                SendMessage(IrcNumericResponceId.ERR_NONICKNAMEGIVEN, client, "No nickname given");
                return;
            }
            if (!IRC.CheckString(message.Params[0], IRC.NICKNAME))
            {
                SendMessage(IrcNumericResponceId.ERR_ERRONEUSNICKNAME, client, "Erroneous nickname");
                return;
            }
            if (!client.IrcServer.CheckNick(message.Params[0]))
            {
                SendMessage(IrcNumericResponceId.ERR_NICKNAMEINUSE, client, "Nickname is already in use");
                return;
            }
            string oldNick = client.Nick;
            client.Channels.ForEach(channel => channel.Nick(client, message.Params[0]));
            if (!string.IsNullOrEmpty(client.User))
            {
                Say(client, "NICK", message.Params[0]);
            }
            client.Nick = message.Params[0];
        }
    }
}
