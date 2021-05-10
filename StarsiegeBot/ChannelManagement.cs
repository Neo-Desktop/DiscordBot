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

        [GroupCommand]
        public async Task NothingHere (CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
        }


        [Group("allow")]
        class Allow
        {
            [Command("add")]
            public async Task AddAllowedChannels(CommandContext ctx, [Description("List of discord channels to allow the bot to respond on.")] params DiscordChannel[] discordChannels)
            {
                string gId = ctx.Guild.Id.ToString();
                // string output = "Added the following channel(s) to the allow list:";
                string output = "";
                foreach (DiscordChannel channel in discordChannels)
                {

                    GuildSettings[gId].AllowChannels.Add(channel.Id.ToString());
                    if (!GuildSettings[gId].AllowChannels.Contains(channel.Id.ToString()))
                    {
                        if (output == "")
                        {
                            output = channel.Mention;
                        }
                        else
                        {
                            output += $", {channel.Mention}";
                        }
                    }
                }

                if (output == "")
                {
                    output = "There was an error adding your channel(s) to the allow list.";
                }
                else
                {
                    output = "Added the following channel(s) to the allow list: " + output;
                }
                await ctx.RespondAsync(StartEmbed(output));
            }
            [Command("remove")]
            public async Task RemoveAllowedChannel(CommandContext ctx, params DiscordChannel[] discordChannels)
            {
                string success = "";
                string failed = "";
                string output = "";
                string gId = ctx.Guild.Id.ToString();
                foreach (DiscordChannel channel in discordChannels)
                {
                    string chanId = channel.Id.ToString();
                    if (GuildSettings[gId].AllowChannels.Remove(chanId))
                    {
                        if (success == "")
                        {
                            success = channel.Mention;
                        }
                        else
                        {
                            success += $", {channel.Mention}";
                        }
                    }
                    else
                    {
                        if (failed == "")
                        {
                            failed = channel.Mention;
                        }
                        else
                        {
                            failed += $", {channel.Mention}";
                        }
                    }
                }
                if (success != "")
                {
                    output += "Successfully removed from allowed channel(s): " + success;
                }
                if (failed != "")
                {
                    if (output != "") output += "\r\n";
                    output += "Failed to removed from allowed channel(s): " + failed;
                }
                await ctx.RespondAsync(StartEmbed(output));
            }
        }

        [Group("deny")]
        class Deny
        {
            [Command("add")]
            public async Task DenyChannels(CommandContext ctx, params DiscordChannel[] discordChannels)
            {
                foreach (DiscordChannel chan in discordChannels)
                {
                    Console.WriteLine(chan.Id.ToString());
                }
            }
        }




        private static DiscordEmbedBuilder StartEmbed(string desc)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Description = desc,
                Color = Program.colours[^2]
            };
            return embed;
        }

    }
}
