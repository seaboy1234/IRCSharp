//    Project:     IRC# Server 
//    File:        Program.cs
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
using System.Net;
using System.Net.Sockets;

namespace IRCSharp.Server
{
    class Program
    {
        public static IrcServer Server { get; private set; }
        public static ServerConfig Config { get; private set; }

        static void Main(string[] args)
        {
            Console.Title = "IRC# Server";
            TcpListener listener = new TcpListener(IPAddress.Any, 6667);

            Server = new IrcServer();
            Config = new ServerConfig();

            Config.Load();
            Server.Hostname = Config.Host;

            listener.Start();
            while (true)
            {
                Socket client = listener.AcceptSocket();
                new IrcClient 
                { 
                    Socket = client, 
                    IrcServer = Server
                }.OnConnect();
            }
        }
    }
}
