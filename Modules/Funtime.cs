using System;
using Discord.WebSocket;
using Discord.Commands;
using System.Threading.Tasks;
using Discord;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
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

        [Command("owo"), Summary("OWO at Rin. `rin!owo`")]
        public async Task OwoAsync(){
            await ReplyAsync($"Ey, stop looking at me like that. {Heh}");
        }

        [Command("yay"), Summary("Force Rin to congratulate someone. `rin!yay [username/nickname/@someone]`")]
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

        [Command("oof"), Summary("A major OOF. `rin!oof`")]
        public async Task OofAsync(){
            await ReplyAsync($"{OOF}{OOF}{OOF}   {OOF}{OOF}{OOF}   {OOF}{OOF}{OOF}\n" +
                             $"{OOF}{Empty}{OOF}   {OOF}{Empty}{OOF}   {OOF}\n"+
                             $"{OOF}{Empty}{OOF}   {OOF}{Empty}{OOF}   {OOF}{OOF}\n"+
                             $"{OOF}{Empty}{OOF}   {OOF}{Empty}{OOF}   {OOF}\n"+
                             $"{OOF}{OOF}{OOF}   {OOF}{OOF}{OOF}   {OOF}\n");
        }
        
        [Command("test"), RequireOwner]
        public async Task TestAsync()
        {
            await ReplyAsync("test");
        }

        [Command("dadjoke"), Alias("badjoke"), Summary("Be the best dad with these dadjokes. `rin!dadjoke` or `rin!badjoke`")]
        public async Task DadjokeAsync()
        {
            await Context.Channel.TriggerTypingAsync();
            string url = "https://icanhazdadjoke.com/";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("User-Agent", "Discord Bot Rin");
            request.Headers.Add("Accept", "text/plain");

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream resStream = response.GetResponseStream();
            byte[] buffer = new byte[256];
            int error = resStream.Read(buffer, 0, 256);
            string joke = Encoding.UTF8.GetString(buffer.Where(b => b != 0).ToArray());
            
            EmbedBuilder embed = new EmbedBuilder()
                .WithDescription($"{joke} {Heh}")
                .WithAuthor($"Dadjoke for {Context.Message.Author.Username}")
                .WithFooter("Dadjokes powered by icanhazdadjoke.com");

            await ReplyAsync ("", false, embed.Build(), null);
        }

        [Command("insult"), Summary("Insults someone badly. Use at own risk."), RequireOwner]
        public async Task InsultAsync([Remainder]IUser target)
        {
            await Context.Channel.TriggerTypingAsync();
            string url = "https://insult.mattbas.org/api/en/insult.txt";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //request.Headers.Add("who", $"{target.Mention}");

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream resStream = response.GetResponseStream();
            byte[] buffer = new byte[256];
            int error = resStream.Read(buffer, 0, 256);
            string insult = Encoding.UTF8.GetString(buffer.Where(b => b != 0).ToArray());
            
            EmbedBuilder embed = new EmbedBuilder()
                .WithDescription($"{insult}. {OOF}")
                .WithAuthor($"Insult for {target.Username}")
                .WithFooter("Insults powered by insults.mattbas.org");

            await ReplyAsync ("", false, embed.Build(), null);
        }

        [Command("tl;dr"), Alias("tldr"), Summary("Sums up the last X messages... maybe accurately")]
        public async Task TlDrAsync([Remainder]int amount)
        {
            await Context.Channel.TriggerTypingAsync();
            if (amount > 250) amount = 250;
            if (amount < 20) amount = 20;
            var messages = await Context.Channel.GetMessagesAsync(amount).FlattenAsync();
            var flatMessages = messages.Where(m => m.GetType() != typeof(Discord.Rest.RestSystemMessage)).Cast<IUserMessage>().ToArray();
            for(int i = 0; i < 5; i++)
            {
                var msg = flatMessages[Rin.random.Next(0, flatMessages.Length)] as IUserMessage;
                await ReplyAsync($"{msg.Author.Username}: {msg.Content}");
            }
        }
    }
}