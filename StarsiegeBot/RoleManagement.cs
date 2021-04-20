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
    class RoleManagement : BotSettings
    {
        public RoleManagement()
        {

        }

        [Command("join")]
        public async Task JoinSelfRole(CommandContext ctx, DiscordRole role)
        {
            await ctx.TriggerTypingAsync();
        }

        [Command("leave")]
        public async Task LeaveSelfRole(CommandContext ctx, DiscordRole role)
        {
            await ctx.TriggerTypingAsync();
        }

    }
}
