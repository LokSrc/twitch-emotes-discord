using System.Collections.Generic;

namespace twitch_emotes_discord
{
    public class RootObject
    {
        public string urlTemplate { get; set; }
        public List<string> bots { get; set; }
        public List<Emote> emotes { get; set; }
    }
}
