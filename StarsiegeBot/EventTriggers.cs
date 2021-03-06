#pragma warning disable IDE0060 // Remove unused parameter
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace StarsiegeBot
{
    class Triggers
    {
        public readonly EventId BotEventId;
        public static DiscordMember notify;
        // protected string BotName { get; set; }
        public Triggers(EventId botEventId) => BotEventId = botEventId;
        public Task CommandExecuted(CommandsNextExtension ext, CommandExecutionEventArgs e)
        {
            //     e.Context.Client.Logger.LogCritical(BotEventId, $"Client is in {e.Context.Client.Guilds.Count}");
            // let's log the name of the command and user
            // e.Context.Client.Logger.LogInformation(BotEventId, $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'");
            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }
        public Task MessageCreated(DiscordClient d, MessageCreateEventArgs e)
        {
            // Ignore all bots. We don't care.
            if (e.Author.IsBot)
            {
                return Task.CompletedTask;
            }
            // d.Logger.LogDebug(BotEventId, "MessageCreated.");
            CommandsNextExtension cnext = d.GetCommandsNext();
            DiscordMessage msg = e.Message;
            // Check if message has valid prefix.
            int cmdStart = msg.GetStringPrefixLength(d.CurrentUser.Mention.Insert(2, "!"));
            List<string> prefixes = BotSettings.GuildSettings["default"].Prefixes;
            // if Guild is not null, we're on a guild. Check for Guild Prefixes.
            if (!(e.Guild is null))
            {
                string gId = e.Guild.Id.ToString();
                bool allowedToPost = false;
                if (BotSettings.GuildSettings[gId].DenyChannels is null)
                {
                    allowedToPost = true;
                }
                else if (BotSettings.GuildSettings[gId].DenyChannels.Contains("all"))
                {
                    if (BotSettings.GuildSettings[gId].AllowChannels.Contains(e.Channel.Id.ToString()))
                    {
                        allowedToPost = true;
                    }
                }
                else
                {
                    if (!BotSettings.GuildSettings[gId].DenyChannels.Contains(e.Channel.Id.ToString()))
                    {
                        allowedToPost = true;
                    }
                }
                if (allowedToPost)
                {
                    Console.WriteLine("We're allowed to post.");
                }
                if (!allowedToPost)
                {
                    Console.WriteLine("We're *NOT* allowed to post.");
                    return Task.CompletedTask;
                }



                // Check to see if the guild has settings.
                if (BotSettings.GuildSettings.ContainsKey(gId))
                {
                    // see if the guild wants global prefixes.
                    if (BotSettings.GuildSettings[gId].UseGlobalPrefix)
                    {
                        // add the Guild prefixes to the list of others.
                        prefixes = prefixes.Concat(BotSettings.GuildSettings[gId].Prefixes).ToList();
                    }
                    else
                    {
                        // dont use defaults? reset list to just the guild prefixes.
                        prefixes = BotSettings.GuildSettings[gId].Prefixes;
                    }
                }
            }


            // hard code the bot's mention as a possible prefix.
            prefixes.Add(d.CurrentUser.Mention.Insert(2, "!"));
            foreach (string item in prefixes)
            {
                cmdStart = msg.GetStringPrefixLength(item);
                if (cmdStart != -1) break;
            }
            // we didn't find a command prefix... Break.
            if (cmdStart == -1) return Task.CompletedTask;
            // Retrieve prefix.
            string prefix = msg.Content.Substring(0, cmdStart);
            string cmdString = msg.Content.Substring(cmdStart);
            // Retrieve full command string.
            Command command = cnext.FindCommand(cmdString, out string args);
            if (command == null) return Task.CompletedTask;
            CommandContext ctx = cnext.CreateContext(msg, prefix, command, args);
            Task.Run(async () => await cnext.ExecuteCommandAsync(ctx));
            return Task.CompletedTask;
        }
        public Task CommandErrored(CommandsNextExtension ext, CommandErrorEventArgs e)
        {
            e.Context.Client.Logger.LogError(BotEventId, $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", DateTime.Now);
            if (e.Exception is ChecksFailedException ex)
            {
                string output = "---\r\n";
                foreach (CheckBaseAttribute item in ex.FailedChecks)
                {
                    output += item.ToString() + "\r\n";
                }
                Console.WriteLine(output);
            }
            return Task.CompletedTask;
        }
        public Task InteractionCreated(DiscordClient d, InteractionCreateEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, $"{e.Interaction.User.Username} started an interaction. {e.Interaction.Data.Name}");
            return Task.CompletedTask;
        }
        public Task ChannelCreated(DiscordClient d, ChannelCreateEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, $"{e.Channel.Name} was created on {e.Guild.Name}. Type: {e.Channel.Type}");
            return Task.CompletedTask;
        }
        public Task ChannelDeleted(DiscordClient d, ChannelDeleteEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "ChannelDeleted.");
            return Task.CompletedTask;
        }
        public Task ApplicationCommandCreated(DiscordClient d, ApplicationCommandEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "ApplicationCommandCreated.");
            return Task.CompletedTask;
        }
        public Task ApplicationCommandDeleted(DiscordClient d, ApplicationCommandEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "ApplicationCommandDeleted.");
            return Task.CompletedTask;
        }
        public Task ApplicationCommandUpdated(DiscordClient d, ApplicationCommandEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "ApplicationCommandUpdated.");
            return Task.CompletedTask;
        }
        public Task ChannelPinsUpdated(DiscordClient d, ChannelPinsUpdateEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "ChannelPinsUpdated.");
            return Task.CompletedTask;
        }
        public Task ChannelUpdated(DiscordClient d, ChannelUpdateEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "ChannelUpdated.");
            return Task.CompletedTask;
        }
        public Task ClientErrored(DiscordClient d, ClientErrorEventArgs e)
        {
            // let's log the details of the error that just
            // occured in our client
            d.Logger.LogError(BotEventId, e.Exception, "Exception occured");
            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }
        public Task DmChannelDeleted(DiscordClient d, DmChannelDeleteEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "ClientErrored.");
            return Task.CompletedTask;
        }
        public Task GuildAvailable(DiscordClient d, GuildCreateEventArgs e)
        {
            string gId = e.Guild.Id.ToString();
            // this guild doesnt have settings, make a skel set for them.
            if (BotSettings.GuildSettings is null)
            { }
            else
            {
                if (!BotSettings.GuildSettings.ContainsKey(gId))
                {
                    Console.WriteLine("making config");
                    GuildSettings item = new GuildSettings
                    {
                        UseAutoRoles = false,
                        UseGlobalPrefix = true,
                        UseLevelRoles = false,
                        UseLevels = false,
                        UseSelfRoles = false,
                        AllowRolesPurchase = false,
                        UseWelcome = false,
                        WelcomeChannel = null,
                        WelcomeMessage = "",
                        SelfRoles = new Dictionary<string, Dictionary<string, int>>(),
                        LevelRoles = new Dictionary<int, List<string>>(),
                        Prefixes = new List<string>()
                    };
                    // this line... is a test line.
                    BotSettings.GuildSettings.Add(gId, item);
                }
            }
            // let's log the name of the guild that was just
            // sent to our client
            d.Logger.LogInformation(BotEventId, $"Guild available: {e.Guild.Name}");
            if (!(notify is null))
                // notify the *primary bot owner if the Guild count is above 75. So we can start bot verification process.
                if (d.Guilds.Count > 75 && !(notify is null))
                    notify.SendMessageAsync("**__WARNING__: BOT IS IN OVER 75 SERVERS.");
            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }
        public Task GuildBanAdded(DiscordClient d, GuildBanAddEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "GuildBanAdded.");
            return Task.CompletedTask;
        }
        public Task GuildBanRemoved(DiscordClient d, GuildBanRemoveEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "GuildBanRemoved.");
            return Task.CompletedTask;
        }
        public Task GuildCreated(DiscordClient d, GuildCreateEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "GuildCreated.");
            return Task.CompletedTask;
        }
        public Task GuildDeleted(DiscordClient d, GuildDeleteEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "GuildDeleted.");
            return Task.CompletedTask;
        }
        public Task GuildDownloadCompleted(DiscordClient d, GuildDownloadCompletedEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "GuildDownloadCompleted.");
            //if(e.Guilds[376937422010974209].Members.ContainsKey(139548200099905536))
            //notify = e.Guilds[376937422010974209].Members[139548200099905536];
            ulong desiredKey = 139548200099905536;
            foreach (KeyValuePair<ulong, DiscordGuild> item in e.Guilds)
            {
                DiscordGuild curr = item.Value;
                if (curr.Members.ContainsKey(desiredKey))
                {
                    notify = curr.Members[desiredKey];
                    break;
                }
            }
            notify.SendMessageAsync("Bot has started up");
            // notify the *primary bot owner if the Guild count is above 75. So we can start bot verification process.
            if (e.Guilds.Count > 75)
                notify.SendMessageAsync("**__WARNING__: BOT IS IN OVER 75 SERVERS.");
            return Task.CompletedTask;
        }
        public Task GuildEmojisUpdated(DiscordClient d, GuildEmojisUpdateEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "GuildEmojisUpdated.");
            return Task.CompletedTask;
        }
        public Task GuildIntegrationsUpdated(DiscordClient d, GuildIntegrationsUpdateEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "GuildIntegrationsUpdated.");
            return Task.CompletedTask;
        }
        public Task GuildMemberAdded(DiscordClient d, GuildMemberAddEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, $"GuildMemberAdded.");
            if (e.Member.IsBot)
            {
                return Task.CompletedTask;
            }
            string gId = e.Guild.Id.ToString();
            if (BotSettings.GuildSettings[gId].UseWelcome &&
                !(BotSettings.GuildSettings[gId].WelcomeChannel is null) &&
                !(BotSettings.GuildSettings[gId].WelcomeMessage is null))
            {
                string msg = WelcomeMessage.WelcomeMessageProcessing(BotSettings.GuildSettings[gId].WelcomeMessage, e.Member, e.Guild);
                BotSettings.GuildSettings[gId].WelcomeChannel.SendMessageAsync(msg);
            }
            return Task.CompletedTask;
        }
        public Task GuildMemberRemoved(DiscordClient d, GuildMemberRemoveEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "GuildMemberRemoved.");
            return Task.CompletedTask;
        }
        public Task GuildMembersChunked(DiscordClient d, GuildMembersChunkEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "GuildMembersChunked.");
            return Task.CompletedTask;
        }
        public Task GuildMemberUpdated(DiscordClient d, GuildMemberUpdateEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "GuildMemberUpdated.");
            return Task.CompletedTask;
        }
        public Task GuildRoleCreated(DiscordClient d, GuildRoleCreateEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "GuildRoleCreated.");
            return Task.CompletedTask;
        }
        public Task GuildRoleDeleted(DiscordClient d, GuildRoleDeleteEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "GuildRoleDeleted.");
            return Task.CompletedTask;
        }
        public Task GuildRoleUpdated(DiscordClient d, GuildRoleUpdateEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "GuildRoleUpdated.");
            return Task.CompletedTask;
        }
        public Task GuildUnavailable(DiscordClient d, GuildDeleteEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "GuildUnavailable.");
            return Task.CompletedTask;
        }
        public Task GuildUpdated(DiscordClient d, GuildUpdateEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "GuildUpdated.");
            return Task.CompletedTask;
        }
        public Task Heartbeated(DiscordClient d, HeartbeatEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "Heartbeated.");
            return Task.CompletedTask;
        }
        public Task InviteCreated(DiscordClient d, InviteCreateEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "InviteCreated.");
            return Task.CompletedTask;
        }
        public Task InviteDeleted(DiscordClient d, InviteDeleteEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "InviteDeleted.");
            return Task.CompletedTask;
        }
        public Task MessageAcknowledged(DiscordClient d, MessageAcknowledgeEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "MessageAcknowledged.");
            return Task.CompletedTask;
        }
        public Task MessageDeleted(DiscordClient d, MessageDeleteEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "MessageDeleted.");
            return Task.CompletedTask;
        }
        public Task MessageReactionAdded(DiscordClient d, MessageReactionAddEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "MessageReactionAdded.");
            return Task.CompletedTask;
        }
        public Task MessageReactionRemoved(DiscordClient d, MessageReactionRemoveEventArgs e)
        {
            //  d.Logger.LogDebug(BotEventId, "MessageReactionRemoved.");
            return Task.CompletedTask;
        }
        public Task MessageReactionRemovedEmoji(DiscordClient d, MessageReactionRemoveEmojiEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "MessageReactionRemovedEmoji.");
            return Task.CompletedTask;
        }
        public Task MessageReactionsCleared(DiscordClient d, MessageReactionsClearEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "MessageReactionsCleared.");
            return Task.CompletedTask;
        }
        public Task MessagesBulkDeleted(DiscordClient d, MessageBulkDeleteEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "MessagesBulkDeleted.");
            return Task.CompletedTask;
        }
        public Task MessageUpdated(DiscordClient d, MessageUpdateEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "MessageUpdated.");
            return Task.CompletedTask;
        }
        public Task PresenceUpdated(DiscordClient d, PresenceUpdateEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "PresenceUpdated.");
            return Task.CompletedTask;
        }
        public Task Ready(DiscordClient d, ReadyEventArgs e)
        {
            // let's log the fact that this event occured
            d.Logger.LogInformation(BotEventId, $"Client is ready to process events. Client is in: {d.Guilds.Count} Guilds.");
            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }
        public Task Resumed(DiscordClient d, ReadyEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "Resumed.");
            return Task.CompletedTask;
        }
        public Task SocketClosed(DiscordClient d, SocketCloseEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "SocketClosed.");
            return Task.CompletedTask;
        }
        public Task SocketErrored(DiscordClient d, SocketErrorEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "SocketErrored.");
            return Task.CompletedTask;
        }
        public Task SocketOpened(DiscordClient d, SocketEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "SocketOpened.");
            return Task.CompletedTask;
        }
        public Task TypingStarted(DiscordClient d, TypingStartEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "TypingStarted.");
            return Task.CompletedTask;
        }
        public Task UnknownEvent(DiscordClient d, UnknownEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "UnknownEvent.");
            return Task.CompletedTask;
        }
        public Task UserSettingsUpdated(DiscordClient d, UserSettingsUpdateEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "UserSettingsUpdated.");
            return Task.CompletedTask;
        }
        public Task UserUpdated(DiscordClient d, UserUpdateEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "UserUpdated.");
            return Task.CompletedTask;
        }
        public Task VoiceServerUpdated(DiscordClient d, VoiceServerUpdateEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "VoiceServerUpdated.");
            return Task.CompletedTask;
        }
        public Task VoiceStateUpdated(DiscordClient d, VoiceStateUpdateEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "VoiceStateUpdated.");
            return Task.CompletedTask;
        }
        public Task WebhooksUpdated(DiscordClient d, WebhooksUpdateEventArgs e)
        {
            // d.Logger.LogDebug(BotEventId, "WebhooksUpdated.");
            return Task.CompletedTask;
        }
    }
}
