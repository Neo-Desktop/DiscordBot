using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

            if (!(GuildSettings is null))
            {
                // guild settings have already been set... stop resetting it.
                return;
            }
            // Check for guild settings file. If it doesnt exist, create it.
            if (!File.Exists("guildSettings.json"))
            {
                // Since we need to creat it, we're going to set some default stuff.
                Console.Write("Guild Settings Config File Not Found. Creating One...");
                GuildSettings item = new GuildSettings
                {
                    UseAutoRoles = false,
                    UseGlobalPrefix = true,
                    UseLevelRoles = false,
                    UseLevels = false,
                    UseSelfRoles = false,
                    AllowRolesPurchase = false
                };
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
        [Description("Work in progress. Does nothing yet.")]
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
        public Dictionary<string, Dictionary<string, int>> SelfRoles { get; set; }
        public Dictionary<int, List<DiscordRole>> LevelRoles { get; set; }
        public List<DiscordRole> HiddenRolesList { get; set; }
        public DiscordChannel WelcomeChannel { get; set; }
        public bool UseWelcome { get; set; }
        public string WelcomeMessage { get; set; }

    }
}
