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
    class Quickchat : BaseCommandModule
    {
        private List<Quickchats> Quickchats;
        private readonly Random rnd = new Random();
        private bool QuickChatsEnabled;

        public Quickchat()
        {
            Console.WriteLine("Quick Chat Commands Loaded");
            // Check to see if the file exists... Otherwise disable all commands.
            if (File.Exists("quickchats.json"))
            {
                // Load the JSON file.
                var json = "";
                using (var fs = File.OpenRead("quickchats.json"))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();
                Dictionary<string, Quickchats> tempQC = JsonConvert.DeserializeObject<Dictionary<string, Quickchats>>(json);
                // set the Quick Chat object.
                Quickchats = Enumerable.ToList(tempQC.Values);
                // enable the commands.
                QuickChatsEnabled = true;
            }
            else
            {
                // file not found. disable commands, leave a comment in console.
                Console.WriteLine(" -- --- --- Quick Chat JSON file not found.");
                QuickChatsEnabled = false;
            }
        }

        [Command("load"), RequireOwner]
        [Description("[Onwer Only] Reloads the QuickChat files. If they exist. Also enables QC's if file is found.")]
        public async Task LoadQuickChats(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            if (File.Exists("quickchats.json"))
            {
                // Load the JSON file.
                var json = "";
                using (var fs = File.OpenRead("quickchats.json"))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();
                Dictionary<string, Quickchats> tempQC = JsonConvert.DeserializeObject<Dictionary<string, Quickchats>>(json);
                // set the Quick Chat object.
                Quickchats = Enumerable.ToList(tempQC.Values);
                // enable the commands.
                QuickChatsEnabled = true;
                await ctx.RespondAsync("Loading Quick Chats successul.");
            }
            else
            {
                QuickChatsEnabled = false;
                await ctx.RespondAsync("Loading Quick Chats failed. File does not exist.");
            }
        }


        [Command("script")]
        [Description("Gives the `say()` for use in the Quickchat.cs file.")]
        public async Task GetQuickChatScript(CommandContext ctx, [Description("ID of Quick chat to get script for.")] int index)
        {
            // trigger some typing on discord's side.
            await ctx.TriggerTypingAsync();
            // If we're disabled, let them know. And exit out.
            if (!QuickChatsEnabled)
            {
                await ctx.RespondAsync("Quick Chat Commands have been disabled. Please contact the bot owners.");
                return;
            }
            // We're good to go... If we have an index, pull it up.
            index = Math.Abs(index);
            // is our index within bounds?
            if (index < Quickchats.Count)
            {
                // yes it is, give the script to them.
                Quickchats chat = Quickchats[index];
                // why does this look broken, yet works? maybe its the ToString() function. I unno.
                await ctx.RespondAsync("```" + chat.ToString() + "```");
            }
            else
            {
                // we couldn't find it... Oh well, let the end user know.
                await ctx.RespondAsync("The ID specified is out of bounds. 0-" + (Quickchats.Count - 1));
            }
        }

        [Command("check")]
        [Description("Runs a check to see how many WAV files are missing from all QC's.")]
        public async Task QuickChatCheck(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            // If we're disabled, let them know. And exit out.
            if (!QuickChatsEnabled)
            {
                await ctx.RespondAsync("Quick Chat Commands have been disabled. Please contact the bot owners.");
                return;
            }
            // Start a count 
            int missingFiles = 0;
            for (int i = 0; i < Quickchats.Count; i++)
            {
                // If this file doesnt exist, log the ID in the console, and add to the count.
                if (!File.Exists($"./qc/{Quickchats[i].soundFile}"))
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
            // If we're disabled, let them know. And exit out.
            if (!QuickChatsEnabled)
            {
                await ctx.RespondAsync("Quick Chat Commands have been disabled. Please contact the bot owners.");
                return;
            }

            // Start a new Message Build.
            DiscordMessageBuilder msg = new DiscordMessageBuilder();
            Quickchats chat;
            // Make an absolute value out of what was given to us. 
            // -1 lets us know that we're doing random. A number out of bounds will also give a random number.
            if (id != -1)
                id = Math.Abs(id);
            if (id == -1 || id > Quickchats.Count)
            {
                int idnum = rnd.Next(Quickchats.Count);
                chat = Quickchats[idnum];
                msg.Content = $"[{idnum}] " + chat.text;
            }
            else
            {
                // we're given a number within bounds. Give them that QC.
                chat = Quickchats[id];
                msg.Content = id + ": " + chat.text;

            }

            // If the sound file to the select QC exists, upload it as well.
            if (File.Exists($"./qc/{chat.soundFile}"))
            {
                // Open the file.
                FileStream sound = new FileStream($"./qc/{chat.soundFile}", FileMode.Open);
                // feed it to the Message Builder.
                msg.WithFile(chat.soundFile, sound);
                // Send the message.
                await ctx.RespondAsync(msg);
                // Close the file.
                sound.Close();
            }
            else
            {
                // No file to upload, just send the the text QC.
                await ctx.RespondAsync(msg);
            }
        }

        [Command("search"), Aliases("s")]
        [Description("Searches all the [T]ext and [S]ound file names for the Search input.")]
        public async Task SearchQuickChats(CommandContext ctx, [RemainingText,Description("What to search for.")] string toSearch)
        {
            await ctx.TriggerTypingAsync();
            // If we're disabled, let them know. And exit out.
            if (!QuickChatsEnabled)
            {
                await ctx.RespondAsync("Quick Chat Commands have been disabled. Please contact the bot owners.");
                return;
            }

            // We're going to build a message.
            using StreamWriter file = new StreamWriter("tempQC.txt");
            DiscordMessageBuilder msg = new DiscordMessageBuilder();
            int Content = 0;
            // We're going to search each QC for the stuff in toSearch.
            for (int i = 0; i < Quickchats.Count; i++)
            {
                // do we have a match?
                if (Quickchats[i].text.ToLower().Contains(toSearch.ToLower()))
                {
                    // add it to the list!
                    await file.WriteLineAsync($"[{i}][T] {Quickchats[i].text}\r\n");
                    Content++;
                }
                if (Quickchats[i].soundFile.ToLower().Contains(toSearch.ToLower()))
                {
                    // add it to the list!
                    await file.WriteLineAsync($"[{i}][S] {Quickchats[i].text}\r\n");
                    Content++;
                }
            }
            file.Close();

            // If message content is still nothing, report we found nothing.
            if (Content == 0)
            {
                msg.Content = "No results found.";
                await ctx.RespondAsync(msg);
            }
            else
            {
                msg.Content = "Here are your results.";
                FileStream upload = new FileStream($"tempQC.txt", FileMode.Open);
                msg.WithFile("tempQC.txt", upload);
                await ctx.RespondAsync(msg);
                upload.Close();
            }
        }

        [Command("toggle"), Aliases("t")]
        [RequireOwner]
        [Description("[Onwer only] Enables or disables, or checks status of QuickChats.")]
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
                    QuickChatsEnabled = true;
                }
                else if (turnOff.Contains(isEnabled))
                {
                    QuickChatsEnabled = false;
                }
                else
                {
                    // we wont do anything here.
                }
                output = $"Quickchats Enabled: {QuickChatsEnabled}";
            }
            else
            {
                QuickChatsEnabled = false;
                output = "QuickChats.json file is missing, and it can not be enabled.";
            }
            await ctx.RespondAsync(output);
        }
    }
    public class Quickchats
    {
        public string text;
        public string soundFile;
        public override string ToString()
        {
            return $"say(0,0, \"{text}\", \"{soundFile}\");";
        }
    }

}
