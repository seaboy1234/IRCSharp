using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public ServerConfig()
        {
            Host = "127.0.0.1";
        }

        public void Save()
        {
            XDocument document = new XDocument();
            XElement config = new XElement("IRCSharpServer", 
                new XElement("Host", Host));
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

            XElement config = document.Element("IRCSharpServer");
            Host = config.Element("Host").Value;
        }
    }
}
