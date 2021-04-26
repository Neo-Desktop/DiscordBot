using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.EventArgs;
using DSharpPlus.VoiceNext.Codec;
using Microsoft.Extensions.Logging;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using System.Threading;

namespace StarsiegeBot
{
    [Group("prefix")]
    class PrefixManagement : BotSettings
    {
        public PrefixManagement()
        {
            Console.WriteLine("Prefix Management Loaded.");
        }
        [GroupCommand]
        public async Task GetPrefixes(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            // store the Guild ID as a string.
            string guild = ctx.Guild.Id.ToString();
            // if this server doesnt have a settings for whatever reason, let them know... why am i not creating it?
            if (!GuildSettings.ContainsKey(guild))
            {
                await ctx.RespondAsync("No guild settings found.");
            }
            else
            {
                // we have server settings... why does this give so much info? it should only show only prefix(es)
                var server = GuildSettings[guild];
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                {
                    Description = $"{ctx.Guild.Name} Prefixes."
                };

                int count = 1;
                // for each prefix, list it in the embed as a field. Show a count.
                foreach (string item in server.Prefixes)
                {
                    embed.AddField($"Prefix {count}", item, true);
                    count++;
                }

                await ctx.RespondAsync(embed);
            }
        }
        [Command("Add")]
        [Description("Adds a new prefix to the Server's list of prefixes.")]
        public async Task AddNewPrefix(CommandContext ctx, string prefix)
        {
            await ctx.TriggerTypingAsync();
            string gId = ctx.Guild.Id.ToString();
            // add the prefix to the server specific settings.
            if (GuildSettings[gId].Prefixes.Count < 25)
            {
                GuildSettings[gId].Prefixes.Add(prefix);
                await ctx.RespondAsync($"Added `{prefix}` as a command prefix.");
            }
            else
            {
                await ctx.RespondAsync("There is a maximum of 25 prefixes allowed.");
            }
        }

        [Command("delete"), Aliases("del")]
        [Description("Removes the specified prefix from accepted prefixes.")]
        public async Task RemovePrefix(CommandContext ctx, string prefix)
        {
            await ctx.TriggerTypingAsync();
            string gId = ctx.Guild.Id.ToString();
            // they have only one prefix and disallow global prefixes, dont let them delete the prefix.
            if (GuildSettings[gId].Prefixes.Count == 1 && !GuildSettings[gId].UseGlobalPrefix)
            {
                await ctx.RespondAsync("Can't remove the last usable prefix from the bot. Add a new prefix, or enable Global Prefix.");
                return;
            }
            // they pass the vibe check, delete the prefix.
            GuildSettings[ctx.Guild.Id.ToString()].Prefixes.Remove(prefix);
            await ctx.RespondAsync($"Removed `{prefix}` as a command prefix.");
        }

        [Command("global")]
        [Description("View or edit the status of allowing Global Prefix(es).")]
        public async Task UseGlobalPrefixes(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync($"Global Prefix usage has been turned **{(GuildSettings[ctx.Guild.Id.ToString()].UseGlobalPrefix ? "on" : "off")}**");
        }
        [Command("global"), Aliases("g")]
        [Description("View or edit the status of allowing Global Prefix(es).")]
        public async Task UseGlobalPrefixes(CommandContext ctx, [Description("True or False")] bool isEnabled)
        {
            await ctx.TriggerTypingAsync();
            // check to make sure that they aren't trying to turn off global prefixes with out first adding a server sepcific prefix.
            int prefixCount = GuildSettings[ctx.Guild.Id.ToString()].Prefixes.Count;
            if (prefixCount <= 0 && !isEnabled)
            {
                await ctx.RespondAsync("You have to add at least one server prefix before you can turn off Global Prefux usage.");
                return;
            }
            // they passed teh vibe check, let them turn off global prefixes.
            GuildSettings[ctx.Guild.Id.ToString()].UseGlobalPrefix = isEnabled;
            await ctx.RespondAsync($"Global Prefix usage has been turned **{(isEnabled ? "on" : "off")}**");
        }
        [Command("global")]
        public async Task UseGlobalPrefixes(CommandContext ctx, [Description("0 is off, 1 is on.")] int isEnabled)
        {
            // since we're just allowing the user to use different ways to turn this function on and off... Redirect the work load there.
            if (isEnabled >= 1)
            {
                await UseGlobalPrefixes(ctx, true);
            }
            else
            {
                await UseGlobalPrefixes(ctx, false);
            }
        }
        [Command("global")]
        public async Task UseGlobalPrefixes(CommandContext ctx, [Description("ON or OFF")] string isEnabled)
        {
            // since we're just trying to make the end user's life easier by different means of turning this on/off. redirect work load that way.
            if (isEnabled.ToLower() == "on")
            {
                await UseGlobalPrefixes(ctx, true);
            }
            else if (isEnabled.ToLower() == "off")
            {
                await UseGlobalPrefixes(ctx, false);
            }
            else
            {
                // they wanna be smart, and we don't wanna, tell them that.
                await ctx.RespondAsync("Please specify ON or OFF");
            }
        }
    }
}
