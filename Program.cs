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

        public DatabaseManager dbManager;
        public Random random;

        //For quick access during runtime?
        public List<GuildSettings> guildSettings;

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

            await RegisterCommandsAsync();

            dbManager = new DatabaseManager();

            guildSettings = dbManager.GetAllGuildSettings(); 
            //new List<GuildSettings>();//await dbManager.GetAllGuildSettings(); //as List<GuildSettings>;
            //foreach(var g in client.Guilds){
               // guildSettings.Add(dbManager.session.Query<GuildSettings>().Where(s => s.guildID == g.Id).First() as GuildSettings);
            //}
            await client.LoginAsync(TokenType.Bot, dbManager.Load<Token>("BotToken").token);

            await client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task RegisterCommandsAsync(){
            random = new Random();
            client.MessageReceived += HandleCommandAsync;
            client.JoinedGuild += OnJoinGuildAsync;
            client.UserJoined += OnUserJoinAsync;
            client.UserLeft += OnUserLeaveAsync;
            await client.SetGameAsync("rin!help");

            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        public async Task OnJoinGuildAsync(SocketGuild guild){
            await guild.DefaultChannel.TriggerTypingAsync();
            await guild.DefaultChannel.SendMessageAsync("Hi there, I'm Rin Bot! \n I'm still in development, so dont count on me all the time.");
            var settings = new GuildSettings(guild);
            guildSettings.Add(settings);
            dbManager.SaveGuildSettings(settings);
        }

        public async Task OnUserJoinAsync(SocketGuildUser user)
        {
            var guild = user.Guild;
            var setting = guildSettings.Find(g => g.guildID == guild.Id);

            if(setting.logChannelID != 0)
            {
                var channel = guild.GetChannel(setting.logChannelID) as IMessageChannel;
                EmbedBuilder embed = new EmbedBuilder()
                    .WithTitle("New User Joined Guild")
                    .WithDescription($"User: {user.Username}#{user.Discriminator}\nMention: {user.Mention}")
                    .WithColor(Color.Blue)
                    .WithFooter($"Joined: {user.JoinedAt}");
                await channel.SendMessageAsync("", false, embed.Build(), null);
            }
        }

        public async Task OnUserLeaveAsync(SocketGuildUser user)
        {
            var guild = user.Guild;
            var setting = guildSettings.Find(g => g.guildID == guild.Id);

            if(setting.logChannelID != 0)
            {
                var channel = guild.GetChannel(setting.logChannelID) as IMessageChannel;
                EmbedBuilder embed = new EmbedBuilder()
                    .WithTitle("User Left Guild")
                    .WithDescription($"User: {user.Username}#{user.Discriminator}\nMention: {user.Mention}")
                    .WithColor(Color.Red);
                await channel.SendMessageAsync("", false, embed.Build(), null);
            }
        }
        public async Task HandleCommandAsync(SocketMessage msg){
            var message = msg as SocketUserMessage;

            if(message is null || message.Author.IsBot)
                return;

            int argPos = 0;
            var context = new SocketCommandContext(client, message);
            
            GuildSettings activeGuildSettings =  guildSettings.Find(s => s.guildID == context.Guild.Id);
            if(activeGuildSettings == null)
            {
                activeGuildSettings = new GuildSettings(context);
                dbManager.SaveGuildSettings(activeGuildSettings);
                guildSettings.Add(activeGuildSettings);
            }

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
            if ( activeGuildSettings.settings[Setting.RinPing] && message.Content.Contains("<@454391745044676608>")) //activeGuildSettings.settings[Setting.RinPing] 
            {
                Random r = new Random();
                var i = r.Next(0,pingTexts.Length);
                await message.Channel.SendMessageAsync(pingTexts[i]);

                return;
            }
            if ( activeGuildSettings.settings[Setting.AllowAtSomeone] && message.Content.Contains("@someone")) //activeGuildSettings.settings[Setting.AllowAtSomeone]
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