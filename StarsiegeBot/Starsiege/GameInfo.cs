using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace StarsiegeBot
{
    [Group("servers")]
    [Description("Gives various reports on the game or master servers.")]
    class GameInfo : BaseCommandModule
    {
        private WebServerItems _gameData;
        public GameInfo()
        {
            Update();
            Console.WriteLine("Starsiege Game Info Commands Loaded");
        }
        private void Update()
        {
            if (_gameData is null)
            {
                using WebClient wc = new WebClient();
                string json = wc.DownloadString("https://alpha.starsiegeplayers.com/api/v1/multiplayer/servers");
                _gameData = JsonConvert.DeserializeObject<WebServerItems>(json);
            }
            else
            {
                using WebClient wc = new WebClient();
                string json = wc.DownloadString("https://alpha.starsiegeplayers.com/api/v1/multiplayer/servers");
                WebServerItems temp = JsonConvert.DeserializeObject<WebServerItems>(json);
                if (temp.RequestTime != _gameData.RequestTime)
                {
                    _gameData = temp;
                }
            }
        }
        [GroupCommand]
        public async Task Overview(CommandContext ctx)
        {
            Update();
            // setup an exit point check.
            bool exit = false;
            // if its null, we're going to exit.
            if (_gameData is null || _gameData.Errors is null)
            {
                exit = true;
            }
            if (exit)
            {
                return;
            }
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Title = "Master|Game Server Overview",
                Description = "Number of Master, and Game servers. Along with number of errors."
            };
            embed.AddField("Masters", _gameData.Masters.Length.ToString(), true);
            embed.AddField("Games", _gameData.Games.Length.ToString(), true);
            embed.AddField("Errors", _gameData.Errors.Length.ToString(), true);
            await ctx.RespondAsync(embed);
        }
        [Command("Errors"), Aliases("e")]
        [Description("Lists all the errors from the server listing.")]
        public async Task ErrorsServer(CommandContext ctx)
        {
            Update();
            // setup an exit point check.
            bool exit = false;
            // if its null, we're going to exit.
            if (_gameData is null || _gameData.Errors is null)
            {
                exit = true;
            }
            if (exit)
            {
                return;
            }
            DiscordMessageBuilder msg = new DiscordMessageBuilder
            {
                Content = "Server Listing Errors:\r\n"
            };
            foreach (string item in _gameData.Errors)
            {
                msg.Content += $"{item}\r\n";
            }
            await ctx.RespondAsync(msg);
        }
        [Command("Masters"), Aliases("m")]
        [Description("Gives all the master servers as a script to be improted into `master.cs`.")]
        public async Task MasterServers(CommandContext ctx)
        {
            Update();
            // setup an exit point check.
            bool exit = false;
            // if its null, we're going to exit.
            if (_gameData is null || _gameData.Masters is null)
            {
                exit = true;
            }
            if (exit)
            {
                return;
            }
            DiscordMessageBuilder msg = new DiscordMessageBuilder
            {
                Content = "```csharp\r\n"
            };
            int count = 0;
            foreach (MasterServer item in _gameData.Masters)
            {
                count++;
                msg.Content += $"$Inet::Master{count} = \"IP:{item.Address}\";\r\n";
            }
            msg.Content += "```";
            await ctx.RespondAsync(msg);
        }
        [Command("games"), Aliases("g")]
        [Description("Currently: Lists games that have been started (Player dropped in). End Result: Lists all games that have a player in the server (either dropped in or not)")]
        public async Task GameServers(CommandContext ctx, [Description("Number of players you want to check for. Game servers will need at least this meaning or more to be listed.")] int playerCount = 1)
        {
            Update();
            playerCount = Math.Abs(playerCount);
            // setup an exit point check.
            bool exit = false;
            // if its null, we're going to exit.
            if (_gameData is null || _gameData.Games is null)
            {
                exit = true;
            }
            // do we need to exit?
            if (exit)
            {
                return;
            }
            DiscordMessageBuilder msg = new DiscordMessageBuilder();
            // we're on our way to greatness!
            int count = 0;
            foreach (GameServer item in _gameData.Games)
            {
                // if (item.PlayerCount > playerCount)
                if (item.GameStatus["Started"])
                {
                    count++;
                    msg.Content += $"{count} | {item.Name} | {item.PlayerCount}/{item.MaxPlayers} | {item.Address}\r\n";
                }
            }
            if (count == 0)
            {
                msg.Content = "No servers matching your requirements. We're sorry";
            }
            await ctx.RespondAsync(msg);
        }
    }
    class WebServerItems
    {
        [JsonProperty("requesttime")]
        public string RequestTime { get; private set; }
        [JsonProperty("errors")]
        public string[] Errors { get; private set; }
        [JsonProperty("masters")]
        public MasterServer[] Masters { get; private set; }
        [JsonProperty("games")]
        public GameServer[] Games { get; private set; }
    }
    class MasterServer
    {
        [JsonProperty("address")]
        public string Address { get; private set; }
        [JsonProperty("CommonName")]
        public string CommonName { get; private set; }
        [JsonProperty("MOTD")]
        public string MOTD { get; private set; }
        [JsonProperty("ServerCount")]
        public int ServerCount { get; private set; }
        [JsonProperty("Ping")]
        public int Ping { get; private set; }
    }
    class GameServer
    {
        [JsonProperty("GameName")]
        public string GameName { get; private set; }
        [JsonProperty("GameVersion")]
        public string GameVersion { get; private set; }
        [JsonProperty("Name")]
        public string Name { get; private set; }
        [JsonProperty("Address")]
        public string Address { get; private set; }
        [JsonProperty("PlayerCount")]
        public int PlayerCount { get; private set; }
        [JsonProperty("GameMode")]
        public int GameMode { get; private set; }
        [JsonProperty("MaxPlayers")]
        public int MaxPlayers { get; private set; }
        [JsonProperty("Ping")]
        public int Ping { get; private set; }
        [JsonProperty("GameStatus")]
        public Dictionary<string, bool> GameStatus { get; private set; }
    }
}
