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
        public Dictionary<Setting, bool> settings;
        public ulong logChannelID = 0;
        //public bool AllowAtSomeone = true;
        //public bool RinPing = true;
        public GuildSettings(SocketCommandContext context)
        {
            guildOwner = context.Guild.OwnerId;
            guildID = context.Guild.Id;
            settings = new Dictionary<Setting, bool>();
            settings.Add(Setting.AllowAtSomeone, true);
            settings.Add(Setting.RinPing, true);
            //settings.Add("", true);
        }
        public GuildSettings(SocketGuild guild)
        {
            guildOwner = guild.OwnerId;
            guildID = guild.Id;
            settings = new Dictionary<Setting, bool>();
            settings.Add(Setting.AllowAtSomeone, true);
            settings.Add(Setting.RinPing, true);
        }

        public GuildSettings(){
            guildOwner = 0;
            guildID = 0;
            settings = new Dictionary<Setting, bool>();
        }
    }
}