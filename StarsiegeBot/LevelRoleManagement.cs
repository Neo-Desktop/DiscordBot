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
    [Group("LevelRole")]
    [RequireGuild, RequirePermissions(Permissions.ManageRoles)]
    class LevelRoleManagement : BotSettings
    {
        public LevelRoleManagement()
        {
            Console.WriteLine("Level Role Management Loaded.");
        }

        [Command("edit")]
        public async Task EditRole(CommandContext ctx, int oldLevel, int newLevel, DiscordRole role)
        {
            await ctx.TriggerTypingAsync();
        }

        [Command("edit")]
        public async Task EditRole(CommandContext ctx, int newLevel, [RemainingText] string role)
        {
            await ctx.TriggerTypingAsync();
        }

        [Command("remove")]
        public async Task RemoveRole(CommandContext ctx, DiscordRole role)
        {
            await ctx.TriggerTypingAsync();
        }

        [Command("remove")]
        public async Task RemoveRole(CommandContext ctx, [RemainingText] string roleName)
        {
            await ctx.TriggerTypingAsync();
        }

    }
}
