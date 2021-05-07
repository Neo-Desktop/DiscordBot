using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace StarsiegeBot.Raven
{
    class TriviaHandler : BaseCommandModule
    {
        private readonly List<Questions> _quizQuestions;
        private static readonly string s_fileName = "Json/trivia.json";
        public TriviaHandler()
        {
            // Load all the Quick Chat texts and sound wav file info.
            string json = "";
            using (FileStream fs = File.OpenRead(s_fileName))
            using (StreamReader sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();
            Dictionary<string, Questions> tempQC = JsonConvert.DeserializeObject<Dictionary<string, Questions>>(json);
            _quizQuestions = Enumerable.ToList(tempQC.Values);
        }
        [Command("quiz")]
        [Hidden]
        [Description("")]
        public async Task Quiz(CommandContext ctx)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            int[] ids = { Program.rnd.Next(_quizQuestions.Count), Program.rnd.Next(_quizQuestions.Count), Program.rnd.Next(_quizQuestions.Count), Program.rnd.Next(_quizQuestions.Count) };
            int selection = Program.rnd.Next(ids.Length);
            int mainId = ids[selection];
            string[] options = { "a", "b", "c", "d" };
            var correctAnswer = DiscordEmoji.FromName(ctx.Client, $":regional_indicator_{options[selection]}:");
            Questions myQuestion = _quizQuestions[mainId];
            embed.Description = myQuestion.Question;
            embed.AddField($":regional_indicator_{options[0]}:", _quizQuestions[ids[0]].Answers[Program.rnd.Next(_quizQuestions[ids[0]].Answers.Length)], true);
            embed.AddField($":regional_indicator_{options[1]}:", _quizQuestions[ids[1]].Answers[Program.rnd.Next(_quizQuestions[ids[1]].Answers.Length)], true);
            embed.AddField("_ _", "_ _", false);
            embed.AddField($":regional_indicator_{options[2]}:", _quizQuestions[ids[2]].Answers[Program.rnd.Next(_quizQuestions[ids[2]].Answers.Length)], true);
            embed.AddField($":regional_indicator_{options[3]}:", _quizQuestions[ids[3]].Answers[Program.rnd.Next(_quizQuestions[ids[3]].Answers.Length)], true);
            DiscordMessage message = await ctx.RespondAsync(embed);
            foreach (string item in options)
            {
                var itemEmote = DiscordEmoji.FromName(ctx.Client, $":regional_indicator_{item}:");
                await message.CreateReactionAsync(itemEmote);
            }
            var result = message.WaitForReactionAsync(ctx.Message.Author, TimeSpan.FromSeconds(5d));
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
            DiscordMessage message = await ctx.RespondAsync("React here!");
            var reactions = await message.CollectReactionsAsync();
            var strBuilder = new StringBuilder();
            Dictionary<DiscordUser, bool> winners = new Dictionary<DiscordUser, bool>();
            Dictionary<DiscordUser, bool> losers = new Dictionary<DiscordUser, bool>();
            DiscordEmoji correctEmote = reactions[0].Emoji;
            foreach (var reaction in reactions)
            {
                foreach (DiscordUser person in reaction.Users)
                {
                    if (!correctEmote.Equals(reaction.Emoji))
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
