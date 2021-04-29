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
    [Aliases("qc"), Group("quickchat")]
    class QuickchatHandler : BaseCommandModule
    {
        private Dictionary<string, Quickchat> quickchats;
        private bool IsQuickChatEnabled;

        public QuickchatHandler()
        {
            Console.WriteLine("Quick Chat Handler Loaded");
            IsQuickChatEnabled = LoadQuickChatsImpl();
        }

        /**
         * LoadQuickChatsImpl loads the quickchats.json file from the current path,
         * parses the file, and populates the private quickchats dictionary.
         **/
        private bool LoadQuickChatsImpl()
        {
            // Check to see if the file exists...
            if (File.Exists("quickchats.json"))
            {
                // Load the JSON file.
                var json = "";
                using (var fs = File.OpenRead("quickchats.json"))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))

                json = sr.ReadToEnd();
                quickchats = JsonConvert.DeserializeObject<Dictionary<string, Quickchat>>(json);

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
            await ctx.TriggerTypingAsync();

            IsQuickChatEnabled = LoadQuickChatsImpl();
            if (IsQuickChatEnabled)
            {
                await ctx.RespondAsync("Loading Quick Chats successful.");
                return;
            }

            await ctx.RespondAsync("Loading Quick Chats failed. File does not exist.");
        }

        [Command("script")]
        [Description("Gives the `say()` for use in the Quickchat.cs file.")]
        public async Task GetQuickChatScript(CommandContext ctx, [Description("ID of Quick chat to get script for.")] int index)
        {
            // trigger some typing on discord's side.
            await ctx.TriggerTypingAsync();

            // If we're disabled, let them know. And exit out.
            if (!IsQuickChatEnabled)
            {
                await ctx.RespondAsync("Quick Chat Commands have been disabled. Please contact the bot owners.");
                return;
            }

            // We're good to go... If we have an index, pull it up.
            index = Math.Abs(index);

            // is our index within bounds?
            if (index < quickchats.Count)
            {
                // yes it is, give the script to them.
                Quickchat chat = quickchats[index.ToString()];
                await ctx.RespondAsync("```" + chat.ToString() + "```");
                return;
            }

            // we couldn't find it... Oh well, let the end user know.
            await ctx.RespondAsync("The ID specified is out of bounds. 0-" + (quickchats.Count - 1));
        }

        [Command("check")]
        [Description("Runs a check to see how many WAV files are missing from all QC's.")]
        public async Task QuickChatCheck(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            // If we're disabled, let them know. And exit out.
            if (!IsQuickChatEnabled)
            {
                await ctx.RespondAsync("Quick Chat Commands have been disabled. Please contact the bot owners.");
                return;
            }

            // Start a count 
            int missingFiles = 0;
            for (int i = 0; i < quickchats.Count; i++)
            {
                // If this file doesnt exist, log the ID in the console, and add to the count.
                if (!File.Exists($"./qc/{quickchats[i.ToString()].soundFile}"))
                {
                    missingFiles++;
                    Console.Write($"{i}, ");
                }
            }

            // Reply to the message and let the user know we're missing X files.
            await ctx.RespondAsync($"Missing files: {missingFiles}");
        }

        [GroupCommand]
        [Description("Gives a specific or random Quickchat. If that QC has a sound file that the bot knows, uploads the sound file too.")]
        public async Task QuickChat(CommandContext ctx, [Description("The ID of the quick Chat.")] int id = -1)
        {
            await ctx.TriggerTypingAsync();
            string output;

            // If we're disabled, let them know. And exit out.
            if (!IsQuickChatEnabled)
            {
                await ctx.RespondAsync("Quick Chat Commands have been disabled. Please contact the bot owners.");
                return;
            }

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
                chat = quickchats[id.ToString()];
                output = id + ": " + chat.text;
            }
            catch (Exception)
            {
                // find a better quick chat.
                while (true)
                {
                    try
                    {
                        id = Program.rnd.Next(quickchats.Count);
                        chat = quickchats[id.ToString()];
                        output = $"[{id}] " + chat.text;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    break;
                }
            }

            // If the sound file to the select QC exists, upload it as well.
            if (File.Exists($"./qc/{chat.soundFile}"))
            {
                // Open the file.
                FileStream sound = new FileStream($"./qc/{chat.soundFile}", FileMode.Open, FileAccess.Read);
                
                // feed it to the Message Builder.
                msg.WithFile(chat.soundFile, sound);
            }

            // Send either the text QC or sound file if we have it
            msg.Content = output;
            await ctx.RespondAsync(msg);
        }

        [Command("search"), Aliases("s")]
        [Cooldown(1,5,CooldownBucketType.Global)]
        [Description("Searches all the [T]ext and [S]ound file names for the Search input.")]
        public async Task SearchQuickChats(CommandContext ctx, [RemainingText,Description("What to search for.")] string toSearch)
        {
            await ctx.TriggerTypingAsync();
            string output;

            // If we're disabled, let them know. And exit out.
            if (!IsQuickChatEnabled)
            {
                await ctx.RespondAsync("Quick Chat Commands have been disabled. Please contact the bot owners.");
                return;
            }

            // We're going to build a message.
            StringBuilder sb = new StringBuilder();
            DiscordMessageBuilder msg = new DiscordMessageBuilder();
            
            // We're going to search each QC for the stuff in toSearch.
            foreach (KeyValuePair<string, Quickchat> qc in quickchats)
            {
                // do we have a match?
                if (qc.Value.text.ToLower().Contains(toSearch.ToLower()))
                {
                    // add it to the list!
                    sb.Append($"[{qc.Key}][T] {quickchats[qc.Key].text}\r\n");
                }
                if (qc.Value.soundFile.ToLower().Contains(toSearch.ToLower()))
                {
                    // add it to the list!
                    sb.Append($"[{qc.Key}][S] {quickchats[qc.Key].text}\r\n");
                }
            }

            // If we have a message return it
            if (sb.Length > 0)
            {
                output = sb.ToString();

                if (sb.Length >= 2000)
                {
                    output = "Here are your results.";
                    MemoryStream mr = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
                    msg.WithFile("results.txt", mr, true);
                }
            }
            else
            {
                // If message content is still nothing, report we found nothing.
                output = "No results found.";
            }

            msg.Content = output;
            await ctx.RespondAsync(msg);
        }

        [Command("toggle"), Aliases("t")]
        [RequireOwner]
        [Description("[Owner only] Enables or disables, or checks status of QuickChats.")]
        public async Task ToggleQC (CommandContext ctx, [RemainingText] string isEnabled = null)
        {
            await ctx.TriggerTypingAsync();
            isEnabled = isEnabled.ToLower();
            string output;
            string[] turnOn = { "on", "true", "1" };
            string[] turnOff = { "off", "false", "0" };

            if (File.Exists("quickchats.json"))
            {
                if (turnOn.Contains(isEnabled))
                {
                    IsQuickChatEnabled = true;
                }
                if (turnOff.Contains(isEnabled))
                {
                    IsQuickChatEnabled = false;
                }

                output = $"Quickchats {(IsQuickChatEnabled ? "Enabled":"Disabled")}";
            }
            else
            {
                IsQuickChatEnabled = false;
                output = "QuickChats.json file is missing, and it can not be enabled.";
            }
            await ctx.RespondAsync(output);
        }
    }
    public class Quickchat
    {
        public string text;
        public string soundFile;
        public override string ToString()
        {
            return $"say(0,0, \"{text}\", \"{soundFile}\");";
        }
    }

}
