using System;
using Discord;
using Discord.Commands;

namespace RinBot{
    public class MBase : ModuleBase<SocketCommandContext>
    {
     public Program Rin { get => Program.Rin; }
     public CommandService RinCommandService {get => Rin.commands; }
    }
}