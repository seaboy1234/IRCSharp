using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRCSharp.Server.Commands
{
    public class InviteCommand : IrcCommand
    {
        public override string Name
        {
            get { return "invite"; }
        }

        // Syntax: INVITE <nick> <channel>
        public override void Run(IIrcUser client, IrcMessage message)
        {
            if (message.Params.Length != 2)
            {
                NeedMoreParamsError(client);
                return;
            }
            if (client.IrcServer.CheckNick(message.Params[0]))
            {
                NotFoundError(client, false);
                return;
            }
            if (!IRC.IsChannel(message.Params[1]) || !client.IrcServer.ChannelExists(message.Params[1]))
            {
                NotFoundError(client, true);
                return;
            }

            IrcChannel channel = client.IrcServer.GetChannel(message.Params[1]);
            IIrcUser user = client.IrcServer.Clients.Find(cli => cli.Nick == message.Params[0]);
            if (!user.ChannelModes.ContainsKey(channel))
            {
                user.ChannelModes.Add(channel, new IrcChannelUserMode());
            }
            IrcChannelUserMode mode = user.ChannelModes[channel];
            mode.IsInvited = true;
            channel.Command(client, "INVITE", user.Nick);
            client.Write(new IrcNumericResponce
            {
                NumericId = IrcNumericResponceId.RPL_INVITING,
                To = channel.Name,
                Extra = client.Nick,
                Message = "invited."
            });
        }
    }
}
