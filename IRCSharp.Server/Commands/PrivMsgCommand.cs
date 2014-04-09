//    Project:     IRC# Server 
//    File:        PrivMsgCommand.cs
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
    public class PrivMsgCommand : IrcCommand
    {
        public override string Name
        {
            get { return "privmsg"; }
        }

        public override void Run(IIrcUser client, IrcMessage message)
        {
            if (message.Params.Length != 2)
            {
                SendMessage(IrcNumericResponceId.ERR_NEEDMOREPARAMS, client, "Need more params.");
                return;
            }
            if (IRC.IsChannel(message.Params[0]))
            {
                IrcChannel channel = client.IrcServer.Channels.Where(chan => chan.Name.ToLower() == message.Params[0].ToLower()).FirstOrDefault();
                if (channel == null)
                {
                    client.Write(new IrcNumericResponce()
                    {
                        NumericId = IrcNumericResponceId.ERR_NOSUCHNICK,
                        To = message.Params[0],
                        Message = "No such nick/channel"
                    });
                    return;
                }
                channel.SendMessage(client, message.Params[1]);
            }
            else
            {
                IIrcUser other = client.IrcServer.Clients.Where(cli => cli.Nick == message.Params[0]).FirstOrDefault();
                if (other == null)
                {
                    client.Write(new IrcNumericResponce()
                    {
                        NumericId = IrcNumericResponceId.ERR_NOSUCHNICK,
                        To = message.Params[0],
                        Message = "No such nick/channel"
                    });
                    return;
                }
                other.Write(new IrcMessage
                {
                    Prefix = client.UserString,
                    Command = "PRIVMSG",
                    Params = new string[] { other.Nick, " :" + message.Params[1] }
                });
            }
        }
    }
}
