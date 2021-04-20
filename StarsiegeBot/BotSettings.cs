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
    [RequireGuild]
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
                item.LevelRoles = new Dictionary<int, List<DiscordRole>>();
                item.Prefixes = new List<string>();
                item.SelfRoles = new Dictionary<string, Dictionary<string, int>>();
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

        [Group("Prefix"), Aliases("p", "prefixes")]
        [Description("Shows the list of prefixes specific to this server.")]
        class BotPrefix : BotSettings
        {
            [GroupCommand]
            public async Task GetPrefixes(CommandContext ctx)
            {
                await ctx.TriggerTypingAsync();
                // store the Guild ID as a string.
                string guild = ctx.Guild.Id.ToString();
                // if this server doesnt have a settings for whatever reason, let them know... why am i not creating it?
                if (!GuildSettings.ContainsKey(guild))
                {
                    await ctx.RespondAsync("No guild settings found.");
                }
                else
                {
                    // we have server settings... why does this give so much info? it should only show only prefix(es)
                    var server = GuildSettings[guild];
                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                    {
                        Description = $"{ctx.Guild.Name} Prefixes."
                    };

                    int count = 1;
                    // for each prefix, list it in the embed as a field. Show a count.
                    foreach (string item in GuildSettings[guild].Prefixes)
                    {
                        embed.AddField($"Prefix {count}", item, true);
                        count++;
                    }

                    await ctx.RespondAsync(embed);
                }
            }
            [Command("Add")]
            [Description("Adds a new prefix to the Server's list of prefixes.")]
            public async Task AddNewPrefix(CommandContext ctx, string prefix)
            {
                await ctx.TriggerTypingAsync();
                string gId = ctx.Guild.Id.ToString();
                // add the prefix to the server specific settings.
                if (GuildSettings[gId].Prefixes.Count < 25)
                {
                    GuildSettings[gId].Prefixes.Add(prefix);
                    await ctx.RespondAsync($"Added `{prefix}` as a command prefix.");
                }
                else
                {
                    await ctx.RespondAsync("There is a maximum of 25 prefixes allowed.");
                }
            }

            [Command("delete"), Aliases("del")]
            [Description("Removes the specified prefix from accepted prefixes.")]
            public async Task RemovePrefix(CommandContext ctx, string prefix)
            {
                await ctx.TriggerTypingAsync();
                string gId = ctx.Guild.Id.ToString();
                // they have only one prefix and disallow global prefixes, dont let them delete the prefix.
                if (GuildSettings[gId].Prefixes.Count == 1 && !GuildSettings[gId].UseGlobalPrefix)
                {
                    await ctx.RespondAsync("Can't remove the last usable prefix from the bot. Add a new prefix, or enable Global Prefix.");
                    return;
                }
                // they pass the vibe check, delete the prefix.
                GuildSettings[ctx.Guild.Id.ToString()].Prefixes.Remove(prefix);
                await ctx.RespondAsync($"Removed `{prefix}` as a command prefix.");
            }

            [Command("global")]
            [Description("View or edit the status of allowing Global Prefix(es).")]
            public async Task UseGlobalPrefixes(CommandContext ctx)
            {
                await ctx.TriggerTypingAsync();
                await ctx.RespondAsync($"Global Prefix usage has been turned **{(GuildSettings[ctx.Guild.Id.ToString()].UseGlobalPrefix ? "on" : "off")}**");
            }
            [Command("global"), Aliases("g")]
            [Description("View or edit the status of allowing Global Prefix(es).")]
            public async Task UseGlobalPrefixes(CommandContext ctx, [Description("True or False")] bool isEnabled)
            {
                await ctx.TriggerTypingAsync();
                // check to make sure that they aren't trying to turn off global prefixes with out first adding a server sepcific prefix.
                int prefixCount = GuildSettings[ctx.Guild.Id.ToString()].Prefixes.Count;
                if (prefixCount <= 0 && !isEnabled)
                {
                    await ctx.RespondAsync("You have to add at least one server prefix before you can turn off Global Prefux usage.");
                    return;
                }
                // they passed teh vibe check, let them turn off global prefixes.
                GuildSettings[ctx.Guild.Id.ToString()].UseGlobalPrefix = isEnabled;
                await ctx.RespondAsync($"Global Prefix usage has been turned **{(isEnabled ? "on" : "off")}**");
            }
            [Command("global")]
            public async Task UseGlobalPrefixes(CommandContext ctx, [Description("0 is off, 1 is on.")] int isEnabled)
            {
                // since we're just allowing the user to use different ways to turn this function on and off... Redirect the work load there.
                if (isEnabled >= 1)
                {
                    await UseGlobalPrefixes(ctx, true);
                }
                else
                {
                    await UseGlobalPrefixes(ctx, false);
                }
            }
            [Command("global")]
            public async Task UseGlobalPrefixes(CommandContext ctx, [Description("ON or OFF")] string isEnabled)
            {
                // since we're just trying to make the end user's life easier by different means of turning this on/off. redirect work load that way.
                if (isEnabled.ToLower() == "on")
                {
                    await UseGlobalPrefixes(ctx, true);
                }
                else if (isEnabled.ToLower() == "off")
                {
                    await UseGlobalPrefixes(ctx, false);
                }
                else
                {
                    // they wanna be smart, and we don't wanna, tell them that.
                    await ctx.RespondAsync("Please specify ON or OFF");
                }
            }
        }

        [Group("SelfRoles"), Aliases(new[] {"sr", "self"})]
        class SelfRoles : BotSettings
        {
            public SelfRoles()
            {

            }

            [Command("add")]
            public async Task AddSelfRole(CommandContext ctx, DiscordRole role, string group = null)
            {
                await ctx.TriggerTypingAsync();
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

            [Command("delete")]
            public async Task DeleteSelfRole(CommandContext ctx, DiscordRole role, string group = null)
            {
                await ctx.TriggerTypingAsync();
            }
        }

        [Group("LevelRoles"), Aliases(new[] {"lr","level"})]
        class LevelRoles : BotSettings
        {
            public LevelRoles()
            {

            }
        }

        [Group("group")]
        class RoleGroups : BotSettings
        {
            public RoleGroups()
            {

            }

            // todo: add groups
            // todo: remove groups.
            //       Do we delete the roles in this group or move them to another group?
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
        public Dictionary<string,Dictionary<string,int>> SelfRoles { get; set; }
        public Dictionary<int,List<DiscordRole>> LevelRoles { get; set; }
    }
}
