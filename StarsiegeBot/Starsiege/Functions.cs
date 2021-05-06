using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StarsiegeBot
{
    [Group("functions"), Aliases("func")]
    class Functions : BaseCommandModule
    {
        private Dictionary<string, SSFunction> _ssFunctions;
        private bool _functionsEnabled;
        private static readonly string s_fileName = "Json/functions.json";
        public Functions()
        {
            Console.WriteLine("Starsiege Function Commands Loaded");

            // Check for file, if not there, disable commands.
            if (File.Exists(s_fileName))
            {
                // Load the Functions JSON file. Has all information regarding Starsiege in-game Functions.
                string json = "";
                using (FileStream fs = File.OpenRead(s_fileName))
                using (StreamReader sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();
                _ssFunctions = JsonConvert.DeserializeObject<Dictionary<string, SSFunction>>(json);
                _functionsEnabled = true;
            }
            else
            {
                _functionsEnabled = false;
                Console.WriteLine(" --- --- --- Functions JSON not found.");
            }
        }

        [GroupCommand]
        [Description("Gives information on desired function.")]
        public async Task StarsiegeFunctions(CommandContext ctx, [RemainingText, Description("The function to attempt to look up.")] string command = "")
        {
            await ctx.TriggerTypingAsync();
            if (!_functionsEnabled)
            {
                await ctx.RespondAsync("Function Commands have been disabled. Please contact the bot owners.");
                return;
            }

            // If the function command is empty make a list of commands in Starsiege. 
            if (command.Equals(string.Empty))
            {
                // Get a list of the Keys.
                List<string> keys = new List<string>(_ssFunctions.Keys);
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
            else if (_ssFunctions.TryGetValue(command.ToLower(), out SSFunction outItem))
            {
                // New Message Embed!
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
                // Set the title to the name of this function.
                embed.WithTitle(outItem.title);
                // set the timestamp to now... why, idk, its not really needed actually.
                embed.WithTimestamp(DateTime.Now);
                // add a random color to the left side bar of the embed.
                embed.WithColor(Program.colours[Program.rnd.Next(0, Program.colours.Length)]);
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
            if (!_functionsEnabled)
            {
                await ctx.RespondAsync("Function Commands have been disabled. Please contact the bot owners.");
                return;
            }
            // they wanted to know how many functions were documented... so tell them!
            await ctx.RespondAsync($"There are {_ssFunctions.Count} functions in the known list.");
        }

        [Command("toggle"), Aliases("t")]
        [RequireOwner]
        public async Task ToggleFunctions(CommandContext ctx, [RemainingText] string isEnabled = null)
        {
            await ctx.TriggerTypingAsync();

            isEnabled = isEnabled.ToLower();

            string output;

            string[] turnOn = { "on", "true", "1" };
            string[] turnOff = { "off", "false", "0" };

            if (File.Exists(s_fileName))
            {
                if (turnOn.Contains(isEnabled))
                {
                    _functionsEnabled = true;
                }
                else if (turnOff.Contains(isEnabled))
                {
                    _functionsEnabled = false;
                }
                else
                {

                }
                output = $"Functions Enabled: {_functionsEnabled}";
            }
            else
            {
                _functionsEnabled = false;
                output = $"{s_fileName} file is missing, and it can not be enabled.";
            }
            await ctx.RespondAsync(output);
        }
        [Command("load"), RequireOwner]
        public async Task LoadFunctions(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            // Check for file, if not there, disable commands.
            if (File.Exists(s_fileName))
            {
                // Load the Functions JSON file. Has all information regarding Starsiege in-game Functions.
                string json = "";
                using (FileStream fs = File.OpenRead(s_fileName))
                using (StreamReader sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();
                _ssFunctions = JsonConvert.DeserializeObject<Dictionary<string, SSFunction>>(json);
                _functionsEnabled = true;
                await ctx.RespondAsync("Loading Quick Chats successul.");
            }
            else
            {
                _functionsEnabled = false;
                await ctx.RespondAsync("Loading Functions failed. File does not exist.");
            }
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


            return $"Name: {name} || Title: {title} || Desc: {description} || Fields: {{{fs}}}";
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
