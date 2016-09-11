# Last.fm-Scrubbler-WPF
Manual Last.fm scrobbling for when a service (or you!) failed to scrobble.

## Features

**Table of Contents**

- [Manual Single Track Scrobbling](#Manual Scrobbling)
- [Friend Scrobbling](#Friend Scrobbling)
- [Database Scrobbling](#Database Scrobbling)
- [CSV Scrobbling](#CSV Scrobbling)
- [File Scrobbling](#File Scrobbling)
- [Media Player Database Scrobbling](#Media Player Database Scrobbling)

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
Allows you to select music files (currently only .mp3) from your computer and scrobble them.
Timestamps will be reversed finishing with the "Finishing Time". So the last track in the list will be scrobbled with the "Finishing Time"

![File Scrobbler](https://ibin.co/2kz1FH1Htpm8.png)
<a name="Media Player Database Scrobbling"/>
### Media Player Database Scrobbling
A lot of media players store their song library in a specific file. Often times, the amount of times a specific song has been played is in there too. This can be used to import your complete listening history of your media player into last.fm. Although without correct timestamps, it is still useful to have all your artists and songs back in your last.fm account.

Currently only works with iTunes xml. You can read how to export your iTunes database as xml [here](http://ccm.net/faq/42531-enable-the-sharing-of-itunes-library-xml-with-other-applications).

![Media Player Database Scrobbling](https://ibin.co/2ueEB36aAC75.png)

Once you click "Scrobble", all selected songs will be scrobbled to your last.fm account, starting with the last track in the list and finishing with the first in the list. Timestamp will be current date time - 1 second.
