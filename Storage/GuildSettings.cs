using System;
using System.Collections.Generic;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace RinBot
{
    public class GuildSettings
    {
        //Owner ID
        public ulong guildOwner;
        public ulong guildID;
        public Dictionary<string, bool> settings;
        //public bool AllowAtSomeone = true;
        //public bool RinPing = true;
        public GuildSettings(SocketCommandContext context)
        {
            guildOwner = context.Guild.OwnerId;
            guildID = context.Guild.Id;
            settings = new Dictionary<string, bool>();
            settings.Add(Setting.AllowAtSomeone.ToString(), true);
            settings.Add(Setting.RinPing.ToString(), true);
            //settings.Add("", true);
        }
        public GuildSettings(SocketGuild guild)
        {
            guildOwner = guild.OwnerId;
            guildID = guild.Id;
            settings = new Dictionary<string, bool>();
            settings.Add(Setting.AllowAtSomeone.ToString(), true);
            settings.Add(Setting.RinPing.ToString(), true);
        }

        public GuildSettings(){
            guildOwner = 0;
            guildID = 0;
            settings = new Dictionary<string, bool>();
        }
    }
}