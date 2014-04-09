//    Project:     IRC# Server 
//    File:        IIrcUser.cs
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
    /// Represents a user on an IrcServer.
    /// </summary>
    public interface IIrcUser
    {
        /// <summary>
        /// Gets or sets this user's Nickname.
        /// </summary>
        string Nick { get; set; }

        /// <summary>
        /// Gets or sets this user's user component (nick!user@host :name)
        /// </summary>
        string User { get; set; }

        /// <summary>
        /// Gets or sets this user's host component (nick!user@host :name)
        /// </summary>
        string Host { get; set; }

        /// <summary>
        /// Gets or sets this user's real name (nick!user@host :name)
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the DateTime of this user's last action.
        /// </summary>
        DateTime Idle { get; set; }

        /// <summary>
        /// Gets this user's <see cref="IRCSharp.Server.IrcUserMode"/>
        /// </summary>
        IrcUserMode Mode { get; }

        /// <summary>
        /// Gets this user's <see cref="IRCSharp.Server.IrcServer"/>
        /// </summary>
        IrcServer IrcServer { get; }

        /// <summary>
        /// Gets a list of IrcChannels this user belongs to.
        /// </summary>
        List<IrcChannel> Channels { get; }

        /// <summary>
        /// Gets a Dictionary of IrcChannels and IrcChannelUserModes for this user.
        /// 
        /// <remarks>
        /// While this collection *should* have all channels this user is connected to, it MAY have some that the user is not.  
        /// For only the channels this user belongs to, use <see cref="IIrcUser.Channels"/>.
        /// </remarks>
        /// </summary>
        Dictionary<IrcChannel, IrcChannelUserMode> ChannelModes { get; set; }

        /// <summary>
        /// Gets this user's full (nick!user@host) Userstring.
        /// </summary>
        string UserString { get; }

        /// <summary>
        /// Writes an <see cref="IRCSharp.IrcMessage"/> to this user's stream.
        /// </summary>
        /// <param name="message"></param>
        void Write(IrcMessage message);

        /// <summary>
        /// Writes an <see cref="IRCSharp.IrcNumericResponce"/> to this user's stream.
        /// </summary>
        /// <param name="responce"></param>
        void Write(IrcNumericResponce responce);

        /// <summary>
        /// Makes this user QUIT the server.
        /// </summary>
        /// <param name="reason">The reason for quitting</param>
        void Quit(string reason);
    }
}
