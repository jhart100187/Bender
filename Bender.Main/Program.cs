using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.Net;
using Discord.Rest;
using Discord.Webhook;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Discord.WebSocket;
using Discord.Commands;

namespace Bender.Main
{
    public class Program
    {
        private IConfiguration _config => new ConfigurationBuilder()
            .SetBasePath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../"))
                .AddJsonFile("appsettings.json").Build();

        private DiscordSocketClient _client { get; set; }

        public static void Main(string[] args)
            => new Program().Initialize().GetAwaiter().GetResult();

        private async Task Initialize()
        {
            using (var services = ConfigureServices())
            {
                _client = services.GetRequiredService<DiscordSocketClient>();
                _client.Log += LogAsync;
                _client.Ready += ReadyAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                await _client.LoginAsync(TokenType.Bot, _config["Token"]);
                await _client.StartAsync();
                await (services.GetRequiredService<CommandHandler>()).InitializeAsync();

                Console.ReadLine();
                await _client.LogoutAsync();
            }
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(_config)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"Connected as -> [{_client.CurrentUser}] :)");
            return Task.CompletedTask;
        }
    }
}
