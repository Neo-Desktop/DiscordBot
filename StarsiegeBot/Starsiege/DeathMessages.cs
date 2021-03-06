using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
namespace StarsiegeBot
{
    [Group("deathmessage"), Aliases("dm")]
    [Description("Gives a random death message. Or interacts with the death messages.")]
    [Cooldown(1, 2, CooldownBucketType.Guild)]
    class DeathMessages : BaseCommandModule
    {
        private DeathMessageLines _dmLines;
        private bool _isEnabled;
        private static readonly string s_fileName = "Json/deathmessages.json";
        public DeathMessages()
        {
            Console.WriteLine("Death Message Commands Loaded.");
            // If the DeathMessages file exists, load it. And enable the commands.
            // Otherwise disable all the commands.
            if (File.Exists(s_fileName))
            {
                // set some JSON text to blank.
                string json = "";
                using (FileStream fs = File.OpenRead(s_fileName))
                using (StreamReader sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();
                // Decode the JSON and set it to dmLines.
                _dmLines = JsonConvert.DeserializeObject<DeathMessageLines>(json);
                // Enable all the commands.
                _isEnabled = true;
            }
            else
            {
                // Echo out that we're missing a file. Disable the commands due to that.
                Console.WriteLine($" --- --- --- --- {s_fileName} is missing");
                _isEnabled = false;
            }
        }
        [Command("load")]
        [RequireOwner]
        public async Task LoadDeathMessages(CommandContext ctx)
        {
            if (File.Exists(s_fileName))
            {
                // set some JSON text to blank.
                string json = "";
                using (FileStream fs = File.OpenRead(s_fileName))
                using (StreamReader sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();
                // Decode the JSON and set it to dmLines.
                _dmLines = JsonConvert.DeserializeObject<DeathMessageLines>(json);
                // Enable all the commands.
                _isEnabled = true;
                await ctx.RespondAsync(StartEmbed("Loading Death Messages successul."));
            }
            else
            {
                // Echo out that we're missing a file. Disable the commands due to that.
                await ctx.RespondAsync(StartEmbed("Can't load Death Messages. File missing."));
                _isEnabled = false;
            }
        }
        [Command("toggle"), Aliases("t")]
        [RequireOwner]
        public async Task ToggleDM(CommandContext ctx, [RemainingText] string isEnabled = null)
        {
            isEnabled = isEnabled.ToLower();
            string output;
            string[] turnOn = { "on", "true", "1" };
            string[] turnOff = { "off", "false", "0" };
            if (File.Exists(s_fileName))
            {
                if (turnOn.Contains(isEnabled))
                {
                    _isEnabled = true;
                }
                else if (turnOff.Contains(isEnabled))
                {
                    _isEnabled = false;
                }
                else
                {
                }
                output = $"DeathMessages Enabled: {_isEnabled}";
            }
            else
            {
                _isEnabled = false;
                output = $"{s_fileName} file is missing, and it can not be enabled.";
            }
            await ctx.RespondAsync(StartEmbed(output));
        }
        [GroupCommand]
        public async Task DeathMessage(CommandContext ctx, [Description("Optional. Person you want to see kill you. If blank, will show generic death message.")] DiscordMember target = null)
        {
            // We're typing! (In all honesty, this is just used to avoid the 'async task not using await' messages I get.
            // IF we're missing the Death Messages dot JSON file, we're going to get disabled... Check for that.
            // If disabled, let the person know the command set is disabled, and return.
            if (!_isEnabled)
            {
                return;
            }
            // set some basic output incase something goes south.
            string line = "Something went wrong.";
            // if there is no target, we're going to "kill" the author.
            if (target == null)
            {
                int choice = Program.rnd.Next(0, _dmLines.Generic.Length);
                line = _dmLines.Generic[choice];
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
                    int choice = Program.rnd.Next(0, _dmLines.Active.Length);
                    line = _dmLines.Active[choice];
                }
                else if (opt == 2)
                {
                    // We picked a Passive death message type. Pick the exact line at random. Set it to the output.
                    int choice = Program.rnd.Next(0, _dmLines.Passive.Length);
                    line = _dmLines.Passive[choice];
                }
                // Give the selected death message to the user.
                await ctx.RespondAsync(StartEmbed(string.Format(line, ctx.Message.Author.Mention, target.Mention)));
            }
        }
        [Command("count")]
        [Description("Gets the total of each type of death message")]
        public async Task DeathMessageCount(CommandContext ctx)
        {
            // IF we're missing the Death Messages dot JSON file, we're going to get disabled... Check for that.
            // If disabled, let the person know the command set is disabled, and return.
            if (!_isEnabled)
            {
                return;
            }
            // We're typing here!
            // List all the types of Death Message types, and how many we have of each one.
            await ctx.RespondAsync(StartEmbed($"Active: {_dmLines.Active.Length}\r\nPassive: {_dmLines.Passive.Length}\r\nGeneric: {_dmLines.Generic.Length}"));
        }
        [Command("Add")]
        [Description("Adds a message to the Pending Death Message Queue. Generic Death Messages require just a [Killed] while any other death message requires both a [Killed] and a [Killer] tags. These will be replaced with the correct information in game, and in the usage of random DM generation.")]
        [Cooldown(1, 2, CooldownBucketType.Guild)]
        public async Task DeathMessageNew(CommandContext ctx, [RemainingText, Description("The Death Message you want added. See help section.")] string msg)
        {
            // IF we're missing the Death Messages dot JSON file, we're going to get disabled... Check for that.
            // If disabled, let the person know the command set is disabled, and return.
            if (!_isEnabled)
            {
                await ctx.RespondAsync("Death Messages Commands have been disabled. Please contact the bot owners.");
                return;
            }
            // Let Discord know we're typing.
            // Message the owner know some one wants to add a new death message, along with a code block to that message.
            // if the Queue is null, make it exist.
            if (_dmLines.Queue is null)
                _dmLines.Queue = new List<string>();
            // add the message to the queue list.
            _dmLines.Queue.Add(msg);
            // Store the new stuff to file...
            await StoreDeathMessages();
            // Tell the person that the message has been added to a 'queue'.
            await ctx.RespondAsync(StartEmbed("Your request has been added to the pending queue of requests."));
        }
        [Command("script")]
        [Description("Gets a script file for all the death messages on the bot.")]
        public async Task DeathMessagesScript(CommandContext ctx)
        {
            // IF we're missing the Death Messages dot JSON file, we're going to get disabled... Check for that.
            // If disabled, let the person know the command set is disabled, and return.
            if (!_isEnabled)
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
            await file.WriteLineAsync($"$deathMessage::genericCount   = {_dmLines.Generic.Length};");
            await file.WriteLineAsync($"$deathMessage::activeCount    = {_dmLines.Active.Length};");
            await file.WriteLineAsync($"$deathMessage::passiveCount   = {_dmLines.Passive.Length};\r\n");
            // Set count to 0. Iterate over each type of Death Message, and put it into the file.
            int count = 0;
            foreach (string death in _dmLines.Generic)
            {
                await file.WriteLineAsync(string.Format($"$deathMessage::generic{count}   = \"{death}\";", "%s", "%s"));
                count++;
            }
            // Reset the count...
            count = 0;
            foreach (string death in _dmLines.Active)
            {
                await file.WriteLineAsync(string.Format($"$deathMessage::active{count}   = \"{death}\";", "%s", "%s"));
                count++;
            }
            // Reset the count one last time...
            count = 0;
            foreach (string death in _dmLines.Passive)
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
        private DiscordEmbedBuilder StartEmbed(string description)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Description = description,
                Color = Program.colours[Program.rnd.Next(0, Program.colours.Length)]
            };
            return embed;
        }

        private async Task StoreDeathMessages()
        {
            string output = JsonConvert.SerializeObject(_dmLines);
            await File.WriteAllTextAsync(s_fileName, output);
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
        public List<string> Flagged { get; set; }
        [JsonProperty("queue")]
        public List<string> Queue { get; set; }
    }
}
