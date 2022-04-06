import click


def log_message(msg):
    """Logs a message to stderr."""
    click.echo(msg, err=True)
