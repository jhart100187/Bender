using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace Bender.Main
{
    public class MemeCommand : ModuleBase
    {
        private readonly IConfiguration _config;

        public MemeCommand(IConfiguration config)
            => _config = config;

        [Command("meme")]
        [Alias("m")]
        [RequireUserPermission(GuildPermission.SendMessages)]
        public async Task SendMeme()
        {
            using (var client = new HttpClient())
            {
                var response = (await client.GetAsync(new Uri(_config["MemeApi"]))).Content.ReadAsStringAsync().Result;

                var url = response.Split("url", StringSplitOptions.RemoveEmptyEntries)[1]
                    .Split("\"", StringSplitOptions.RemoveEmptyEntries)[1];

                var embed = new EmbedBuilder();
                embed.ImageUrl = url;

                await ReplyAsync(null, false, embed.Build());
            }
        }
    }
}