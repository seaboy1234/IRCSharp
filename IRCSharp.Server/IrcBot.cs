//    Project:     IRC# Server 
//    File:        IrcBot.cs
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
    /// Represents a server-controlled IRCBot.  WIP
    /// </summary>
    public class IrcBot : IIrcUser
    {
        public string Nick { get; set; }
        public string User { get; set; }
        public string Host { get; set; }
        public string Name { get; set; }
        public DateTime Idle { get; set; }
        public IrcUserMode Mode { get; set; }
        public IrcServer IrcServer { get; set; }
        public List<IrcChannel> Channels { get; set; }
        public Dictionary<IrcChannel, IrcChannelUserMode> ChannelModes { get; set; }

        public string UserString
        {
            get { return string.Format("{0}!{1}@{2}", Nick, User, Host); }
        }

        public virtual void OnJoin(IrcChannel channel)
        {

        }

        public virtual void OnSpeak(IrcChannel channel, string message)
        {

        }

        public virtual void OnLeave(IrcChannel channel)
        {

        }

        public void JoinChannel(IrcChannel channel)
        {

        }

        public void SayOnChannel(IrcChannel channel)
        {

        }

        public void Quit(string reason)
        {

        }

        #region Layer 1
        public void Write(IrcMessage message)
        {
            
        }

        public void Write(IrcNumericResponce responce)
        {
            
        }
        #endregion
    }
}
