//    Project:     IRC# Server 
//    File:        IrcChannel.cs
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
using IRCSharp.Server.Commands;


namespace IRCSharp.Server
{
    /// <summary>
    /// Represents an IRC Channel.
    /// </summary>
    public class IrcChannel
    {
        /// <summary>
        /// Gets or sets this IrcChannel's name with prefix.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets a list of users on this IrcChannel.
        /// </summary>
        public List<IIrcUser> Users { get; private set; }

        /// <summary>
        /// Gets this IrcChannel's mode.
        /// </summary>
        public IrcChannelMode Mode { get; private set; }

        /// <summary>
        /// Gets the creation date and time of this channel.
        /// </summary>
        public DateTime Created { get; private set; }

        /// <summary>
        /// Gets this IrcChannel's IrcServer.
        /// </summary>
        public IrcServer IrcServer { get; private set; }

        public IrcChannel(IrcServer server)
        {
            Users = new List<IIrcUser>();
            Mode = new IrcChannelMode(this);
            Created = DateTime.Now;
            IrcServer = server;
        }

        /// <summary>
        /// Broadcasts an <see cref="IRCSharp.IrcMessage"/> to all users listening on this channel.
        /// </summary>
        /// <param name="sender">The IIrcUser that sent the message.</param>
        /// <param name="message">The IrcMessage</param>
        /// <param name="ignoreSender">Whether to ignore the sender when sending this message.</param>
        public void SendMessage(IIrcUser sender, IrcMessage message, bool ignoreSender = false)
        {
            Users.Where(user =>
            {
                if (ignoreSender)
                {
                    return user != sender;
                }
                return true;
            }).ForEach(user => user.Write(message));
        }

        /// <summary>
        /// Sends a PRIVMSG to this channel from a sender.
        /// </summary>
        /// <param name="sender">The IIrcUser that sent this message.</param>
        /// <param name="message">The string contents of the message.</param>
        public void SendMessage(IIrcUser sender, string message)
        {
            Command(sender, "PRIVMSG", Name + " :" + message, true);
        }

        /// <summary>
        /// Joins this IrcChannel, provided the key is correct.
        /// </summary>
        public void Join(IIrcUser client, string key = "")
        {
            if (Mode.Key != key)
            {
                return;
            }

            if (!client.ChannelModes.ContainsKey(this))
            {
                client.ChannelModes.Add(this, new IrcChannelUserMode());
            }
            IrcChannelUserMode mode = client.ChannelModes[this];
            if (Mode.IsInviteOnly)
            {
                // TODO: Invite only.
            }
            if (Users.Count == 0 && (Created - DateTime.Now).TotalSeconds < 5)
            {
                mode.IsCreator = true;
            }
            Users.Add(client);

            Command(client, "JOIN", Name);
            if (string.IsNullOrEmpty(Mode.Topic))
            {
                client.Write(new IrcNumericResponce
                {
                    NumericId = IrcNumericResponceId.RPL_NOTOPIC,
                    To = Name, 
                    Message = "No topic"
                });
            }
            else
            {
                client.Write(new IrcNumericResponce
                {
                    NumericId = IrcNumericResponceId.RPL_TOPIC,
                    To = Name, 
                    Message = Mode.Topic
                });
            }
            IrcCommand.Find("names").Run(client, new IrcMessage { Params = new string[]{ Name } });
        }

        /// <summary>
        /// Leaves this IrcChannel with a part message.
        /// </summary>
        public void Part(IIrcUser client, string message)
        {
            Users.Remove(client);
            Command(client, "PART", Name + " :" + message);
        }

        /// <summary>
        /// Sends a command to the users in this channel.
        /// </summary>
        /// <param name="sender">The IIrcUser who is sending the command.</param>
        /// <param name="command">The command being sent.</param>
        /// <param name="message">The contents of this command.</param>
        /// <param name="ignoreSender">Whether ignore the sender when sending this command.</param>
        public void Command(IIrcUser sender, string command, string message, bool ignoreSender = false)
        {
            IrcMessage ircMessage = new IrcMessage();
            ircMessage.Prefix = sender.Nick;
            ircMessage.Command = command;

            string paramaters = message.Split(':')[0].Trim();
            string trailing = "";
            if (message.Split(':').Length == 2)
            {
                trailing = message.Split(':')[1].Trim();
            }
            List<string> paramList = new List<string>();
            paramList.AddRange(paramaters.Split(' '));
            if (!string.IsNullOrEmpty(trailing))
            {
                paramList.Add(trailing);
            }

            ircMessage.Params = paramList.ToArray();
            SendMessage(sender, ircMessage, ignoreSender);
        }

        /// <summary>
        /// Updates a client's nickname.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="newNick"></param>
        public void Nick(IIrcUser client, string newNick)
        {
            Command(client, "NICK", newNick, true);
        }

        /// <summary>
        /// Destroys this IrcChannel.
        /// </summary>
        public void Destroy()
        {
            Users.Clear();
            IrcServer.Clients
                .Where(client => client.ChannelModes.ContainsKey(this))
                .ForEach(client => client.ChannelModes.Remove(this));
        }
    }
}
