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
            if (message.Params.Length >=2)
            {
                if (message[1].Contains('b') ||
                    message[1].Contains('I') ||
                    message[1].Contains('e'))
                {
                    ParseAccessMode(sender, message[1], message.Params.Length == 3 ? message[2] : "");
                    return;
                }
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
            else if(message.Params.Length == 2 && !string.IsNullOrEmpty(message.Params[1]))
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
            mode += OpSetTopic ? "t" : "";
            mode += IsSecret ? "s" : "";
            mode += IsVoiceOnly ? "v" : "";
            mode += Key != "" ? "k" : "";
            mode += UserLimit != -1 ? "l" : "";
            mode = string.IsNullOrEmpty(mode) ? "" : "+" + mode;
            return mode;
        }

        private void ParseMember(IIrcUser sender, IIrcUser user, string mode)
        {
            bool setTrue = mode[0] == '+';
            if (!setTrue && mode[0] != '-')
            {
                return;
            }
            IrcChannelUserMode senderMode = Channel.GetUserMode(sender);
            IrcChannelUserMode userMode = Channel.GetUserMode(user);
            bool isSelf = (sender == user && !setTrue);
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
            UpdateChannelMode(sender, newMode + " " + user.Nick);
        }

        private void ParseAccessMode(IIrcUser sender, string mode, string mask)
        {
            bool setTrue = mode[0] == '+';
            if (!setTrue && mode[0] != '-')
            {
                return;
            }

            if (string.IsNullOrEmpty(mask))
            {
                if (!setTrue)
                {
                    return;
                }
                List<string> list = null;
                IrcNumericResponceId numeric;
                IrcNumericResponceId endNumeric;
                switch (mode[1])
                {
                    case 'b':
                        list = BanMask;
                        numeric = IrcNumericResponceId.RPL_BANLIST;
                        endNumeric = IrcNumericResponceId.RPL_ENDOFBANLIST;
                        break;
                    case 'I':
                        list = InviteMask;
                        numeric = IrcNumericResponceId.RPL_INVITELIST;
                        endNumeric = IrcNumericResponceId.RPL_ENDOFINVITELIST;
                        break;
                    case 'e':
                        list = ExceptionMask;
                        numeric = IrcNumericResponceId.RPL_EXCEPTLIST;
                        endNumeric = IrcNumericResponceId.RPL_ENDOFEXCEPTLIST;
                        break;
                    default:
                        return;
                }
                string items = string.Join(" ", list);
                sender.Write(new IrcNumericResponce
                {
                    NumericId = numeric,
                    To = Channel.Name,
                    Extra = items
                });
                sender.Write(new IrcNumericResponce
                {
                    NumericId = endNumeric,
                    To = Channel.Name,
                    Message = "End of list"
                });
                return;
            }

            IrcChannelUserMode senderMode = Channel.GetUserMode(sender);

            if (!senderMode.IsOperator)
            {
                return;
            }

            List<string> masks = mask.Split(' ').ToList();
            Action<IEnumerable<string>> action = null;
            if (mode[1] == 'b')
            {
                if (setTrue)
                {
                    masks.RemoveGroup(BanMask);
                }
                action = setTrue ? BanMask.AddRange : (Action<IEnumerable<string>>)BanMask.RemoveGroup;
            }
            if (mode[1] == 'I')
            {
                if (setTrue)
                {
                    masks.RemoveGroup(InviteMask);
                }
                action = setTrue ? InviteMask.AddRange : (Action<IEnumerable<string>>)InviteMask.RemoveGroup;
            }
            if (mode[1] == 'e')
            {
                if (setTrue)
                {
                    masks.RemoveGroup(ExceptionMask);
                }
                action = setTrue ? ExceptionMask.AddRange : (Action<IEnumerable<string>>)ExceptionMask.RemoveGroup;
            }
            if (action == null)
            {
                return;
            }
            if (masks.Count == 0)
            {
                return;
            }
            action(masks);
            UpdateChannelMode(sender, mode + " " + string.Join(" ", masks));
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
            if (mode.Split(' ')[0].Length <= 1)
            {
                return;
            }
            Channel.Command(sender, "MODE", Channel.Name + " " + mode);
        }
    }
}
