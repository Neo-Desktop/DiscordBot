using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
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
    [Group("ex")]
    [Hidden]

    [Description("Experimental commands. May break the bot.")]
    class ExperiemntalCommands : BaseCommandModule
    {
        [Command("exit"), RequireOwner]
        public async Task ExitProgram (CommandContext ctx)
        {
            await ctx.RespondAsync("Attempting a shutdown...");
            System.Environment.Exit(0);
        }

        [Command("Report")]
        public async Task Report (CommandContext ctx)
        {
            await ctx.RespondAsync("Report not generated");
        }
    }
}
