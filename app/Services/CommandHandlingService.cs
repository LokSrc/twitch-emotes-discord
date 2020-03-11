using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace twitch_emotes_discord
{
    public class CommandHandlingService
    {
        private readonly CommandService         _commands;
        private readonly DiscordSocketClient    _client;
        private readonly IServiceProvider       _services;

        public CommandHandlingService(IServiceProvider services)
        {
            _commands   = services.GetRequiredService<CommandService>();
            _client     = services.GetRequiredService<DiscordSocketClient>();
            _services   = services;

            // Hooks
            _commands.CommandExecuted += CommandExecutedAsync;
            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            await _services.GetService<EmoteService>().LoadStreamers();
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if(!command.IsSpecified)
                return;
            if(result.IsSuccess)
                return;
            await context.Channel.SendMessageAsync($"error: {result}");
        }

        public async Task MessageReceivedAsync(SocketMessage rawMsg)
        {
            if(!(rawMsg is SocketUserMessage message)) return;
            if(message.Source != MessageSource.User) return;

            var argPos = 0;
            if(!message.HasCharPrefix(';', ref argPos)) return;

            var context = new SocketCommandContext(_client, message);
            await _commands.ExecuteAsync(context, argPos, _services);
        }

    }
}
