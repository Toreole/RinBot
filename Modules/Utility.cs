using System;
using Discord.WebSocket;
using Discord.Commands;
using System.Threading.Tasks;
using Discord;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace RinBot
{
    public class UtilityModule : MBase
    {
        [Command("help"), Summary("its help, what do you expect. `rin!help [command]`")]
        public async Task HelpAsync(){
            await ReplyAsync("Help yourself...");

            var commandList = 
            "**Useful Commands:**\n" +
            "`help` `info`\n" +
            "**Fun Commands:**\n"+
            "`yay` `owo` `oof` `dadjoke`\n" +
            "**Moderator Commands:**\n" +
            "`purge` \n" +
            "**Other Stuff:** \n" + 
            "Try using @someone and see what happens. ;) \n";

            await Context.Channel.TriggerTypingAsync();
            await Task.Delay(500);
            EmbedBuilder builder = new EmbedBuilder()
            .WithTitle("Rin Command List")
            .WithFooter("Made by Toreole")
            .WithDescription(commandList);

            await ReplyAsync("", false, builder.Build(), null);
        }

        [Command("help")]
        public async Task HelpAsync([Remainder]string command)
        {
            await Context.Channel.TriggerTypingAsync();
            var commandlist = RinCommandService.Commands;
            var target = commandlist.Where(c => c.Name == command);
            string output;
            if(target.Count() == 0)
            {
                output = $"Command `{command}` has no does not exist.";
            } 
            else 
            {
                var com = target.First() as CommandInfo;
                output = (!string.IsNullOrEmpty(com.Summary))? $"**{command}** : {com.Summary}" : $"Command `{command}` has no summary, try it out!";
            }
            await ReplyAsync(output);
#region old_help_linq
            //var assembly = Assembly.GetEntryAssembly();

            //Get method with CommandAttribute named string command, that also has a SummaryAttribute. The Summary is the target of this method.

            //Get all commands from the assembly oof not nice.
            //List<MethodInfo> commands = assembly.GetTypes().SelectMany(t => t.GetMethods()).ToList();

            //MethodInfo targetCommand = commands.Find(x => x.GetCustomAttributes().Where(at => at.GetType().Equals(typeof(CommandAttribute)) && (at as CommandAttribute).Text.Equals(command)).Any());

            //if(targetCommand == null)
            //{
            //    await ReplyAsync($"Could not find command `{command}`");
            //    return;
            //}
            //string summary = $"Command `{command}` has no summary, just try it.";

            //var sums = targetCommand.GetCustomAttributes().Where(mi => mi.GetType().Equals(typeof(SummaryAttribute))).ToArray(); //
            
            //if(sums.Length == 0){
            //    await ReplyAsync(summary);
            //    return;
            //}

            //summary = (sums[0] as SummaryAttribute).Text;
            
            //string output = $"{command} : {summary}";
            //await ReplyAsync(output);
#endregion
        }

        [Command("purge"), RequireBotPermission(GuildPermission.ManageMessages), Summary("Deletes a given amount of messages, only for Moderators and above: `rin!purge 10`"), RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task PurgeAsync([Remainder] int amount)
        {
            //Console.WriteLine($"Purge {amount}");
            var messages = await Context.Channel.GetMessagesAsync(amount).FlattenAsync();
            var flatMessages = messages.Cast<IUserMessage>();
            
            var channel = Context.Channel as ITextChannel;
            await channel.DeleteMessagesAsync(flatMessages);
            //foreach(IMessage msg in messages)
               // await Context.Channel.DeleteMessageAsync(msg);
        }

        [Command("info"), RequireBotPermission(ChannelPermission.SendMessages), Summary("General info about Rin Bot.")]
        public async Task InfoAsync(){
            string info = 
                "**Info on Rin Bot** \n" +
                "Rin Bot is a neat little Bot for all kinds of memes and other stuff. \n" + 
                "Her personality is based on Rin Tohsaka from Fate/Stay Night Unlimited Bladeworks. \n" +
                "She's been in development as a fun project since the 9th June 2018. \n" +
                "Currently she's hosted on my own dekstop PC, alongside the RavenDB for data storage. \n" +
                "*Have fun with this bot!*";
            EmbedBuilder embed = new EmbedBuilder()
                .WithAuthor("Rin Bot")
                .WithDescription(info)
                .WithFooter("Made by Toreole");
            await ReplyAsync("", false, embed.Build(), null);
        }
    }
}