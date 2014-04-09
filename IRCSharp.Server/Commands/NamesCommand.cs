//    Project:     IRC# Server 
//    File:        NamesCommand.cs
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
    public class NamesCommand : IrcCommand
    {
        public override string Name
        {
            get { return "names"; }
        }

        public override void Run(IIrcUser client, IrcMessage message)
        {
            IrcChannel[] channels;
            if (message.Params.Length == 0)
            {
                channels = client.IrcServer.Channels.ToArray();
            }
            else
            {
                string[] channelList = message.Params[0].ToLower().Split(',');
                channels = client.IrcServer.Channels.Where(channel => 
                    channelList.Contains(channel.Name.Replace(",", "").ToLower())
                    ).ToArray();
            }
            channels.ForEach(channel =>
            {
                client.Write(new IrcNumericResponce
                {
                    NumericId = IrcNumericResponceId.RPL_NAMEREPLY,
                    Extra = "= " + channel.Name,
                    Message = string.Join(" ", 
                        channel.Users
                        .Where(user => !user.Mode.IsInvisible || channel.Users.Contains(client) || client.Mode.IsOperator)
                        .Select(user => string.Format("{0}{1}", user.ChannelModes[channel].NickPrefix, user.Nick)))
                });
            });
            client.Write(new IrcNumericResponce
            {
                NumericId = IrcNumericResponceId.RPL_ENDOFNAMES,
                Extra = client.Nick, 
                Message = "End of names"
            });
        }
    }
}
