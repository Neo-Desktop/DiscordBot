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
    [RequireGuild, RequirePermissions(Permissions.ManageChannels)]
    class WelcomeMessage : BotSettings
    {
        [GroupCommand]
        public async Task GeneralWelcome (CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            string gId = ctx.Guild.Id.ToString();

            DiscordEmbedBuilder embed = StartEmbed("Welcome Message Info");
            if (GuildSettings[gId].WelcomeChannel is null)
                embed.AddField("Channel", "*None*");
            else
                embed.AddField("Channel", GuildSettings[gId].WelcomeChannel.Mention, true);
            embed.AddField("Enabled?", GuildSettings[gId].UseWelcome.ToString(), true);
            if (GuildSettings[gId].WelcomeMessage is null || GuildSettings[gId].WelcomeMessage == "")
                embed.AddField("Message", "*None*");
            else
                embed.AddField("Message", GuildSettings[gId].WelcomeMessage);
            await ctx.RespondAsync(embed);
        }
        [GroupCommand]
        public async Task GeneralWelcome(CommandContext ctx, bool enable, DiscordChannel channel)
        {
            await WelcomeChannel(ctx, channel);
            await WelcomeEnabled(ctx, enable);
        }
        [GroupCommand]
        public async Task GeneralWelcome(CommandContext ctx, DiscordChannel channel, bool enable)
        {
            await WelcomeChannel(ctx, channel);
            await WelcomeEnabled(ctx, enable);
        }
        [Command("enable")]
        public async Task WelcomeEnabled(CommandContext ctx, bool enable)
        {
            await ctx.TriggerTypingAsync();
            string gId = ctx.Guild.Id.ToString();

            DiscordEmbedBuilder embed = StartEmbed("Welcome Message Status");
            embed.AddField("Old", GuildSettings[gId].UseWelcome.ToString(), true);
            GuildSettings[gId].UseWelcome = enable;
            embed.AddField("New", GuildSettings[gId].UseWelcome.ToString(), true);
            await ctx.RespondAsync(embed);
        }
        [Command("Channel")]
        public async Task WelcomeChannel(CommandContext ctx, DiscordChannel channel)
        {
            await ctx.TriggerTypingAsync();
            string gId = ctx.Guild.Id.ToString();
            DiscordEmbedBuilder embed = StartEmbed("Welcome Message Channel");
            if (GuildSettings[gId].WelcomeChannel != null)
            {
                embed.AddField("Old", GuildSettings[gId].WelcomeChannel.Mention, true);
            }
            else
            {
                embed.AddField("Old", "*None*");
            }

            GuildSettings[gId].WelcomeChannel = channel;
            embed.AddField("New", GuildSettings[gId].WelcomeChannel.Mention, true);
            await ctx.RespondAsync(embed);
        }
        [Command("Channel")]
        public async Task WelcomeChannel(CommandContext ctx, [RemainingText]string here)
        {
            await ctx.TriggerTypingAsync();
            string gId = ctx.Guild.Id.ToString();
            DiscordEmbedBuilder embed = StartEmbed("Welcome Message Channel");
            if (GuildSettings[gId].WelcomeChannel != null)
            {
                embed.AddField("Old", GuildSettings[gId].WelcomeChannel.Mention, true);
            }
            else
            {
                embed.AddField("Old", "*None*");
            }

            GuildSettings[gId].WelcomeChannel = ctx.Channel;
            embed.AddField("New", GuildSettings[gId].WelcomeChannel.Mention, true);
            await ctx.RespondAsync(embed);
        }
        [Command("Message")]
        [Description("Hello. Run `>welcome help` for more info.")]
        public async Task MessageOfWelcome(CommandContext ctx, [RemainingText]string msg)
        {
            await ctx.TriggerTypingAsync();
            string gId = ctx.Guild.Id.ToString();
            DiscordEmbedBuilder embed = StartEmbed("Welcome Message String");
            if (GuildSettings[gId].WelcomeMessage == "" || GuildSettings[gId].WelcomeMessage is null)
                embed.AddField("Old", "*None*");
            else
                embed.AddField("Old", GuildSettings[gId].WelcomeMessage);
            GuildSettings[gId].WelcomeMessage = msg;
            embed.AddField("New", GuildSettings[gId].WelcomeMessage);
            string example = WelcomeMessageProcessing(GuildSettings[gId].WelcomeMessage, ctx.Member, ctx.Guild);
            await ctx.RespondAsync($"An example:\r\n> {example}", embed);
        }
        [Command("Help")]
        public async Task WelcomeHelp(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            DiscordEmbedBuilder embed = StartEmbed("List of placeholders and their intended replacement. Channel tags can be inserted without a placeholder.");
            embed.Title = "Welcome Message Help";
            embed.AddField("[Discrim]", "Inserts the username of the member.");
            embed.AddField("[UName]", "Inserts the username of the member.");
            embed.AddField("[GName]", "Inserts your Guild/Server Name.");
            embed.AddField("[Mention]", "Inserts your Guild/Server Name.");
            await ctx.RespondAsync(embed);
        }
        public static string WelcomeMessageProcessing(string msg, DiscordMember member, DiscordGuild guild)
        {
            string output = msg
                .Replace("[discrim]", member.Discriminator, StringComparison.OrdinalIgnoreCase)
                .Replace("[gname]", guild.Name, StringComparison.OrdinalIgnoreCase)
                .Replace("[uname]", member.Username, StringComparison.OrdinalIgnoreCase)
                .Replace("[mention]", member.Mention, StringComparison.OrdinalIgnoreCase);
            return output;
        }

        private DiscordEmbedBuilder StartEmbed(string desc)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Description = desc,
                Color = Program.colours[0]
            };
            return embed;
        }
    }
}
