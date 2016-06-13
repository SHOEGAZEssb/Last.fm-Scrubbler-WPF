# Last.fm-Scrubbler-WPF
Manual Last.fm scrobbling for when a service (or you!) failed to scrobble.

## Features
### Manual Single Track Scrobbling
Allows you to enter artist, track and album info aswell as when you listened to the song and lets you scrobble it.

![ManualScrobble](https://ibin.co/2jj4riPWJvZB.png)

### Friend Scrobbling
Allows you to fetch recent scrobbles of any last.fm user and scrobble them to your account.

![FriendScrobble](https://imagebin.ca/2jj5WvbSDVRp/FriendScrobble.png)

### Database Scrobbling
Search Last.fm for artists and albums and scrobble one or more tracks from it.

![ManualScrobble Artist Search](https://ibin.co/2jj5mE7b1g6j.png)

![ManualScrobble Album Search](https://ibin.co/2jj5zxquKBgv.png)

![ManualScrobble Tracklist](https://ibin.co/2jj6BpRDoGFl.png)

### CSV Scrobbling
Allows you to load a .csv file and scrobble the info contained in it.
Currently only supports the following structure:

"Artist, Album, Track, Timestamp"
Needs to be excactly in this structure to work.
Individual fields can be enclosed by quotes and NEED to be enclosed by quotes if the field contains a comma.
For example:

"ArtistWith, CommaInTheName", Album, Track, 06/13/2016 19:54 

CSV scrobbling has two modes. They can be changed with the "Scrobbling Mode" ComboBox.

Normal Mode:

In this mode the tracks will be scrobbled with the timestamp from the csv field. Only scrobbles newer than 14 days can be scrobbled.


Import Mode:

In this mode the tracks will be scrobbled with the timestamp calculated from the "Finish Time" and the selected duration between each track. This allows the scrobbling of the tracks even if the timestamp in the csv is older than 14 days. Therefore the first (upmost) track in the csv file will be scrobbled with the "Finish Time".

![CSV Scrobbling](https://ibin.co/2kAkY5hpYtEi.png)
Normal Mode

![CSV Scrobbling Import Mode](https://ibin.co/2kXudUqJloTY.png)
Import Mode
