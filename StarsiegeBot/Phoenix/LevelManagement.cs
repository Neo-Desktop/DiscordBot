#pragma warning disable IDE0060 // Remove unused parameter
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace StarsiegeBot
{
    [Group("Level")]
    [RequireGuild, RequirePermissions(Permissions.ManageRoles)]
    [Description("Work in progress. Does nothing yet.")]
    class LevelManagement : BotSettings
    {
        public LevelManagement()
        {
            Console.WriteLine("Level Management Loaded.");
        }
        [GroupCommand]
        [Description("Work in progress. Does nothing yet.")]
        public async Task ListLevels(CommandContext ctx)
        {
        }
        [Command("AddLevel"), Aliases("al")]
        [Description("Work in progress. Does nothing yet.")]
        public async Task AddLevel(CommandContext ctx, int newLevelGroup)
        {
        }
        [Command("DeleteLevel"), Aliases("dl")]
        [Description("Work in progress. Does nothing yet.")]
        public async Task DeleteLevel(CommandContext ctx, int oldLevelGroup)
        {
        }
        [Command("edit")]
        [Description("Work in progress. Does nothing yet.")]
        public async Task EditLevel(CommandContext ctx, int oldLeve, int newLevel)
        {
        }
    }
}
