using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace StarsiegeBot
{
    class Program
    {
        public readonly EventId BotEventId;
        public  static Triggers Events;
        
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
            Events = new Triggers(BotEventId);
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

            // up next, let's register our commands
            this.Commands.RegisterCommands<Commands>();
            this.Commands.RegisterCommands<BotSettings>(); // Main Folder. Test Items.
            this.Commands.RegisterCommands<WelcomeMessage>(); // Main Folder. Test Items.
            this.Commands.RegisterCommands<PrefixManagement>();
            // this.Commands.RegisterCommands<SnappleFacts>();
            // this.Commands.RegisterCommands<RoleManagement>(); // Main Folder. Test Items.
            // this.Commands.RegisterCommands<LevelRoleManagement>(); // Main Folder. Test Items.
            // this.Commands.RegisterCommands<GroupRoleManagement>(); // Main Folder. Test Items.
            // this.Commands.RegisterCommands<LevelManagement>(); // Main Folder. Test Items.
            // this.Commands.RegisterCommands<GroupManagement>(); // Main Folder. Test Items.
            // this.Commands.RegisterCommands<StarsiegeCommands>();

            // All these commands are in the STARSIEGE folder.
            //if (BotName.Contains("ssp"))
            //{
            this.Commands.RegisterCommands<QuickchatHandler>();
            this.Commands.RegisterCommands<Functions>();
            this.Commands.RegisterCommands<DeathMessages>();
            this.Commands.RegisterCommands<GameInfo>();
            //}
            Task thisTimerf = StartTimer(new CancellationToken());

            // let's enable voice
            this.Voice = this.Client.UseVoiceNext();

            // Hook all possible Event Handlers in the system.
            this.Client.ChannelCreated += Events.ChannelCreated;
            this.Client.ChannelDeleted += Events.ChannelDeleted;
            this.Client.ChannelPinsUpdated += Events.ChannelPinsUpdated;
            this.Client.ChannelUpdated += Events.ChannelUpdated;
            this.Client.ClientErrored += Events.ClientErrored;
            this.Client.DmChannelDeleted += Events.DmChannelDeleted;
            this.Client.GuildAvailable += Events.GuildAvailable;
            this.Client.GuildBanAdded += Events.GuildBanAdded;
            this.Client.GuildBanRemoved += Events.GuildBanRemoved;
            this.Client.GuildCreated += Events.GuildCreated;
            this.Client.GuildDeleted += Events.GuildDeleted;
            this.Client.GuildDownloadCompleted += Events.GuildDownloadCompleted;
            this.Client.GuildEmojisUpdated += Events.GuildEmojisUpdated;
            this.Client.GuildIntegrationsUpdated += Events.GuildIntegrationsUpdated;
            this.Client.GuildMemberAdded += Events.GuildMemberAdded;
            this.Client.GuildMemberRemoved += Events.GuildMemberRemoved;
            this.Client.GuildMembersChunked += Events.GuildMembersChunked;
            this.Client.GuildMemberUpdated += Events.GuildMemberUpdated;
            this.Client.GuildRoleCreated += Events.GuildRoleCreated;
            this.Client.GuildRoleDeleted += Events.GuildRoleDeleted;
            this.Client.GuildRoleUpdated += Events.GuildRoleUpdated;
            this.Client.GuildUnavailable += Events.GuildUnavailable;
            this.Client.GuildUpdated += Events.GuildUpdated;
            this.Client.Heartbeated += Events.Heartbeated;
            this.Client.InviteCreated += Events.InviteCreated;
            this.Client.InviteDeleted += Events.InviteDeleted;
            this.Client.MessageAcknowledged += Events.MessageAcknowledged;
            this.Client.MessageCreated += Events.MessageCreated;
            this.Client.MessageDeleted += Events.MessageDeleted;
            this.Client.MessageReactionAdded += Events.MessageReactionAdded;
            this.Client.MessageReactionRemoved += Events.MessageReactionRemoved;
            this.Client.MessageReactionRemovedEmoji += Events.MessageReactionRemovedEmoji;
            this.Client.MessageReactionsCleared += Events.MessageReactionsCleared;
            this.Client.MessagesBulkDeleted += Events.MessagesBulkDeleted;
            this.Client.MessageUpdated += Events.MessageUpdated;
            this.Client.PresenceUpdated += Events.PresenceUpdated;
            this.Client.Ready += Events.Ready;
            this.Client.Resumed += Events.Resumed;
            this.Client.SocketClosed += Events.SocketClosed;
            this.Client.SocketErrored += Events.SocketErrored;
            this.Client.SocketOpened += Events.SocketOpened;
            this.Client.TypingStarted += Events.TypingStarted;
            this.Client.UnknownEvent += Events.UnknownEvent;
            this.Client.UserSettingsUpdated += Events.UserSettingsUpdated;
            this.Client.UserUpdated += Events.UserUpdated;
            this.Client.VoiceServerUpdated += Events.VoiceServerUpdated;
            this.Client.VoiceStateUpdated += Events.VoiceStateUpdated;
            this.Client.WebhooksUpdated += Events.WebhooksUpdated;
            this.Client.ApplicationCommandCreated += Events.ApplicationCommandCreated;
            this.Client.ApplicationCommandDeleted += Events.ApplicationCommandDeleted;
            this.Client.ApplicationCommandUpdated += Events.ApplicationCommandUpdated;
            this.Client.InteractionCreated += Events.InteractionCreated;
            this.Commands.CommandExecuted += Events.CommandExecuted;
            this.Commands.CommandErrored += Events.CommandErrored;


            // finally, let's connect and log in
            await this.Client.ConnectAsync();

            // and this is to prevent premature quitting
            await Task.Delay(-1);
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
