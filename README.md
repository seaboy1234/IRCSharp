IRC♯ Server
========
*An IRC Server In C#*

IRC# Server is an Internet Relay Chat Server written in C#.  At first, I made this as a school project, and have decided to continue this as an open-source project.

## The IRC Protocol ##

IRC, or Internet Relay Chat is an 8-bit protocol that uses the Transmission Control Protocol.  All messages are separated by a `CR-LF` or `\r\n`.

A typical message over IRC looks something like  
`:irc.example.net 001 john :Welcome to the IRC Network!`  
 or  
`:john PRIVMSG #example :Hello, World!`.

Messages sent from the client to the server will tend to look something like `PRIVMSG #example :Hello, World!`. The first `:` marks a prefix to the command, such as a nickname or a server.  The last `:` marks the last parameter if it contains spaces.  

Using this, let's break down the example commands.  
`:irc.example.net 001 john :Welcome to the IRC Network!`  

The prefix is `irc.example.net`.  
The command is `001`.  
The first parameter is `john`, and the last is `Welcome to the IRC Network!`

`PRIVMSG #example :Hello, World!`.  
There is no prefix.  
The command is `PRIVMSG`.  
The first parameter is `#example` and the last is `Hello, World!`.

`PRIVMSG #example Hello, World!`  
The last parameter is **`World!`** because there is no `:` before the **`Hello`**.
## Contributing ##
Fork the project and begin coding.  
You should read [RFC 2810](https://tools.ietf.org/html/rfc2810), [RFC 2811](https://tools.ietf.org/html/rfc2811), [RFC 2812](https://tools.ietf.org/html/rfc2812), and [RFC 2813](https://tools.ietf.org/html/rfc2813).  You may also want to read the [IRCv3 docs](http://ircv3.org/).