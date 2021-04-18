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

namespace StarsiegeBot
{

    [Group("deathmessage"), Aliases("dm")]
    [Description("Gives a random death message. Or interacts with the death messages.")]
    class DeathMessages : BaseCommandModule
    {
        private readonly Random rnd = new Random();
        private readonly DeathMessageLines dmLines;

        public DeathMessages()
        {
            Console.WriteLine("Death Message Commands Loaded.");
            var json = "";
            using (var fs = File.OpenRead("DeathMessages.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();
            dmLines = JsonConvert.DeserializeObject<DeathMessageLines>(json);
        }

        [GroupCommand]
        public async Task DeathMessage(CommandContext ctx, [Description("Optional. Person you want to see kill you. If blank, will show generic death message.")]DiscordMember target = null)
        {
            await ctx.TriggerTypingAsync();
            string line = "Something went wrong.";
            if (target == null)
            {
                int choice = rnd.Next(0, dmLines.generic.Length);
                line = dmLines.generic[choice];
                await ctx.RespondAsync(string.Format(line, ctx.Message.Author.Mention));
            }
            else
            {
                int opt = rnd.Next(1, 3);
                if (opt == 1)
                {
                    int choice = rnd.Next(0, dmLines.active.Length);
                    line = dmLines.active[choice];
                }
                else if (opt == 2)
                {
                    int choice = rnd.Next(0, dmLines.passive.Length);
                    line = dmLines.passive[choice];
                }
                await ctx.RespondAsync(string.Format(line, ctx.Message.Author.Mention, target.Mention));
            }
        }

        [Command("inventory"), Aliases("count")]
        [Description("Gets the total of each type of death message")]
        public async Task DeathMessageCount(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync($"\r\nActive: {dmLines.active.Length}\r\nPassive: {dmLines.passive.Length}\r\nGeneric: {dmLines.generic.Length}");
        }

        [Command("Add")]
        [Description("Adds a message to the Pending Death Message Queue. Generic Death Messages require just a [Killed] while any other death message requires both a [Killed] and a [Killer] tags. These will be replaced with the correct information in game, and in the usage of random DM generation.")]
        public async Task DeathMessageNew(CommandContext ctx, [RemainingText,Description("The Death Message you want added. See help section.")] string msg)
        {
            await ctx.TriggerTypingAsync();
            await Program.notify.SendMessageAsync($"{ctx.Message.Author.Username} wantes to add the death messages. ```{msg}```");
            await ctx.RespondAsync("Your request has been added to the pending queue of requests.");
        }


        [Command("script")]
        [Description("Gets a script file for all the death messages on the bot.")]
        public async Task DeathMessagesScript(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            if (File.Exists("deathmessages.cs"))
            {
                File.Delete("deathmessages.cs");
            }

            using StreamWriter file = new StreamWriter("deathmessages.cs");

            await file.WriteLineAsync($"$deathMessage::genericCount   = {dmLines.generic.Length};");
            await file.WriteLineAsync($"$deathMessage::activeCount    = {dmLines.active.Length};");
            await file.WriteLineAsync($"$deathMessage::passiveCount   = {dmLines.passive.Length};\r\n");

            int count = 0;
            foreach (string death in dmLines.generic)
            {
                await file.WriteLineAsync(string.Format($"$deathMessage::generic{count}   = \"{death}\";", "%s", "%s"));
                count++;
            }
            count = 0;
            foreach (string death in dmLines.active)
            {
                await file.WriteLineAsync(string.Format($"$deathMessage::active{count}   = \"{death}\";", "%s", "%s"));
                count++;
            }
            count = 0;
            foreach (string death in dmLines.passive)
            {
                await file.WriteLineAsync(string.Format($"$deathMessage::passive{count}   = \"{death}\";", "%s", "%s"));
                count++;
            }

            DiscordMessageBuilder msg = new DiscordMessageBuilder();
            file.Close();

            FileStream sound = new FileStream($"deathmessages.cs", FileMode.Open);
            msg.WithFile(sound);
            msg.Content ="Here is your request for a death message script.";
            await ctx.RespondAsync(msg);
        }
    }
    class DeathMessageLines
    {
        [JsonProperty("active")]
        public string[] active { get; set; }
        [JsonProperty("passive")]
        public string[] passive { get; set; }
        [JsonProperty("generic")]
        public string[] generic { get; set; }

        public string[] flagged { get; set; }

        public string[] queue { get; set; }
    }

}
