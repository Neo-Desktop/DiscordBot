#pragma warning disable IDE0060 // Remove unused parameter
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
    [Group("Group")]
    [RequireGuild, RequirePermissions(Permissions.ManageRoles)]
    class GroupManagement : BotSettings
    {
        public GroupManagement()
        {
            Console.WriteLine("Group Management Loaded.");
        }

        [GroupCommand]
        public async Task ListGroups(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            string gId = ctx.Guild.Id.ToString();
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

            embed.WithTitle($"{ctx.Guild.Name} Groups");
            embed.WithDescription("An overview of all role groups.");
            foreach (KeyValuePair<string, Dictionary<string, int>> item in GuildSettings[gId].SelfRoles)
            {
                embed.AddField(item.Key, item.Value.Count.ToString());
            }
            await ctx.RespondAsync(embed);
        }

        [Command("add")]
        public async Task AddGroup(CommandContext ctx, [RemainingText] string groupName)
        {
            await ctx.TriggerTypingAsync();
            string gId = ctx.Guild.Id.ToString();
            if (GuildSettings[gId].SelfRoles.ContainsKey(groupName))
            {
                await ctx.RespondAsync($"The group `{groupName}` already exists.");
            }
            else
            {
                var temp = new Dictionary<string, int>();
                GuildSettings[gId].SelfRoles.Add(groupName, temp);
                await ctx.RespondAsync($"The group `{groupName}` was added.");
            }
        }

        // TODO: Delete all roles or assign new group?
        [Command("delete")]
        public async Task RemoveGroup(CommandContext ctx, [RemainingText] string groupName)
        {
            await ctx.TriggerTypingAsync();
            string gId = ctx.Guild.Id.ToString();
            if (GuildSettings[gId].SelfRoles.ContainsKey(groupName))
            {
                GuildSettings[gId].SelfRoles.Remove(groupName);
                await ctx.RespondAsync($"Group `{groupName}` removed.");
            }
            else
            {
                await ctx.RespondAsync($"The group `{groupName}` doesn't exist.");
            }
        }

        [Command("edit")]
        public async Task EditGroup(CommandContext ctx, [RemainingText] string groupInfo)
        {
            await ctx.TriggerTypingAsync();
            // string gId = ctx.Guild.Id.ToString();

        }
    }
}
