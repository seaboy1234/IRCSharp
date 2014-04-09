//    Project:     IRC# Server 
//    File:        IrcChannelMode.cs
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


namespace IRCSharp.Server
{
    /// <summary>
    /// Represents the MODE for an IrcChannel.
    /// </summary>
    public class IrcChannelMode
    {
        /// <summary>
        /// Gets or sets whether this channel is annonymous.  I.e. all users appear as annonymous!annonymous@annonymous.
        /// 
        /// Mode: +a
        /// </summary>
        public bool IsAnnonymous { get; set; }

        /// <summary>
        /// Gets or sets whether this channel is invite only.
        /// 
        /// Mode +i
        /// </summary>
        public bool IsInviteOnly { get; set; }

        /// <summary>
        /// Gets or sets whether this channel is voice only.
        /// 
        /// Mode: +m
        /// </summary>
        public bool IsVoiceOnly { get; set; }

        /// <summary>
        /// Gets or sets whether to receive messages from the outside.
        /// 
        /// Mode: +n
        /// </summary>
        public bool IsInsideOnly { get; set; }

        /// <summary>
        /// Gets or sets whether this channel is a quiet channel.
        /// 
        /// Mode: +q
        /// </summary>
        public bool IsQuiet { get; set; }

        /// <summary>
        /// Gets or sets whether only ChOps can set the topic.
        /// 
        /// Mode: +t
        /// </summary>
        public bool OpSetTopic { get; set; }

        /// <summary>
        /// Gets or sets whether this channel is private.
        /// 
        /// Mode: +p
        /// 
        /// <remarks>IsPrivate and IsSecret (+p and +s, respectively) MUST NOT be true at the same time as per RFC 2813.</remarks>
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Gets or sets whether this channel is secret.
        /// 
        /// Mode +s
        /// <remarks>IsPrivate and IsSecret (+p and +s, respectively) MUST NOT be true at the same time as per RFC 2813.</remarks>
        /// </summary>
        public bool IsSecret { get; set; }

        /// <summary>
        /// Gets or sets the UserLimit for this channel.
        /// 
        /// Mode: +l
        /// </summary>
        public int UserLimit { get; set; }

        /// <summary>
        /// Gets or sets this channel's key.
        /// 
        /// Mode: +k
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets this channel's topic.
        /// 
        /// Command: TOPIC #channel
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Gets this channel's BanMask.
        /// 
        /// Mode: +b
        /// </summary>
        public List<string> BanMask { get; private set; }

        /// <summary>
        /// Gets this channel's ban exception mask.
        /// 
        /// Mode: +e
        /// </summary>
        public List<string> ExceptionMask { get; private set; }

        /// <summary>
        /// Gets this channel's invite bypass mask,
        /// 
        /// Mode: +I
        /// </summary>
        public List<string> InviteMask { get; private set; }

        public IrcChannel Channel { get; private set; }

        public IrcChannelMode(IrcChannel channel)
        {
            Key = "";
            Topic = "IRC#";
            UserLimit = -1;
            BanMask = new List<string>();
            ExceptionMask = new List<string>();
            InviteMask = new List<string>();
            Channel = channel;
        }

        public void Parse(IIrcUser sender, IrcMessage message)
        {
            if (!sender.ChannelModes.ContainsKey(Channel))
            {
                sender.ChannelModes.Add(Channel, new IrcChannelUserMode());
            }
            if (message.Params.Length >= 3)
            {
                if (IRC.CheckString(message.Params[2], IRC.NICKNAME))
                {
                    IIrcUser user = sender.IrcServer.Clients.Where(client => client.Nick == message.Params[2]).FirstOrDefault();
                    if (user == null)
                    {
                        sender.Write(new IrcNumericResponce
                        {
                            NumericId = IrcNumericResponceId.ERR_NOSUCHNICK,
                            To = message.Params[2],
                            Message = "No such nick"
                        });
                        return;
                    }
                    ParseMember(sender, user, message.Params[1]);
                }
                else
                {
                    ParseAccessMode(sender, message.Params[1], message.Params[2]);
                }
            }
            else if(message.Params.Length == 2)
            {
                ParseChannelMode(sender, message.Params[1]);
            }
            else if (message.Params.Length == 1)
            {
                sender.Write(new IrcMessage
                {
                    Prefix = Channel.Name,
                    Command = "MODE",
                    Params = ToString().Split(' ')
                });
            }
        }

        public override string ToString()
        {
            string mode = "";
            mode += IsAnnonymous ? "a" : "";
            mode += IsInsideOnly ? "n" : "";
            mode += IsInviteOnly ? "i" : "";
            mode += IsPrivate ? "p" : "";
            mode += IsQuiet ? "q" : "";
            mode += IsSecret ? "s" : "";
            mode += IsVoiceOnly ? "v" : "";
            mode += Key != "" ? "k" : "";
            mode += UserLimit != -1 ? "l" : "";
            mode = string.IsNullOrEmpty(mode) ? "" + mode : "+";
            return mode;
        }

        private void ParseMember(IIrcUser sender, IIrcUser user, string mode)
        {
            bool setTrue = mode[0] == '+';
            if (!setTrue && mode[0] != '-')
            {
                return;
            }
            IrcChannelUserMode senderMode = sender.ChannelModes[Channel];
            if (!user.ChannelModes.ContainsKey(Channel))
            {
                user.ChannelModes.Add(Channel, new IrcChannelUserMode());
            }
            IrcChannelUserMode userMode = user.ChannelModes[Channel];
            bool isSelf = (sender == user && setTrue);
            string newMode = setTrue ? "+" : "-";
            foreach (char ch in mode)
            {
                if (ch == 'a' && (senderMode.IsAdmin || isSelf))
                {
                    userMode.IsAdmin = setTrue;
                    newMode += 'a';
                }
                if (ch == 'o' && (senderMode.IsOperator|| isSelf))
                {
                    userMode.IsOperator = setTrue;
                    newMode += 'o';
                }
                if (ch == 'h' && (senderMode.IsHalfOp || isSelf))
                {
                    userMode.IsHalfOp = setTrue;
                    newMode += 'h';
                }
                if (ch == 'v' && (senderMode.IsHalfOp || isSelf))
                {
                    userMode.IsVoiced = setTrue;
                    newMode += 'v';
                }
            }
            UpdateChannelMode(sender, user.Nick + " " + newMode);
        }

        private void ParseAccessMode(IIrcUser sender, string mode, string mask)
        {
            bool setTrue = mode[0] == '+';
            if (!setTrue && mode[0] != '-')
            {
                return;
            }
            IrcChannelUserMode senderMode = sender.ChannelModes[Channel];

            if (!senderMode.IsOperator)
            {
                return;
            }

            string[] masks = mask.Split(' ');
            Action<IEnumerable<string>> action = null;
            if (mode[1] == 'b')
            {
                action = setTrue ? BanMask.AddRange : (Action<IEnumerable<string>>)BanMask.RemoveGroup;
            }
            if (mode[1] == 'I')
            {
                action = setTrue ? InviteMask.AddRange : (Action<IEnumerable<string>>)InviteMask.RemoveGroup;
            }
            if (mode[1] == 'e')
            {
                action = setTrue ? ExceptionMask.AddRange : (Action<IEnumerable<string>>)ExceptionMask.RemoveGroup;
            }
            if (action == null)
            {
                return;
            }
            action(masks);
            UpdateChannelMode(sender, mode);
        }

        private void ParseChannelMode(IIrcUser sender, string mode)
        {
            bool setTrue = mode[0] == '+';
            if (!setTrue && mode[0] != '-')
            {
                return;
            }
            IrcChannelUserMode senderMode = sender.ChannelModes[Channel];
            string newMode = setTrue ? "+" : "-";

            foreach (char ch in mode)
            {
                if (ch == 'a' && senderMode.IsOperator)
                {
                    IsAnnonymous = setTrue;
                    newMode += "a";
                }
                if (ch == 'i' && senderMode.IsOperator)
                {
                    IsInviteOnly = setTrue;
                    newMode += "i";
                }
                if (ch == 'm' && senderMode.IsHalfOp)
                {
                    IsVoiceOnly = setTrue;
                    newMode += "m";
                }
                if (ch == 'q' && senderMode.IsAdmin)
                {
                    IsQuiet = setTrue;
                    newMode += "q";
                }
                // RFC 2811: The channel flags 'p' and 's' MUST NOT both be set at the same time.
                if (ch == 'p' && senderMode.IsAdmin && !IsSecret)
                {
                    IsPrivate = setTrue;
                    newMode += "p";
                }
                if (ch == 's' && senderMode.IsAdmin && !IsPrivate)
                {
                    IsSecret = setTrue;
                    newMode += "s";
                }
                // End RFC 2811.

                if (ch == 't' && senderMode.IsHalfOp)
                {
                    OpSetTopic = setTrue;
                    newMode += "t";
                }
            }
            UpdateChannelMode(sender, newMode);
        }

        private void UpdateChannelMode(IIrcUser sender, string mode)
        {
            if (mode.Length <= 1)
            {
                return;
            }
            Channel.Command(sender, "MODE", Channel.Name + " " + mode);
        }
    }
}
