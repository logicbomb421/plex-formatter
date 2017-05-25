# Plex Media Formatter

###### A simple media preparation utility for use with Plex Media Server.

## Purpose
###### The purpose of this utility is to ensure file names and directory structures are in the [correct format](https://support.plex.tv/hc/en-us/categories/200028098-Media-Preparation) for Plex Media Server to recognize and import.

## Usage
#### Create the formatter
###### The formatters differ based on the media being formatted, but each are based off the `FormatterBase` abstract class. The current available filter types are:
- `TvFormatter` (though this is still heavily in development)
- `MovieFormatter`

###### Each formatter requires the source media (file or directory of files), title (however you want it to appear once formatted), as well as the year the media was created.
###### Each file will be processed into a `PlexMedia` object, available for reference via the `FormatterBase.Media` property.
##### Example

```
var formatter = new MovieFormatter(@"C:\Users\Me\Downloads\StarWarsIV", "Star Wars Episode IV: A New Hope" , "1977");

var validated = formatter.Validate();
if (validated.Status != PlexFormatterResult.ResultStatus.Success)
{
    //failure
}

var formatted = formatter.Format();
if (formatted.Status != PlexFormatterResult.ResultStatus.Success)
{
    //failure
}

var imported = formatter.Import();
if (imported.Status != PlexFormatterResult.ResultStatus.Success)
{
    //failure
}
```

#### Validate
###### If you'd like, you can manually run the validation process that occurs when you run the `Format` method. This returns a `PlexFormatterResult` that contains a success status, and log of any errors encountered..

###### There are currently a few constraints the files must adhere to in order to be processed by this utility.

##### For TV Shows
###### The filename for each episode must contain the title, as well as a token to identify the season and episode. The order and casing in which these occur does not matter. For instance, `MyTvShow.s01.E07.someOtherInfo.mp4`, `S01.MyTvShow.E07.someOtherInfo.mp4`, and `MyTvShow.s01e07.someOtherInfo.mp4` are all valid.

##### For Movies
###### The filename only needs to contain the title and may contain the year of the movie. Year validity range is 1900 to end of current decade.

#### Format
###### This method will first validate the files in the source directory as described above. Next, each `PlexMedia` object in the local `Media` collection will processed into a state readable by Plex.

#### Import
###### This will import the media via the `PlexMedia` objects in the local `Media` collection.
