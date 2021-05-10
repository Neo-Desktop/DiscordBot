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
    [Group("role")]
    [RequireGuild, RequirePermissions(Permissions.ManageRoles)]
    class RoleManagement : BotSettings
    {
        public RoleManagement()
        {
            Console.WriteLine("Role Management Loaded.");
        }
        [Command("join")]
        [Description("Work in progress. Does nothing yet.")]
        public async Task JoinSelfRole(CommandContext ctx, DiscordRole role)
        {
            await ctx.TriggerTypingAsync();
        }
        [Command("join")]
        [Description("Work in progress. Does nothing yet.")]
        public async Task JoinSelfRole(CommandContext ctx, [RemainingText] string role)
        {
            await ctx.TriggerTypingAsync();
        }
        [Command("leave")]
        [Description("Work in progress. Does nothing yet.")]
        public async Task LeaveSelfRole(CommandContext ctx, DiscordRole role)
        {
            await ctx.TriggerTypingAsync();
        }
        [Command("leave")]
        [Description("Work in progress. Does nothing yet.")]
        public async Task LeaveSelfRole(CommandContext ctx, [RemainingText] string role)
        {
            await ctx.TriggerTypingAsync();
        }
        private bool VerifiedRole(DiscordGuild guild, DiscordRole role)
        {
            return false;
        }
        private bool VerifiedRole(DiscordGuild guild, string role)
        {
            return false;
        }
    }
}
