import click
import traceback, sys
from . import pmf
from .helpers import log_message


def main():
    try:
        pmf(prog_name="pmf", auto_envvar_prefix="PMF")
    except Exception as ex:
        msg = ", ".join(ex.args)
        log_message(f"\n{click.style(f'FATAL: Error running command: {msg}', fg='white', bg='red')}\n")
        # indent the error a bit
        formatted_exc = "\n".join([f"  {ef}" for ef in traceback.format_exc().splitlines()])
        log_message(click.style(formatted_exc, fg="red"))
        return sys.exit(getattr(ex, "exit_code", 127))


if __name__ == "__main__":
    main()
