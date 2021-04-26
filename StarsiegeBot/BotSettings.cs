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
        [GroupCommand]
        public async Task BaseItem(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
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
        public DiscordChannel WelcomeChannel { get; set; }
        public bool useWelcomeMessage { get; set; }
    }
}
