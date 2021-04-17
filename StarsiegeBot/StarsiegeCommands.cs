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

namespace StarsiegeBot
{
    [Group("ex")]
    [Hidden]
    
    [Description("Experimental commands. May break the bot.")]
    class StarsiegeCommands : BaseCommandModule
    {
        private readonly List<Questions> QuizQuestions;
        private readonly Random rnd = new Random();
        public StarsiegeCommands()
        {
            // Load all the Quick Chat texts and sound wav file info.
            var json = "";
            using (var fs = File.OpenRead("questions.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();
            Dictionary<string, Questions> tempQC = JsonConvert.DeserializeObject<Dictionary<string, Questions>>(json);
            QuizQuestions = Enumerable.ToList(tempQC.Values);

        }
        [Command("quiz")]
        [Hidden]
        [Description("")]
        public async Task Quiz(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

            int[] ids = { rnd.Next(QuizQuestions.Count), rnd.Next(QuizQuestions.Count), rnd.Next(QuizQuestions.Count), rnd.Next(QuizQuestions.Count)};
            int selection = rnd.Next(ids.Length);
            int mainId = ids[selection];
            string[] options = { "a", "b", "c", "d" };

            var correctAnswer = DiscordEmoji.FromName(ctx.Client, $":regional_indicator_{options[selection]}:");

            Questions myQuestion = QuizQuestions[mainId];

            embed.Description = myQuestion.Question;

            embed.AddField($":regional_indicator_{options[0]}:", QuizQuestions[ids[0]].Answers[rnd.Next(QuizQuestions[ids[0]].Answers.Length)], true);
            embed.AddField($":regional_indicator_{options[1]}:", QuizQuestions[ids[1]].Answers[rnd.Next(QuizQuestions[ids[1]].Answers.Length)], true);
            embed.AddField("_ _", "_ _", false);
            embed.AddField($":regional_indicator_{options[2]}:", QuizQuestions[ids[2]].Answers[rnd.Next(QuizQuestions[ids[2]].Answers.Length)], true);
            embed.AddField($":regional_indicator_{options[3]}:", QuizQuestions[ids[3]].Answers[rnd.Next(QuizQuestions[ids[3]].Answers.Length)], true);

            var message = await ctx.RespondAsync(embed);
            foreach (var item in options)
            {
                var itemEmote = DiscordEmoji.FromName(ctx.Client, $":regional_indicator_{item}:");
                await message.CreateReactionAsync(itemEmote);
            }
            var result = message.WaitForReactionAsync(ctx.Message.Author,TimeSpan.FromSeconds(5d));
            if (!result.Result.TimedOut)
            {
                if (result.Result.Result.Emoji == correctAnswer) await ctx.RespondAsync("**CORRECT!!**");
                else await ctx.RespondAsync($"Sorry, the right answer was {correctAnswer}");
            }
            else await ctx.RespondAsync("Sorry, time ran out!");
        }

        [Command("test")]
        [Hidden]
        public async Task CollectionCommand(CommandContext ctx)
        {
            var message = await ctx.RespondAsync("React here!");
            var reactions = await message.CollectReactionsAsync();

            var strBuilder = new StringBuilder();
            Dictionary<DiscordUser, bool> winners = new Dictionary<DiscordUser, bool>();
            Dictionary<DiscordUser, bool> losers = new Dictionary<DiscordUser, bool>();
            var correctEmote = reactions[0].Emoji;
            foreach (var reaction in reactions)
            {
                foreach (var person in reaction.Users)
                {
                    if ( ! correctEmote.Equals(reaction.Emoji))
                    {
                        losers.Add(person, true);
                    }
                    else
                    {
                        winners.Add(person, true);
                    }
                }
            }
            foreach (KeyValuePair<DiscordUser, bool> pair in winners)
            {
                if (losers.ContainsKey(pair.Key))
                {
                    winners.Remove(pair.Key);
                }
            }

            strBuilder.AppendLine("Winners are:");
            foreach (KeyValuePair<DiscordUser, bool> item in winners)
            {
                strBuilder.AppendLine(item.Key.Mention);
            }

            await ctx.RespondAsync(strBuilder.ToString());
        }
    }

    public class Questions
    {
        [JsonProperty("question")]
        public string Question { get; private set; }
        [JsonProperty("answer")]
        public string[] Answers { get; private set; }
        [JsonProperty("points")]
        public int Points { get; private set; }
    }
}
