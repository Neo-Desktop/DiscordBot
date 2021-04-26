#pragma warning disable IDE0060 // Remove unused parameter
using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Text.RegularExpressions;
using System.Linq;

namespace StarsiegeBot
{
    class Commands : BaseCommandModule
    {
        // this class depends a lot on the random stuff. so make it class level.
        private readonly Random rnd = new Random();

        // start: neo wanted this shit....
        private string[] insult = {
            "> {0} slaps {1} around a bit with a large trout.",
            "> {0} puts a jellyfish on {1}'s head.",
            "> {0} slaps {1} around a bit with the mustache of Caanon.",
            "> {0} slaps {1} around a bit with a questionable rubber object.",
            "> {0} slaps {1} around a bit with a rubber chicken."
        };
        private uint insultIndex;
        public string Insult {
            get {
                if (insultIndex >= insult.Length) {
                    insultIndex = 0;
                }
                return insult[insultIndex++ % insult.Length];
            }
        }
        // end: neo's shit.

        public Commands()
        {
            // Let the console know we've loaded basic commands.
            Console.WriteLine("Basic Commands Loaded");
        }

        [Command("dice"), Aliases("r", "d", "roll")]
        [Description("A classic dice throwing command. Limited to 1 use per 2 seconds. Format: <NumberOfDice>d<SidesOnDie> ")]
        [Cooldown(1, 2, CooldownBucketType.Guild)]
        public async Task Dice(CommandContext ctx, [RemainingText,Description("XdY")] String throwing = "d")
        {
            // let the end user know the bot is doing something...
            await ctx.TriggerTypingAsync();
            // in case the end user used caps, lower them.
            throwing = throwing.ToLower();
            // set some int items.
            int totalResult = 0;
            int totalDie = 0;
            // set the results to an empty string.
            string overallResults = String.Empty;
            // Prep the regex statement.
            Regex regex = new Regex("(\\d{0,})(d)(-{0,1}\\d{0,})");
            // Get a collection of regex matches from the dice we're through.
            MatchCollection matches = regex.Matches(throwing);
            // For each match do this.
            foreach (Match m in matches)
            {
                // if we have a match, do some stuff!
                if (m.Success)
                {
                    // set an empty string, set a counter. 
                    string dieResults = String.Empty;
                    int dieResult = 0;
                    // attempt to get the number of dice being "thrown" and the number of sides those die have.
                    bool volTest = int.TryParse(m.Groups[1].ToString(), out int volume);
                    bool sideTest = int.TryParse(m.Groups[3].ToString(), out int sides);

                    // They failed? Make some stuff up... We default to a 1d6 throw.
                    if (!volTest)
                        volume = 1;
                    if (!sideTest)
                        sides = 6;

                    // for each die we roll, get a result.
                    for (int i = 0; i < volume; i++)
                    {
                        // increase the total dice thrown.
                        ++totalDie;
                        // this is the result.
                        // This line looks weird, I know, so here is what is going on. If the number of sides is negative, we adjust for that.
                        // since the rnd.Next is inclusive, exclusive, we have to adjust for that.
                        int roll = rnd.Next(Math.Min(1, sides), Math.Max(sides + 1, 0));
                        // do we have our first result? No, make it! Store the results in a string to show the end user.
                        if (dieResults.Equals(String.Empty))
                            dieResults = roll.ToString();
                        else
                            dieResults += $", {roll}";
                        // Increase total results for both All Dice (totalResult) and for this specific set of die (dieResult)
                        totalResult += roll;
                        dieResult += roll;
                    }
                    // If the end user is rolling more than 10 of any kind of die, dont show the results of the rolls. Just 
                    if(volume < 11)
                        dieResults = $"{volume}d{sides}: Total: " + dieResult + " Avg: " + Math.Round(dieResult * 1.0 / volume, 2) + " Results: " + dieResults;
                    else
                        dieResults = $"{volume}d{sides}: Total: " + dieResult + " Avg: " + Math.Round(dieResult * 1.0 / volume, 2);
                    overallResults += "```" + dieResults + "```\r\n";
                }
            }
            // If we roll more than one kinda dice for them, give them extra info.
            if (matches.Count != 1)
                await ctx.RespondAsync(overallResults + "\r\nTotal: " + totalResult + " Avg: " + Math.Round(totalResult * 1.0 / totalDie * 1.0, 2));
            else
                await ctx.RespondAsync(overallResults);
        }

        [Command("random")]
        [Description("Random number generator.")]
        public async Task Random(CommandContext ctx, [Description("Lowest number choice. (Inclusive)")] int min, [Description("Highest Number Choice. (Inclusive)")] int max)
        {
            await ctx.TriggerTypingAsync();
            // Find which number given is lower, which is higher.
            // since rnd.Nex is inclusive, exclusive, increase the max by one.
            int i = Math.Min(min, max);
            int a = Math.Max(min, max) + 1;
            // output the results
            await ctx.RespondAsync($"🎲 Your random number is: {rnd.Next(i, a)}");
        }

        [Command("random")]
        public async Task Random(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            // some one didn't read the HELP file on Random. Explain it to them.
            await ctx.RespondAsync($"Gives you a random number between <Option 1> and <Option 2> Try someething like `random 3 30`!");
        }

        [Command("8ball"), Aliases(new String[] { "ball", "8" })]
        [Description("The Magic 8-Ball is a fortune-telling or seeking advice")]
        public async Task Eightball(CommandContext ctx, [RemainingText, Description("Your question to the 8 ball")] String remainingText = "")
        {
            await ctx.TriggerTypingAsync();
            // Load all the traditional 8-Ball answers into an array.
            string[] results = new string[] { "It is certain.", "It is decidedly so.", "Without a doubt.", "Yes - definitely.", "You may rely on it.", "As I see it, yes.", "Most likely.", "Outlook good.", "Yes.", "Signs point to yes.", "Reply hazy, try again", "Ask again later.", "Better not tell you now.", "Cannot predict now.", "Concentrate and ask again.", "Don't count on it.", "My reply is no.", "My sources say no", "Outlook not so good.", "Very doubtful." };
            // give the end user the randomly choosen result.
            await ctx.RespondAsync(results[rnd.Next(0, results.Length)]);
        }

        [Command("slap")]
        public async Task Slap(CommandContext ctx, DiscordMember user = null)
        {
            await ctx.TriggerTypingAsync();
            // we're going to find out who we're slapping. Declare their place holders now.
            string user1;
            string user2;

            // If we have some one not slapping some one, or they're trying to slap themself, the one getting slapped is the 
            // author, the one doing the slapping is the bot.
            if (user == null || user.Mention == ctx.Message.Author.Mention)
            {
                user1 = ctx.Client.CurrentUser.Mention;
                user2 = ctx.Message.Author.Mention;
            }
            // We're attempting to slap user that isn't the author... Lets do it!
            else
            {
                user1 = ctx.Message.Author.Mention;
                user2 = user.Mention;
            }
            // pick a random insult, and populate the names correctly.
            await ctx.RespondAsync(string.Format(Insult, user1, user2));
        }

        [Command("rpsls")]
        [Description("Rock Paper Scissors Lizard Spock")]
        public async Task Rpsls(CommandContext ctx, [RemainingText, Description("Your choice of hand sign to  use.")] string text = "")
        {
            await ctx.TriggerTypingAsync();
            // load up the correct choices.
            string[] choices = new string[] { "rock", "paper", "scissors", "lizard", "spock" };
            // lets work in all lower case letters. makes life easier.
            text = text.ToLower();
            // make a choice for the bot.
            string botChoice = choices[rnd.Next(0, choices.Length)];
            // create a result string.
            string result;

            // the end user didn't pick anything... lets hand them something instead.
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
                // tell the user we're giving them a thing.
                await ctx.RespondAsync(result);
            }
            // the end user actually wants to throw down... Lets go!
            else
            {
                if (choices.Contains(text))
                {
                    // This looks complicated. Its not really. Each line of this if statement is a condition in which the bot wins the round, and the player loses.
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
                    // The bot and the end user got the same result.
                    else if (text.Equals(botChoice))
                    {
                        result = $"We both picked {text}, looks like we drew, go again?";
                    }
                    // The end user won the game.
                    else
                    {
                        result = $"{text} beats {botChoice}. **YOU WIN!!**";
                    }
                }
                else
                {
                    // the end user didn't pick something that is within the choice set... Tell them they need to play better.
                    result = "Please pick a valid option! Options: " + String.Join(", ", choices);
                }
                // Actually send out all our info now.
                await ctx.RespondAsync(result);
            }
        }

        [Command("rps")]
        [Description("Rock Paper Scissors")]
        public async Task Rps(CommandContext ctx, [RemainingText, Description("Your choice of hand sign to  use.")] string text = "")
        {
            await ctx.TriggerTypingAsync();
            /* --- THIS FUNCTION IS A COPY AND PASTE OF RPSLS. AND ALL LOGIC IS IDENTICAL TO IT. PLEASE REFER TO THAT METHOD. ---
            // --- THIS FUNCTION IS A COPY AND PASTE OF RPSLS. AND ALL LOGIC IS IDENTICAL TO IT. PLEASE REFER TO THAT METHOD. ---
            // --- THIS FUNCTION IS A COPY AND PASTE OF RPSLS. AND ALL LOGIC IS IDENTICAL TO IT. PLEASE REFER TO THAT METHOD. --- */
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
            // delcare an embed builder.
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            // Title the ebemd with the bot's name and discrim.
            embed.WithTitle("About " + ctx.Client.CurrentUser.Username + "#" + ctx.Client.CurrentUser.Discriminator);
            // add the bot's avatar as the Thumbnail.
            embed.WithThumbnail(ctx.Client.CurrentUser.GetAvatarUrl(DSharpPlus.ImageFormat.Png, 32));
            // Add a bit of color... the color from the member's top most role.
            embed.WithColor(ctx.Member.Color);
            // output various data abotu the bot.
            embed.AddField("D#+ Ver", ctx.Client.VersionString, true);
            embed.AddField("Guild Count", ctx.Client.Guilds.Count.ToString(), true);
            embed.AddField("Ping", ctx.Client.Ping.ToString(), true);
            embed.AddField("Shard Count", ctx.Client.ShardCount.ToString(), true);
            embed.AddField("Shard", ctx.Client.ShardId.ToString(), true);

            // list all the owners of the bot.
            string owners = "";
            foreach (DiscordUser owner in ctx.Client.CurrentApplication.Owners)
            {
                owners += owner.Username + "#" + owner.Discriminator+"\r\n";
            }

            embed.AddField("Owner(s)", owners, false);
            // send the data to the end user.
            await ctx.RespondAsync(embed);
        }

        [Command("credits")]
        [Description("Shows all those that have helped with my bot(s) over the years!")]
        public async Task Credits(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            // just some people that have helped me build the bot.
            string credits = "```asciidoc\r\n= CREDITS (Outdated) =\r\n" +
                "• UndeadShadow            :: Flood Protection. Keepign teh bot from killing itself, and others.\r\n" +
                "• Da_D_Master             :: For giving me my start in my first couple bots, and being a friendly rival.\r\n" +
                "• ThinkExist & Bored.com  :: Quotes for the Quote Database\r\n" +
                "• Hanibal, TaMeD, Liquid- :: For awlays proving the points of others.\r\n" +
                "• Neo                     :: For all their hard work on all the things I've ever asked about.\r\n" +
                "• Alias                   :: For helping me learn how to make IRC bots back in the day.\r\n" +
                "• Drake, Eledore          :: Supporting the development of the bot and supplying code for it.\r\n" +
                "\r\n```";
            await ctx.RespondAsync(credits);
        }

        [Command("coin")]
        [Description("Flips a coin for ya!")]
        public async Task Coinflip(CommandContext ctx)
        {
            // We're going to randomly select heads or tails and then give it to the user.
            await ctx.TriggerTypingAsync();
            string[] choices = { "heads", "tails" };
            await ctx.RespondAsync("The coin landed on... " + choices[rnd.Next(0, choices.Length)]);
        }

        [Command("say")]
        [Description("Make the bot say something, perhaps silly?!")]
        public async Task MsgSay(CommandContext ctx, [RemainingText, Description("The stuff you want the bot to say.")] string msg = "")
        {
            // This is a redundant bot check. We really wanna make sure that we're not spamming an unprotected bot with commands, and that they are spamming us back.
            if (ctx.Message.Author.IsBot)
                return;
            await ctx.TriggerTypingAsync();
            // did the end user supply us something to say? no whine about it.
            if (msg != "")
                await ctx.RespondAsync(msg);
            else
                await ctx.RespondAsync("I need something to say...");
        }
    }
}
