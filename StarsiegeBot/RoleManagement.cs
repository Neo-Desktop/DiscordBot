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
    [Group("role")]
    [RequireGuild,RequirePermissions(Permissions.ManageRoles)]
    class RoleManagement : BotSettings
    {
        public RoleManagement()
        {
            Console.WriteLine("Role Management Loaded.");
        }

        [Command("join")]
        public async Task JoinSelfRole(CommandContext ctx, DiscordRole role)
        {
            await ctx.TriggerTypingAsync();
        }

        [Command("join")]
        public async Task JoinSelfRole(CommandContext ctx, [RemainingText] string role)
        {
            await ctx.TriggerTypingAsync();
        }

        [Command("leave")]
        public async Task LeaveSelfRole(CommandContext ctx, DiscordRole role)
        {
            await ctx.TriggerTypingAsync();
        }

        [Command("leave")]
        public async Task LeaveSelfRole(CommandContext ctx, [RemainingText] string role)
        {
            await ctx.TriggerTypingAsync();
        }

        private bool VerifiedRole(DiscordGuild guild, DiscordRole role)
        {
            return false;
        }
        private bool VerifiedRole(DiscordGuild guild, string role)
        {
            return false;
        }
    }
}
