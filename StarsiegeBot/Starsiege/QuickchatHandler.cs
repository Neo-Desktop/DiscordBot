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
    [Aliases("qc"), Group("quickchat")]
    class QuickchatHandler : BaseCommandModule
    {
        private Dictionary<string, Quickchat> _quickchats;
        private bool _isEnabled;
        private static readonly string s_fileName = "Json/quickchats.json";
        public QuickchatHandler()
        {
            Console.WriteLine("Quick Chat Handler Loaded");
            _isEnabled = LoadQuickChatsImpl();
        }
        /**
         * LoadQuickChatsImpl loads the quickchats.json file from the current path,
         * parses the file, and populates the private quickchats dictionary.
         **/
        private bool LoadQuickChatsImpl()
        {
            // Check to see if the file exists...
            if (File.Exists(s_fileName))
            {
                // Load the JSON file.
                string json = "";
                using (FileStream fs = File.OpenRead(s_fileName))
                using (StreamReader sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();
                _quickchats = JsonConvert.DeserializeObject<Dictionary<string, Quickchat>>(json);
                // enable the commands.
                return true;
            }
            // else, file not found. disable commands, leave a comment in console.
            Console.WriteLine(" -- --- --- Quick Chat JSON file not found.");
            return false;
        }
        [Command("load"), RequireOwner]
        [Description("[Owner Only] Reloads the QuickChat files, if they exist. Also enables QC's if the file is found.")]
        public async Task LoadQuickChats(CommandContext ctx)
        {
            string output;
            _isEnabled = LoadQuickChatsImpl();
            if (_isEnabled)
            {
                output = "Loading Quick Chats successful.";
            }
            else
            {
                output = "Loading Quick Chats failed. File does not exist.";
            }
            await ctx.RespondAsync(StartEmbed(output));
        }
        [Command("script")]
        [Description("Gives the `say()` for use in the Quickchat.cs file.")]
        public async Task GetQuickChatScript(CommandContext ctx, [Description("ID of Quick chat to get script for.")] int index)
        {
            // If we're disabled, let them know. And exit out.
            if (!_isEnabled)
            {
                return;
            }
            // trigger some typing on discord's side.
            // We're good to go... If we have an index, pull it up.
            index = Math.Abs(index);
            // is our index within bounds?
            if (index < _quickchats.Count)
            {
                // yes it is, give the script to them.
                Quickchat chat = _quickchats[index.ToString()];
                await ctx.RespondAsync("```" + chat.ToString() + "```");
                return;
            }
            // we couldn't find it... Oh well, let the end user know.
            await ctx.RespondAsync("The ID specified is out of bounds. 0-" + (_quickchats.Count - 1));
        }
        [Command("check")]
        [Description("Runs a check to see how many WAV files are missing from all QC's.")]
        public async Task QuickChatCheck(CommandContext ctx)
        {
            // If we're disabled, let them know. And exit out.
            if (!_isEnabled)
            {
                return;
            }
            // Start a count
            int missingFiles = 0;
            for (int i = 0; i < _quickchats.Count; i++)
            {
                // If this file doesnt exist, log the ID in the console, and add to the count.
                if (!File.Exists($"./qc/{_quickchats[i.ToString()].SoundFile}"))
                {
                    missingFiles++;
                    Console.Write($"{i}, ");
                }
            }
            // Reply to the message and let the user know we're missing X files.
            await ctx.RespondAsync(StartEmbed($"Missing files: {missingFiles}"));
        }
        [GroupCommand]
        [Description("Gives a specific or random Quickchat. If that QC has a sound file that the bot knows, uploads the sound file too.")]
        public async Task QuickChat(CommandContext ctx, [Description("The ID of the quick Chat.")] int id = -1)
        {
            // If we're disabled, let them know. And exit out.
            if (!_isEnabled)
            {
                return;
            }
            string output;
            // Start a new Message Build.
            DiscordMessageBuilder msg = new DiscordMessageBuilder();
            Quickchat chat;
            // try to get the quick chat, if it doesn't exist, return a random one
            try
            {
                if (id < -1)
                {
                    id = Math.Abs(id);
                }
                chat = _quickchats[id.ToString()];
                output = id + ": " + chat.Text;
            }
            catch (Exception)
            {
                // find a better quick chat.
                while (true)
                {
                    try
                    {
                        id = Program.rnd.Next(_quickchats.Count);
                        chat = _quickchats[id.ToString()];
                        output = $"[{id}] " + chat.Text;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    break;
                }
            }
            // If the sound file to the select QC exists, upload it as well.
            msg.Embed = StartEmbed(output);
            if (File.Exists($"./qc/{chat.SoundFile}"))
            {
                // Open the file.
                FileStream sound = new FileStream($"./qc/{chat.SoundFile}", FileMode.Open, FileAccess.Read);
                // feed it to the Message Builder.
                msg.WithFile(chat.SoundFile, sound);
            }
            // Send either the text QC or sound file if we have it
            // msg.Content = output;
            await ctx.RespondAsync(msg);
        }
        [Command("search"), Aliases("s")]
        [Cooldown(1, 5, CooldownBucketType.Global)]
        [Description("Searches all the [T]ext and [S]ound file names for the Search input.")]
        public async Task SearchQuickChats(CommandContext ctx, [RemainingText, Description("What to search for.")] string toSearch)
        {
            string output;
            // If we're disabled, let them know. And exit out.
            if (!_isEnabled)
            {
                await ctx.RespondAsync("Quick Chat Commands have been disabled. Please contact the bot owners.");
                return;
            }
            // We're going to build a message.
            StringBuilder sb = new StringBuilder();
            DiscordMessageBuilder msg = new DiscordMessageBuilder();
            int count = 0;
            // We're going to search each QC for the stuff in toSearch.
            foreach (KeyValuePair<string, Quickchat> qc in _quickchats)
            {
                // do we have a match?
                if (qc.Value.Text.ToLower().Contains(toSearch.ToLower()))
                {
                    // add it to the list!
                    sb.Append($"[{qc.Key}][T] {_quickchats[qc.Key].Text}\r\n");
                    count++;
                }
                if (qc.Value.SoundFile.ToLower().Contains(toSearch.ToLower()))
                {
                    // add it to the list!
                    sb.Append($"[{qc.Key}][S] {_quickchats[qc.Key].Text}\r\n");
                    count++;
                }
            }
            // If we have a message return it
            if (sb.Length > 0)
            {
                output = $"{ctx.Member.Mention}, there were {count} total matches for `{toSearch}`.";
                MemoryStream mr = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
                msg.WithFile($"{ctx.Member.DisplayName}_results.txt", mr, true);
            }
            else
            {
                // If message content is still nothing, report we found nothing.
                output = $"Sorry {ctx.Member.Mention}, no results were found for `{toSearch}`.";
            }
            msg.Content = output;
            await ctx.RespondAsync(msg);
        }
        [Command("flag")]
        [Description("Flags a Quick Chat for moderator checking.")]
        public async Task FlagQC(CommandContext ctx, [Description("ID of QC to flag.")] int id, [RemainingText, Description("The reason you believe it needs flagged.")] string flagReason = "")
        {
            Quickchat chat = _quickchats[id.ToString()];
            DiscordEmbedBuilder embed;
            foreach (FlagEvent item in chat.Flags)
            {
                if (item.UserId == ctx.Member.Id)
                {
                    item.Reason = flagReason;
                    embed = StartEmbed($"Each user can only flag an item once. Updating your reason to `{flagReason}`");
                    string dateReason = DateTime.UtcNow.ToString();
                    embed.WithFooter(dateReason);
                    await ctx.RespondAsync(embed);
                    //return;
                }
            }
            //if (chat.Flags.ContainsKey(ctx.Member.Id))
            //{
            //    chat.Flags[ctx.Member.Id] = flagReason;
            //    await ctx.RespondAsync(StartEmbed($"Each user can only flag an item once. Updating your reason to `{flagReason}`"));
            //}
            // chat.Flags.Add(ctx.Member.Id, flagReason);
            //FlagEvent event = new FlagEvent();
            embed = StartEmbed($"Flagging QC ID `{id}` with reason of `{flagReason}`. (F{chat.Flags.Count})");
            chat.IsFlagged = true;
            string output = JsonConvert.SerializeObject(_quickchats);
            await File.WriteAllTextAsync(s_fileName, output);
            await ctx.RespondAsync(embed);
        }
        [Command("flag-count")]
        public async Task FlagCount(CommandContext ctx)
        {
            int count = 0;
            foreach (KeyValuePair<string, Quickchat> chat in _quickchats)
            {
                if (chat.Value.IsFlagged)
                {
                    count++;
                }
            }
            await ctx.RespondAsync(StartEmbed($"There are {count} flagged Quick Chats."));
        }
        [Command("toggle"), Aliases("t")]
        [RequireOwner]
        [Description("[Owner only] Enables or disables, or checks status of QuickChats.\r\nTo turn the feature on, use `TRUE`, `ON`, or `1`\r\nTo turn off, use `FALSE`, `OFF`, or `0`")]
        public async Task ToggleQC(CommandContext ctx, [RemainingText] string isEnabled = null)
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
                if (turnOff.Contains(isEnabled))
                {
                    _isEnabled = false;
                }
                output = $"Quickchats {(_isEnabled ? "Enabled" : "Disabled")}";
            }
            else
            {
                _isEnabled = false;
                output = $"{s_fileName} file is missing, and it can not be enabled.";
            }
            await ctx.RespondAsync(StartEmbed(output));
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
    }
    public class Quickchat
    {
        [JsonProperty("text")]
        public string Text { get; protected set; }
        [JsonProperty("soundFile")]
        public string SoundFile { get; protected set; }
        public bool IsFlagged { get; set; }
        [JsonProperty("flags")]
        public List<FlagEvent> Flags { get; set; }
        public override string ToString()
        {
            return $"say(0,0, \"{Text}\", \"{SoundFile}\");";
        }
    }
    public class FlagEvent : IEquatable<FlagEvent>
    {
        public ulong UserId { get; set; }
        public string Reason { get; set; }
        public string Timestamp { get; set; }
        public FlagEvent(ulong UserId, string Reason, string Timestamp)
        {
            this.UserId = UserId;
            this.Reason = Reason;
            this.Timestamp = Timestamp;
        }
        public bool Equals(FlagEvent e)
        {
            return UserId == e.UserId;
        }
        public bool Equals(ulong s)
        {
            return UserId == s;
        }
        public override bool Equals(object o)
        {
            return false;
        }
        public override int GetHashCode()
        {
            return UserId.GetHashCode();
        }
    }
}
