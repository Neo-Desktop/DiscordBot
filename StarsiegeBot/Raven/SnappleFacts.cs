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
    [Group("snapple")]
    class SnappleFacts : BaseCommandModule
    {
        private List<string> facts;
        private bool IsEnabled;

        public SnappleFacts()
        {
            Console.WriteLine("Snapple Commands Loaded");
            IsEnabled = Load();
        }

        [GroupCommand, Description("Get a Snapple \"Real Fact\".")]
        public async Task Snapplefact(CommandContext ctx, [Description("[Optional] A Snapple Fact ID")] int snapId = -1)
        {
            await ctx.TriggerTypingAsync();
            string output;
            snapId--;
            try
            {
                output = facts[snapId]);
                return;
            }
            catch (Exception)
            {
                output = facts[Program.rnd.Next(0, facts.Count)];
            }
            await ctx.RespondAsync(StartEmbed(output));
        }

        [Command("count"), Description("Get a Snapple \"Real Fact\".")]
        public async Task SnappleCount(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            DiscordEmbedBuilder embed = StartEmbed(facts.Count + " facts in the Snapple \"Real Facts\" Bank");
            await ctx.RespondAsync(embed);
        }

        [Command("load"), RequireOwner]
        public async Task Load(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            DiscordEmbedBuilder embed = StartEmbed("Reloading Snapple Facts");
            embed.AddField("Old", (IsEnabled ? "Enabled" : "Disabled"));
            _ = Load();
            embed.AddField("New", (IsEnabled ? "Enabled" : "Disabled"));
            await ctx.RespondAsync(embed);
        }

        [Command("toggle")]
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
            if (File.Exists("snapple.json"))
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
                    embed.AddField("New", (IsEnabled ? "Enabled" : "Disabled"));
                    embed.WithFooter(":warning: Invalid command given.");
                }
            }
            else
            {
                IsEnabled = false;
                embed.AddField("New", (IsEnabled ? "Enabled" : "Disabled"));
                embed.WithFooter(":warning: snapple.json file is missing, and it can not be enabled.");
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
            if (File.Exists("snapple.json"))
            {
                string json = File.ReadAllText("snapple.json");
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
