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
    [Group("bot")]
    [Description("Allows, viewing and editing settings on the bot.")]
    class BotSettings : BaseCommandModule
    {
        public static Dictionary<string, GuildSettings> GuildSettings;

        public BotSettings()
        {
            Console.WriteLine("Bot Setting Commands Loaded");
            // Check for guild settings file. If it doesnt exist, create it.
            if (!File.Exists("guildSettings.json"))
            {
                // Since we need to creat it, we're going to set some default stuff.
                Console.Write("Guild Settings Config File Not Found. Creating One...");
                GuildSettings item = new GuildSettings();
                item.UseAutoRoles = false;
                item.UseGlobalPrefix = true;
                item.UseLevelRoles = false;
                item.UseLevels = false;
                item.UseSelfRoles = false;
                item.AllowRolesPurchase = false;
                item.UseAutoRoles = false;
                item.LevelRoles = new Dictionary<string, int>();
                item.Prefixes = new List<string>();
                item.SelfRoles = new Dictionary<string, int>();
                item.Prefixes.Add(">");
                // this line... is a test line.

                GuildSettings = new Dictionary<string, GuildSettings>
                {
                    { "default", item }
                };

                // Now that we have the initial stuff in memory, write it to file.
                string output = JsonConvert.SerializeObject(GuildSettings);
                File.WriteAllTextAsync("guildSettings.json", output);
                // in form the console reader that we're done making the initial config.
                Console.WriteLine(" Done!");
            }
            else
            {
                // load the config file to memory.
                var json = "";
                using (var fs = File.OpenRead("guildSettings.json"))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();
                GuildSettings = JsonConvert.DeserializeObject<Dictionary<string, GuildSettings>>(json);
            }
        }

        [Group("Prefix")]
        class BotPrefix : BotSettings
        {
            [GroupCommand]
            public async Task GetPrefixes(CommandContext ctx)
            {
                await ctx.TriggerTypingAsync();
                // store the Guild ID as a string.
                string guild = ctx.Guild.Id.ToString();
                // if this server doesnt have a settings for whatever reason, let them know... why am i not creating it?
                if (! GuildSettings.ContainsKey(guild))
                {
                    await ctx.RespondAsync("No guild settings found.");
                }
                else
                {
                    // we have server settings... why does this give so much info? it should only show only prefix(es)
                    var server = GuildSettings[guild];
                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                    {
                        Description = $"{ctx.Guild.Name} Settings."
                    };
                    embed.AddField("Prefix(es)", server.Prefixes.ToString());
                    embed.AddField("Use Global Prefix", server.UseGlobalPrefix.ToString(), true);
                    embed.AddField("Self Roles Enabled", server.UseSelfRoles.ToString(), true);
                    embed.AddField("Level Roles Enabled", server.UseLevelRoles.ToString(), true);
                    embed.AddField("Self Role Buy", server.AllowRolesPurchase.ToString(), true);
                    await ctx.RespondAsync(embed);
                }
            }
            [Command("Add")]
            [Description("Adds a new prefix to the Server's list of prefixes.")]
            public async Task AddNewPrefix (CommandContext ctx, string prefix)
            {
                await ctx.TriggerTypingAsync();
                GuildSettings[ctx.Guild.Id.ToString()].Prefixes.Add(prefix);
                await ctx.RespondAsync($"Added `{prefix}` as a command prefix.");
            }

            [Command("delete"), Aliases("del")]
            [Description("Removes the specified prefix from accepted prefixes.")]
            public async Task RemovePrefix (CommandContext ctx, string prefix)
            {
                await ctx.TriggerTypingAsync();
                GuildSettings.Remove(prefix);
                await ctx.RespondAsync($"Removed `{prefix}` as a command prefix.");
            }

            [Command("global"), Aliases("g")]
            [Description("View or edit the status of allowing Global Prefix(es).")]
            public async Task UseGlobalPrefixes(CommandContext ctx, bool isEnabled)
            {
                await ctx.TriggerTypingAsync();
                GuildSettings[ctx.Guild.Id.ToString()].UseGlobalPrefix = isEnabled;
                await ctx.RespondAsync($"Global Prefix usage has been turned **{(isEnabled ? "on":"off")}**");

            }
        }
    }

    public class GuildSettings
    {
        public bool UseGlobalPrefix { get; set; }
        public bool UseSelfRoles { get; set; }
        public bool UseLevelRoles { get; set; }
        public bool UseLevels { get; set; }
        public bool AllowRolesPurchase { get; set; }
        public bool UseAutoRoles { get; set; }
        public List<String> Prefixes { get; set; }
        public Dictionary<string,int> SelfRoles { get; set; }
        public Dictionary<string,int> LevelRoles { get; set; }
    }
}
