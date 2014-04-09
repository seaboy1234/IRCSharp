//    Project:     IRC# Server 
//    File:        IrcServer.cs
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
using System.Threading;
using System.Threading.Tasks;
using seaboy1234.Logging;

namespace IRCSharp.Server
{
    public class IrcServer
    {
        public static Logger Logger { get; private set; }

        public string Hostname { get; set; }
        public string Motd { get; set; }

        public List<IrcChannel> Channels { get; set; }
        public List<IIrcUser> Clients { get; set; }

        public IrcServer()
        {
            Channels = new List<IrcChannel>();
            Clients = new List<IIrcUser>();
            Logger = new Logger("IRC# Server", new ConsoleLogListener(LogLevel.Debug));

            Task.Factory.StartNew(DispatchPings);
            Task.Factory.StartNew(PruneChannels);
        }

        public void NewClient(IIrcUser client)
        {
            Clients.Add(client);
        }

        public void UserLeft(IIrcUser ircClient, string reason)
        {
            foreach (IrcChannel channel in ircClient.Channels)
            {
                channel.Part(ircClient, reason);
            }
            Clients.Remove(ircClient);
        }

        public void Broadcast(IrcMessage message)
        {
            Clients.ForEach(client => client.Write(message));
        }

        public bool CheckNick(string nick)
        {
            return !Clients.TrueForAny(client => client.Nick == nick);
        }

        public void JoinChannel(IIrcUser client, string channel)
        {
            IrcChannel chan = Channels.Where(ch => ch.Name == channel).FirstOrDefault();
            if (chan == null)
            {
                chan = new IrcChannel(this)
                {
                    Name = channel
                };
                Channels.Add(chan);
            }
            Logger.Log(LogLevel.Info, "{0} joined channel {1}", client.Nick, chan.Name);
            client.Channels.Add(chan);
            chan.Join(client);
        }

        public void LeaveChannel(IIrcUser client, string channel, string reason)
        {
            IrcChannel chan = client.Channels.Where(ch => ch.Name == channel).FirstOrDefault();
            if (chan == null)
            {
                if (Channels.Where(ch => ch.Name == channel).Count() != 0)
                {
                    client.Write(new IrcNumericResponce()
                    {
                        NumericId = IrcNumericResponceId.ERR_NOTONCHANNEL,
                        Message = "You're not on that channel",
                        To = channel
                    });
                }
                else
                {
                    client.Write(new IrcNumericResponce()
                    {
                        NumericId = IrcNumericResponceId.ERR_NOSUCHCHANNEL,
                        Message = "No such channel",
                        To = channel
                    });
                }
                return;
            }
            Logger.Log(LogLevel.Info, "{0} parted channel {1}", client.Nick, chan.Name);
            chan.Part(client, reason);
            client.Channels.Remove(chan);
        }
        private void DispatchPings()
        {
            Clients
                .Where(client => ((int)(DateTime.Now - client.Idle).TotalMinutes) >= 1)
                .ForEach(client =>
                {
                    if (((int)(DateTime.Now - client.Idle).TotalMinutes) >= 2)
                    {
                        client.Quit("ping timeout");
                    }
                    client.Write(new IrcMessage
                    {
                        Command = "PING",
                        Params = Hostname.Split(' '),
                        Prefix = Hostname
                    });
                });
            Thread.Sleep((int)TimeSpan.FromMinutes(2).TotalMilliseconds);
            Task.Factory.StartNew(DispatchPings);
        }
        private void PruneChannels()
        {
            Channels.RemoveGroup(Channels.Where(channel => channel.Users.Count == 0).ForEach(channel => channel.Destroy()));
            Thread.Sleep((int)TimeSpan.FromMinutes(2).TotalMilliseconds);
            Task.Factory.StartNew(PruneChannels);
        }
    }
}
