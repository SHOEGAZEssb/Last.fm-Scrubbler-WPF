# Last.fm-Scrubbler-WPF
Manual Last.fm scrobbling for when a service (or you!) failed to scrobble.
### IMPORTANT
This app is still in beta. It did not get a lot of testing. I recommend trying to scrobble to a test account first and see if the things you want to scrobble do so correctly. Especially if you scrobble a lot of tracks at once there can be some problems. Please be careful with your accounts.
### Donate ###
This tool will always be free, but if it was helpful to you, consider donating to further support its development.

[![](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=2B2EPP7NNKBYW)

## Features

**Table of Contents**

- [Manual Single Track Scrobbling](#Manual Scrobbling)
- [Friend Scrobbling](#Friend Scrobbling)
- [Database Scrobbling](#Database Scrobbling)
- [CSV Scrobbling](#CSV Scrobbling)
- [File Scrobbling](#File Scrobbling)
- [Media Player Database Scrobbling](#Media Player Database Scrobbling)
- [iTunes Scrobbling](#iTunes Scrobbling)

<a name="Manual Scrobbling"/>
### Manual Single Track Scrobbling
Allows you to enter artist, track and album info aswell as when you listened to the song and lets you scrobble it.

![ManualScrobble](https://ibin.co/2jj4riPWJvZB.png)
<a name="Friend Scrobbling"/>
### Friend Scrobbling
Allows you to fetch recent scrobbles of any last.fm user and scrobble them to your account.

![FriendScrobble](https://imagebin.ca/2jj5WvbSDVRp/FriendScrobble.png)
<a name="Database Scrobbling"/>
### Database Scrobbling
Search Last.fm for artists and albums and scrobble one or more tracks from it.

![ManualScrobble Artist Search](https://ibin.co/2jj5mE7b1g6j.png)

![ManualScrobble Album Search](https://ibin.co/2jj5zxquKBgv.png)

![ManualScrobble Tracklist](https://ibin.co/2jj6BpRDoGFl.png)
<a name="CSV Scrobbling"/>
### CSV Scrobbling
Allows you to load a .csv file and scrobble the info contained in it.
Currently only supports the following structure:

"Artist, Album, Track, Timestamp"
Needs to be excactly in this structure to work.

I recommend downloading your last.fm csv with [this tool](http://benjaminbenben.com/lastfm-to-csv/)

Individual fields can be enclosed by quotes and NEED to be enclosed by quotes if the field contains a comma.
For example:

"ArtistWith, CommaInTheName", Album, Track, 06/13/2016 19:54 

CSV scrobbling has two modes. They can be changed with the "Scrobbling Mode" ComboBox.

Normal Mode:

In this mode the tracks will be scrobbled with the timestamp from the csv field. Only scrobbles newer than 14 days can be scrobbled.


Import Mode:

In this mode the tracks will be scrobbled with the timestamp calculated from the "Finish Time" and the selected duration between each track. This allows the scrobbling of the tracks even if the timestamp in the csv is older than 14 days. Therefore the first (upmost) track in the csv file will be scrobbled with the "Finish Time".

![CSV Scrobbling](https://ibin.co/2ugcrHqbpbu6.png)
Normal Mode

![CSV Scrobbling Import Mode](https://ibin.co/2kXudUqJloTY.png)
Import Mode
<a name="File Scrobbling"/>
### File Scrobbling
Allows you to select music files (currently .mp3, .m4a and .wma) from your computer and scrobble them.
Timestamps will be reversed finishing with the "Finishing Time". So the last track in the list will be scrobbled with the "Finishing Time"

![File Scrobbler](https://ibin.co/2kz1FH1Htpm8.png)
<a name="Media Player Database Scrobbling"/>
### Media Player Database Scrobbling
A lot of media players store their song library in a specific file. Often times, the amount of times a specific song has been played is in there too. This can be used to import your complete listening history of your media player into last.fm. Although without correct timestamps, it is still useful to have all your artists and songs back in your last.fm account.

Currently works with iTunes, Winamp and Windows Media Player. 

You can read how to export your iTunes database as xml [here](http://ccm.net/faq/42531-enable-the-sharing-of-itunes-library-xml-with-other-applications).

You can read how to export your Winamp database as xml [here](http://forums.winamp.com/showthread.php?t=334048) (only step 1 is required).

![Media Player Database Scrobbling](https://ibin.co/2ueEB36aAC75.png)

Once you click "Scrobble", all selected songs will be scrobbled to your last.fm account, starting with the last track in the list and finishing with the first in the list. Timestamp will be current date time - 1 second.
<a name="iTunes Scrobbling"/>
### iTunes Scrobbling
This basically works like the normal Last.fm desktop app. Once you connected to iTunes a timer will measure how long you listened to that song. If you play 50% of the track, it will be scrobbled. The ProgressBar on the bottom indicates when the track will be scrobbled.

![iTunes Scrobbling](https://ibin.co/30cuCVX33Gke.png)
