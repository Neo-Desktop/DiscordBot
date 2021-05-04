using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace StarsiegeBot
{
    [Group("bot")]
    [RequireGuild]
    [Description("Allows, viewing and editing settings on the bot.")]
    class BotSettings : BaseCommandModule
    {
        public static Dictionary<string, GuildSettings> GuildSettings;
        private readonly string fileName = "guildsettings.json";
        private readonly CancellationTokenSource cancelToken;
        public BotSettings()
        {
            Console.WriteLine("Bot Setting Commands Loaded");

            if (!(GuildSettings is null))
            {
                // guild settings have already been set... stop resetting it.
                return;
            }
            // Check for guild settings file. If it doesnt exist, create it.
            if (!File.Exists(fileName))
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
                item.LevelRoles = new Dictionary<int, List<string>>();
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
                File.WriteAllTextAsync(fileName, output);
                // in form the console reader that we're done making the initial config.
                Console.WriteLine(" Done!");
            }
            else
            {
                // load the config file to memory.
                var json = "";
                using (var fs = File.OpenRead(fileName))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();
                GuildSettings = JsonConvert.DeserializeObject<Dictionary<string, GuildSettings>>(json);
            }
            cancelToken = new CancellationTokenSource();
            _ = StartTimer(cancelToken.Token);
        }
        [GroupCommand]
        [Description("Reports simplified data about the server settings.")]
        public async Task BaseItem(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var guild = GuildSettings[ctx.Guild.Id.ToString()];
            DiscordEmbedBuilder embed = StartEmbed(ctx.Guild.Name + " Server settings.");
            embed.AddField("On/Off Items", "_ _");
            embed.AddField("Global Prefix", (guild.UseGlobalPrefix ? "On" : "Off"), true);
            embed.AddField("Self Roles", (guild.UseSelfRoles ? "On" : "Off"), true);
            embed.AddField("Level Roles", (guild.UseLevelRoles ? "On" : "Off"), true);
            embed.AddField("Auto Roles", (guild.UseAutoRoles ? "On" : "Off"), true);
            embed.AddField("Levels", (guild.UseLevels ? "On" : "Off"), true);
            embed.AddField("Welcome Msg", (guild.UseWelcome ? "On" : "Off"), true);
            embed.AddField("Counts", "_ _");
            embed.AddField("Prefixes", guild.Prefixes.Count.ToString(), true);
            embed.AddField("Self Groups", guild.SelfRoles.Count.ToString(), true);
            embed.AddField("Levels", guild.LevelRoles.Count.ToString(), true);
            await ctx.RespondAsync(embed);
        }

        [Command("status"), RequireOwner]
        [Description("Sets the bot's nickname on this server.")]
        public async Task SetStatus (CommandContext ctx, string type, [RemainingText] string newStatus = null)
        {
            await ctx.TriggerTypingAsync();
            var aType = type.ToLower() switch
            {
                "competing" => ActivityType.Competing,
                "listening" => ActivityType.ListeningTo,
                "playing" => ActivityType.Playing,
                "streaming" => ActivityType.Streaming,
                "watching" => ActivityType.Watching,
                _ => ActivityType.Custom,
            };
            DiscordActivity act = new DiscordActivity(newStatus, aType);
            await ctx.Client.UpdateStatusAsync(act);
            await ctx.RespondAsync("Test.");
        }

        [Command("username"), RequireOwner]
        public async Task SetUsername (CommandContext ctx, [RemainingText] string newName)
        {
            await ctx.TriggerTypingAsync();
            await ctx.Client.UpdateCurrentUserAsync(newName);
            await ctx.RespondAsync("How's my name name?");
        }

        private async Task StartTimer(CancellationToken cancellationToken)
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    string output = JsonConvert.SerializeObject(BotSettings.GuildSettings);
                    await File.WriteAllTextAsync("guildsettings.json", output);
                    await Task.Delay(TimeSpan.FromSeconds(120), cancellationToken);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Token Cancelled.");
                        break;
                    }
                }
            });
        }
        private DiscordEmbedBuilder StartEmbed(string desc)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Description = desc,
                Color = Program.colours[^2]
            };
            return embed;
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
        public List<string> Prefixes { get; set; }
        // List<"Prefix">
        public List<string> HiddenRolesList { get; set; }
        // List<"RoleId">
        public Dictionary<string, Dictionary<string, int>> SelfRoles { get; set; }
        // Dict < "GroupName", Dict<"RoleID", Price>>
        public Dictionary<int, List<string>> LevelRoles { get; set; }
        // Dict< Level, List<"RoleID">>
        public DiscordChannel WelcomeChannel { get; set; }
        public bool UseWelcome { get; set; }
        public string WelcomeMessage { get; set; }
    }
}
