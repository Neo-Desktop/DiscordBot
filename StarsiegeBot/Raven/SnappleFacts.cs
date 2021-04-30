using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Newtonsoft.Json;


namespace StarsiegeBot
{
    class SnappleFacts : BaseCommandModule
    {
        private List<string> facts;

        public SnappleFacts()
        {
            Console.WriteLine("Snapple Commands Loaded");
            string json = File.ReadAllText("snapple.json");
            facts = JsonConvert.DeserializeObject<List<string>>(json);
        }

        [Command("snapple"), Description("Get a Snapple \"Real Fact\".")]
        public async Task Snapplefact(CommandContext ctx, [Description("[Optional] A Snapple Fact ID")]int snapId = -1)
        {
            await ctx.TriggerTypingAsync();

            snapId--;
            try
            {
                await ctx.RespondAsync(facts[snapId]);
                return;
            }
            catch(Exception)
            {
                await ctx.RespondAsync(facts[Program.rnd.Next(0, facts.Count)]);
            }

            //await ctx.RespondAsync("**Warning:** The Id's used here, are not the same as Snapple's fact numbers! (We renumbered them when we imported them.)");

        }

        [Command("snapplecount"), Description("Get a Snapple \"Real Fact\".")]
        public async Task SnappleCount(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync(facts.Count + " facts in the Snapple \"Real Facts\" Bank");
        }

    }
}
