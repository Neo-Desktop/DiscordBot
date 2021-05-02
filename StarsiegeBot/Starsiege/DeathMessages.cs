﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StarsiegeBot
{

    [Group("deathmessage"), Aliases("dm")]
    [Description("Gives a random death message. Or interacts with the death messages.")]
    [Cooldown(1, 2, CooldownBucketType.Guild)]
    class DeathMessages : BaseCommandModule
    {
        private DeathMessageLines dmLines;
        private bool DeathMessagesEnabled;
        private static readonly string fileName = "Json/deathmessages.json";

        public DeathMessages()
        {
            Console.WriteLine("Death Message Commands Loaded.");
            // If the DeathMessages file exists, load it. And enable the commands.
            // Otherwise disable all the commands.
            if (File.Exists(fileName))
            {
                // set some JSON text to blank.
                var json = "";
                using (var fs = File.OpenRead(fileName))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();
                // Decode the JSON and set it to dmLines.
                dmLines = JsonConvert.DeserializeObject<DeathMessageLines>(json);
                // Enable all the commands.
                DeathMessagesEnabled = true;
            }
            else
            {
                // Echo out that we're missing a file. Disable the commands due to that.
                Console.WriteLine($" --- --- --- --- {fileName} is missing");
                DeathMessagesEnabled = false;
            }
        }

        [Command("load")]
        [RequireOwner]
        public async Task LoadDeathMessages(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            if (File.Exists(fileName))
            {
                // set some JSON text to blank.
                var json = "";
                using (var fs = File.OpenRead(fileName))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();
                // Decode the JSON and set it to dmLines.
                dmLines = JsonConvert.DeserializeObject<DeathMessageLines>(json);
                // Enable all the commands.
                DeathMessagesEnabled = true;
                await ctx.RespondAsync("Loading Death Messages successul.");
            }
            else
            {
                // Echo out that we're missing a file. Disable the commands due to that.
                await ctx.RespondAsync("Can't load Death Messages. File missing.");
                DeathMessagesEnabled = false;
            }
        }

        [Command("toggle"), Aliases("t")]
        [RequireOwner]
        public async Task ToggleDM(CommandContext ctx, [RemainingText] string isEnabled = null)
        {
            await ctx.TriggerTypingAsync();

            isEnabled = isEnabled.ToLower();

            string output;

            string[] turnOn = { "on", "true", "1" };
            string[] turnOff = { "off", "false", "0" };

            if (File.Exists(fileName))
            {
                if (turnOn.Contains(isEnabled))
                {
                    DeathMessagesEnabled = true;
                }
                else if (turnOff.Contains(isEnabled))
                {
                    DeathMessagesEnabled = false;
                }
                else
                {

                }
                output = $"DeathMessages Enabled: {DeathMessagesEnabled}";
            }
            else
            {
                DeathMessagesEnabled = false;
                output = $"{fileName} file is missing, and it can not be enabled.";
            }
            await ctx.RespondAsync(output);
        }

        [GroupCommand]
        public async Task DeathMessage(CommandContext ctx, [Description("Optional. Person you want to see kill you. If blank, will show generic death message.")] DiscordMember target = null)
        {
            // We're typing! (In all honesty, this is just used to avoid the 'async task not using await' messages I get.
            await ctx.TriggerTypingAsync();

            // IF we're missing the Death Messages dot JSON file, we're going to get disabled... Check for that.
            // If disabled, let the person know the command set is disabled, and return.
            if (!DeathMessagesEnabled)
            {
                await ctx.RespondAsync("Death Messages Commands have been disabled. Please contact the bot owners.");
                return;
            }

            // set some basic output incase something goes south.
            string line = "Something went wrong.";
            // if there is no target, we're going to "kill" the author.
            if (target == null)
            {
                int choice = Program.rnd.Next(0, dmLines.Generic.Length);
                line = dmLines.Generic[choice];
                await ctx.RespondAsync(string.Format(line, ctx.Message.Author.Mention));
            }
            // we have a valid target... Have the bot kill them.
            else
            {
                // Pick a death message type at random.
                int opt = Program.rnd.Next(1, 3);
                // If we picked active death messages...
                if (opt == 1)
                {
                    // Pick an Active death message at random, and set our output to it.
                    int choice = Program.rnd.Next(0, dmLines.Active.Length);
                    line = dmLines.Active[choice];
                }
                else if (opt == 2)
                {
                    // We picked a Passive death message type. Pick the exact line at random. Set it to the output.
                    int choice = Program.rnd.Next(0, dmLines.Passive.Length);
                    line = dmLines.Passive[choice];
                }
                // Give the selected death message to the user.
                await ctx.RespondAsync(string.Format(line, ctx.Message.Author.Mention, target.Mention));
            }
        }

        [Command("inventory"), Aliases("count")]
        [Description("Gets the total of each type of death message")]
        public async Task DeathMessageCount(CommandContext ctx)
        {
            // IF we're missing the Death Messages dot JSON file, we're going to get disabled... Check for that.
            // If disabled, let the person know the command set is disabled, and return.
            if (!DeathMessagesEnabled)
            {
                await ctx.RespondAsync("Death Messages Commands have been disabled. Please contact the bot owners.");
                return;
            }
            // We're typing here!
            await ctx.TriggerTypingAsync();
            // List all the types of Death Message types, and how many we have of each one.
            await ctx.RespondAsync($"\r\nActive: {dmLines.Active.Length}\r\nPassive: {dmLines.Passive.Length}\r\nGeneric: {dmLines.Generic.Length}");
        }

        [Command("Add")]
        [Description("Adds a message to the Pending Death Message Queue. Generic Death Messages require just a [Killed] while any other death message requires both a [Killed] and a [Killer] tags. These will be replaced with the correct information in game, and in the usage of random DM generation.")]
        [Cooldown(1, 2, CooldownBucketType.Guild)]
        public async Task DeathMessageNew(CommandContext ctx, [RemainingText, Description("The Death Message you want added. See help section.")] string msg)
        {
            // IF we're missing the Death Messages dot JSON file, we're going to get disabled... Check for that.
            // If disabled, let the person know the command set is disabled, and return.
            if (!DeathMessagesEnabled)
            {
                await ctx.RespondAsync("Death Messages Commands have been disabled. Please contact the bot owners.");
                return;
            }
            // Let Discord know we're typing.
            await ctx.TriggerTypingAsync();
            // Message the owner know some one wants to add a new death message, along with a code block to that message.

            // if the Queue is null, make it exist.
            if (dmLines.Queue is null)
                dmLines.Queue = new List<string>();
            // add the message to the queue list.
            dmLines.Queue.Add(msg);
            // Store the new stuff to file...
            await StoreDeathMessages();
            // Tell the person that the message has been added to a 'queue'.
            await ctx.RespondAsync("Your request has been added to the pending queue of requests.");
        }

        [Command("script")]
        [Description("Gets a script file for all the death messages on the bot.")]
        public async Task DeathMessagesScript(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            // IF we're missing the Death Messages dot JSON file, we're going to get disabled... Check for that.
            // If disabled, let the person know the command set is disabled, and return.
            if (!DeathMessagesEnabled)
            {
                await ctx.RespondAsync("Death Messages Commands have been disabled. Please contact the bot owners.");
                return;
            }

            // If this file exists... We could do one of two things. Delete it and remake it, or serve it up.
            if (File.Exists("deathmessages.cs"))
            {
                File.Delete("deathmessages.cs");
            }

            // We're going to remake it. Open it. 
            using StreamWriter file = new StreamWriter("deathmessages.cs");

            // Write out the total lines for each. Requirement for Starsiege.
            await file.WriteLineAsync($"$deathMessage::genericCount   = {dmLines.Generic.Length};");
            await file.WriteLineAsync($"$deathMessage::activeCount    = {dmLines.Active.Length};");
            await file.WriteLineAsync($"$deathMessage::passiveCount   = {dmLines.Passive.Length};\r\n");

            // Set count to 0. Iterate over each type of Death Message, and put it into the file.
            int count = 0;
            foreach (string death in dmLines.Generic)
            {
                await file.WriteLineAsync(string.Format($"$deathMessage::generic{count}   = \"{death}\";", "%s", "%s"));
                count++;
            }
            // Reset the count...
            count = 0;
            foreach (string death in dmLines.Active)
            {
                await file.WriteLineAsync(string.Format($"$deathMessage::active{count}   = \"{death}\";", "%s", "%s"));
                count++;
            }
            // Reset the count one last time...
            count = 0;
            foreach (string death in dmLines.Passive)
            {
                await file.WriteLineAsync(string.Format($"$deathMessage::passive{count}   = \"{death}\";", "%s", "%s"));
                count++;
            }

            // We're done writing to it... So close the file.
            file.Close();

            // We're going to Message Build the response.
            DiscordMessageBuilder msg = new DiscordMessageBuilder();
            // Open the file to upload.
            FileStream sound = new FileStream($"deathmessages.cs", FileMode.Open);
            // Tell DSharp that we want to upload this file.
            msg.WithFile("deathmessages.cs", sound);
            // Add some content so its not a blank file.
            msg.Content = "Here is your request for a death message script.";
            // send that message.
            await ctx.RespondAsync(msg);
            // We're done with the file, so close access to it.
            sound.Close();
        }

        private async Task StoreDeathMessages()
        {
            string output = JsonConvert.SerializeObject(dmLines);
            await File.WriteAllTextAsync(fileName, output);
            return;
        }
    }
    class DeathMessageLines
    {
        [JsonProperty("active")]
        public string[] Active { get; set; }
        [JsonProperty("passive")]
        public string[] Passive { get; set; }
        [JsonProperty("generic")]
        public string[] Generic { get; set; }
        [JsonProperty("flagged")]
        public List<String> Flagged { get; set; }
        [JsonProperty("queue")]
        public List<String> Queue { get; set; }
    }
}
