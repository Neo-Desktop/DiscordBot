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
    [Group("welcome")]
    class WelcomeMessage : BotSettings
    {
        [Command("enable")]
        public async Task WelcomeEnabled(CommandContext ctx, bool enable)
        {
            await ctx.TriggerTypingAsync();
            string gId = ctx.Guild.Id.ToString();

            GuildSettings[gId].UseWelcome = enable;

            await ctx.RespondAsync($"Use welcome message? {enable}");
        }

        [Command("Channel")]
        public async Task WelcomeChannel(CommandContext ctx, [RemainingText]string here)
        {
            await ctx.TriggerTypingAsync();
            string gId = ctx.Guild.Id.ToString();
            GuildSettings[gId].WelcomeChannel = ctx.Channel;
            await ctx.RespondAsync("This channel is the welcome channle now!");
        }

        [Command("Message")]
        public async Task MessageOfWelcome(CommandContext ctx, [RemainingText]string msg)
        {
            await ctx.TriggerTypingAsync();
            string gId = ctx.Guild.Id.ToString();
            GuildSettings[gId].WelcomeMessage = msg;
            await ctx.RespondAsync("welcome message set.");
        }

    }
}
