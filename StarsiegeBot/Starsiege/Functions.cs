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
    [Group("functions"), Aliases("func")]
    class Functions : BaseCommandModule
    {
        private readonly DiscordColor[] colours = { DiscordColor.Aquamarine, DiscordColor.Azure, DiscordColor.Black, DiscordColor.Blue, DiscordColor.Blurple,
            DiscordColor.Brown, DiscordColor.Chartreuse, DiscordColor.CornflowerBlue, DiscordColor.Cyan, DiscordColor.DarkBlue, DiscordColor.DarkButNotBlack,
            DiscordColor.DarkGray, DiscordColor.DarkGreen, DiscordColor.DarkRed, DiscordColor.Gold, DiscordColor.Goldenrod, DiscordColor.Gray, DiscordColor.Grayple,
            DiscordColor.Green, DiscordColor.HotPink, DiscordColor.IndianRed, DiscordColor.LightGray, DiscordColor.Lilac, DiscordColor.Magenta, DiscordColor.MidnightBlue,
            DiscordColor.None, DiscordColor.NotQuiteBlack, DiscordColor.Orange, DiscordColor.PhthaloBlue, DiscordColor.PhthaloGreen, DiscordColor.Purple, DiscordColor.Red,
            DiscordColor.Rose, DiscordColor.SapGreen, DiscordColor.Sienna, DiscordColor.SpringGreen, DiscordColor.Teal, DiscordColor.Turquoise, DiscordColor.VeryDarkGray,
            DiscordColor.Violet, DiscordColor.Wheat, DiscordColor.White, DiscordColor.Yellow };
        private readonly Random rnd = new Random();
        private readonly Dictionary<string, SSFunction> ssFunctions;
        private readonly bool FunctionsEnabled;

        public Functions()
        {
            Console.WriteLine("Starsiege Function Commands Loaded");

            // Check for file, if not there, disable commands.
            if (File.Exists("functions.json"))
            {
                // Load the Functions JSON file. Has all information regarding Starsiege in-game Functions.
                var json = "";
                using (var fs = File.OpenRead("functions.json"))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();
                ssFunctions = JsonConvert.DeserializeObject<Dictionary<string, SSFunction>>(json);
                FunctionsEnabled = true;
            }
            else
            {
                FunctionsEnabled = false;
                Console.WriteLine(" --- --- --- Functions JSON not found.");
            }
        }

        [GroupCommand]
        [Description("Gives information on desired function.")]
        public async Task StarsiegeFunctions(CommandContext ctx, [RemainingText, Description("The function to attempt to look up.")] string command = "")
        {
            await ctx.TriggerTypingAsync();
            if (!FunctionsEnabled)
            {
                await ctx.RespondAsync("Function Commands have been disabled. Please contact the bot owners.");
                return;
            }

            // If the function command is empty make a list of commands in Starsiege. 
            if (command.Equals(string.Empty))
            {
                // Get a list of the Keys.
                List<string> keys = new List<string>(ssFunctions.Keys);
                // get a new output.
                string output = string.Empty;
                // loop over the keys.
                foreach (string item in keys)
                {
                    // if our output is empty, add the first time.
                    if (output.Equals(string.Empty))
                    {
                        output = $"`{item}`";
                    }
                    // output is nto empty, add second or further item with a comma.
                    else
                    {
                        output += $", `{item}`";
                    }
                }
                // send the output.
                await ctx.RespondAsync("List of known Functions in Starsiege: " + output);
            }

            // we have a command the want to try to find.
            else if (ssFunctions.TryGetValue(command.ToLower(), out SSFunction outItem))
            {
                // New Message Embed!
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
                // Set the title to the name of this function.
                embed.WithTitle(outItem.title);
                // set the timestamp to now... why, idk, its not really needed actually.
                embed.WithTimestamp(DateTime.Now);
                // add a random color to the left side bar of the embed.
                embed.WithColor(colours[rnd.Next(0, colours.Length)]);
                // Message Description, I dont remember what it is any more, sorry!
                embed.WithDescription(outItem.description);
                // iterate over each field item, and add them to the embed.
                foreach (Fields item in outItem.fields)
                {
                    embed.AddField(item.name, item.value, true);
                }
                // Build the Embed.
                DiscordEmbed em = embed.Build();
                // send the embed.
                await ctx.RespondAsync(embed: em);
            }
            // we couldn't find that exact command... let them know.
            else
            {
                await ctx.RespondAsync("That command wasn't food, pot-head.");
            }
        }

        [Command("count")]
        public async Task FunctionCount(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            // check ot make sure these commands are enabled. if disabled, report it.
            if (!FunctionsEnabled)
            {
                await ctx.RespondAsync("Function Commands have been disabled. Please contact the bot owners.");
                return;
            }
            // they wanted to know how many functions were documented... so tell them!
            await ctx.RespondAsync($"There are {ssFunctions.Count} functions in the known list.");
        }

    }

    public class SSFunction
    {
        public string name;
        public string title;
        public string description;
        public Fields[] fields;

        public override string ToString()
        {
            string fs = "";
            foreach (Fields field in fields)
            {
                fs += $"[{field}] ";
            }


            return $"Name: {name} || Title: {title} || Desc: {description} || Fields: {fs}";
        }
    }
    public class Fields
    {
        public string name;
        public string value;
        public override string ToString()
        {
            return $"Name: {name} || Value: {value}";
        }
    }
}
