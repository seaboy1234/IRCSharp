//    Project:     IRC# Server 
//    File:        ServerConfig.cs
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
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace IRCSharp.Server
{
    public class ServerConfig
    {
        /// <summary>
        /// Gets or sets the Host element.
        /// </summary>
        public string Host { get; set; }

        public short[] Ports { get; set; }
        public IPAddress[] Addresses { get; set; }

        public ServerConfig()
        {
            Host = "127.0.0.1";
            Ports = new short[] { 6667 };
            Addresses = new IPAddress[] { IPAddress.Any };
        }

        public void Save()
        {
            XDocument document = new XDocument();
            XElement config = new XElement("IRCSharpServer", 
                new XElement("Host", Host));
            XElement listenOn = new XElement("ListenOn");
            for (int i = 0; i < Ports.Length; i++ )
            {
                XElement element = new XElement("Server", 
                    new XAttribute("IPAddress", Addresses[i].ToString()), 
                    new XAttribute("Port", Ports[i].ToString()));
                listenOn.Add(element);
            }
            config.Add(listenOn);
            document.Add(config);
            document.Save("IRCSharp.xml");
        }

        public void Load()
        {
            XDocument document;
            if (!File.Exists("IRCSharp.xml"))
            {
                Save();
            }
            document = XDocument.Load("IRCSharp.xml");

            try
            {
                XElement config = document.Element("IRCSharpServer");
                XElement listenOn = config.Element("ListenOn");
                IEnumerable<XElement> servers = listenOn.Elements("Server");
                
                Host = config.Element("Host").Value;
                Ports = new short[servers.Count()];
                Addresses = new IPAddress[servers.Count()];

                for (int i = 0; i < servers.Count(); i++)
                {
                    Addresses[i] = IPAddress.Parse(servers.ElementAt(i).Attribute("IPAddress").Value);
                    Ports[i] = short.Parse(servers.ElementAt(i).Attribute("Port").Value);
                }
            }
            catch (NullReferenceException)
            {
                Save();
                Load();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
