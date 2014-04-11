//    Project:     IRC# Server 
//    File:        ModeCommand.cs
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
    public class ModeCommand : IrcCommand
    {
        public override string Name
        {
            get { return "mode"; }
        }

        public override void Run(IIrcUser client, IrcMessage message)
        {
            string name = message.Params[0];
            if (IRC.IsChannel(name))
            {
                IrcChannel channel = client.IrcServer.GetChannel(message.Params[0]);
                if (channel == null)
                {
                    NotFoundError(client, true);
                    return;
                }
                if (message.Params.Length < 1)
                {
                    NeedMoreParamsError(client);
                    return;
                }
                channel.Mode.Parse(client, message);
                return;
            }
            if (message.Params.Length < 2)
            {
                NeedMoreParamsError(client);
                return;
            }
            IIrcUser other = client.IrcServer.Clients.Find(user => user.Nick.ToLower() == name.ToLower());
            if (other == null)
            {
                NotFoundError(client, false);
                return;
            }
            string mode = message.Params[1];
            if (!((client == other) || client.Mode.IsOperator))
            {
                PermissionsError(client);
                return;
            }
            other.Mode.Parse(mode);
            client.Write(new IrcNumericResponce
            {
                NumericId = IrcNumericResponceId.RPL_UMODEIS,
                To = other.Nick,
                Extra = other.Mode.ToString()
            });
            if (other != client)
            {
                other.Write(new IrcNumericResponce
                {
                    NumericId = IrcNumericResponceId.RPL_UMODEIS,
                    To = other.Nick,
                    Extra = other.Mode.ToString()
                });
            }
        }
    }
}
