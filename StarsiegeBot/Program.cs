using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.VoiceNext;
using DSharpPlus.VoiceNext.Codec;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace StarsiegeBot
{
    class Program
    {
        public static DiscordMember notify;
        public readonly EventId BotEventId = new EventId(42, "Bot-Ex04");
        
        public DiscordClient Client { get; set; }
        public CommandsNextExtension Commands { get; set; }
        public VoiceNextExtension Voice { get; set; }

        protected string BotName { get; set; }

        private async Task StartTimer(CancellationToken cancellationToken)
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    string output = JsonConvert.SerializeObject(BotSettings.GuildSettings);
                    await File.WriteAllTextAsync("guildSettings.json", output);
                    await Task.Delay(TimeSpan.FromSeconds(120), cancellationToken);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Token Cancelled.");
                        break;
                    }
                }
            });
        }

        public static void Main(string[] args)
        {
            // since we cannot make the entry method asynchronous,
            // let's pass the execution to asynchronous code
            var prog = new Program(args);
            prog.
                RunBotAsync().
                GetAwaiter().
                GetResult();
        }

        public Program(string[] args)
        {
            if (args.Length > 0)
                BotName = args[0].ToLower();
            else
                BotName = "ssp";

            BotEventId = new EventId(276, BotName);
        }

        public async Task RunBotAsync()
        {

            // next, let's load the values from that file
            // to our client's configuration
            var cfg = new DiscordConfiguration
            {
                Token = Environment.GetEnvironmentVariable(BotName + "Token"),
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
                Intents = DiscordIntents.All,
            };

            // then we want to instantiate our client
            this.Client = new DiscordClient(cfg);

            // next, let's hook some events, so we know
            // what's going on

            this.Client.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(15) 
            });

            // up next, let's set up our commands
            var ccfg = new CommandsNextConfiguration
            {
                // let's use the string prefix defined in config.json
                StringPrefixes = new[] { Environment.GetEnvironmentVariable("CommandPrefix") },

                // enable responding in direct messages
                EnableDms = true,

                // enable mentioning the bot as a command prefix
                EnableMentionPrefix = true,

                UseDefaultCommandHandler = false
            };

            // and hook them up
            this.Commands = this.Client.UseCommandsNext(ccfg);

            // Hook all possible Event Handlers in the system.
            this.Client.ChannelCreated += this.Event_ChannelCreated;
            this.Client.ChannelDeleted += this.Event_ChannelDeleted;
            this.Client.ChannelPinsUpdated += this.Event_ChannelPinsUpdated;
            this.Client.ChannelUpdated += this.Event_ChannelUpdated;
            this.Client.ClientErrored += this.Event_ClientErrored;
            this.Client.DmChannelDeleted += this.Event_DmChannelDeleted;
            this.Client.GuildAvailable += this.Event_GuildAvailable;
            this.Client.GuildBanAdded += this.Event_GuildBanAdded;
            this.Client.GuildBanRemoved += this.Event_GuildBanRemoved;
            this.Client.GuildCreated += this.Event_GuildCreated;
            this.Client.GuildDeleted += this.Event_GuildDeleted;
            this.Client.GuildDownloadCompleted += this.Event_GuildDownloadCompleted;
            this.Client.GuildEmojisUpdated += this.Event_GuildEmojisUpdated;
            this.Client.GuildIntegrationsUpdated += this.Event_GuildIntegrationsUpdated;
            this.Client.GuildMemberAdded += this.Event_GuildMemberAdded;
            this.Client.GuildMemberRemoved += this.Event_GuildMemberRemoved;
            this.Client.GuildMembersChunked += this.Event_GuildMembersChunked;
            this.Client.GuildMemberUpdated += this.Event_GuildMemberUpdated;
            this.Client.GuildRoleCreated += this.Event_GuildRoleCreated;
            this.Client.GuildRoleDeleted += this.Event_GuildRoleDeleted;
            this.Client.GuildRoleUpdated += this.Event_GuildRoleUpdated;
            this.Client.GuildUnavailable += this.Event_GuildUnavailable;
            this.Client.GuildUpdated += this.Event_GuildUpdated;
            this.Client.Heartbeated += this.Event_Heartbeated;
            this.Client.InviteCreated += this.Event_InviteCreated;
            this.Client.InviteDeleted += this.Event_InviteDeleted;
            this.Client.MessageAcknowledged += this.Event_MessageAcknowledged;
            this.Client.MessageCreated += this.Event_MessageCreated;
            this.Client.MessageDeleted += this.Event_MessageDeleted;
            this.Client.MessageReactionAdded += this.Event_MessageReactionAdded;
            this.Client.MessageReactionRemoved += this.Event_MessageReactionRemoved;
            this.Client.MessageReactionRemovedEmoji += this.Event_MessageReactionRemovedEmoji;
            this.Client.MessageReactionsCleared += this.Event_MessageReactionsCleared;
            this.Client.MessagesBulkDeleted += this.Event_MessagesBulkDeleted;
            this.Client.MessageUpdated += this.Event_MessageUpdated;
            this.Client.PresenceUpdated += this.Event_PresenceUpdated;
            this.Client.Ready += this.Event_Ready;
            this.Client.Resumed += this.Event_Resumed;
            this.Client.SocketClosed += this.Event_SocketClosed;
            this.Client.SocketErrored += this.Event_SocketErrored;
            this.Client.SocketOpened += this.Event_SocketOpened;
            this.Client.TypingStarted += this.Event_TypingStarted;
            this.Client.UnknownEvent += this.Event_UnknownEvent;
            this.Client.UserSettingsUpdated += this.Event_UserSettingsUpdated;
            this.Client.UserUpdated += this.Event_UserUpdated;
            this.Client.VoiceServerUpdated += this.Event_VoiceServerUpdated;
            this.Client.VoiceStateUpdated += this.Event_VoiceStateUpdated;
            this.Client.WebhooksUpdated += this.Event_WebhooksUpdated;

            this.Client.ApplicationCommandCreated += this.Event_ApplicationCommandCreated;
            this.Client.ApplicationCommandDeleted += this.Event_ApplicationCommandDeleted;
            this.Client.ApplicationCommandUpdated += this.Event_ApplicationCommandUpdated;

            this.Client.InteractionCreated += this.Event_InteractionCreated;

            this.Commands.CommandExecuted += this.Event_CommandExecuted;
            this.Commands.CommandErrored += this.Event_CommandErrored;

            // up next, let's register our commands
            this.Commands.RegisterCommands<Commands>();
            // this.Commands.RegisterCommands<SnappleFacts>();
            this.Commands.RegisterCommands<BotSettings>(); // Main Folder. Test Items.

            // All these commands are in the STARSIEGE folder.
            //if (BotName.Contains("ssp"))
            //{
                this.Commands.RegisterCommands<Quickchat>();
                this.Commands.RegisterCommands<Functions>();
                this.Commands.RegisterCommands<DeathMessages>();
            //}
            Task thisTimer = StartTimer(new CancellationToken());

            // let's enable voice
            this.Voice = this.Client.UseVoiceNext();
            
            // finally, let's connect and log in
            await this.Client.ConnectAsync();

            // and this is to prevent premature quitting
            await Task.Delay(-1);
        }
        private Task Event_MessageCreated(DiscordClient d, MessageCreateEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_MessageCreated.");

            // Ignore all bots. We don't care.
            if (e.Author.IsBot)
            {
                return Task.CompletedTask;
            }

            CommandsNextExtension cnext = d.GetCommandsNext();
            DiscordMessage msg = e.Message;

            // Check if message has valid prefix.
            // json file loaded...
            int cmdStart = msg.GetStringPrefixLength("!");

            // if Guild is null, do nothing for now.
            if (e.Guild is null)
            {

            }
            else
            {
                string gId = e.Guild.Id.ToString();
                List<string> prefixes = BotSettings.GuildSettings["default"].Prefixes;
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

                // check each prefix. break on the one that is evoked.
                foreach (string item in prefixes)
                {
                    cmdStart = msg.GetStringPrefixLength(item);
                    if (cmdStart != -1) break;
                }
                // we didn't find a command prefix... Break.
                if (cmdStart == -1) return Task.CompletedTask;

                // Retrieve prefix.
                var prefix = msg.Content.Substring(0, cmdStart);
                var cmdString = msg.Content.Substring(cmdStart);

                // Retrieve full command string.
                var command = cnext.FindCommand(cmdString, out var args);
                if (command == null) return Task.CompletedTask;

                var ctx = cnext.CreateContext(msg, prefix, command, args);
                Task.Run(async () => await cnext.ExecuteCommandAsync(ctx));
            }

            return Task.CompletedTask;
        }

        private Task Event_CommandExecuted(CommandsNextExtension ext, CommandExecutionEventArgs e)
        {
            if (e.Context.Client.Guilds.Count > 75)
                e.Context.Client.Logger.LogCritical(BotEventId, $"Client is in {e.Context.Client.Guilds.Count}");
            // let's log the name of the command and user
            e.Context.Client.Logger.LogInformation(BotEventId, $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'");

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private async Task Event_CommandErrored(CommandsNextExtension ext, CommandErrorEventArgs e)
        {
            // let's log the error details
            e.Context.Client.Logger.LogError(BotEventId, $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", DateTime.Now);

        }

        private Task Event_InteractionCreated(DiscordClient d, InteractionCreateEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, $"{e.Interaction.User.Username} started an interaction. {e.Interaction.Data.Name}");
            return Task.CompletedTask;
        }
        private Task Event_ChannelCreated(DiscordClient d, ChannelCreateEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, $"{e.Channel.Name} was created on {e.Guild.Name}. Type: {e.Channel.Type}");
            return Task.CompletedTask;
        }
        private Task Event_ChannelDeleted(DiscordClient d, ChannelDeleteEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_ChannelDeleted.");
            return Task.CompletedTask;
        }
        private Task Event_ApplicationCommandCreated(DiscordClient d, ApplicationCommandEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_ApplicationCommandCreated.");
            return Task.CompletedTask;
        }
        private Task Event_ApplicationCommandDeleted(DiscordClient d, ApplicationCommandEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_ApplicationCommandDeleted.");
            return Task.CompletedTask;
        }
        private Task Event_ApplicationCommandUpdated(DiscordClient d, ApplicationCommandEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_ApplicationCommandUpdated.");
            return Task.CompletedTask;
        }
        private Task Event_ChannelPinsUpdated(DiscordClient d, ChannelPinsUpdateEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_ChannelPinsUpdated.");
            return Task.CompletedTask;
        }
        private Task Event_ChannelUpdated(DiscordClient d, ChannelUpdateEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_ChannelUpdated.");
            return Task.CompletedTask;
        }
        private Task Event_ClientErrored(DiscordClient d, ClientErrorEventArgs e)
        {
            // let's log the details of the error that just 
            // occured in our client
            d.Logger.LogError(BotEventId, e.Exception, "Exception occured");

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }
        private Task Event_DmChannelDeleted(DiscordClient d, DmChannelDeleteEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_ClientErrored.");
            return Task.CompletedTask;
        }
        private Task Event_GuildAvailable(DiscordClient d, GuildCreateEventArgs e)
        {
            string gId = e.Guild.Id.ToString();
            // this guild doesnt have settings, make a skel set for them.
            if (!BotSettings.GuildSettings.ContainsKey(gId))
            {
                Console.WriteLine("making config");
                GuildSettings item = new GuildSettings();
                item.UseAutoRoles = false;
                item.UseGlobalPrefix = true;
                item.UseLevelRoles = false;
                item.UseLevels = false;
                item.UseSelfRoles = false;
                item.AllowRolesPurchase = false;
                item.UseAutoRoles = false;
                item.LevelRoles = new Dictionary<string, int>();
                item.Prefixes = new List<string>();
                item.SelfRoles = new Dictionary<string, int>();
                // this line... is a test line.
                BotSettings.GuildSettings.Add(gId, item);
            }

            // let's log the name of the guild that was just
            // sent to our client
            d.Logger.LogInformation(BotEventId, $"Guild available: {e.Guild.Name}");

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }
        private Task Event_GuildBanAdded(DiscordClient d, GuildBanAddEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_GuildBanAdded.");
            return Task.CompletedTask;
        }
        private Task Event_GuildBanRemoved(DiscordClient d, GuildBanRemoveEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_GuildBanRemoved.");
            return Task.CompletedTask;
        }
        private Task Event_GuildCreated(DiscordClient d, GuildCreateEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_GuildCreated.");
            return Task.CompletedTask;
        }
        private Task Event_GuildDeleted(DiscordClient d, GuildDeleteEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_GuildDeleted.");
            return Task.CompletedTask;
        }
        private Task Event_GuildDownloadCompleted(DiscordClient d, GuildDownloadCompletedEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_GuildDownloadCompleted.");
            notify = e.Guilds[376937422010974209].Members[139548200099905536];
            return Task.CompletedTask;
        }
        private Task Event_GuildEmojisUpdated(DiscordClient d, GuildEmojisUpdateEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_GuildEmojisUpdated.");
            return Task.CompletedTask;
        }
        private Task Event_GuildIntegrationsUpdated(DiscordClient d, GuildIntegrationsUpdateEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_GuildIntegrationsUpdated.");
            return Task.CompletedTask;
        }
        private Task Event_GuildMemberAdded(DiscordClient d, GuildMemberAddEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_GuildMemberAdded.");
            return Task.CompletedTask;
        }
        private Task Event_GuildMemberRemoved(DiscordClient d, GuildMemberRemoveEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_GuildMemberRemoved.");
            return Task.CompletedTask;
        }
        private Task Event_GuildMembersChunked(DiscordClient d, GuildMembersChunkEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_GuildMembersChunked.");
            return Task.CompletedTask;
        }
        private Task Event_GuildMemberUpdated(DiscordClient d, GuildMemberUpdateEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_GuildMemberUpdated.");
            return Task.CompletedTask;
        }
        private Task Event_GuildRoleCreated(DiscordClient d, GuildRoleCreateEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_GuildRoleCreated.");
            return Task.CompletedTask;
        }
        private Task Event_GuildRoleDeleted(DiscordClient d, GuildRoleDeleteEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_GuildRoleDeleted.");
            return Task.CompletedTask;
        }
        private Task Event_GuildRoleUpdated(DiscordClient d, GuildRoleUpdateEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_GuildRoleUpdated.");
            return Task.CompletedTask;
        }
        private Task Event_GuildUnavailable(DiscordClient d, GuildDeleteEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_GuildUnavailable.");
            return Task.CompletedTask;
        }
        private Task Event_GuildUpdated(DiscordClient d, GuildUpdateEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_GuildUpdated.");
            return Task.CompletedTask;
        }
        private Task Event_Heartbeated(DiscordClient d, HeartbeatEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_Heartbeated.");
            return Task.CompletedTask;
        }
        private Task Event_InviteCreated(DiscordClient d, InviteCreateEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_InviteCreated.");
            return Task.CompletedTask;
        }
        private Task Event_InviteDeleted(DiscordClient d, InviteDeleteEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_InviteDeleted.");
            return Task.CompletedTask;
        }
        private Task Event_MessageAcknowledged(DiscordClient d, MessageAcknowledgeEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_MessageAcknowledged.");
            return Task.CompletedTask;
        }
        private Task Event_MessageDeleted(DiscordClient d, MessageDeleteEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_MessageDeleted.");
            return Task.CompletedTask;
        }
        private Task Event_MessageReactionAdded(DiscordClient d, MessageReactionAddEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_MessageReactionAdded.");
            return Task.CompletedTask;
        }
        private Task Event_MessageReactionRemoved(DiscordClient d, MessageReactionRemoveEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_MessageReactionRemoved.");
            return Task.CompletedTask;
        }
        private Task Event_MessageReactionRemovedEmoji(DiscordClient d, MessageReactionRemoveEmojiEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_MessageReactionRemovedEmoji.");
            return Task.CompletedTask;
        }
        private Task Event_MessageReactionsCleared(DiscordClient d, MessageReactionsClearEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_MessageReactionsCleared.");
            return Task.CompletedTask;
        }
        private Task Event_MessagesBulkDeleted(DiscordClient d, MessageBulkDeleteEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_MessagesBulkDeleted.");
            return Task.CompletedTask;
        }
        private Task Event_MessageUpdated(DiscordClient d, MessageUpdateEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_MessageUpdated.");
            return Task.CompletedTask;
        }
        private Task Event_PresenceUpdated(DiscordClient d, PresenceUpdateEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_PresenceUpdated.");
            return Task.CompletedTask;
        }
        private Task Event_Ready(DiscordClient d, ReadyEventArgs e)
        {
            // let's log the fact that this event occured
            d.Logger.LogInformation(BotEventId, $"Client is ready to process events. Client is in: {d.Guilds.Count} Guilds.");

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }
        private Task Event_Resumed(DiscordClient d, ReadyEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_Resumed.");
            return Task.CompletedTask;
        }
        private Task Event_SocketClosed(DiscordClient d, SocketCloseEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_SocketClosed.");
            return Task.CompletedTask;
        }
        private Task Event_SocketErrored(DiscordClient d, SocketErrorEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_SocketErrored.");
            return Task.CompletedTask;
        }
        private Task Event_SocketOpened(DiscordClient d, SocketEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_SocketOpened.");
            return Task.CompletedTask;
        }
        private Task Event_TypingStarted(DiscordClient d, TypingStartEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_TypingStarted.");
            return Task.CompletedTask;
        }
        private Task Event_UnknownEvent(DiscordClient d, UnknownEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_UnknownEvent.");
            return Task.CompletedTask;
        }
        private Task Event_UserSettingsUpdated(DiscordClient d, UserSettingsUpdateEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_UserSettingsUpdated.");
            return Task.CompletedTask;
        }
        private Task Event_UserUpdated(DiscordClient d, UserUpdateEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_UserUpdated.");
            return Task.CompletedTask;
        }
        private Task Event_VoiceServerUpdated(DiscordClient d, VoiceServerUpdateEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_VoiceServerUpdated.");
            return Task.CompletedTask;
        }
        private Task Event_VoiceStateUpdated(DiscordClient d, VoiceStateUpdateEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_VoiceStateUpdated.");
            return Task.CompletedTask;
        }
        private Task Event_WebhooksUpdated(DiscordClient d, WebhooksUpdateEventArgs e)
        {
            d.Logger.LogDebug(BotEventId, "Event_WebhooksUpdated.");
            return Task.CompletedTask;
        }

        // this structure will hold data from config.json
        public struct ConfigJson
        {
            [JsonProperty("token")]
            public string Token { get; private set; }

            [JsonProperty("prefix")]
            public string CommandPrefix { get; private set; }
        }
    }
}
