//    Project:     IRC# Server 
//    File:        IrcStream.cs
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
using System.Net.Sockets;
using System.Text;

namespace IRCSharp
{
    public class IrcStream
    {
        private NetworkStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;

        /// <summary>
        /// Gets or sets whether to autoflush on write.
        /// </summary>
        public bool AutoFlush 
        { 
            get { return _writer.AutoFlush; } 
            set { _writer.AutoFlush = value; } 
        }

        public IrcStream(Socket socket)
        {
            _stream = new NetworkStream(socket);
            _reader = new StreamReader(_stream);
            _writer = new StreamWriter(_stream);
        }

        /// <summary>
        /// Writes an <see cref="IRCSharp.IrcMessage"/> to the stream.
        /// </summary>
        public void Write(IrcMessage message)
        {
            _writer.WriteLine(message);
        }

        /// <summary>
        /// Writes an <see cref="IRCSharp.IrcNumericResponce"/> to the stream.
        /// </summary>
        public void Write(IrcNumericResponce responce)
        {
            _writer.WriteLine(responce);
        }

        /// <summary>
        /// Reads an <see cref="IRCSharp.IrcMessage"/> from the stream.
        /// </summary>
        /// <returns></returns>
        public IrcMessage ReadMessage()
        {
            IrcMessage message = new IrcMessage();
            string line = _reader.ReadLine();
            message.Parse(line);
            return message;
        }

        /// <summary>
        /// Reads an IrcNumericResponce from the stream.
        /// </summary>
        /// <returns></returns>
        public IrcNumericResponce ReadResponce()
        {
            IrcNumericResponce responce = new IrcNumericResponce();
            string line = _reader.ReadLine();
            responce.Parse(line);
            return responce;
        }
    }
}
