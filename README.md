
# Last.fm-Scrubbler-WPF
Manual Last.fm scrobbling for when a service (or you!) failed to scrobble.

[![Build status](https://ci.appveyor.com/api/projects/status/6dm7kutqw56irq97?svg=true)](https://ci.appveyor.com/project/coczero/last-fm-scrubbler-wpf)
### IMPORTANT
This app is still in beta. It did not get a lot of testing. I recommend trying to scrobble to a test account first and see if the things you want to scrobble do so correctly. Especially if you scrobble a lot of tracks at once there can be some problems. Please be careful with your accounts.
### Scrobble Cap ###
Previously the Last.fm-Scrubbler-WPF did not have a scrobbles per day limit. This has now changed since some people used the Scrubbler to scrobble so much tracks, that it caused problems for the last.fm page. The scrobble limit is currently 2800 scrobbles per day. When you try to scrobble more than that you will get an error message. I know this makes some functions less useful, but it is better to be safe. There are plans to add a "scrobble queue", so if you need to scrobble more than 2800 tracks, they get added to a queue and are automatically scrobbled after some time. You can check how many scrobbles you have left in the user selection view.

![UserView](https://i.imgur.com/cVTPSmW.png)
### Donate ###
This tool will always be free, but if it was helpful to you, consider donating to further support its development.

[![](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=2B2EPP7NNKBYW)

## Features

**Scrobblers**
- [Manual Single Track Scrobbling](#manual-single-track-scrobbling)
- [Friend Scrobbling](#friend-scrobbling)
- [Database Scrobbling](#database-scrobbling)
- [CSV Scrobbling](#csv-scrobbling)
- [File Scrobbling](#file-scrobbling)
- [Media Player Database Scrobbling](#media-player-database-scrobbling)
- [iTunes Scrobbling](#itunes-scrobbling)
- [Spotify Desktop Scrobbling](#spotify-desktop-scrobbling)
- [Setlist.fm Scrobbling](#setlistfm-scrobbling)

**Extra Functions**
- [Paste Your Taste](#paste-your-taste)
- [CSV Downloader](#csv-downloader)
- [Collage Creator](#collage-creator)
- [Milestone Checker](#milestone-checker)

### Manual Single Track Scrobbling
Allows you to enter artist, track and album info aswell as when you listened to the song and lets you scrobble it.

![ManualScrobble](https://i.imgur.com/CzaVRgK.png)

### Friend Scrobbling
Allows you to fetch recent scrobbles of any last.fm user and scrobble them to your account.

![FriendScrobble](https://i.imgur.com/UdgKIDT.png)

### Database Scrobbling
Search Last.fm for artists and albums and scrobble one or more tracks from it.

![ManualScrobble Artist Search](https://i.imgur.com/quuzcDw.png)

![ManualScrobble Album Search](https://i.imgur.com/gPaSYDR.png)

![ManualScrobble Tracklist](https://i.imgur.com/Yfjx75a.png)

### CSV Scrobbling
Allows you to load a .csv file and scrobble the info contained in it.
You can download the csv of any account with the built-in [CSV Downloader](#csv-downloader).
If the .csv file you have has a different data sequence, you can configure the field indexes via the "Settings" button.

![CSV Scrobbling Settings](https://i.imgur.com/RZUvCvZ.png)

Individual fields can be enclosed by quotes and NEED to be enclosed by quotes if the field contains any of the set delimiters.
For example:

"ArtistWith, CommaInTheName", Album, Track, 06/13/2016 19:54, AlbumArtist, 00:02:33 

CSV scrobbling has two modes. They can be changed with the "Scrobbling Mode" ComboBox.

Normal Mode:

In this mode the tracks will be scrobbled with the timestamp from the csv field. Only scrobbles newer than 14 days can be scrobbled.


Import Mode:

In this mode the tracks will be scrobbled with the timestamp calculated from the "Finish Time" and the selected duration between each track. This allows the scrobbling of the tracks even if the timestamp in the csv is older than 14 days. Therefore the first (upmost) track in the csv file will be scrobbled with the "Finish Time".

![CSV Scrobbling](https://i.imgur.com/4M5J0V4.png)
Normal Mode

![CSV Scrobbling Import Mode](https://i.imgur.com/nnBh7os.png)
Import Mode

### File Scrobbling
Allows you to select music files (currently .flac, .mp3, .m4a and .wma) from your computer and scrobble them.
Timestamps will be reversed finishing with the "Finishing Time". So the last track in the list will be scrobbled with the "Finishing Time"

![File Scrobbler](https://i.imgur.com/z5CZgPv.png)

### Media Player Database Scrobbling
A lot of media players store their song library in a specific file. Often times, the amount of times a specific song has been played is in there too. This can be used to import your complete listening history of your media player into last.fm. Although without correct timestamps, it is still useful to have all your artists and songs back in your last.fm account.

Currently works with iTunes, Winamp and Windows Media Player. 

You can read how to export your iTunes database as xml [here](http://ccm.net/faq/42531-enable-the-sharing-of-itunes-library-xml-with-other-applications).

You can read how to export your Winamp database as xml [here](http://forums.winamp.com/showthread.php?t=334048) (only step 1 is required).

![Media Player Database Scrobbling](https://i.imgur.com/PgC0QoA.png)

Once you click "Scrobble", all selected songs will be scrobbled to your last.fm account, starting with the last track in the list and finishing with the first in the list. Timestamp will be current date time - 1 second.

### iTunes Scrobbling
This basically works like the normal Last.fm desktop app. Once you connected to iTunes a timer will measure how long you listened to that song. If you play 50% of the track (tracks under 30 seconds will work), it will be scrobbled. The ProgressBar on the bottom indicates when the track will be scrobbled.

![iTunes Scrobbling](https://i.imgur.com/cxMIrvl.png)

### Spotify Desktop Scrobbling
With this you can scrobble a local desktop client just like iTunes. Scrobbling songs shorter than 30 seconds will work.
Important note: scrobblings songs on repeat does NOT work currently.

![Spotify Desktop Scrobbling](https://i.imgur.com/m8uk7hB.png)

### SetlistFM Scrobbling
This feature allows you to search for artists on [Setlist.fm](https://setlist.fm) and scrobble their live sets. The search is still very basic. 

![Setlist.fm Artist Search](https://i.imgur.com/gx4Q4M6.png)

When you click on an artist, the setlists of that artist are shown.

![Setlist.fm Setlist Results](https://i.imgur.com/zqcoTYm.png)

And finally when you click on a setlist its tracks are shown and you can select which ones to scrobble and with what timestamps.  You can also add a custom album string. If you don't want to add a custom album string, the tracks will be scrobbled with blank album info.

![Setlist.fm Setlist Tracks](https://i.imgur.com/uUZGI6A.png)

As always, timestamps are reversed meaning the first track in the list gets scrobbled with the oldest timestamp and the last track in the list is scrobbled with the "Finishing Time".

### Paste Your Taste
With this you can create a "Paste Your Taste" text. You can select the time period and how many artists you want to include.

![Paste Your Taste](https://i.imgur.com/rYwwFcR.png)

### CSV Downloader
This allows you to download the data of any Last.fm user as a csv file. The format of the csv file will be "Artist, Album, Track, Timestamp, Album Artist, Timestamp".

![CSV Downloader](https://i.imgur.com/itgokSP.png)

### Collage Creator
This allows you to create collages of your top artists and album arranged in a grid with artist or album photo. Just pick a size, enter your username and the collage will be created and uploaded to imgur (optional) and you can save it locally.

![Collage Creator](https://i.imgur.com/jrsqznV.png)

### Milestone Checker
This allows you to see the nth played track from your library. Simply fetch the scrobble data and you can check either your nth played tracks or your xth played track! Unfortunately, some old imported scrobbles have incorrect timestamps that can influence your milestones.

![Milestone Checker](https://i.imgur.com/qGqvQts.png)
