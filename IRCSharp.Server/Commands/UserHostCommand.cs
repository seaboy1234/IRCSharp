//    Project:     IRC# Server 
//    File:        UserHostCommand.cs
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
    public class UserHostCommand : IrcCommand
    {
        public override string Name
        {
            get { return "userhost"; }
        }

        public override void Run(IIrcUser client, IrcMessage message)
        {
            string user = "";
            if (message.Params.Length != 1)
            {
                user = client.Nick;
            }
            user = message.Params[0];
            if (client.IrcServer.CheckNick(user))
            {
                SendMessage(IrcNumericResponceId.ERR_NOSUCHNICK, client, "No such nick");
                return;
            }
            IIrcUser other = client.IrcServer.Clients.Where(cli => cli.Nick == user).FirstOrDefault();
            SendMessage(IrcNumericResponceId.RPL_USERHOST, client, string.Format("{0}=+{1}@{2}", other.Nick, other.User, other.Host));
        }
    }
}
