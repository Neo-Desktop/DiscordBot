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
    [Group("notfact")]
    class NotFacts : BaseCommandModule
    {
        private List<string> _notfacts;
        private bool _isEnabled;
        private static readonly string s_fileName = "Json/notfacts.json";
        public NotFacts()
        {
            Console.WriteLine("NotFacts Command Loaded");
            _isEnabled = Load();
        }
        [GroupCommand, Description("Get a NotFact")]
        public async Task NotFact(CommandContext ctx, [Description("[Optional] A NotFact ID")] int notFactID = 0)
        {
            if (!_isEnabled)
                return;
            string output;
            notFactID--;
            try
            {
                output = _notfacts[notFactID];
            }
            catch (Exception)
            {
                output = _notfacts[Program.rnd.Next(0, _notfacts.Count)];
            }
            await ctx.RespondAsync(StartEmbed(output));
        }
        [Command("count"), Aliases("c"), Description("Get a count of the NotFacts")]
        public async Task NotFactCount(CommandContext ctx)
        {
            if (!_isEnabled)
                return;
            await ctx.RespondAsync(StartEmbed(_notfacts.Count + " NotFacts in the NotFacts Bank"));
        }
        [Command("load"), RequireOwner, Aliases("l")]
        public async Task Load(CommandContext ctx)
        {
            // trigger the typing.
            // start the response.
            DiscordEmbedBuilder embed = StartEmbed("Reloading NotFacts");
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
            // lower case teh input.
            isEnabled = isEnabled.ToLower();
            string[] turnOn = { "on", "true", "1" };
            string[] turnOff = { "off", "false", "0" };
            //start our response.
            DiscordEmbedBuilder embed = StartEmbed("NotFact Toggle");
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
                _notfacts = JsonConvert.DeserializeObject<List<string>>(json);
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
