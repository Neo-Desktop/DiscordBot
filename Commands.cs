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

namespace StarsiegeBot
{
    class Commands : BaseCommandModule
    {
        private readonly Random rnd = new Random();

        public Commands()
        {
            Console.WriteLine("Basic Commands Loaded");
        }

        [Command("dice"), Aliases("r", "d", "roll")]
        [Description("A classic dice throwing command. Limited to 1 use per 2 seconds. Format: <NumberOfDice>d<SidesOnDie> ")]
        [Cooldown(1, 2, CooldownBucketType.User)]
        public async Task Dice(CommandContext ctx, [RemainingText,Description("XdY")] String throwing = "d")
        {
            await ctx.TriggerTypingAsync();
            throwing = throwing.ToLower();
            int totalResult = 0;
            int totalDie = 0;
            string overallResults = String.Empty;
            Regex regex = new Regex("(\\d{0,})(d)(-{0,1}\\d{0,})");
            MatchCollection matches = regex.Matches(throwing);
            foreach (Match m in matches)
            {
                if (m.Success)
                {
                    string dieResults = String.Empty;
                    int dieResult = 0;
                    bool volTest = int.TryParse(m.Groups[1].ToString(), out int volume);
                    bool sideTest = int.TryParse(m.Groups[3].ToString(), out int sides);

                    if (!volTest)
                        volume = 1;
                    if (!sideTest)
                        sides = 6;

                    for (int i = 0; i < volume; i++)
                    {
                        ++totalDie;
                        int roll = rnd.Next(Math.Min(1, sides), Math.Max(sides + 1, 0));
                        if (dieResults.Equals(String.Empty))
                            dieResults = roll.ToString();
                        else
                            dieResults += $", {roll}";
                        totalResult += roll;
                        dieResult += roll;
                    }
                    if(volume < 11)
                        dieResults = $"{volume}d{sides}: Total: " + dieResult + " Avg: " + Math.Round(dieResult * 1.0 / volume, 2) + " Results: " + dieResults;
                    else
                        dieResults = $"{volume}d{sides}: Total: " + dieResult + " Avg: " + Math.Round(dieResult * 1.0 / volume, 2);
                    overallResults += "```" + dieResults + "```\r\n";
                }
            }
            if (matches.Count != 1)
                await ctx.RespondAsync(overallResults + "\r\nTotal: " + totalResult + " Avg: " + Math.Round(totalResult * 1.0 / totalDie * 1.0, 2));
            else
                await ctx.RespondAsync(overallResults);
        }

        [Command("random")]
        [Description("Random number generator.")]
        public async Task Random(CommandContext ctx, [Description("Lowest number choice. (Inclusive)")] int min, [Description("Highest Number Choice. (Inclusive)")] int max)
        {
            max += 1;
            var rnd = new Random();
            int i = Math.Min(min, max);
            int a = Math.Max(min, max);

            await ctx.RespondAsync($"🎲 Your random number is: {rnd.Next(i, a)}");
        }
        [Command("random")]
        public async Task Random(CommandContext ctx)
        {
            await ctx.RespondAsync($"Gives you a random number between <Option 1> and <Option 2> Try someething like `random 3 30`!");
        }
        [Command("8ball"), Aliases(new String[] { "ball", "8" })]
        [Description("The Magic 8-Ball is a fortune-telling or seeking advice")]
        public async Task Eightball(CommandContext ctx, [RemainingText, Description("Your question to the 8 ball")] String remainingText = "")
        {
            string[] results = new string[] { "It is certain.", "It is decidedly so.", "Without a doubt.", "Yes - definitely.", "You may rely on it.", "As I see it, yes.", "Most likely.", "Outlook good.", "Yes.", "Signs point to yes.", "Reply hazy, try again", "Ask again later.", "Better not tell you now.", "Cannot predict now.", "Concentrate and ask again.", "Don't count on it.", "My reply is no.", "My sources say no", "Outlook not so good.", "Very doubtful." };
            await ctx.RespondAsync(results[rnd.Next(0, results.Length)]);
        }

        [Command("slap")]
        public async Task Slap(CommandContext ctx, DiscordMember user = null)
        {
            string[] insults = {
                "{0} slaps {1} around a bit with a large trout.",
                "{0} puts a jellyfish on {1}'s head.",
                "{0} slaps {1} around a bit with the mustache of Caanon.",
                "{0} slaps {1} around a bit with a questionable rubber object.",
                "{0} slaps {1} around a bit with a rubber chicken."
                };
            string user1;
            string user2;

            if (user == null || user.Mention == ctx.Message.Author.Mention)
            {
                user1 = ctx.Client.CurrentUser.Mention;
                user2 = ctx.Message.Author.Mention;
            }
            else
            {
                user1 = ctx.Message.Author.Mention;
                user2 = user.Mention;
            }

            await ctx.RespondAsync(string.Format(insults[rnd.Next(insults.Length)], user1, user2));
        }

        [Command("rpsls")]
        [Description("Rock Paper Scissors Lizard Spock")]
        public async Task Rpsls(CommandContext ctx, [RemainingText, Description("Your choice of hand sign to  use.")] string text = "")
        {
            string[] choices = new string[] { "rock", "paper", "scissors", "lizard", "spock" };
            text = text.ToLower();
            string botChoice = choices[rnd.Next(0, choices.Length)];
            string result;

            if (text.Equals(""))
            {
                result = "I'm handing you a";
                switch (botChoice)
                {
                    case "rock":
                        result += " shiny rock.";
                        break;
                    case "paper":
                        result += " crumpled paper.";
                        break;
                    case "lizard":
                        result += " deadly lizard.";
                        break;
                    case "scissors":
                        result += " set of child proof scissors.";
                        break;
                    case "spock":
                        result = "I present, Mr. Spock!";
                        break;
                }
                await ctx.RespondAsync(result);
            }
            else
            {
                if (choices.Contains(text))
                {
                    if (((botChoice == "scissors") && (text == "paper")) ||
                        ((botChoice == "paper") && (text == "rock")) ||
                        ((botChoice == "rock") && (text == "lizard")) ||
                        ((botChoice == "lizard") && (text == "spock")) ||
                        ((botChoice == "spock") && (text == "scissors")) ||
                        ((botChoice == "scissors") && (text == "lizard")) ||
                        ((botChoice == "lizard") && (text == "paper")) ||
                        ((botChoice == "paper") && (text == "spock")) ||
                        ((botChoice == "spock") && (text == "rock")) ||
                        ((botChoice == "rock") && (text == "scissors")))
                    {
                        result = $"{botChoice} beats {text}. I'm sorry about this.";
                    }
                    else if (text.Equals(botChoice))
                    {
                        result = $"We both picked {text}, looks like we drew, go again?";
                    }
                    else
                    {
                        result = $"{text} beats {botChoice}. **YOU WIN!!**";
                    }
                }
                else
                {
                    result = "Please pick a valid option! Options: " + String.Join(", ", choices);
                }
                await ctx.RespondAsync(result);
            }
        }

        [Command("rps")]
        [Description("Rock Paper Scissors")]
        public async Task Rps(CommandContext ctx, [RemainingText, Description("Your choice of hand sign to  use.")] string text = "")
        {
            string[] choices = new string[] { "rock", "paper", "scissors" };
            text = text.ToLower();
            string botChoice = choices[rnd.Next(0, choices.Length)];
            string result;

            if (text.Equals(""))
            {
                result = "I'm handing you a";
                switch (botChoice)
                {
                    case "rock":
                        result += " shiny rock.";
                        break;
                    case "paper":
                        result += " crumpled paper.";
                        break;
                    case "scissors":
                        result += " set of child proof scissors.";
                        break;
                }
                await ctx.RespondAsync(result);
            }
            else
            {
                if (choices.Contains(text))
                {
                    if (((botChoice == "scissors") && (text == "paper")) ||
                        ((botChoice == "paper") && (text == "rock")) ||
                        ((botChoice == "rock") && (text == "scissors")))
                    {
                        result = $"{botChoice} beats {text}. I'm sorry about this.";
                    }
                    else if (text.Equals(botChoice))
                    {
                        result = $"We both picked {text}, looks like we drew, go again?";
                    }
                    else
                    {
                        result = $"{text} beats {botChoice}. **YOU WIN!!**";
                    }
                }
                else
                {
                    result = "Please pick a valid option! Options: " + String.Join(", ", choices);
                }
                await ctx.RespondAsync(result);
            }
        }

        [Command("about")]
        [Description("Gives some basic info about the bot")]
        public async Task About(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            DiscordMember member = ctx.User as DiscordMember;
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            embed.WithTitle("About " + ctx.Client.CurrentUser.Username + "#" + ctx.Client.CurrentUser.Discriminator);

            embed.WithThumbnail(ctx.Client.CurrentUser.GetAvatarUrl(DSharpPlus.ImageFormat.Png, 32));
            embed.WithColor(member.Color);
            embed.AddField("D#+ Ver", ctx.Client.VersionString, true);
            embed.AddField("Guild Count", ctx.Client.Guilds.Count.ToString(), true);
            embed.AddField("Ping", ctx.Client.Ping.ToString(), true);
            embed.AddField("Shard Count", ctx.Client.ShardCount.ToString(), true);
            embed.AddField("Shard", ctx.Client.ShardId.ToString(), true);

            string owners = "";
            foreach (DiscordUser owner in ctx.Client.CurrentApplication.Owners)
            {
                owners += owner.Username + "#" + owner.Discriminator+"\r\n";
            }

            //DiscordUser owner = ctx.Client.CurrentApplication.Owners.First();
            embed.AddField("Owner(s)", owners, false);
            await ctx.RespondAsync(embed);
        }

        [Command("credits")]
        [Description("Shows all those that have helped with my bot(s) over the years!")]
        public async Task Credits(CommandContext ctx)
        {
            string credits = "```asciidoc\r\n= CREDITS (Outdated) =\r\n" +
                "• UndeadShadow            :: Flood Protection. Keepign teh bot from killing itself, and others.\r\n" +
                "• Da_D_Master             :: For giving me my start in my first couple bots, and being a friendly rival.\r\n" +
                "• ThinkExist & Bored.com  :: Quotes for the Quote Database\r\n" +
                "• Hanibal, TaMeD, Liquid- :: For awlays proving the points of others.\r\n" +
                "• Neo                     :: For all their hard work on all the things I've ever asked about.\r\n" +
                "• Alias                   :: For helping me learn how to make IRC bots back in the day.\r\n" +
                "• Drake                   :: Supporting the development of the bot and supplying code for it.\r\n" +
                "\r\n```";
            await ctx.RespondAsync(credits);
        }

        [Command("coin")]
        [Description("Flips a coin for ya!")]
        public async Task Coinflip(CommandContext ctx)
        {
            string[] choices = { "heads", "tails" };
            await ctx.RespondAsync("The coin landed on... " + choices[rnd.Next(0, choices.Length)]);
        }

        [Command("say")]
        [Description("Make the bot say something, perhaps silly?!")]
        public async Task MsgSay(CommandContext ctx, [RemainingText, Description("The stuff you want the bot to say.")] string msg = "")
        {
            if (msg != "")
                await ctx.RespondAsync(msg);
            else
                await ctx.RespondAsync("I need something to say...");
        }
    }
}
