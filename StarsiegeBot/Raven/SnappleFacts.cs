using DSharpPlus.CommandsNext;
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
    [Group("snapple"), Aliases("snap")]
    class SnappleFacts : BaseCommandModule
    {
        private List<string> facts;
        private bool IsEnabled;
        private readonly string fileName = "Json/snapple.json";

        public SnappleFacts()
        {
            Console.WriteLine("Snapple Commands Loaded");
            IsEnabled = Load();
        }

        [GroupCommand, Description("Get a Snapple \"Real Fact\".")]
        public async Task Snapplefact(CommandContext ctx, [Description("[Optional] A Snapple Fact ID")] int snapId = 0)
        {
            // if we're not enabled, exit out.
            if (!IsEnabled)
                return;
            // trigger typing.
            await ctx.TriggerTypingAsync();
            // store our output.    
            string output;
            // @neo, why is this here?
            snapId--;
            try
            {
                // attempt to get the desired fact.
                output = facts[snapId];
            }
            catch (Exception)
            {
                // we couldn't find the exact one, give them a random one.
                output = facts[Program.rnd.Next(0, facts.Count)];
            }
            // give them what they want.
            await ctx.RespondAsync(StartEmbed(output));
        }

        [Command("count"), Description("Get a Snapple \"Real Fact\"."), Aliases("c")]
        public async Task SnappleCount(CommandContext ctx)
        {
            if (!IsEnabled)
                return;
            await ctx.TriggerTypingAsync();
            // trigger the typing, and report how many facts are in teh bank.
            await ctx.RespondAsync(StartEmbed(facts.Count + " facts in the Snapple \"Real Facts\" Bank"));
        }

        [Command("load"), RequireOwner, Aliases("l")]
        public async Task Load(CommandContext ctx)
        {
            // trigger the typing.
            await ctx.TriggerTypingAsync();
            // start the response.
            DiscordEmbedBuilder embed = StartEmbed("Reloading Snapple Facts");
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
        public async Task Enable (CommandContext ctx, [RemainingText]string isEnabled = null)
        {
            // trigger the typing.
            await ctx.TriggerTypingAsync();
            // lower case teh input.
            isEnabled = isEnabled.ToLower();
            string[] turnOn = { "on", "true", "1" };
            string[] turnOff = { "off", "false", "0" };
            //start our response.
            DiscordEmbedBuilder embed = StartEmbed("Snapple Toggle");
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
                facts = JsonConvert.DeserializeObject<List<string>>(json);
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
