#pragma warning disable IDE0060 // Remove unused parameter
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
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using System.Threading;

namespace StarsiegeBot
{
    [Group("GroupRole")]
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
        public async Task AddSelfRole(CommandContext ctx, [Description("The role to add.")]DiscordRole role, [Description("The price of the role for buyable roles. 0 for no cost. Free free to leave blank if not using Buyable roles.")]int price = 0, [Description("What group to put the role in. If not using groups, you can leave blank.")]string group = null)
        {
            await ctx.TriggerTypingAsync();
            if (price == -1)
            {
                return;
            }
        }

        [Command("delete")]
        [Description("Work in progress. Does nothing yet.")]
        public async Task DeleteSelfRole(CommandContext ctx, [Description("The role to delete.")] DiscordRole role, [Description("The group to remove the role from. If left blank, will remove from all groups.")]string group = null)
        {
            await ctx.TriggerTypingAsync();
        }

        [Command("Edit")]
        [Description("Work in progress. Does nothing yet.")]
        public async Task EditRole(CommandContext ctx, DiscordRole role, int newPrice = 0, [RemainingText] string newGroup = null)
        {
            await ctx.TriggerTypingAsync();
        }
    }
}
