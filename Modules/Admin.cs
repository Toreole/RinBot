using System;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Threading.Tasks;
using System.Collections.Generic;
using static RinBot.Emotes;

namespace RinBot
{
    //Admin commands n shit lol.
    public class Admin : MBase
    {
        //Toggle for allowance of @someone (important)
        //Settings per Guild stored in DB, unloaded if unused.
        [Command("toggle"), Alias("t"), Summary("Toggles a setting (not case sensitive).\n**Available Settings:** `AllowAtSomeone` `RinPing`, more coming soon.\n**Usage:** `rin!toggle [setting]`"), RequireUserPermission(GuildPermission.Administrator)]
        public async Task ToggleSettingAsync([Remainder]string setting)
        {
            var session = DBManager.session;
            Setting targetSetting;
            string targetString;
            if(Enum.TryParse<Setting>(setting, true, out targetSetting))
            {
                targetString = targetSetting.ToString();
                var guildSettings = DBManager.GetGuildSettings(Context.Guild.Id);
                if(guildSettings == null)
                {
                    //Change the targeted Value.
                    guildSettings = new GuildSettings(Context);

                    var settingValue = guildSettings.settings[targetSetting];
                    guildSettings.settings[targetSetting] = !settingValue;
                    await ReplyAsync($"Setting {targetString}` has been set to `{!settingValue}`.");
                    DBManager.SaveGuildSettings(guildSettings);
                    Rin.guildSettings.Add(guildSettings);
                } 
                else
                {
                    bool settingValue;
                    //Get current value, then change it and insert back into DB
                    //if the value exists, do this:
                    if(guildSettings.settings.TryGetValue(targetSetting, out settingValue))
                    {
                        guildSettings.settings[targetSetting] = !settingValue;
                        await ReplyAsync($"Setting `{targetString}` has been set to `{!settingValue}`.");
                    }
                    else //if the value for the setting doesnt exist, add it!
                    {
                        guildSettings.settings.Add(targetSetting, false);   
                        await ReplyAsync($"Setting `{setting}` has been initialized as `false`. (Did not exist in this Context yet.)");
                    }
                    //await ReplyAsync($"Setting {targetSetting.ToString()}` has been set to `{!settingValue}`.");
                    DBManager.SaveGuildSettings(guildSettings);
                }
            } 
            else
            {
                await ReplyAsync($"Setting name {setting} does not exist.");
            }
        }

        [Command("settings"), Summary("Show your current settings for the server."), RequireUserPermission(GuildPermission.Administrator)]
        public async Task ShowSettingsAsync()
        {
            var session = DBManager.session;
            var guildSettings = DBManager.GetGuildSettings(Context.Guild.Id);
            if(guildSettings == null)
            {
                guildSettings = new GuildSettings(Context);
                DBManager.SaveGuildSettings(guildSettings);
                Rin.guildSettings.Add(guildSettings);
            }

            string settingList = $"**Settings for {Context.Guild.Name}** \n";
            foreach (KeyValuePair<Setting, bool> entry in guildSettings.settings)
            {
                settingList += $"`{entry.Key.ToString()}` : `{entry.Value.ToString()}` \n";
            }
            if(guildSettings.logChannelID != 0)
            settingList += $"Log Channel: #{Context.Guild.GetChannel(guildSettings.logChannelID).Name}";
            await ReplyAsync(settingList);
        }

        [Command("serverlist"), RequireOwner]
        public async Task ServerListAsnc()
        {
            string servers = $"Here, im on these Servers ! {Wink2}\n";
            foreach(var guild in Rin.client.Guilds)
            {
                //Console.WriteLine();
                servers += $"{guild.Name}, by: {guild.Owner.Username}\n";
            }
            await Context.Message.Author.SendMessageAsync(servers);
            await Context.Message.DeleteAsync();
        }

        [Command("setlog"), RequireUserPermission(GuildPermission.Administrator), Summary("Set Server Log channel (User Join/Leave)\n**Usage:** rin!setlog #channel")]
        public async Task SetLogAsync([Remainder]IChannel channel)
        {
            var settings = DBManager.GetGuildSettings(Context.Guild.Id);
            if(settings == null)
                settings = new GuildSettings(Context.Guild);
            settings.logChannelID = channel.Id;
            DBManager.SaveGuildSettings(settings);
            await ReplyAsync($"Log Channel set to: #{channel.Name}");
        }

        [Command("nolog"), RequireUserPermission(GuildPermission.Administrator), Summary("Disable Server Log. `rin!help setlog` for more information.")]
        public async Task NoLogAsync()
        {
            var settings = DBManager.GetGuildSettings(Context.Guild.Id);
            if(settings == null)
                settings = new GuildSettings(Context.Guild);
            settings.logChannelID = 0;
            DBManager.SaveGuildSettings(settings);
            await ReplyAsync("Log Channel disabled.");
        }
    }
}