using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace twitch_emotes_discord
{
    public class EmoteService
    {
        private readonly HttpClient _http;
        private readonly string _path = @"./streamers.json";

        private HashSet<string> _streamers;
        private Dictionary<string, Emote> _emotes;
        private JsonSerializer _serializer = new JsonSerializer();

        public EmoteService(HttpClient http)
        {
            _http = http;
            _streamers = new HashSet<string>();
            _emotes = new Dictionary<string, Emote>();
            Console.WriteLine("EmoteService Loaded.");
        }

        public async Task LoadStreamers()
        {
            try
            {
            using (StreamReader json = File.OpenText(_path))
            {
                HashSet<string> streamers = (HashSet<string>)_serializer.Deserialize(json, typeof(HashSet<string>));
                _streamers = streamers;
            }
            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine("Streamers file not found!");
            }

            foreach (string streamer in _streamers)
            {
                await LoadEmotesAsync(streamer, true);
            }
        }

        public async Task<string> GetEmoteAsync(string emote)
        {
            emote = emote.Trim().ToLower();
            Emote value;
            if(!_emotes.TryGetValue(emote, out value)) return "null";
            return await FetchImage(value);
        }

        public async Task<int> AddStreamer(string streamer)
        {
            return await LoadEmotesAsync(streamer);
        }

        public async Task ForceReload()
        {
            _emotes = new Dictionary<string, Emote>();
            foreach(string streamer in _streamers)
            {
                await LoadEmotesAsync(streamer);
            }
        }

        public List<string> ListStreamers()
        {
            return _streamers.ToList();
        }

        public List<string> ListEmotes()
        {
            return _emotes.Keys.ToList();
        }

        private async Task<int> LoadEmotesAsync(string streamer, bool initializing = false)
        {
            streamer = streamer.Trim().ToLower();

            var resp = await _http.GetAsync($"https://api.betterttv.net/2/channels/{streamer}");
            string str = await resp.Content.ReadAsStringAsync();

            RootObject result = JsonConvert.DeserializeObject<RootObject>(str);
            if (result.emotes.Count < 1) return 0;

            if (_streamers.Add(streamer))
            {
                using (StreamWriter json = File.CreateText(_path))
                {
                    _serializer.Serialize(json, _streamers);
                }
            } else if (!initializing) return 0;

            foreach (Emote em in result.emotes) {
                try
                {
                    _emotes.Add(em.code.Trim().ToLower(), em);
                }
                catch
                {
                    //Console.WriteLine("Duplicate emote entry!");
                }
            }
            return 1;
        }

        private async Task<string> FetchImage(Emote emote, string sizeMultiplier = "3x")
        {
            string id = emote.id;
            string size = sizeMultiplier;
            string emoteFile = $@"./cache/{id}_{size}.{emote.imageType}";
            string emoteUri = $@"https://cdn.betterttv.net/emote/{id}/{size}";
            if (File.Exists(emoteFile)) return emoteFile;
            if (!Directory.Exists(@"./cache")) Directory.CreateDirectory(@"./cache");
            using (WebClient wClient = new WebClient())
            {
                await Task.Run(() => wClient.DownloadFile(new Uri(emoteUri), emoteFile));
            }
            return emoteFile;
        }
    }
}
