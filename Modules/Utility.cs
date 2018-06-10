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
    public class UtilityModule : ModuleBase<SocketCommandContext>
    {
        [Command("help"), Summary("its help, what do you expect")]
        public async Task HelpAsync(){
            await ReplyAsync("Help yourself...");

            var commandList = 
            "**Useful Commands:**\n" +
            "`help` \n" +
            "**Weirdo Commands:**\n"+
            "`yay` `owo` `oof`\n" +
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
        public async Task HelpAsync([Remainder]string command){
            await Context.Channel.TriggerTypingAsync();

            var assembly = Assembly.GetEntryAssembly();

            //Get method with CommandAttribute named string command, that also has a SummaryAttribute. The Summary is the target of this method.

            //Get all commands from the assembly oof not nice.
            List<MethodInfo> commands = assembly.GetTypes().SelectMany(t => t.GetMethods()).ToList();

            MethodInfo targetCommand = commands.Find(x => x.GetCustomAttributes().Where(at => at.GetType().Equals(typeof(CommandAttribute)) && (at as CommandAttribute).Text.Equals(command)).Any());

            if(targetCommand == null)
            {
                await ReplyAsync($"Could not find command `{command}`");
                return;
            }
            string summary = $"Command `{command}` has no summary, just try it.";

            var sums = targetCommand.GetCustomAttributes().Where(mi => mi.GetType().Equals(typeof(SummaryAttribute))).ToArray(); //

            if(sums.Length == 0){
                await ReplyAsync(summary);
                return;
            }

            summary = (sums[0] as SummaryAttribute).Text;
            
            string output = $"{command} : {summary}";
            await ReplyAsync(output);
        }
    }
}