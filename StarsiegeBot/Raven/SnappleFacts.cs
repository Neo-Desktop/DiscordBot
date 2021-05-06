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
        private List<string> _facts;
        private bool _isEnabled;
        private static readonly string s_fileName = "Json/snapple.json";

        public SnappleFacts()
        {
            Console.WriteLine("Snapple Commands Loaded");
            _isEnabled = Load();
        }

        [GroupCommand, Description("Get a Snapple \"Real Fact\".")]
        public async Task Snapplefact(CommandContext ctx, [Description("[Optional] A Snapple Fact ID")] int snapId = 0)
        {
            // if we're not enabled, exit out.
            if (!_isEnabled)
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
                output = _facts[snapId];
            }
            catch (Exception)
            {
                // we couldn't find the exact one, give them a random one.
                output = _facts[Program.rnd.Next(0, _facts.Count)];
            }
            // give them what they want.
            await ctx.RespondAsync(StartEmbed(output));
        }

        [Command("count"), Description("Get a Snapple \"Real Fact\"."), Aliases("c")]
        public async Task SnappleCount(CommandContext ctx)
        {
            if (!_isEnabled)
                return;
            await ctx.TriggerTypingAsync();
            // trigger the typing, and report how many facts are in teh bank.
            await ctx.RespondAsync(StartEmbed(_facts.Count + " facts in the Snapple \"Real Facts\" Bank"));
        }

        [Command("load"), RequireOwner, Aliases("l")]
        public async Task Load(CommandContext ctx)
        {
            // trigger the typing.
            await ctx.TriggerTypingAsync();
            // start the response.
            DiscordEmbedBuilder embed = StartEmbed("Reloading Snapple Facts");
            // our old setting...
            embed.AddField("Old", (_isEnabled ? "Enabled" : "Disabled"));
            // run this, we dont care about its actual results.
            _ = Load();
            // report our new setting.
            embed.AddField("New", (_isEnabled ? "Enabled" : "Disabled"));
            // since the only thing that *should* go wrong is a missing file, report it if thats the case.
            if (!File.Exists(s_fileName))
                embed.WithFooter($":warning: {s_fileName} file is missing.");
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
            DiscordEmbedBuilder embed = StartEmbed("Snapple Toggle");
            embed.AddField("Old", (_isEnabled ? "Enabled" : "Disabled"));
            // check for file.
            if (File.Exists(s_fileName))
            {
                // compare desired input against on or off values.
                if (turnOn.Contains(isEnabled))
                {
                    _isEnabled = true;
                    embed.AddField("New", (_isEnabled ? "Enabled" : "Disabled"));
                }
                else if (turnOff.Contains(isEnabled))
                {
                    _isEnabled = false;
                    embed.AddField("New", (_isEnabled ? "Enabled" : "Disabled"));
                }
                else
                {
                    // we dont know what they were trying to do, throw an error in the footer with a :warning: emote.
                    embed.AddField("New", (_isEnabled ? "Enabled" : "Disabled"));
                    embed.WithFooter(":warning: Invalid command given?");
                }
            }
            else
            {
                // file is missing, disable this, regardless of their desires. Report the file is missing in the footer with a :warning:
                _isEnabled = false;
                embed.AddField("New", (_isEnabled ? "Enabled" : "Disabled"));
                embed.WithFooter($":warning: {s_fileName} file is missing, and it can not be enabled.");
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
            if (File.Exists(s_fileName))
            {
                string json = File.ReadAllText(s_fileName);
                _facts = JsonConvert.DeserializeObject<List<string>>(json);
                ret = true;
            }
            else
            {
                ret = false;
            }
            _isEnabled = ret;
            return ret;
        }
    }
}
