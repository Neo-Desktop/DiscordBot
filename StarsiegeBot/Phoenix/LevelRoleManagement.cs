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
    [Group("LevelRole")]
    [RequireGuild, RequirePermissions(Permissions.ManageRoles)]
    class LevelRoleManagement : BotSettings
    {
        public LevelRoleManagement()
        {
            Console.WriteLine("Level Role Management Loaded.");
        }
        [Command("edit")]
        [Description("Work in progress. Does nothing yet.")]
        public async Task EditRole(CommandContext ctx, int oldLevel, int newLevel, DiscordRole role)
        {
            await ctx.TriggerTypingAsync();
        }
        [Command("edit")]
        [Description("Work in progress. Does nothing yet.")]
        public async Task EditRole(CommandContext ctx, int newLevel, [RemainingText] string role)
        {
            await ctx.TriggerTypingAsync();
        }
        [Command("remove")]
        [Description("Work in progress. Does nothing yet.")]
        public async Task RemoveRole(CommandContext ctx, DiscordRole role)
        {
            await ctx.TriggerTypingAsync();
        }
        [Command("remove")]
        [Description("Work in progress. Does nothing yet.")]
        public async Task RemoveRole(CommandContext ctx, [RemainingText] string roleName)
        {
            await ctx.TriggerTypingAsync();
        }
    }
}
