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
        }

        [Command("add")]
        public async Task AddGroup(CommandContext ctx, [RemainingText] string groupName)
        {
            await ctx.TriggerTypingAsync();
        }

        // TODO: Delete all roles or assign new group?
        [Command("delete")]
        public async Task RemoveGroup(CommandContext ctx, [RemainingText] string groupName)
        {
            await ctx.TriggerTypingAsync();
        }

        [Command("edit")]
        public async Task EditGroup(CommandContext ctx, [RemainingText] string groupInfo)
        {
            await ctx.TriggerTypingAsync();
        }
    }
}
