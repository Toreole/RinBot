using System;
using Discord.WebSocket;
using Discord.Commands;
using System.Threading.Tasks;
using Discord;
using System.Linq;
using static RinBot.Emotes;

namespace RinBot
{
    public class Funtime : MBase
    {
        string[] proudText = {
            "I'm proud of you {0}, whatever you did. <:RinWink2:454380215435526146>",
            "Great job {0}!",
            "Didn't fuck this one up, eh {0}!",
            "Awesome stuff {0}, really!",
            "Could have been a lot worse {0}, a whole lot worse. "
            };


        [Command("owo"), Summary("owo at rin")]
        public async Task OwoAsync(){
            await ReplyAsync($"Ey, stop looking at me like that. {Heh}");
        }

        [Command("yay"), Summary("congratulate a user, @someone, or *maybe* yourself.")]
        public async Task YayAsync([Remainder]IUser user){
            Random random = new Random();
            var index = random.Next(0, proudText.Length);
            var msg = string.Format(proudText[index], user.Mention);

            await ReplyAsync(msg);
        }

        [Command("yay")]
        public async Task YayAsync([Remainder]string text){
            if(text.Contains("@someone"))
            {
                Random random = new Random();
                var message = Context.Message;
                var possible = Context.Guild.Users.Where(x => x.Id != Context.Client.CurrentUser.Id && x != message.Author);
                var target = possible.ElementAt(random.Next(0, possible.Count()));
                //await message.Channel.SendMessageAsync($"{target.Mention} {text}");
                var index = random.Next(0, proudText.Length);
                var msg = string.Format(proudText[index], target.Mention);
                await message.Channel.SendMessageAsync(msg);
                await message.DeleteAsync();
            }
            else 
            {
                await Context.Channel.SendMessageAsync($"Is {text} even a thing?");
            }
        }

        [Command("yay")]
        public async Task YayAsync(){
            await ReplyAsync($"You wanna congratulate yourself? {Eeeeh} For what?");
        }

        [Command("oof"), Summary("a major oof")]
        public async Task OofAsync(){
            await ReplyAsync("<:oof:455340425394520065><:oof:455340425394520065><:oof:455340425394520065>   <:oof:455340425394520065><:oof:455340425394520065><:oof:455340425394520065>   <:oof:455340425394520065><:oof:455340425394520065><:oof:455340425394520065>\n" +
                             "<:oof:455340425394520065>       <:oof:455340425394520065>   <:oof:455340425394520065>       <:oof:455340425394520065>   <:oof:455340425394520065>\n"+
                             "<:oof:455340425394520065>       <:oof:455340425394520065>   <:oof:455340425394520065>       <:oof:455340425394520065>   <:oof:455340425394520065><:oof:455340425394520065>\n"+
                             "<:oof:455340425394520065>       <:oof:455340425394520065>   <:oof:455340425394520065>       <:oof:455340425394520065>   <:oof:455340425394520065>\n"+
                             "<:oof:455340425394520065><:oof:455340425394520065><:oof:455340425394520065>   <:oof:455340425394520065><:oof:455340425394520065><:oof:455340425394520065>   <:oof:455340425394520065>\n");
        }
        
        [Command("test")]
        public async Task TestAsync(){
            await ReplyAsync("test");
        }
    }
}