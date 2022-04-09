# Plex Media Formatter

###### A simple media preparation utility for use with Plex Media Server.

## Purpose

The purpose of this utility is to ensure file names and directory structures are in the [correct format](https://support.plex.tv/hc/en-us/categories/200028098-Media-Preparation)
for Plex Media Server to recognize and import.

## Usage

### CLI

```
Usage: pmf format [OPTIONS] SRC

  Formats media.

Options:
  -T, --title TEXT                The title of the media. If omitted, the
                                  basename of the source directory will be
                                  used.
  -t, --media-type [Movie|TV Show]
                                  The type of media being formatted.
                                  [required]
  -r, --regex-match TEXT          The regex pattern to use to match media name
                                  pieces.  [required]
  -o, --output-format TEXT        The output format for each file using
                                  matched values from the --regex-match
                                  capture.  [required]
  -f, --file-formats TEXT         A list of possibe formats for media files.
                                  [default: {'mp4', 'mkv'}]
  --plex-media-root TEXT          The root directory for Plex media.
  -n, --dry-run                   If set, no files will actually be moved.
  --clean                         If set, the source file will be deleted once
                                  copied to the formatted destination.
  --help                          Show this message and exit.
```

All arguments and commands can be specified with environment variables using the `PMF_FORMAT_` prefix. For example, to set the title option (`--title`), one could also set the `PMF_FORMAT_TITLE=abcdef` environment variable.

### Docker

The plex media formatter is also available as a Docker image at `mhill421/plex-media-formatter`.
