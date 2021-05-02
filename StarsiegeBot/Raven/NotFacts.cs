﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace StarsiegeBot
{
    [Group("notfact")]
    class NotFacts : BaseCommandModule
    {
        private List<string> notfacts;
        private bool IsEnabled;
        private readonly string fileName = "Json/notfacts.json";

        public NotFacts()
        {
            Console.WriteLine("NotFacts Command Loaded");
            IsEnabled = Load();
        }

        [GroupCommand, Description("Get a NotFact")]
        public async Task NotFact(CommandContext ctx, [Description("[Optional] A NotFact ID")] int notFactID = 0)
        {
            if (!IsEnabled)
                return;
            await ctx.TriggerTypingAsync();
            string output;
            notFactID--;
            try
            {
                output = notfacts[notFactID];
            }
            catch (Exception)
            {
                output = notfacts[Program.rnd.Next(0, notfacts.Count)];
            }
            await ctx.RespondAsync(StartEmbed(output));
        }

        [Command("count"), Aliases("c"), Description("Get a count of the NotFacts")]
        public async Task NotFactCount(CommandContext ctx)
        {
            if (!IsEnabled)
                return;
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync(StartEmbed(notfacts.Count + " NotFacts in the NotFacts Bank"));
        }

        [Command("load"), RequireOwner, Aliases("l")]
        public async Task Load(CommandContext ctx)
        {
            // trigger the typing.
            await ctx.TriggerTypingAsync();
            // start the response.
            DiscordEmbedBuilder embed = StartEmbed("Reloading NotFacts");
            // our old setting...
            embed.AddField("Old", (IsEnabled ? "Enabled" : "Disabled"));
            // run this, we dont care about its actual results.
            _ = Load();
            // report our new setting.
            embed.AddField("New", (IsEnabled ? "Enabled" : "Disabled"));
            // since the only thing that *should* go wrong is a missing file, report it if thats the case.
            if (!File.Exists(fileName))
                embed.WithFooter($":warning: {fileName} file is missing.");
            // send our results.
            await ctx.RespondAsync(embed);
        }

        [Command("toggle"), RequireOwner, Aliases("t")]
        public async Task Enable(CommandContext ctx, [RemainingText] string isEnabled = null)
        {
            // trigger the typing.
            await ctx.TriggerTypingAsync();
            // lower case teh input.
            isEnabled = isEnabled.ToLower();
            string[] turnOn = { "on", "true", "1" };
            string[] turnOff = { "off", "false", "0" };
            //start our response.
            DiscordEmbedBuilder embed = StartEmbed("NotFact Toggle");
            embed.AddField("Old", (IsEnabled ? "Enabled" : "Disabled"));
            // check for file.
            if (File.Exists(fileName))
            {
                // compare desired input against on or off values.
                if (turnOn.Contains(isEnabled))
                {
                    IsEnabled = true;
                    embed.AddField("New", (IsEnabled ? "Enabled" : "Disabled"));
                }
                else if (turnOff.Contains(isEnabled))
                {
                    IsEnabled = false;
                    embed.AddField("New", (IsEnabled ? "Enabled" : "Disabled"));
                }
                else
                {
                    // we dont know what they were trying to do, throw an error in the footer with a :warning: emote.
                    embed.AddField("New", (IsEnabled ? "Enabled" : "Disabled"));
                    embed.WithFooter(":warning: Invalid command given?");
                }
            }
            else
            {
                // file is missing, disable this, regardless of their desires. Report the file is missing in the footer with a :warning:
                IsEnabled = false;
                embed.AddField("New", (IsEnabled ? "Enabled" : "Disabled"));
                embed.WithFooter($":warning: {fileName} file is missing, and it can not be enabled.");
            }
            await ctx.RespondAsync(embed);
        }


        private DiscordEmbedBuilder StartEmbed(string desc)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Description = desc,
                Color = Program.colours[Program.rnd.Next(0, Program.colours.Length)]
            };
            return embed;
        }

        private bool Load()
        {
            bool ret;
            if (File.Exists(fileName))
            {
                string json = File.ReadAllText(fileName);
                notfacts = JsonConvert.DeserializeObject<List<string>>(json);
                ret = true;
            }
            else
            {
                ret = false;
            }
            IsEnabled = ret;
            return ret;
        }
    }
}