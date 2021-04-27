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
    [Group("Level")]
    [RequireGuild, RequirePermissions(Permissions.ManageRoles)]
    class LevelManagement : BotSettings
    {
        public LevelManagement()
        {
            Console.WriteLine("Level Management Loaded.");
        }

        [GroupCommand]
        public async Task ListLevels(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
        }

        [Command("AddLevel"), Aliases("al")]
        public async Task AddLevel(CommandContext ctx, int newLevelGroup)
        {
            await ctx.TriggerTypingAsync();
        }


        [Command("DeleteLevel"), Aliases("dl")]
        public async Task DeleteLevel(CommandContext ctx, int oldLevelGroup)
        {
            await ctx.TriggerTypingAsync();
        }

        [Command("edit")]
        public async Task EditLevel (CommandContext ctx, int oldLeve, int newLevel)
        {
            await ctx.TriggerTypingAsync();
        }
    }
}
