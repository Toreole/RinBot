using System;
using Discord;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using static RinBot.Emotes;

namespace RinBot
{
    public class Program
    {
        public static Program Rin { get; private set; }

        public CommandService commands;
        public DiscordSocketClient client;
        public IServiceProvider services;

        public Random random;

        string[] pingTexts = { 
            $"Why do you ping me?! {Despair}",
            $"PLS NO PING {Reeee}",
            "...", 
            "Cut it out!", 
            $"{Baka}",
            "Can u don't?", 
            "No u" , 
            $"{Eeeeh}"
            };

        static void Main(string[] args) => new Program().GoRin().GetAwaiter().GetResult();

        public async Task GoRin(){
            Rin = this;

            client = new DiscordSocketClient();
            commands = new CommandService();
            services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commands)
                .BuildServiceProvider();

            Console.WriteLine("Please enter login token:");
            //string token = Console.ReadLine();

            await RegisterCommandsAsync();

            await client.LoginAsync(TokenType.Bot, "");

            await client.StartAsync();
            
            await Task.Delay(-1);
        }

        private async Task RegisterCommandsAsync(){
            random = new Random();
            client.MessageReceived += HandleCommandAsync;
            await client.SetGameAsync("rin!help");

            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        public async Task HandleCommandAsync(SocketMessage msg){
            var message = msg as SocketUserMessage;

            if(message is null || message.Author.IsBot)
                return;

            int argPos = 0;
            var context = new SocketCommandContext(client, message);

            if (message.HasStringPrefix("rin!", ref argPos))
            {
                var result = await commands.ExecuteAsync(context, argPos, services);
                
                if (!result.IsSuccess)
                    Console.WriteLine(result.ErrorReason);
                if(result.Error == CommandError.UnknownCommand)
                {
                    await message.Channel.SendMessageAsync("I can't do that! (yet?) Use rin!help if you're unsure!");
                }
                return;
            }
            if (message.Content.Contains("<@454391745044676608>"))
            {
                Random r = new Random();
                var i = r.Next(0,pingTexts.Length);
                await message.Channel.SendMessageAsync(pingTexts[i]);

                return;
            }
            if (message.Content.Contains("@someone"))
            {
                var possible = context.Guild.Users.Where(x => x.Id != client.CurrentUser.Id && x != message.Author);
                var target = possible.ElementAt(random.Next(0, possible.Count()));
                //await message.Channel.SendMessageAsync($"{target.Mention} {text}");
                var text = message.Content.Replace("@someone", $"{target.Mention}");
                await message.Channel.SendMessageAsync($"{text} - by {message.Author.Username}, using @someone");
                await message.DeleteAsync();
            }
        }
    }
}