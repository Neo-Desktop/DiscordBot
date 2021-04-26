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
    [Group("GroupRole")]
    class GroupRoleManagement : BotSettings
    {
        public GroupRoleManagement()
        {
            Console.WriteLine("Group Role Management Loaded.");
        }

        [GroupCommand]
        public async Task ListGroups(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
        }

        [Command("add")]
        public async Task AddSelfRole(CommandContext ctx, DiscordRole role, int price = 0, string group = null)
        {
            await ctx.TriggerTypingAsync();
            if (price == -1)
            {
                return;
            }
        }

        [Command("delete")]
        public async Task DeleteSelfRole(CommandContext ctx, DiscordRole role, string group = null)
        {
            await ctx.TriggerTypingAsync();
        }

        [Command("Edit")]
        public async Task EditRole(CommandContext ctx, DiscordRole role, int newPrice = 0, [RemainingText] string newGroup = null)
        {
            await ctx.TriggerTypingAsync();
        }
    }
}
