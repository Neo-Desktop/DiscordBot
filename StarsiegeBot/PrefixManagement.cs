using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace StarsiegeBot
{
    [Group("prefix")]
    [RequireGuild, RequirePermissions(Permissions.ManageRoles)]
    class PrefixManagement : BotSettings
    {
        public PrefixManagement()
        {
            Console.WriteLine("Prefix Management Loaded.");
        }
        [GroupCommand]
        public async Task GetPrefixes(CommandContext ctx)
        {
            // store the Guild ID as a string.
            string guild = ctx.Guild.Id.ToString();
            // if this server doesnt have a settings for whatever reason, let them know... why am i not creating it?
            if (!GuildSettings.ContainsKey(guild))
            {
                await ctx.RespondAsync("No guild settings found.");
            }
            else
            {
                // we have server settings... why does this give so much info? it should only show only prefix(es)
                GuildSettings server = GuildSettings[guild];
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                {
                    Description = $"{ctx.Guild.Name} Prefixes."
                };
                int count = 1;
                // for each prefix, list it in the embed as a field. Show a count.
                foreach (string item in server.Prefixes)
                {
                    embed.AddField($"Prefix {count}", item, true);
                    count++;
                }
                await ctx.RespondAsync(embed);
            }
        }
        [Command("Add")]
        [Description("Adds a new prefix to the Server's list of prefixes.")]
        public async Task AddNewPrefix(CommandContext ctx, string prefix)
        {
            string gId = ctx.Guild.Id.ToString();
            // add the prefix to the server specific settings.
            if (GuildSettings[gId].Prefixes.Count < 25)
            {
                GuildSettings[gId].Prefixes.Add(prefix);
                await ctx.RespondAsync($"Added `{prefix}` as a command prefix.");
            }
            else
            {
                await ctx.RespondAsync("There is a maximum of 25 prefixes allowed.");
            }
        }
        [Command("delete"), Aliases("del")]
        [Description("Removes the specified prefix from accepted prefixes.")]
        public async Task RemovePrefix(CommandContext ctx, string prefix)
        {
            string gId = ctx.Guild.Id.ToString();
            // they have only one prefix and disallow global prefixes, dont let them delete the prefix.
            if (GuildSettings[gId].Prefixes.Count == 1 && !GuildSettings[gId].UseGlobalPrefix)
            {
                await ctx.RespondAsync("Can't remove the last usable prefix from the bot. Add a new prefix, or enable Global Prefix.");
                return;
            }
            // they pass the vibe check, delete the prefix.
            GuildSettings[ctx.Guild.Id.ToString()].Prefixes.Remove(prefix);
            await ctx.RespondAsync($"Removed `{prefix}` as a command prefix.");
        }
        [Command("global")]
        [Description("View or edit the status of allowing Global Prefix(es).")]
        public async Task UseGlobalPrefixes(CommandContext ctx)
        {
            await ctx.RespondAsync($"Global Prefix usage has been turned **{(GuildSettings[ctx.Guild.Id.ToString()].UseGlobalPrefix ? "on" : "off")}**");
        }
        [Command("global"), Aliases("g")]
        [Description("View or edit the status of allowing Global Prefix(es).")]
        public async Task UseGlobalPrefixes(CommandContext ctx, [Description("True or False")] bool isEnabled)
        {
            // check to make sure that they aren't trying to turn off global prefixes with out first adding a server sepcific prefix.
            int prefixCount = GuildSettings[ctx.Guild.Id.ToString()].Prefixes.Count;
            if (prefixCount <= 0 && !isEnabled)
            {
                await ctx.RespondAsync("You have to add at least one server prefix before you can turn off Global Prefux usage.");
                return;
            }
            // they passed teh vibe check, let them turn off global prefixes.
            GuildSettings[ctx.Guild.Id.ToString()].UseGlobalPrefix = isEnabled;
            await ctx.RespondAsync($"Global Prefix usage has been turned **{(isEnabled ? "on" : "off")}**");
        }
        [Command("global")]
        public async Task UseGlobalPrefixes(CommandContext ctx, [Description("ON or OFF"), RemainingText] string isEnabled = null)
        {
            isEnabled = isEnabled.ToLower();
            string gId = ctx.Guild.Id.ToString();
            int prefixCount = GuildSettings[gId].Prefixes.Count;
            string[] turnOn = { "on", "true", "1" };
            string[] turnOff = { "off", "false", "0" };
            DiscordEmbedBuilder embed = StartEmbed("Global Prefix Usage");
            embed.AddField("Old", Program.YesNo(GuildSettings[gId].UseGlobalPrefix));
            // since we're just trying to make the end user's life easier by different means of turning this on/off. redirect work load that way.
            if (turnOn.Contains(isEnabled))
            {
                await UseGlobalPrefixes(ctx, true);
                GuildSettings[gId].UseGlobalPrefix = true;
                embed.AddField("New", Program.YesNo(GuildSettings[gId].UseGlobalPrefix));
            }
            else if (turnOff.Contains(isEnabled))
            {
                if (prefixCount > 0)
                {
                    GuildSettings[gId].UseGlobalPrefix = false;
                    embed.AddField("New", Program.YesNo(GuildSettings[gId].UseGlobalPrefix));
                }
                else
                {
                    embed = EmbedError("Could not turn off Global Prefix Usage. (Are you missing a server prefix?)");
                }
            }
            else
            {
            }
            await ctx.RespondAsync(embed);
        }
        private DiscordEmbedBuilder StartEmbed(string desc)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Description = desc,
                Color = Program.colours[1]
            };
            return embed;
        }
        private DiscordEmbedBuilder EmbedError(string desc)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Description = desc,
                Color = Program.colours[^1]
            };
            return embed;
        }
    }
}
