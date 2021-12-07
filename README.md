# CLI Kit

Simplifies downloading the latest version of a CLI tool. It also keeps track of the installed version and allows for a quick upgrade.

## Usage

> Unless specified otherwise (not yet supported), the tools are downloaded to `$HOME/.clikit/bin/`. Make sure this path is accessible.
```bash
export PATH=$PATH:$HOME/.clikit/bin/
```

Downloading the latest version of a tool

```bash
clik add kubectl
```

Downloading multiple tools at once

```bash
clik add kubectl terraform
```

Downloading a specific version of a tool

```bash
clik add kubectl:1.21.7
clik add kubectl@1.21.7
```

Listing tools available for download

```bash
clik list
```

Look for updates for the downloaded tools

```bash
clik update
```

Look for updates and automatically install new versions

```bash
clik update --install
```

## Available Tools

The list of available tools can be found on [packages.yml](packages.yaml) on this repository. Updating this list doesn't require a new release of this CLI.


## TODO

- allow a configuration file to customize where to download the tools
- read local packages.y(a)ml file if it exists
- self-update
