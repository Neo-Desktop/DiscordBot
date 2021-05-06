#pragma warning disable IDE0060 // Remove unused parameter
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StarsiegeBot
{
    [Group("GroupRole"), Aliases("gr")]
    [Description("Work in progress. Does nothing yet.")]
    [RequireGuild, RequirePermissions(Permissions.ManageRoles)]
    class GroupRoleManagement : BotSettings
    {
        public GroupRoleManagement()
        {
            Console.WriteLine("Group Role Management Loaded.");
        }

        [GroupCommand]
        public async Task ListGroups(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
        }

        [Command("add")]
        [Description("Work in progress. Does nothing yet.")]
        public async Task AddSelfRole(CommandContext ctx,
            [Description("The role to add.")] DiscordRole role,
            [Description("The price of the role for buyable roles. 0 for no cost. Free free to leave blank if not using Buyable roles.")] int price = 0,
            [Description("What group to put the role in. If not using groups, you can leave blank."), RemainingText] string groupName = null)
        {
            await ctx.TriggerTypingAsync();
            price = Math.Abs(price);

            // Fix price.
            // Verify group exists.
            // Since we allow roles to be in several groups, dont have to verify that.
            // 
            string gId = ctx.Guild.Id.ToString();

            if (GuildSettings[gId].SelfRoles.ContainsKey(groupName))
            {
                GuildSettings[gId].SelfRoles[groupName].Add(role.Id.ToString(), price);
                await ctx.RespondAsync(StartEmbed($"{role.Mention} added to the group `{groupName}` with the price of {price}"));
            }
            else
            {
                // group not found, `Create it?` or `Cancel`?

            }

        }

        [Command("delete")]
        [Description("Work in progress. Does nothing yet.")]
        public async Task DeleteSelfRole(CommandContext ctx,
            [Description("The role to delete.")] DiscordRole role,
            [Description("The group to remove the role from. If left blank, will remove from all groups."), RemainingText] string group = null)
        {
            await ctx.TriggerTypingAsync();
            // Verify role is in a group.
            // Verify group selected exists.
            // If group null, remove from all groups?
        }

        [Command("Edit")]
        [Description("Work in progress. Does nothing yet.")]
        public async Task EditRole(CommandContext ctx, DiscordRole role, int newPrice = 0, [RemainingText] string newGroup = null)
        {
            await ctx.TriggerTypingAsync();
        }


        private DiscordEmbedBuilder StartEmbed(string desc)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Description = desc,
                Color = Program.colours[3]
            };
            return embed;
        }
    }
}
