#Plex Media Formatter

###### A simple media preparation utility for use with Plex Media Server.

## Purpose
###### The purpose of this utility is to ensure file names and directory structures are in the [correct format](https://support.plex.tv/hc/en-us/categories/200028098-Media-Preparation) for Plex Media Server to recognize and import.

## Usage
#### Create the formatter
###### The formatters differ based on the media being formatted, but each adhere to the `IFormatter` interface. The current available filter types are:
- `TvFormatter`
- `MovieFormatter`

###### Each formatter requires the title of the media (however you want it to appear once formatted), as well as the source directory for the files to format and copy.

##### Example

```
IFormatter formatter = new TvFormatter("MyFavoriteTvShow", @"C:\Users\Me\Downloads\MyFavoriteTvShow");
```

#### Validate
###### If you'd like, you can manually run the validation process that occurs when you run the `FormatAndImport` method. This returns a `PlexFormatterResult` that contains a success status, log of any errors encountered, and a `FileInfo[]` item with the files that did not pass validation.

###### There are currently a few constraints the files must adhere to in order to be processed by this utility.

##### For TV Shows
###### The filename for each episode must contain the title, as well as a token to identify the season and episode. The order and casing in which these occur does not matter. For instance, `MyTvShow.s01.E07.someOtherInfo.mp4`, `S01.MyTvShow.E07.someOtherInfo.mp4`, and `MyTvShow.s01e07.someOtherInfo.mp4` are all valid.

##### For Movies
###### The filename only needs to contain the title and year of the movie. Year validity range is 1900 to current year + 3 (because reasons).

#### FormatAndImport
###### This method will first validate the files in the source directory as described above, format the filenames correctly, and move them to the specified Plex directory.