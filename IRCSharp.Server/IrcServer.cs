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
        /// <summary>
        /// Gets the server's logger.
        /// </summary>
        public static Logger Logger { get; private set; }

        /// <summary>
        /// Gets or sets this server's Hostname.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// Gets or sets this server's Message of the Day.
        /// </summary>
        public string Motd { get; set; }

        /// <summary>
        /// Gets this server's list of active channels.
        /// </summary>
        public List<IrcChannel> Channels { get; private set; }

        /// <summary>
        /// Gets this server's list of connected clients.
        /// </summary>
        public List<IIrcUser> Clients { get; private set; }

        /// <summary>
        /// Gets the date and time that this server was started.
        /// </summary>
        public DateTime Created { get; private set; }

        public IrcServer()
        {
            Channels = new List<IrcChannel>();
            Clients = new List<IIrcUser>();
            Logger = new Logger("IRC# Server", new TextWriterLogListener(Console.Out, LogLevel.Debug));

            Created = DateTime.Now;
            Task.Factory.StartNew(DispatchPings);
            Task.Factory.StartNew(PruneChannels);
        }

        /// <summary>
        /// Causes the server to acknowlage a new client.
        /// </summary>
        /// <param name="client"></param>
        public void NewClient(IIrcUser client)
        {
            Clients.Add(client);
        }

        /// <summary>
        /// Causes the server to acknowlage a user leaving.
        /// </summary>
        /// <param name="ircClient"></param>
        /// <param name="reason"></param>
        public void UserLeft(IIrcUser ircClient, string reason)
        {
            foreach (IrcChannel channel in ircClient.Channels)
            {
                channel.Part(ircClient, reason);
            }
            Clients.Remove(ircClient);
        }

        /// <summary>
        /// Broadcasts an <see cref="IRCSharp.IrcMessage"/> to the entire server.
        /// </summary>
        /// <param name="message"></param>
        public void Broadcast(IrcMessage message)
        {
            Clients.ForEach(client => client.Write(message));
        }

        /// <summary>
        /// Checks if a nickname is available.
        /// 
        /// <remarks>Does not check if a nickname is valid.  Use <see cref="IRCSharp.Server.IRC.CheckString"/> to check.</remarks>
        /// </summary>
        /// <param name="nick"></param>
        /// <returns></returns>
        public bool CheckNick(string nick)
        {
            return !Clients.TrueForAny(client => client.Nick == nick);
        }

        /// <summary>
        /// Causes a client to join a channel.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="channel"></param>
        public void JoinChannel(IIrcUser client, string channel)
        {
            IrcChannel chan = GetChannel(channel);
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

        /// <summary>
        /// Checks if a channel exists.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool ChannelExists(string channel)
        {
            return Channels.Where(chan => chan == channel).Count() > 0;
        }

        /// <summary>
        /// Gets an IrcChannel.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public IrcChannel GetChannel(string channel)
        {
            return Channels.Find(chan => chan == channel);
        }

        /// <summary>
        /// Causes a client to part a channel.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="channel"></param>
        /// <param name="reason"></param>
        public void LeaveChannel(IIrcUser client, string channel, string reason)
        {
            IrcChannel chan = GetChannel(channel);
            if (chan == null)
            {
                if (!ChannelExists(channel))
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

        /// <summary>
        /// Dispatches PINGs to inactive users and prunes them if they do not respond.
        /// </summary>
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

        /// <summary>
        /// Prunes empty channels.
        /// </summary>
        private void PruneChannels()
        {
            Channels.RemoveGroup(Channels.Where(channel => channel.Users.Count == 0).ForEach(channel => channel.Destroy()));
            Thread.Sleep((int)TimeSpan.FromMinutes(2).TotalMilliseconds);
            Task.Factory.StartNew(PruneChannels);
        }
    }
}
