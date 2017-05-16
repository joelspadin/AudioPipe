# AudioPipe

AudioPipe is an application that captures the audio being sent to one playback 
device and redirects it to a different one. Its main purpose is to work around
an issue with Final Fantasy XIV where audio stops working until you restart the
game if you change the system playback device. By using AudioPipe instead, you 
can effectively change the playback device without breaking the game's audio.

AudioPipe uses WASAPI capture via [CSCore](https://github.com/filoe/cscore).
The UI code is based on [EarTrumpet](https://github.com/File-New-Project/EarTrumpet).
[The icon](https://thenounproject.com/term/audio-to-audio/914488/) is by Oliviu Stoian.