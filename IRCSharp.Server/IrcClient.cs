//    Project:     IRC# Server 
//    File:        IrcClient.cs
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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using IRCSharp.Server.Commands;


namespace IRCSharp.Server
{
    /// <summary>
    /// Represents a remote client on the IrcServer.
    /// </summary>
    public class IrcClient : IIrcUser
    {
        private Thread _network;

        public Socket Socket { get; set; }

        public string Nick { get; set; }
        public string User { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }

        public DateTime Idle { get; set; }

        public List<IrcChannel> Channels { get; set; }
        public Dictionary<IrcChannel, IrcChannelUserMode> ChannelModes { get; set; }

        public IrcUserMode Mode { get; set; }
        public IrcServer IrcServer { get; set; }
        public IrcStream Stream { get; set; }

        public string UserString
        {
            get { return string.Format("{0}!{1}@{2}", Nick, User, Host); }
        }

        #region Layer 1
        public void OnConnect()
        {
            Stream = new IrcStream(Socket) { AutoFlush = true };            
            Channels = new List<IrcChannel>();
            Mode = new IrcUserMode();
            ChannelModes = new Dictionary<IrcChannel, IrcChannelUserMode>();
            Idle = DateTime.Now;

            IrcServer.NewClient(this);
            Write(new IrcMessage()
            {
                Prefix = IrcServer.Hostname,
                Command = "NOTICE",
                Params = "AUTH,***Looking up your hostname.".Split(',')
            });
            Task.Factory.StartNew(() =>
            {
                System.Net.IPHostEntry ip = System.Net.Dns.GetHostEntry(((IPEndPoint)Socket.RemoteEndPoint).Address);
                Host = ip.HostName;
                Write(new IrcMessage()
                {
                    Prefix = IrcServer.Hostname,
                    Command = "NOTICE",
                    Params = "AUTH,***Found your hostname.".Split(',')
                });
            });
            Task.Factory.StartNew(() =>
            {
                Write(new IrcMessage()
                {
                    Prefix = IrcServer.Hostname,
                    Command = "NOTICE",
                    Params = "AUTH,***CHECKING IDENT".Split(',')
                });
                try
                {
                    TcpClient tcp = new TcpClient();
                    tcp.Connect(new IPEndPoint(((IPEndPoint)Socket.RemoteEndPoint).Address, 113));
                    var writer = new StreamWriter(tcp.GetStream());
                    var reader = new StreamReader(tcp.GetStream());
                    writer.WriteLine("{0}, {1}", ((IPEndPoint)Socket.RemoteEndPoint).Port, 6667);
                    writer.Flush();
                    if (reader.ReadLine().Contains("USERID"))
                    {
                        Write(new IrcMessage()
                        {
                            Prefix = IrcServer.Hostname,
                            Command = "NOTICE",
                            Params = "AUTH,***IDENT SUCCESSFUL".Split(',')
                        });
                    }
                }
                catch
                {
                    Write(new IrcMessage()
                    {
                        Prefix = IrcServer.Hostname,
                        Command = "NOTICE",
                        Params = "AUTH,***IDENT FAILED.  CONTINUE.".Split(',')
                    });
                }
            });
            _network = new Thread(Read) { IsBackground = true };
            _network.Start();
        }

        public void Quit(string reason)
        {
            _network.Join();
            Socket.Close();
            IrcServer.UserLeft(this, reason);
        }

        public void Write(IrcMessage message)
        {
            Stream.Write(message);
        }
        public void Write(IrcNumericResponce responce)
        {
            if (responce.Host != IrcServer.Hostname)
            {
                responce.Host = IrcServer.Hostname;
            }
            if (string.IsNullOrEmpty(responce.To))
            {
                responce.To = Nick;
            }
            Stream.Write(responce);
        }

        public void Read()
        {
            while (true)
            {
                IrcMessage message;
                try
                {
                    message = Stream.ReadMessage();
                }
                catch (Exception)
                {
                    IrcServer.UserLeft(this, "connection reset by peer");
                    Socket.Close();
                    return;
                }
                Idle = DateTime.Now;
                if (message.Command == null)
                {
                    continue;
                }
                string line = message.ToString();

                IrcCommand command = IrcCommand.Find(message.Command);
                if (command == null)
                {
                    Console.WriteLine(line);
                    continue;
                }
                if (command.RequireRegistered && !Mode.IsRegistered)
                {
                    Write(new IrcNumericResponce
                    {
                        NumericId = IrcNumericResponceId.ERR_NOTREGISTERED,
                        Message = "You're not registered.",
                        Extra = command.Name.ToUpper()
                    });
                }

                command.Run(this, message);
            }
        }
        #endregion
    }
}
