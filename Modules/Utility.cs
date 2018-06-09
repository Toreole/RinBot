using System;
using Discord.WebSocket;
using Discord.Commands;
using System.Threading.Tasks;

namespace RinBot
{
    public class UtilityModule : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task HelpAsync(){
            await ReplyAsync("hello");
        }
    }
}