using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
namespace StarsiegeBot
{
    class Program
    {
        public readonly EventId BotEventId;
        public static Triggers Events;
        public static readonly DiscordColor[] colours = { DiscordColor.Aquamarine, DiscordColor.Azure, DiscordColor.Black, DiscordColor.Blue, DiscordColor.Blurple,
            DiscordColor.Brown, DiscordColor.Chartreuse, DiscordColor.CornflowerBlue, DiscordColor.Cyan, DiscordColor.DarkBlue, DiscordColor.DarkButNotBlack,
            DiscordColor.DarkGray, DiscordColor.DarkGreen, DiscordColor.DarkRed, DiscordColor.Gold, DiscordColor.Goldenrod, DiscordColor.Gray, DiscordColor.Grayple,
            DiscordColor.Green, DiscordColor.HotPink, DiscordColor.IndianRed, DiscordColor.LightGray, DiscordColor.Lilac, DiscordColor.Magenta, DiscordColor.MidnightBlue,
            DiscordColor.None, DiscordColor.NotQuiteBlack, DiscordColor.Orange, DiscordColor.PhthaloBlue, DiscordColor.PhthaloGreen, DiscordColor.Purple,
            DiscordColor.Rose, DiscordColor.SapGreen, DiscordColor.Sienna, DiscordColor.SpringGreen, DiscordColor.Teal, DiscordColor.Turquoise, DiscordColor.VeryDarkGray,
            DiscordColor.Violet, DiscordColor.Wheat, DiscordColor.White, DiscordColor.Yellow, DiscordColor.Red };
        public static readonly Random rnd = new Random();
        public DiscordClient Client { get; set; }
        public CommandsNextExtension Commands { get; set; }
        public VoiceNextExtension Voice { get; set; }
        public string BotName { get; protected set; }
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
                BotName = "sspdev";
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
            Client = new DiscordClient(cfg);
            // next, let's hook some events, so we know
            // what's going on
            Client.UseInteractivity(new InteractivityConfiguration()
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
            Commands = Client.UseCommandsNext(ccfg);
            // up next, let's register our commands
            // These commands are available to all bots.
            Commands.RegisterCommands<BotSettings>(); // Main Folder. Test Items.
            Commands.RegisterCommands<PrefixManagement>();
            Commands.RegisterCommands<ChannelManagement>();
            // These are in-dev commands. Experimental, may break things.
            if (BotName.Contains("sspdev"))
            {
                Commands.RegisterCommands<ExperiemntalCommands>(); // Experimental commands file.
            }
            // These are bot specific.
            if (BotName.Contains("ssp") || BotName.Contains("phoenix"))
            {
                Commands.RegisterCommands<WelcomeMessage>(); // Main Folder. Test Items.
            }
            // Server management options, reserved for PhoenixBot
            if (BotName.Contains("phoenix"))
            {
                Commands.RegisterCommands<GroupManagement>(); // Main Folder. Test Items.
                Commands.RegisterCommands<GroupRoleManagement>(); // Main Folder. Test Items.
                Commands.RegisterCommands<LevelManagement>(); // Main Folder. Test Items.
                Commands.RegisterCommands<LevelRoleManagement>(); // Main Folder. Test Items.
                Commands.RegisterCommands<RoleManagement>(); // Main Folder. Test Items.
            }
            // Game related, commands of luck. Pure entertainment commands. Reserved for RavenBot.
            if (BotName.Contains("raven"))
            {
                Commands.RegisterCommands<GameCommands>();
                Commands.RegisterCommands<NotFacts>(); // Raven Folder.
                Commands.RegisterCommands<SnappleFacts>(); // Raven folder.
            }
            // All these commands are in the STARSIEGE folder.
            // Commands for the Starsiege Players bot, on the Starsiege Server.
            if (BotName.Contains("ssp"))
            {
                Commands.RegisterCommands<QuickchatHandler>();
                Commands.RegisterCommands<Functions>();
                Commands.RegisterCommands<DeathMessages>();
                Commands.RegisterCommands<GameInfo>();
            }
            // let's enable voice
            Voice = Client.UseVoiceNext();
            // Hook all possible Event Handlers in the system.
            Client.ChannelCreated += Events.ChannelCreated;
            Client.ChannelDeleted += Events.ChannelDeleted;
            Client.ChannelPinsUpdated += Events.ChannelPinsUpdated;
            Client.ChannelUpdated += Events.ChannelUpdated;
            Client.ClientErrored += Events.ClientErrored;
            Client.DmChannelDeleted += Events.DmChannelDeleted;
            Client.GuildAvailable += Events.GuildAvailable;
            Client.GuildBanAdded += Events.GuildBanAdded;
            Client.GuildBanRemoved += Events.GuildBanRemoved;
            Client.GuildCreated += Events.GuildCreated;
            Client.GuildDeleted += Events.GuildDeleted;
            Client.GuildDownloadCompleted += Events.GuildDownloadCompleted;
            Client.GuildEmojisUpdated += Events.GuildEmojisUpdated;
            Client.GuildIntegrationsUpdated += Events.GuildIntegrationsUpdated;
            Client.GuildMemberAdded += Events.GuildMemberAdded;
            Client.GuildMemberRemoved += Events.GuildMemberRemoved;
            Client.GuildMembersChunked += Events.GuildMembersChunked;
            Client.GuildMemberUpdated += Events.GuildMemberUpdated;
            Client.GuildRoleCreated += Events.GuildRoleCreated;
            Client.GuildRoleDeleted += Events.GuildRoleDeleted;
            Client.GuildRoleUpdated += Events.GuildRoleUpdated;
            Client.GuildUnavailable += Events.GuildUnavailable;
            Client.GuildUpdated += Events.GuildUpdated;
            Client.Heartbeated += Events.Heartbeated;
            Client.InviteCreated += Events.InviteCreated;
            Client.InviteDeleted += Events.InviteDeleted;
            Client.MessageAcknowledged += Events.MessageAcknowledged;
            Client.MessageCreated += Events.MessageCreated;
            Client.MessageDeleted += Events.MessageDeleted;
            Client.MessageReactionAdded += Events.MessageReactionAdded;
            Client.MessageReactionRemoved += Events.MessageReactionRemoved;
            Client.MessageReactionRemovedEmoji += Events.MessageReactionRemovedEmoji;
            Client.MessageReactionsCleared += Events.MessageReactionsCleared;
            Client.MessagesBulkDeleted += Events.MessagesBulkDeleted;
            Client.MessageUpdated += Events.MessageUpdated;
            Client.PresenceUpdated += Events.PresenceUpdated;
            Client.Ready += Events.Ready;
            Client.Resumed += Events.Resumed;
            Client.SocketClosed += Events.SocketClosed;
            Client.SocketErrored += Events.SocketErrored;
            Client.SocketOpened += Events.SocketOpened;
            Client.TypingStarted += Events.TypingStarted;
            Client.UnknownEvent += Events.UnknownEvent;
            Client.UserSettingsUpdated += Events.UserSettingsUpdated;
            Client.UserUpdated += Events.UserUpdated;
            Client.VoiceServerUpdated += Events.VoiceServerUpdated;
            Client.VoiceStateUpdated += Events.VoiceStateUpdated;
            Client.WebhooksUpdated += Events.WebhooksUpdated;
            Client.ApplicationCommandCreated += Events.ApplicationCommandCreated;
            Client.ApplicationCommandDeleted += Events.ApplicationCommandDeleted;
            Client.ApplicationCommandUpdated += Events.ApplicationCommandUpdated;
            Client.InteractionCreated += Events.InteractionCreated;
            Commands.CommandExecuted += Events.CommandExecuted;
            Commands.CommandErrored += Events.CommandErrored;
            // finally, let's connect and log in
            await Client.ConnectAsync();
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
        public static string YesNo(bool test)
        {
            return (test ? "Enabled" : "Disabled");
        }
    }
}
