using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord;
using Discord.Commands;

namespace twitch_emotes_discord
{

    public class CommandModule : ModuleBase<SocketCommandContext>
    {
        public EmoteService EmoteService { get; set; }

        [Command("emote")]
        [Alias("e","twitch")]
        public async Task PepegaAsync(params string[] prms)
        {
            if (!Context.IsPrivate)
                await Context.Message.DeleteAsync();
            if (prms.Length < 1)
            {
                await Context.User.SendMessageAsync("Emote identifier not provided!.");
                return;
            }
            string path = await EmoteService.GetEmoteAsync(prms[0]);
            if (path == "null")
            {
                await Context.User.SendMessageAsync("Emote not found. Check for misspelling and try adding a new streamer (whos emote it is) with command ;addStreamer STREAMER");
                return;
            }
            await Context.Channel.SendFileAsync(path, "Requested by: " + Context.Message.Author.Mention);
        }

        // Discord doesnt allow messages large as this :(
        //[Command("listEmotes")]
        //[Alias("le")]
        //public async Task ListEmotesAsync()
        //{
        //    await Context.Message.DeleteAsync();
        //    List<string> l = EmoteService.ListEmotes();
        //    string msg = "```";
        //    msg += "\n Emotes loaded:\n";
        //    msg += ListToMsg(l);
        //    msg += "```";
        //    await Context.User.SendMessageAsync(msg);
        //}

        [Command("listStreamers")]
        [Alias("ls")]
        public async Task ListStreamersAsync()
        {
            if (!Context.IsPrivate)
                await Context.Message.DeleteAsync();
            List<string> l = EmoteService.ListStreamers();
            string msg = "```";
            msg += "\n Streamers in database:\n";
            msg += ListToMsg(l);
            msg += "```";
            await Context.User.SendMessageAsync(msg);
        }

        [Command("addStreamer")]
        [Alias("as","addstreamer")]
        public async Task AddStreamer(params string[] prms)
        {
            if(!Context.IsPrivate)
                await Context.Message.DeleteAsync();
            if (prms.Length < 1)
            {
                await Context.User.SendMessageAsync("Please provide streamer name to add. ;addStreamer STREAMER");
                return;
            }
            int res = await EmoteService.AddStreamer(prms[0]);
            if (res == 0)
            {
                await Context.User.SendMessageAsync("Streamer was not found or is already in database. Check with ;listStreamers");
                return;
            }
            await Context.User.SendMessageAsync($"Streamer: {prms[0]} succesfully added to database!");
        }

        [Command("forceReload")]
        [Alias("reload","r")]
        public async Task ForceReload()
        {
            if (!Context.IsPrivate)
                await Context.Message.DeleteAsync();
            await EmoteService.ForceReload();
            await Context.User.SendMessageAsync("Emote lists reloaded!");
        }

        [Command("help")]
        public async Task Help()
        {
            if (!Context.IsPrivate)
                await Context.Message.DeleteAsync();
            string msg = "```\nAvailable commands:\n  emote/e/twitch            Request emote. Example ug: ;e pepega\n  listStreamers/ls          List streamers in database. Only emotes from these streamers can be used\n  addStreamer/as            Add streamer to database. Example ug. ;as sodapoppin\n  forceReload/reload/r      Reload emote database. Only to be used when new emotes are published.\n```";
                await Context.User.SendMessageAsync(msg);
        }

        private string ListToMsg(List<string> list)
        {
            string msg = "";
            foreach (string s in list)
            {
                msg += "  " + s;
                msg += "\n";
            }
            return msg;
        }
    }
}
