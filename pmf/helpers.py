import os
import click
from typing import List


def log_message(msg):
    """Logs a message to stderr."""
    click.echo(msg, err=True)


def validate_file_extension(name: str, allowed: List[str]) -> bool:
    return os.path.splitext(name)[1] in allowed
