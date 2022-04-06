import click
from ._version import __version__


@click.group()
@click.version_option(__version__)
@click.pass_context
def pmf(ctx):
    """Plex media formatter"""


from .format import format  # noqa

pmf.add_command(format)
