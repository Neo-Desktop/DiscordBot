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
        private readonly List<Quickchats> Quickchats;
        private readonly Random rnd = new Random();

        public Quickchat()
        {
            Console.WriteLine("Quick Chat Commands Loaded");
            // Load all the Quick Chat texts and sound wav file info.
            var json = "";
            using (var fs = File.OpenRead("quickchats.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();
            Dictionary<string, Quickchats> tempQC = JsonConvert.DeserializeObject<Dictionary<string, Quickchats>>(json);
            Quickchats = Enumerable.ToList(tempQC.Values);

        }

        [Command("script")]
        [Description("Gives the `say()` for use in the Quickchat.cs file.")]
        public async Task GetQuickChatScript(CommandContext ctx, [Description("ID of Quick chat to get script for.")] int index)
        {
            await ctx.TriggerTypingAsync();
            if (index < Quickchats.Count || index >= 0)
            {
                Quickchats chat = Quickchats[index];
                await ctx.RespondAsync("```" + chat.ToString() + "```");
            }
            else
            {
                await ctx.RespondAsync("The ID specified is out of bounds. 0-" + (Quickchats.Count - 1));
            }
        }

        [Command("check")]
        [Description("Runs a check to see how many WAV files are missing from all QC's.")]
        public async Task QuickChatCheck(CommandContext ctx)
        {
            int missingFiles = 0;
            for (int i = 0; i < Quickchats.Count; i++)
            {
                if (!File.Exists($"./qc/{Quickchats[i].soundFile}"))
                {
                    missingFiles++;
                    Console.Write($"{i}, ");
                }
            }
            await ctx.RespondAsync($"Missing files: {missingFiles}");
        }

        [GroupCommand]
        [Description("Gives a specific or random Quickchat. If that QC has a sound file that the bot knows, uploads the sound file too.")]
        public async Task QuickChat(CommandContext ctx, [Description("The ID of the quick Chat.")] int id = -1)
        {
            await ctx.TriggerTypingAsync();
            DiscordMessageBuilder msg = new DiscordMessageBuilder();

            Quickchats chat;

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
                chat = Quickchats[id];
                msg.Content = id + ": " + chat.text;

            }
            if (File.Exists($"./qc/{chat.soundFile}"))
            {
                FileStream sound = new FileStream($"./qc/{chat.soundFile}", FileMode.Open);
                msg.WithFile(chat.soundFile, sound);
                await ctx.RespondAsync(msg);
                sound.Close();
            }
            else
            {
                await ctx.RespondAsync(msg);
            }
        }

        [Command("search"), Aliases("s")]
        public async Task SearchQuickChats(CommandContext ctx, [RemainingText] string toSearch)
        {
            await ctx.TriggerTypingAsync();
            DiscordMessageBuilder msg = new DiscordMessageBuilder();
            msg.Content = "";
            for (int i = 0; i < Quickchats.Count; i++)
            {
                if (Quickchats[i].text.ToLower().Contains(toSearch.ToLower()))
                {
                    msg.Content += $"[{i}] {Quickchats[i].text}\r\n";
                }
            }
            if (msg.Content == "")
            {
                msg.Content = "No results found.";
            }
            await ctx.RespondAsync(msg);
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
