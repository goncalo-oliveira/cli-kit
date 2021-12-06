# GitPak

Simplifies downloading the latest version of a CLI tool. It also keeps track of the installed version and allows for a quick upgrade.

## Usage

> Unless specified otherwise (not yet supported), the tools are downloaded to `$HOME/.gitpak/bin/`. Make sure this path is accessible.
```bash
export PATH=$PATH:$HOME/.gitpak/bin/
```

Downloading the latest version of the tool

```bash
gitpak add kubectl
```

Downloading a specific version of the tool

```bash
gitpak add kubectl --version 1.21.7
gitpak add kubectl:1.21.7
gitpak add kubectl@1.21.7
```

Listing tools available for download

```bash
gitpak list
```

Look for updates for the downloaded tools

```bash
gitpak update
```

Look for updates and automatically install new versions

```bash
gitpak update --install
```

## Available Tools

The list of available tools can be found on [packages.yml]() on this repository. Updating this list doesn't require a new release of this CLI.


## TODO

- allow a configuration file to customize where to download the tools
- read local packages.y(a)ml file if it exists
