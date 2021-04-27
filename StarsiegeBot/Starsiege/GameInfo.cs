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
using System.Net;

namespace StarsiegeBot
{
    [Group("servers")]
    class GameInfo : BaseCommandModule
    {
        private WebServerItems gameData;
        public GameInfo()
        {
            Update();
        }
        private void Update()
        {
            if (gameData is null)
            {
                using WebClient wc = new WebClient();
                var json = wc.DownloadString("https://alpha.starsiegeplayers.com/api/v1/multiplayer/servers");
                gameData = JsonConvert.DeserializeObject<WebServerItems>(json);
            }
            else
            {
                using WebClient wc = new WebClient();
                var json = wc.DownloadString("https://alpha.starsiegeplayers.com/api/v1/multiplayer/servers");
                WebServerItems temp = JsonConvert.DeserializeObject<WebServerItems>(json);
                if (temp.RequestTime != gameData.RequestTime)
                {
                    gameData = temp;
                }
            }
        }

        [Command("Errors")]
        public async Task ErrorsServer(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            Update();
            // setup an exit point check.
            bool exit = false;
            // if its null, we're going to exit.
            if (gameData is null || gameData.Errors is null)
            {
                exit = true;
            }
            if (exit)
            {
                return;
            }
            DiscordMessageBuilder msg = new DiscordMessageBuilder();
            msg.Content = "Server Listing Errors:\r\n";
            foreach (var item in gameData.Errors)
            {
                msg.Content += $"{item}\r\n";
            }
            await ctx.RespondAsync(msg);
        }

        [Command("Masters")]
        public async Task MasterServers (CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            Update();
            // setup an exit point check.
            bool exit = false;
            // if its null, we're going to exit.
            if (gameData is null || gameData.Masters is null)
            {
                exit = true;
            }
            if (exit)
            {
                return;
            }
            DiscordMessageBuilder msg = new DiscordMessageBuilder();
            msg.Content = "```csharp\r\n";
            int count = 0;
            foreach (var item in gameData.Masters)
            {
                count++;
                msg.Content += $"$Inet::Master{count} = \"IP:{item.Address}\";\r\n";
            }
            msg.Content += "```";
            await ctx.RespondAsync(msg);
        }

        [Command("games")]
        public async Task GameServers(CommandContext ctx, int playerCount = 1)
        {
            await ctx.TriggerTypingAsync();
            Update();
            playerCount = Math.Abs(playerCount);
            // setup an exit point check.
            bool exit = false;
            // if its null, we're going to exit.
            if(gameData is null || gameData.Games is null)
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
            foreach (var item in gameData.Games)
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
                msg.Content = "No active servers. We're sorry";
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
