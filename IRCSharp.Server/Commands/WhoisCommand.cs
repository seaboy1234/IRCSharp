//    Project:     IRC# Server 
//    File:        WhoisCommand.cs
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
    public class WhoisCommand : IrcCommand
    {
        public override string Name
        {
            get { return "whois"; }
        }

        public override void Run(IIrcUser client, IrcMessage message)
        {
            IIrcUser other = client.IrcServer.Clients.Where(cli => cli.Nick.ToLower() == message.Params[0].ToLower()).FirstOrDefault();
            if (other == null)
            {
                // TODO: no such nick.
                return;
            }
            client.Write(new IrcNumericResponce
            {
                NumericId = IrcNumericResponceId.RPL_WHOISUSER,
                To = other.Nick,
                Extra = string.Format("{0} {1}", other.User, other.Host),
                Message = other.Name
            });
            client.Write(new IrcNumericResponce
            {
                NumericId = IrcNumericResponceId.RPL_WHOISIDLE,
                To = other.Nick,
                Extra = ((int)(DateTime.Now - other.Idle).TotalSeconds).ToString(),
                Message = "seconds idle"
            });
            client.Write(new IrcNumericResponce
            {
                NumericId = IrcNumericResponceId.RPL_WHOISCHANNELS,
                To = other.Nick,
                Message = string.Join(" ", other.Channels.Select(channel => other.ChannelModes[channel].NickPrefix + channel.Name).ToArray())
            });
            client.Write(new IrcNumericResponce
            {
                NumericId = IrcNumericResponceId.RPL_ENDOFWHOIS,
                To = other.Nick,
                Message = "End of WHOIS list."
            });
        }
    }
}
