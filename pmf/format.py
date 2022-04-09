import click
import os, re, shutil
from typing import List
from .helpers import log_message, validate_file_extension

def _copy_file(src: str, dst: str, dry_run: bool = False):
    if dry_run:
        return log_message("Dry run: NOT copying file!")
    shutil.copy2(src, dst)

def _rm_file(path: str, dry_run: bool = False):
    if dry_run:
        return log_message("Dry run: NOT removing file!")
    os.unlink(path)

def _mkdirp(path: str, dry_run: bool = False):
    if dry_run:
        return log_message("Dry run: NOT creating directory!")
    os.makedirs(path, exist_ok=True)

def _chown(path: str, user: str, group: str, dry_run: bool = False):
    if dry_run:
        return log_message("Dry run: NOT setting ownership!")
    shutil.chown(path, user=user, group=group)

def _chmod(path: str, permissions: str, dry_run: bool = False):
    if dry_run:
        return log_message("Dry run: NOT setting permissions!")
    return os.chmod(path, permissions)

@click.command()
@click.argument("src", envvar="PMF_FORMAT_SRC", type=click.Path(exists=True))
@click.option(
    "-T",
    "--title",
    help="The title of the media. If omitted, the basename of the source directory will be used.",
    type=click.STRING,
)
@click.option(
    "-t",
    "--media-type",
    help="The type of media being formatted.",
    show_choices=True,
    type=click.Choice({"Movie", "TV Show"}),
    required=True,
)
@click.option(
    "-r", "--regex-match", help="The regex pattern to use to match media name pieces.", type=click.STRING, required=True
)
@click.option(
    "-o",
    "--output-format",
    help="The output format for each file using matched values from the --regex-match capture.",
    type=click.STRING,
    required=True,
)
@click.option(
    "-f",
    "--file-formats",
    help="A list of possibe formats for media files.",
    multiple=True,
    show_default=True,
    default={"mkv", "mp4"},
    type=click.STRING,
    callback=lambda _, __, val: [v if v.startswith(".") else f".{v}" for v in val],
)
@click.option(
    "--plex-media-root", help="The root directory for Plex media.", type=click.STRING, default="/mnt/ds918/Plex Media"
)
@click.option("-n", "--dry-run", help="If set, no files will actually be moved.", is_flag=True)
@click.option(
    "--clean", help="If set, the source file will be deleted once copied to the formatted destination.", is_flag=True
)
@click.option("-u", "--user", help="The name of the user to set on the copied file.", type=click.STRING)
@click.option("-g", "--group", help="The name of the group to set on the copied file.", type=click.STRING)
def format(
    src: str,
    title: str,
    media_type: str,
    regex_match,
    output_format: str,
    file_formats: List[str],
    plex_media_root: str,
    dry_run: bool,
    clean: bool,
    user: str,
    group: str,
    **kwargs,
):
    """Formats media."""
    media_title = title or os.path.basename(src)
    log_message(
        "\n".join(
            [
                "",
                f"Dry Run: {dry_run}",
                "Media Information",
                f"  {'Title':16}: {media_title}",
                f"  {'Source':16}: {src}",
                f"  {'Plex Media Root':16}: {plex_media_root}",
                f"  {'Type':16}: {media_type}",
                f"  {'Regex Match':16}: {regex_match}",
                f"  {'Output Format':16}: {output_format}",
                "",
            ]
        )
    )
    regex = re.compile(regex_match)
    work = []
    log_message("Matching and formatting filenames...\n")
    for file_ in [f for f in os.scandir(src) if f.is_file() and validate_file_extension(f.name, file_formats)]:
        match = regex.match(file_.name)
        if not match:
            raise click.ClickException(f"No match found for {file_.name}. Not proceeding!")
        formatted = regex.sub(output_format, file_.name)
        log_message("\n".join([file_.name, f"  {'Matches':12}: {match.groups()}", f"  {'Formatted':12}: {formatted}"]))
        work.append({"from": file_.path, "to": formatted})

    log_message("")
    media_dest = f"{plex_media_root}/{media_type}s/{media_title}"
    for item in work:
        final = f"{media_dest}/{item['to']}"
        _mkdirp(os.path.dirname(final), dry_run)
        log_message(f"Moving {item['from']}\n   └─> {final}")
        # if dry_run:
        #     continue
        # shutil.copy2(item["from"], final)
        _copy_file(item["from"], final, dry_run)

        if user or group:
            log_message(f"Setting file ownership to {user}:{group}")
            _chown(final, user, group, dry_run)

        mode = "0770"
        log_message(f"Setting file permissions to {mode}")
        _chmod(final, mode , dry_run)

        if clean:
            log_message("Removing original file...")
            # os.unlink(item["from"])
            _rm_file(item["from"], dry_run)

    log_message("\nAll media moved successfully!")
