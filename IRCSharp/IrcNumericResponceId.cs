//    Project:     IRC# Server 
//    File:        IrcNumericResponceId.cs
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

namespace IRCSharp
{
    public enum IrcNumericResponceId
    {
        /// <summary>
        /// The first message sent after client registration. The text used varies widely.
        /// </summary>
        RPL_WELCOME = 1,
        /// <summary>
        /// Part of the post-registration greeting. Text varies widely.
        /// </summary>
        RPL_YOURHOST = 2,
        /// <summary>
        /// Part of the post-registration greeting. Text varies widely.
        /// </summary>
        RPL_CREATED = 3,
        /// <summary>
        /// Part of the post-registration greeting. Text varies widely.
        /// </summary>
        RPL_MYINFO = 4,
        [Obsolete("Obsolete")]
        RPL_BOUNCE = 5,
        RPL_ISUPPORT = 5,

        RPL_USERHOST = 302,
        RPL_ISON = 303,
        RPL_AWAY = 301,
        RPL_UNAWAY = 305,
        RPL_NOWAWAY = 306,
        RPL_WHOISUSER = 311,
        RPL_WHOISSERVER = 312,
        RPL_WHOISOPERATOR = 313,
        RPL_WHOISIDLE = 317,
        RPL_ENDOFWHOIS = 318,
        RPL_WHOISCHANNELS = 319,
        RPL_WHOWASUSER = 314,
        RPL_ENDOFWHOWAS = 369,
        [Obsolete("Obsolete. Not used.")]
        RPL_LISTSTART = 321,
        RPL_LIST = 322,
        RPL_LISTEND = 323,
        RPL_UNIQOPIS = 325,
        RPL_CHANNELMODIS = 324,
        RPL_NOTOPIC = 331,
        RPL_TOPIC = 332,
        RPL_INVITING = 341,
        [Obsolete("Obsolete. Not used.")]
        RPL_SUMMONING = 342,
        RPL_INVITELIST = 346,
        RPL_ENDOFINVITELIST = 347,
        RPL_EXCEPTLIST = 348,
        RPL_ENDOFEXCEPTLIST = 349,
        RPL_VERSION = 351,
        RPL_WHOREPLY = 352,
        RPL_ENDOFWHO = 315,
        RPL_NAMEREPLY = 353,
        RPL_ENDOFNAMES = 366,
        RPL_LINKS = 364,
        RPL_ENDOFLINKS = 365,
        RPL_BANLIST = 367,
        RPL_ENDOFBANLIST = 368,
        RPL_INFO = 371,
        RPL_ENDOFINFO = 374,
        RPL_MOTDSTART = 375,
        RPL_MOTD = 372,
        RPL_ENDOFMOTD = 376,
        RPL_YOUREOPER = 381,
        RPL_REHASHING = 382,
        RPL_YOURESERVICE = 383,
        RPL_TIME = 391,
        RPL_USERSSTART = 392,
        RPL_USERS = 393,
        RPL_ENDOFUSERS = 394,
        RPL_NOUSERS = 395,

        #region IRCv3
        RPL_LOGGEDIN = 900,
        RPL_LOGGEDOUT = 901,
        RPL_SASLSUCCESS = 903,
        RPL_SASLMECHS = 908,

        ERR_NICKLOCKED = 902,
        ERR_SASLFAIL = 904,
        ERR_SASLTOOLONG = 905,
        ERR_SASLABORTED = 906,
        ERR_SASLALREADY = 907,
        #endregion

        RPL_TRACELINK = 200,
        RPL_TRACECONNECTING = 201,
        RPL_TRACEHANDSHAKE = 202,
        RPL_TRACEUNKNOWN = 203,
        RPL_TRACEOPERATOR = 204,
        RPL_TRACESERVER = 206,
        RPL_TRACESERVICE = 207,
        RPL_TRACENEWTYPE = 208,
        RPL_TRACECLASS = 209,
        [Obsolete("Unused")]
        RPL_TRACECONNECT = 210,
        RPL_TRACELOG = 261,
        RPL_TRACEEND = 262,
        RPL_STATSLINKINFO = 211,
        RPL_STATSCOMMANDS = 212,
        RPL_ENDOFSTATS = 219,
        RPL_STATUSUPTIME = 242,
        RPL_STATSOLINE = 243,
        RPL_UMODEIS = 221,
        RPL_SERVLIST = 234,
        RPL_SERVLISTEND = 235,
        RPL_LUSERCLIENT = 251,
        RPL_LUSEROP = 252,
        RPL_LUSERUNKNOWN = 253,
        RPL_LUSERCHANNELS = 254,
        RPL_LUSERME = 255,
        RPL_ADMINME = 256,
        RPL_ADMINLOC1 = 257,
        RPL_ADMINLOC2 = 258,
        RPL_ADMINEMAIL = 259,
        RPL_TRYAGAIN = 263,

        ERR_NOSUCHNICK = 401,
        ERR_NOSUCHSERVER = 402,
        ERR_NOSUCHCHANNEL = 403,
        ERR_CANNOTSENDTOCHAN = 404,
        ERR_TOOMANYCHANNELS = 405,
        ERR_WASNOSUCHNICK = 406,
        ERR_NOSUCHSERVICE = 408,
        ERR_NOORIGN = 409,
        ERR_NORECIPIENT = 411,
        ERR_NOTEXTTOSEND = 412,
        ERR_NOTOPLEVEL = 413,
        ERR_WILDTOPLEVEL = 414,
        ERR_BADMASK = 415,
        ERR_UNKNOWNCOMMAND = 421,
        ERR_NOMOTD = 422,
        ERR_NOADMININFO = 423,
        ERR_FILEERROR = 424,
        ERR_NONICKNAMEGIVEN = 431,
        ERR_ERRONEUSNICKNAME = 432,
        ERR_NICKNAMEINUSE = 433,
        ERR_NICKCOLLISION = 436,
        ERR_UNABAILRESOURCE = 437,
        ERR_USERNOTINCHANNEL = 441,
        ERR_NOTONCHANNEL = 442,
        ERR_USERONCHANNEL = 443,
        ERR_NOLOGIN = 444,
        ERR_SUMMONDISABLED = 445,
        ERR_USERSDISABLED = 446,
        ERR_NOTREGISTERED = 451,
        ERR_NEEDMOREPARAMS = 461,
        ERR_ALREADYREGISTERED = 462,
        ERR_NOPERMFORHOST = 463,
        ERR_YOUREBANNEDCREEP = 465,
        [Obsolete("Obsolete. Not used.")]
        ERR_YOUWILLBEBANNED = 466,
        ERR_KEYSET = 467,
        ERR_CHANNELISFULL = 471,
        ERR_UNKNOWNMODE = 472,
        ERR_INVITEONLYCHAN = 473,
        ERR_BANNEDFROMCHAN = 474,
        ERR_BADCHANNELKEY = 475,
        ERR_BANCHANMASK = 476,
        ERR_NOCHANMODES = 477,
        ERR_BANLISTISFULL = 478,
        ERR_NOPRIVLEGES = 481,
        ERR_CHANOPRIVSNEEDED = 482,
        ERR_CANTKILLSERVER = 483,
        ERR_RESTRICTED = 484,
        ERR_UNIQOPRIVSNEEDED = 485,
        ERR_NOOPERHOST = 491,
        ERR_UMODEUNKNOWNFLAG = 501,
        ERR_USERSDONTMATCH = 502
    }
}
