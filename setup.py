from setuptools import setup, find_packages

PACKAGE_NAME = BIN_NAME = "pmf"

with open(f"{PACKAGE_NAME}/_version.py", "r") as fd:
    exec(fd.read())

setup(
    name=PACKAGE_NAME,
    version=__version__,  # noqa: _version.py
    url="https://github.com/logicbomb421/plex-formatter",
    author="Michael Hill",
    author_email="mhill421@gmail.com",
    packages=find_packages(),
    install_requires=["click==8.1.2"],
    extras_require={"development": ["black==22.3.0", "jedi==0.18.1", "flake8==4.0.1"]},
    entry_points={"console_scripts": [f"{BIN_NAME}={PACKAGE_NAME}.__main__:main"]},
)
