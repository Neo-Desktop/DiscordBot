using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Newtonsoft.Json;


namespace StarsiegeBot
{
    class NotFacts : BaseCommandModule
    {
        private List<string> notfacts;

        public NotFacts()
        {
            Console.WriteLine("NotFacts Command Loaded");
            string json = File.ReadAllText("notfacts.json");
            notfacts = JsonConvert.DeserializeObject<List<string>>(json);
        }

        [Command("notfact"), Description("Get a NotFact")]
        public async Task Snapplefact(CommandContext ctx, [Description("[Optional] A NotFact ID")] int notFactID = 0)
        {
            await ctx.TriggerTypingAsync();

            notFactID--;
            try
            {
                await ctx.RespondAsync(notfacts[notFactID]);
                return;
            }
            catch(Exception)
            {
                await ctx.RespondAsync(notfacts[Program.rnd.Next(0, notfacts.Count)]);
            }

            // await ctx.RespondAsync("**Warning:** The Id's used here are arbitrary");
        }

        [Command("notfactcount"), Description("Get a count of the NotFacts")]
        public async Task SnappleCount(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync(notfacts.Count + " NotFacts in the NotFacts Bank");
        }

    }
}
