//    Project:     IRC# Server 
//    File:        UserCommand.cs
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
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace IRCSharp.Server.Commands
{
    public class UserCommand : IrcCommand
    {
        public override string Name
        {
            get { return "user"; }
        }

        public override bool RequireRegistered
        {
            get { return false; }
        }

        public override void Run(IIrcUser client, IrcMessage message)
        {
            if (client.Mode.IsRegistered)
            {
                client.Write(new IrcNumericResponce
                {
                    NumericId = IrcNumericResponceId.ERR_ALREADYREGISTERED,
                    Message = "You're already registered.",
                    Extra = "USER"
                });
                return;
            }
            client.User = message.Params[0];
            if (string.IsNullOrEmpty(client.Nick))
            {
                client.Nick = string.Format("${0}_{1}", client.User, string.Concat(Guid.NewGuid().ToString().Replace("-", "").First(5)));
            }
            byte mode = 0;
            byte.TryParse(message.Params[1], out mode);
            client.Mode.Parse(mode);
            client.Name = message.Params.Last();

            SendMessage(IrcNumericResponceId.RPL_WELCOME, client, string.Format("Welcome to the Internet Relay Network!  You're known as {0}!{1}@{2}", client.Nick, client.User, client.Host));
            SendMessage(IrcNumericResponceId.RPL_YOURHOST, client, "Your host is running IRCSharp 0.0.1-DEV");
            SendMessage(IrcNumericResponceId.RPL_CREATED, client, "This server was created on 2014-04-04.");
            SendMessage(IrcNumericResponceId.RPL_MYINFO, client, "127.0.0.1 IRCSharp aiwrOos anipqsvkl");
            client.Write(new IrcNumericResponce() { 
                NumericId = IrcNumericResponceId.RPL_ISUPPORT,
                Extra = "CASEMAPPING=ascii PREFIX=(qaohv)~!@%+ FNC",
                Message = "are supported by this server"
            });
            client.Write(new IrcNumericResponce()
            {
                NumericId = IrcNumericResponceId.RPL_ISUPPORT,
                Extra = "CHANTYPES=!#&+ INVEX=I RFC2812",
                Message = "are supported by this server"
            });
            SendMessage(IrcNumericResponceId.ERR_NOMOTD, client, ":)");
            if (mode > 0)
            {
                Say(client, "MODE", client.Mode.ToString());
            }
            client.Mode.IsRegistered = true;
        }
    }
}
