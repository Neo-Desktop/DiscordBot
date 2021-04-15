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

        public Functions()
        {
            Console.WriteLine("Starsiege Function Commands Loaded");
            // Load the Functions JSON file. Has all information regarding Starsiege in-game Functions.
            var json = "";
            using (var fs = File.OpenRead("functions.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();
            ssFunctions = JsonConvert.DeserializeObject<Dictionary<string, SSFunction>>(json);
        }

        [GroupCommand]
        [Description("Gives information on desired function.")]
        public async Task StarsiegeFunctions(CommandContext ctx, [RemainingText, Description("The function to attempt to look up.")] string command = "")
        {
            await ctx.TriggerTypingAsync();
            if (command.Equals(string.Empty))
            {
                List<string> keys = new List<string>(ssFunctions.Keys);
                string output = string.Empty;
                foreach (string item in keys)
                {
                    if (output.Equals(string.Empty))
                    {
                        output = $"`{item}`";
                    }
                    else
                    {
                        output += $", `{item}`";
                    }
                }
                await ctx.RespondAsync("List of known Functions in Starsiege: " + output);
            }


            else if (ssFunctions.TryGetValue(command.ToLower(), out SSFunction outItem))
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
                embed.WithTitle(outItem.title);
                embed.WithTimestamp(DateTime.Now);
                embed.WithColor(colours[rnd.Next(0, colours.Length)]);
                embed.WithDescription(outItem.description);
                foreach (Fields item in outItem.fields)
                {
                    embed.AddField(item.name, item.value, true);
                }
                DiscordEmbed em = embed.Build();
                await ctx.RespondAsync(embed: em);
            }
            else
            {
                await ctx.RespondAsync("That command wasn't food, pot-head.");
            }
        }

        [Command("count")]
        public async Task FunctionCount(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
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
