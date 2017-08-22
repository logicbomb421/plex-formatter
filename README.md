# Plex Media Formatter

###### A simple media preparation utility for use with Plex Media Server.

## Purpose
The purpose of this utility is to ensure file names and directory structures are in the [correct format](https://support.plex.tv/hc/en-us/categories/200028098-Media-Preparation) 
for Plex Media Server to recognize and import.

## Usage

### Importer

The importer is a simple WPF application that interacts with the plex-formatter API backend. The app takes in the 
file or directory, media name, and any additional information, and imports it into the local Plex library.

Currently only movie imports are implemented.

### API

#### `FormatterBase<T>`

All formatters inherit from this base class, which is a base implementation of `IFormatter<T>`.
In both cases, `T` is constrained to the type `IPlexMedia`.

#### Create the formatter
The formatters differ based on the media being formatted. The current available formatter types are:
- `TvFormatter` (still in development)
- `MovieFormatter`

Planned formatters:

- `PhotoFormatter`
- `MusicFormatter`

Each formatter requires, at minimum, the source media (file or directory of files) and title (however you want it 
to appear in Plex), and can potentially require additional data depending on the media type being imported
(such as a season and episode for TV or the year a movie was released).

Each file will be processed into an object that implements `IPlexMedia` (e.g. `PlexTvMedia`), available for 
reference via the `FormatterBase.Media` property.

##### Example

```c#
var formatter = new MovieFormatter(@"C:\Users\Me\Downloads\StarWarsIV", "Star Wars Episode IV: A New Hope" , "1977");

var validated = formatter.Validate();
if (validated.Status != PlexFormatterResult.ResultStatus.Success)
{
    //failure
    //further info can be obtained via the result's Log and possibly Data members.
}

var formatted = formatter.Format();
if (formatted.Status != PlexFormatterResult.ResultStatus.Success)
{
    //failure
    //further info can be obtained via the result's Log and possibly Data members.
}

var imported = formatter.Import();
if (imported.Status != PlexFormatterResult.ResultStatus.Success)
{
    //failure
    //further info can be obtained via the result's Log and possibly Data members.
}
```

#### Validate
If you'd like, you can manually run the validation process that occurs when you run the `Format` method. 
This returns a `Result` that contains a success status, and log of any errors encountered..

##### There are currently a few constraints the files must adhere to in order to be processed by this utility.

###### For TV Shows
The filename for each episode must contain a token to identify the season and episode. The order and casing in 
which these occur does not matter. For instance, `MyTvShow.s01.E07.someOtherInfo.mp4`, 
`S01.MyTvShow.E07.someOtherInfo.mp4`, and `MyTvShow.s01e07.someOtherInfo.mp4` are all valid.

This is based on a regex pattern that is currently a work-in-progress. You can see all the supported season token 
formats [here](https://regex101.com/r/oU8nbH/15), and episode token formats can be found [here](https://regex101.com/r/gK6UOa/8).

###### For Movies
The filename only needs to contain the title and may contain the year of the movie. Year validity range is 
1900 to end of current decade.

#### Format
This method will first validate the files in the source directory as described above. Next, each `PlexMedia` 
object in the local `Media` collection will processed into a state readable by Plex. This mostly entails determining
the final title and full path to the media.

#### Import
This will actually copy the media to the plex directory for that type.

## Roadmap

Currently trying to reach a basic MVP, which would include all formatters created and working (basic imports). 
See the 1.0 milestone for specific details on what I'm considering MVP.

Once past 1.0, the plan is to implement logic for all the 'edge-case' types of import (e.g. multi-episode files)
