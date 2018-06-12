using System;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RinBot
{
    //Admin commands n shit lol.
    public class Admin : MBase
    {
        //Toggle for allowance of @someone (important)
        //Settings per Guild stored in DB, unloaded if unused.
        [Command("toggle"), Summary("Toggles a setting. \n Available Settings: `AllowAtSomeone` `RinPing`, more coming soon."), RequireUserPermission(GuildPermission.Administrator)]
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

                    var settingValue = guildSettings.settings[targetString];
                    guildSettings.settings[targetString] = !settingValue;
                    await ReplyAsync($"Setting {targetString}` has been set to `{!settingValue}`.");
                    DBManager.SaveGuildSettings(guildSettings);
                    Rin.guildSettings.Add(guildSettings);
                } 
                else
                {
                    bool settingValue;
                    //Get current value, then change it and insert back into DB
                    //if the value exists, do this:
                    if(guildSettings.settings.TryGetValue(targetString, out settingValue))
                    {
                        guildSettings.settings[targetString] = !settingValue;
                        await ReplyAsync($"Setting `{targetString}` has been set to `{!settingValue}`.");
                    }
                    else //if the value for the setting doesnt exist, add it!
                    {
                        guildSettings.settings.Add(targetString, false);   
                    }
                    //await ReplyAsync($"Setting {targetSetting.ToString()}` has been set to `{!settingValue}`.");
                    DBManager.SaveGuildSettings(guildSettings);
                    await ReplyAsync($"Setting `{setting}` has been initialized as `false`. (Did not exist in this Context yet.)");
                }
            } 
            else
            {
                await ReplyAsync($"Setting name {setting} does not exist.");
            }
        }

        [Command("settinglist"), Summary("Show your current settings for the server"), RequireUserPermission(GuildPermission.Administrator)]
        public async Task ShowSettingsAsync(){
            var session = DBManager.session;
            var guildSettings = DBManager.GetGuildSettings(Context.Guild.Id);
            if(guildSettings == null)
            {
                guildSettings = new GuildSettings(Context);
                DBManager.SaveGuildSettings(guildSettings);
                Rin.guildSettings.Add(guildSettings);
            }

            string settingList = $"**Settings for {Context.Guild.Name}** \n";
            foreach (KeyValuePair<string, bool> entry in guildSettings.settings)
            {
                settingList += $"`{entry.Key}` : `{entry.Value.ToString()}` \n";
            }
            await ReplyAsync(settingList);
        }

        [Command("serverlist"), RequireOwner]
        public async Task ServerListAsnc()
        {
            foreach(var guild in Rin.client.Guilds)
            {
                Console.WriteLine($"{guild.Name}, {guild.Owner.Username}");
            }
            await Task.Delay(1);
        }
    }
}