# About
This project is simple Discord message bot which fetches BetterTTV emote and sends it to the channel.
Also gif emotes are supported! Requested emotes are stored to cache for future use. BetterTTV API offers only
fetching with streamer/uploader searches, so a [list](/app/streamers.json) of streamers/uploaders is used and updated [] while bot is running.

# Usage
Clone the repository and run docker.sh.
After that run docker-run.sh to start the docker container.

Now you can add the bot to a server or use it in dm channels with following commands:

```
  emote/e/twitch            Request emote. Example ug: ;e pepega
  listStreamers/ls          List streamers in database. Only emotes from these streamers can be used
  addStreamer/as            Add streamer to database. Example ug. ;as sodapoppin
  forceReload/reload/r      Reload emote database. Only to be used when new emotes are published.
```

# Dependencies
This project depends on:  
    Discord.Net

# License and copyright

Licensed under the [GNU GPLv3 License](LICENSE).
