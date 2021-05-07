using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace StarsiegeBot
{
    [Group("channel")]
    [RequireGuild]
    [Description("Allows or Denies the bot from responding to commands on specific channels.\r\nA DENY will override an ALLOW. Unless DENY is set to ALL.")]
    class ChannelManagement : BotSettings
    {
        public ChannelManagement()
        {
            Console.WriteLine("Channel Management Module loaded.");
        }

        [Command("Allow")]
        public async Task AllowChannels(CommandContext ctx, [Description("List of discord channels to allow the bot to respond on.")]params DiscordChannel[] discordChannels)
        {

        }
        [Command("Allow")]
        public async Task AllowChannels(CommandContext ctx, string discordChannels = "")
        {

        }

        [Command("deny")]
        public async Task DenyChannels(CommandContext ctx, params DiscordChannel[] discordChannels)
        {
            foreach (DiscordChannel chan in discordChannels)
            {
                Console.WriteLine(chan.Id.ToString());
            }
        }
        [Command("deny")]
        public async Task DenyChannels(CommandContext ctx, [Description("List of discord channels to deny the bot to respond on.")] string discordChannels = "")
        {

        }
    }
}
