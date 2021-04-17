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

namespace StarsiegeBot
{
    [Group("deathmessage"), Aliases("dm")]
    [Description("Gives a random death message. Or interacts with the death messages.")]
    class DeathMessages : BaseCommandModule
    {
        private readonly Random rnd = new Random();
        private readonly string[] ActiveDeathMessages = new string[] { "{0} ventilated {1}.", "{0} donated {1}'s body to science.", "{0} introduces {1} to the agony of defeat.", "{0} punched {1}'s ticket.", "{0} demonstrates death without dignity to {1}.", "{0} invites {1} to explore the past tense of being.", "{0} offs {1}.", "{0} gives {1} an out of body experience. ", "{0} burns {1} to smelly jelly.", "{0} makes a righteous mess of {1}.", "{0} composts {1}.", "{0} invites {1} to join the scenery.", "{0} gets medieval on {1}.", "{0} wipes the floor with {1}.", "{0} parties down on {1}.", "{0} opens a can of whoopaz on {1}.", "{0} massacred {1}.", "{0} inflicts a coup de grace on {1}.", "{0} showed {1} how its done.", "{0} donated {1}'s body to big dick Leon.", "{0} introduced {1} to God.", "{0} spanked {1} in front of everyone.", "{0} abused {1}.", "{0} invites {1}'s insides to explore the outside.", "{0} farmed {1} for points.", "{0} ruined {1}.", "{0} got a footshot on {1}.", "{0} makes a splattered mess of {1}.", "{0} tapped {1}'s tenderness.", "{0} didnt go easy on {1}.", "{0} raped, pillaged, and savaged {1}.", "{0} wipes their ass with {1}.", "{0} put {1} out of their misery.", "{0} made {1} shutup.", "{0} made {1} look like a noob.", "{0} owns {1}.", "{0} punked out {1}.", "{0} trolled {1} hard.", "{0} flashy-thinged {1} to death.", "{0} neuralyzed {1}.", "{0} gave {1} a close encounter of the MIB kind.", "{0} blew off one of {1}'s heads.", "{0}'s Noisy Cricket misfired in {1}'s direction.", "{0} shoved a Series 4 De-Atomizer in {1}'s mouth & pulled the trigger.", "{0} shot {1} where it don't grow back.", "{0} shot down an Arquillian Battle Cruiser and it crash-landed on {1}. ", "{0} spilled coffee on {1}'s new suit.", "{0} stepped on {1}'s new shades.", "{0} made an 'Edgar-suit' out of {1}.", "{0}'s Noisy Cricket got wicked on {1}.", "{0} gets sawn in half by {0}.", "{0} died. I blame {0}.", "{0} was axe murdered by {0}.", "{0}'s melon was split by {0}.", "{0} was put on the chop block on {0}.", "{0} was sliced and diced by {0}.", "{0} is split from crotch to sternum by {0}.", "{0} is split in two with a powerful axe blow from {0}.", "{0}'s death put another notch on {0}'s axe..", "{0} escapes infection from {0} by dying first.", "{0} dies from {0}'s mysterious tropical disease.", "{0} eats {0}'s pineapple.", "{0} was gibbed by {0}'s grenade.", "{0} receives a gift from {0}.", "{0} plays pass-the-parcel with {0}.", "{0} was knife-murdered by {0}.", "{0} gets knifed from behind by {0}.", "{0} was nailed by {0}.", "{0} caught one too many nails from {0}.", "{0} ran into {0}'s nails.", "{0} was turned into {0}'s pin-cushion.", "{0} gets no say in it, no say in it at all! sings {0}.", "{0} surfs on a grenade from {0}.", "{0} got up-close and personal with {0}'s grenade.", "{0} played catch with {0}'s grenade.", "{0} received a pineapple enema from {0}.", "{0} fetches {0}'s pineapple.", "{0} caught too much shrapnel from {0}'s grenade.", "{0} tried to pick up {0}'s hot potato.", "{0} thought {0} was tossing him a spare grenade.", "{0} stops to ponder the technical details of {0}'s grenade.", "{0} gets a hole in their heart from {0}'s railgun.", "{0} gets derailed by {0}.", "{0} rides {0}'s rocket.", "{0} was gibbed by {0}'s rocket.", "{0} rides {0}'s firecracker.", "{0} chewed on {0}'s boomstick.", "{0} got too close to {0}'s muzzleflash.", "{0} practices being {0}'s clay pigeon.", "{0} was on the receiving end of {0}'s shotgun barrel.", "{0} was fed a lead diet by {0}.", "{0} got blasted by {0}'s last resort.", "{0} got more than a powderburn from {0}'s shotgun blast.", "{0} was punctured by {0}.", "{0} is put to sleep by {0}.", "{0} is all partied out by {0}.", "{0} killed {1}", "{0} knifed {1}", "{0} eviscerated {1}", "{0} melee killed {1}", "{0} whacked {1}", "{0} beat down {1}", "{0} murdered {1}", "{0} battered {1}", "{0} ended {1}", "{0} rifled {1}", "{0} shot down {1}", "{0} floored {1}", "{0} riddled {1}", "{0} drilled {1}", "{0} submachine gunned {1}", "{0} ruined {1}", "{0} bombed {1}", "{0} detonated {1}", "{0} exploded {1}", "{0} obliterated {1}", "{0} destroyed {1}", "{0} erased {1}", "{0} ripped apart {1}", "{0} torn apart {1}", "{0} wiped out {1}", "{0} pulverised {1}", "{0} shotgunned {1}", "{0} sniped {1}", "{0} picked off {1}", "{0} scoped {1}", "{0} pistoled {1}", "{0} blasted {1}", "{0} plugged {1}", "{0} machine gunned {1}", "{0} sprayed {1}", "{0} flattened {1}", "{0} ran over {1}", "{0} ran {1} down", "{0} mowed down {1}", "{0} chopped up {1}", "{0} killed {1}", "{0} tried to catch {1}", "{0} was crushed by {1}", "{0} was jumped by {1}", "{0} stomps {1}", "{0} was literally stomped into particles by {1}", "{0} was ax-murdered by {1}", "{0} was lead poisoned by {1}", "{0} was body pierced by {1}", "{0} was nailed by {1}", "{0} was perforated by {1}", "{0} was punctured by {1}", "{0} was ventilated by {1}", "{0} was straw-cuttered by {1}", "{0} gets a natural disaster from {1}", "{0} was axed to pieces by {1}", "{0} was instagibbed by {1}", "{0} was railed by {1}", "{0} was telefragged by {1}", "{0} squishes {1}", "{0} was capped by {1}", "{0} takes a bullet in the chest from {1}", "{0} succumbs to sniperfire from {1}", "{0} gets a third eye from {1}", "{0} gets their head blown off by {1}", "{0} is made legless by {1}", "{0} gets their legs blown off by {1}", "{0} gets a sucking chest wound from {1}", "{0}'s liver is blown out by {1}", "{0} is neutered by {1}", "{0} torso is removed by {1}", "{0} gets sawn in half by {1}", "{0} is reduced to ashes by {1}", "{0} grappled with {1}", "{0} gets knifed from behind by {1}", "{0} was stabbed by {1}", "{0} is ass-knifed by {1}", "{0} is put to sleep by {1}", "{0} was spanner-murdered by {1}", "{0} was spanner-wacked by {1}", "{0} is split in two with a powerful axe blow from {1}", "{0} was put on the chop block by {1}", "{0}'s mellon was split by {1}", "{0} was slit open by {1}", "{0} received a pineapple enema from {1}", "{0} surfs on a grenade from {1}", "{0} was knife-murdered by {1}", "{0} caught one too many nails from {1}", "{0} was fed a lead diet by {1}", "{0} got in the way of {1}", "{0} mows down teammate {1}", "{0} checks their glasses after killing {1}", "{0} killed their supposed friend {1}", "{0} didn't survive the operation by {1}", "{0} softens {1}'s fall", "{0} chewed on {1}'s boomstick", "{0} ate 8 loads of {1}'s buckshot", "{0} ate 2 loads of {1}'s buckshot", "{0} eats {1}'s pineapple", "{0} was gibbed by {1}'s grenade", "{0} was smeared by {1}'s quad rocket", "{0} was brutalized by {1}'s quad rocket", "{0} was gibbed by {1}'s rocket", "{0} rides {1}'s rocket", "{0} accepts {1}'s shaft", "{0} accepts {1}'s discharge", "{0} drains {1}'s batteries", "{0} stepped on too many of {1}'s caltrops", "{0} is charred by {1}'s flash grenade", "{0}'s brain is fried by {1}'s flash grenade", "{0} was bombed by {1}'s AirStrike call", "{0}'s chest explodes from {1}'s sniper round", "{0} is beheaded by {1}'s round", "{0}'s labotomized by {1}'s sniper round", "{0}'s legs explode open from {1}'s shot", "{0} collects {1}'s bullet spray.", "{0} enjoys {1}'s machinegun", "{0} gets flayed by {1}'s nail grenade", "{0} gets perforated by {1}'s nail grenade", "{0} gets too friendly with {1}'s Proxi grenade", "{0} is reamed by {1}'s rocket", "{0}'s bunghole was ripped by {1}'s rocket", "{0} was swiss-cheesed by {1}'s bird gun", "{0}'s head is popped by {1}'s shotgun", "{0} reaches orbit via {1}'s detpack", "{0} cut the red wire of {1}'s detpack", "{0} is nuked by {1}'s detpack", "{0} swallows {1}'s grenade", "{0} was split in half by {1}'s grenade", "{0} gets spammed by {1}'s Mirv grenade", "{0} does a dance on {1}'s Mirv grenade", "{0} gets juiced by {1}'s Mirv grenad", "{0} is shreaded by {1}'s AirMirv", "{0} is caught by {1}'s pipebomb trap", "{0} fell victim to {1}'s fireworks", "{0} is shreaded by {1}'s pipebomb trap", "{0} dies from {1}'s mysterious tropical disease", "{0} escapes infection from {1} by dying first", "{0} dies from {1}'s social disease", "{0} was perforated by {1}'s nailgun", "{0} gets shredded by {1}'s 20mm cannon", "{0} is burnt up by {1}'s flame", "{0} is fried by {1}'s fire", "{0} feels {1}'s fire of wrath", "{0} is grilled by {1}'s flame", "{0} burns to death by {1}'s flame", "{0} is boiled alive by {1}'s heat", "{0} is cremated by {1}s incinerator", "{0} is grilled by {1}'s BBQ", "{0} gets cooked by {1}'s incendiary rocket", "{0} gets well done by {1}'s incendiary rocket", "{0} gags on {1}'s noxious gasses", "{0} sniffs to much of {1}'s glue", "{0} is over-dosed by {1}'s ludes", "{0} didn't insert the correct change into {1}'s dispenser", "{0} thought {1}'s dispenser was a mechanical bull", "{0} was killed by {1}'s Laser Drone", "{0} was vaporized by {1}'s Laser Drone", "{0} stands near some ammo as {1}'s EMP nukes it", "{0}'s ammo detonates him as {1}'s EMP fries it", "{0}'s gets vaporized by {1}'s EMP grenade", "{0} gets a hole in their heart from {1}'s railgun", "{0} spews juice thru holes from {1}'s railgun", "{0} gets destroyed by {1}'s exploding sentrygun", "{0} hates {1}'s sentry gun", "{0} is creamed by {1}'s sentry gun", "{0} is mown down by {1}'s sentry gun", "{0}'s spine is extracted by {1}'s sentry gun", "{0}'s was obliterated by {1}'s tesla coil", "{0} is split from crotch to sternum by {1}'s axe swing", "{0} was sliced and diced by {1}'s blade", "{0}'s death put another notch on {1}'s axe", "{0} caught too much shrapnel from {1}'s grenade", "{0} fetched {1}'s pineapple", "{0} got up-close and personal with {1}'s grenade", "{0} played catch with {1}'s grenade", "{0} stops to ponder the technical details of {1}'s grenade", "{0} thought {1} was tossing him a spare grenade", "{0} tried to pick up {1}'s hot potato", "{0} tries to hatch {1}'s grenade", "{0} ran into {1}'s nails", "{0} was turned into {1}'s pin-cushion", "{0} got blasted by {1}'s last resort", "{0} got more than a powderburn from {1}'s shotgun blast", "{0} got too close to {1}'s muzzleflash", "{0} practices being {1}'s clay pigeon", "{0} was on the receiving end of {1}'s shotgun barrel", "{0}'s leg was amputated by {1}'s spike", "{0} gets ventilated by {1}'s super-shotgun blast", "{0} got a double-dose of {1}'s buckshot", "{0} unfortunately forgot {1} carried a super-shotgun", "{0} was turned into swiss cheese by {1}'s buckshot", "{0}'s body got chuck full of {1}'s lead pellets", "{0} swims with {1}'s toaster", "{0} gets a frag for the other team with {1}'s death", "{0} rips {1} a new one" };
        private readonly string[] PassiveDeathMessages = new string[] { "{1} is chewed up and spit out by {0}", "{1} got curb stomped by {0}", "{1} gets the Vulcan Nerve Pinch from {0}", "{1} felt the burn from {0}.", "{1} took it hard and hot from {0}", "{1} is now a crispy lawn ornament in {0}'s yard.", "{1} brought a knife to the gun fight with {0}.", "{1} is snuffed by {0}.", "{1} gets a flying wedgie from {0}.", "{1} gets a nice wet vivisection from {0}.", "{1}'s ticket was punched by {0}.", "{1}'s personal space was violated by {0}.", "{1} is curb stomped by {0}.", "{1} makes an unsuccessful pass at {0}.", "{1} gets a little chin music from {0}.", "{1} receives a Chicago Overcoat from {0}.", "{1} is cut down in the prime of life by {0}.", "{1} gets drilled by {0}.", "{1} got eighty-sixed by {0}.", "{1} plays tackling dummy for {0}.", "{1} got slaughtered by {0}.", "{1} is annihilated by a lucky shot from {0}.", "{1} is chewed up and shit out by {0}.", "{1} is womped on by {0}.", "{1} got on {0}'s last nerve.", "{1} took one to many hits from {0}.", "{1} tried to run away from {0}.", "{1} is another buried body in {0}'s yard.", "{1} got cheap shotted by {0}.", "{1} got knocked the FUCK out by {0}.", "{1} has to respawn after fighting {0}.", "{1}'s online life is ended by {0}.", "{1}'s personal space is violated by {0}.", "{1} is humbled by {0}, haha.", "{1} makes an unsuccessful gay pass at {0}.", "{1} is just another point on the scoreboard for {0}.", "{1} is in put in the hospital after facing {0}.", "{1} got a beat down in the prime of life by {0}.", "{1} never had a chance going up against {0}.", "{1}'s scap popped when fighting {0}.", "{1} lagged out and lost to {0}.", "{1} is annihilated by a very lucky shot from {0}.", "{1} underestimated the power of {0}'s Noisy Cricket.", "{1} didn't get a chance to put their Ray-Ban's on before {0}'s flashy-thing went off.", "{1} got a Carbonizer in the back of the head from {0}.", "{1} was cut to pieces by {0}'s Series 4 De-Atomizer.", "{1} got their atoms rearranged by {0}." };
        private readonly string[] GenericDeathMessages = new string[] { "{0} died.", "{0} had an accident.", "{0} gave up.", "{0} is fail!", "{0} went to get their gun back.", "{0} had their Series 4 De-Atomizer pointed the wrong way.", "{0} flashy-thinged himself to death.", "{0} just stepped on their last cockroach.", "{0} was swallowed alive by a giant interstellar cockroach.", "{0} died impossibly!", "{0} grenades himself.", "{0} got splatted by their own grenade.", "{0} sat on their own grenade.", "{0} got to know their grenade too well.", "{0} caught the end of their own grenade.", "{0} got too close to their own grenade.", "{0} let their own grenade get the best of him.", "{0} tiptoed over their own grenade.", "{0} stared at their grenade too long.", "{0} becomes bored with life.", "{0} checks if their gun is loaded.", "{0} committed suicide", "{0} took the easy way out", "{0} sleeps with the fishes", "{0} sucks it down", "{0} gulped a load of slime", "{0} can't exist on slime alone", "{0} burst into flames", "{0} turned into hot slag", "{0} visits the Volcano God", "{0} cratered", "{0} fell to their death", "{0} blew up", "{0} was spiked", "{0} was zapped", "{0} ate a lavaball", "{0} died", "{0} tried to leave", "{0} was squished", "{0} was telefragged by their teammate", "{0} squished a teammate", "{0} mows down a teammate", "{0} checks their glasses", "{0} gets a frag for the other team", "{0} loses another friend", "{0} was crushed by their teammate", "{0} was jumped by their teammate", "{0} died impossibly!", "{0} dispenses with himself.", "{0} detonates an ammo box too close to him", "{0} makes a crater", "{0} shoots their teammate one too many times", "{0} obstructs their team's sentry gun", "{0} tried to use the ", "{0} can't swim worth a crap!", "{0} can't breathe water", "{0} visits the Hell fires", "{0} was mauled by a Rottweiler", "{0} blew up", "{0} was crushed", "{0} was shot", "{0} was torn up by an enemy Rottweiler", "{0} was stopped by an enemy autoturret", "{0} didn't survive the operation.", "{0} tripped off the worldmap.", "{0} got knocked the fuck out!" };

        public DeathMessages()
        {
            Console.WriteLine("Death Message Commands Loaded.");
        }

        [GroupCommand]
        public async Task DeathMessage(CommandContext ctx, [Description("Optional. Person you want to see kill you. If blank, will show generic death message.")]DiscordMember target = null)
        {
            await ctx.TriggerTypingAsync();
            string line = "Something went wrong.";
            if (target == null)
            {
                int choice = rnd.Next(0, GenericDeathMessages.Length);
                line = GenericDeathMessages[choice];
                await ctx.RespondAsync(string.Format(line, ctx.Message.Author.Mention));
            }
            else
            {
                int opt = rnd.Next(1, 3);
                if (opt == 1)
                {
                    int choice = rnd.Next(0, ActiveDeathMessages.Length);
                    line = ActiveDeathMessages[choice];
                }
                else if (opt == 2)
                {
                    int choice = rnd.Next(0, PassiveDeathMessages.Length);
                    line = PassiveDeathMessages[choice];
                }
                await ctx.RespondAsync(string.Format(line, ctx.Message.Author.Mention, target.Mention));
            }
        }

        [Command("count")]
        [Description("Gets the total of each type of death message")]
        public async Task DeathMessageCount(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync($"\r\nActive: {ActiveDeathMessages.Length}\r\nPassive: {PassiveDeathMessages.Length}\r\nGeneric: {GenericDeathMessages.Length}");
        }

        [Command("script")]
        [Description("Gets a script file for all the death messages on the bot.")]
        public async Task DeathMessagesScript(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            if (File.Exists("deathmessages.cs"))
            {
                File.Delete("deathmessages.cs");
            }

            using StreamWriter file = new StreamWriter("deathmessages.cs");

            await file.WriteLineAsync($"$deathMessage::genericCount   = {GenericDeathMessages.Length};");
            await file.WriteLineAsync($"$deathMessage::activeCount    = {ActiveDeathMessages.Length};");
            await file.WriteLineAsync($"$deathMessage::passiveCount   = {PassiveDeathMessages.Length};\r\n");

            int count = 0;
            foreach (string death in GenericDeathMessages)
            {
                await file.WriteLineAsync(string.Format($"$deathMessage::generic{count}   = \"{death}\";", "%s", "%s"));
                count++;
            }
            count = 0;
            foreach (string death in ActiveDeathMessages)
            {
                await file.WriteLineAsync(string.Format($"$deathMessage::active{count}   = \"{death}\";", "%s", "%s"));
                count++;
            }
            count = 0;
            foreach (string death in PassiveDeathMessages)
            {
                await file.WriteLineAsync(string.Format($"$deathMessage::passive{count}   = \"{death}\";", "%s", "%s"));
                count++;
            }

            DiscordMessageBuilder msg = new DiscordMessageBuilder();
            file.Close();

            FileStream sound = new FileStream($"deathmessages.cs", FileMode.Open);
            msg.WithFile(sound);
            msg.Content ="Here is your request for a death message script.";
            await ctx.RespondAsync(msg);
        }
    }
}
