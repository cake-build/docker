[![Build](https://github.com/cake-build/docker/actions/workflows/build.yml/badge.svg)](https://github.com/cake-build/docker/actions/workflows/build.yml) [![Docker Pulls](https://img.shields.io/docker/pulls/cakebuild/cake.svg)](https://hub.docker.com/r/cakebuild/cake/tags/) [![Docker Stars](https://img.shields.io/docker/stars/cakebuild/cake.svg)](https://hub.docker.com/r/cakebuild/cake/tags/)

# Cake docker images 🍰🐳

Cake official Docker files with .NET SDK and Cake global tool pre-installed.

## Images

Images are currently continuously built for last 10 versions of Cake version 1.0 or newer.
Currently based on official Microsoft images available ( [mcr.microsoft.com/dotnet/sdk](https://github.com/microsoft/containerregistry) ).

To to pin to a specific version suffix with Cake version i.e. `cakebuild/cake:sdk-6.0` becomes `cakebuild/cake:sdk-6.0-v1.3.0`.

Tags are added dynamically as new are added to Microsoft container registry, check https://hub.docker.com/r/cakebuild/cake/tags for currently available tags.

### Linux Images

| Image                                         | Based on                                              |
|-----------------------------------------------|-------------------------------------------------------|
| cakebuild/cake:sdk-6.0                        | mcr.microsoft.com/dotnet/sdk:6.0                      |
| cakebuild/cake:sdk-5.0                        | mcr.microsoft.com/dotnet/sdk:5.0                      |
| cakebuild/cake:sdk-3.1                        | mcr.microsoft.com/dotnet/sdk:3.1                      |
| cakebuild/cake:sdk-6.0-alpine                 | mcr.microsoft.com/dotnet/sdk:6.0-alpine               |
| cakebuild/cake:sdk-6.0-alpine3.14             | mcr.microsoft.com/dotnet/sdk:6.0-alpine3.14           |
| cakebuild/cake:sdk-6.0-bullseye-slim          | mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim        |
| cakebuild/cake:sdk-6.0-cbl-mariner1.0         | mcr.microsoft.com/dotnet/sdk:6.0-cbl-mariner1.0       |
| cakebuild/cake:sdk-6.0-focal                  | mcr.microsoft.com/dotnet/sdk:6.0-focal                |
| cakebuild/cake:sdk-5.0-alpine based           | mcr.microsoft.com/dotnet/sdk:5.0-alpine               |
| cakebuild/cake:sdk-5.0-alpine3.12 based       | mcr.microsoft.com/dotnet/sdk:5.0-alpine3.12           |
| cakebuild/cake:sdk-5.0-alpine3.13 based       | mcr.microsoft.com/dotnet/sdk:5.0-alpine3.13           |
| cakebuild/cake:sdk-5.0-alpine3.14             | mcr.microsoft.com/dotnet/sdk:5.0-alpine3.14           |
| cakebuild/cake:sdk-5.0-bullseye-slim          | mcr.microsoft.com/dotnet/sdk:5.0-bullseye-slim        |
| cakebuild/cake:sdk-5.0-buster-slim            | mcr.microsoft.com/dotnet/sdk:5.0-buster-slim          |
| cakebuild/cake:sdk-5.0-cbl-mariner1.0         | mcr.microsoft.com/dotnet/sdk:5.0-cbl-mariner1.0       |
| cakebuild/cake:sdk-5.0-focal                  | mcr.microsoft.com/dotnet/sdk:5.0-focal                |
| cakebuild/cake:sdk-3.1-alpine                 | mcr.microsoft.com/dotnet/sdk:3.1-alpine               |
| cakebuild/cake:sdk-3.1-alpine3.12             | mcr.microsoft.com/dotnet/sdk:3.1-alpine3.12           |
| cakebuild/cake:sdk-3.1-alpine3.13             | mcr.microsoft.com/dotnet/sdk:3.1-alpine3.13           |
| cakebuild/cake:sdk-3.1-alpine3.14             | mcr.microsoft.com/dotnet/sdk:3.1-alpine3.14           |
| cakebuild/cake:sdk-3.1-bionic                 | mcr.microsoft.com/dotnet/sdk:3.1-bionic               |
| cakebuild/cake:sdk-3.1-bullseye               | mcr.microsoft.com/dotnet/sdk:3.1-bullseye             |
| cakebuild/cake:sdk-3.1-buster                 | mcr.microsoft.com/dotnet/sdk:3.1-buster               |
| cakebuild/cake:sdk-3.1-cbl-mariner1.0         | mcr.microsoft.com/dotnet/sdk:3.1-cbl-mariner1.0       |
| cakebuild/cake:sdk-3.1-focal                  | mcr.microsoft.com/dotnet/sdk:3.1-focal                |


### Windows Images

| Image                                             | Based on                                              |
|---------------------------------------------------|-------------------------------------------------------|
| cakebuild/cake:sdk-3.1-nanoserver-1809            | mcr.microsoft.com/dotnet/sdk:3.1-nanoserver-1809      |
| cakebuild/cake:sdk-3.1-nanoserver-1903            | mcr.microsoft.com/dotnet/sdk:3.1-nanoserver-1903      |
| cakebuild/cake:sdk-3.1-nanoserver-1909            | mcr.microsoft.com/dotnet/sdk:3.1-nanoserver-1909      |
| cakebuild/cake:sdk-3.1-nanoserver-2004            | mcr.microsoft.com/dotnet/sdk:3.1-nanoserver-2004      |
| cakebuild/cake:sdk-3.1-nanoserver-2009            | mcr.microsoft.com/dotnet/sdk:3.1-nanoserver-2009      |
| cakebuild/cake:sdk-3.1-nanoserver-20H2            | mcr.microsoft.com/dotnet/sdk:3.1-nanoserver-20H2      |
| cakebuild/cake:sdk-3.1-nanoserver-ltsc2022        | mcr.microsoft.com/dotnet/sdk:3.1-nanoserver-ltsc2022  |
| cakebuild/cake:sdk-5.0-nanoserver-1809            | mcr.microsoft.com/dotnet/sdk:5.0-nanoserver-1809      |
| cakebuild/cake:sdk-5.0-nanoserver-1909            | mcr.microsoft.com/dotnet/sdk:5.0-nanoserver-1909      |
| cakebuild/cake:sdk-5.0-nanoserver-2004            | mcr.microsoft.com/dotnet/sdk:5.0-nanoserver-2004      |
| cakebuild/cake:sdk-5.0-nanoserver-2009            | mcr.microsoft.com/dotnet/sdk:5.0-nanoserver-2009      |
| cakebuild/cake:sdk-5.0-nanoserver-20H2            | mcr.microsoft.com/dotnet/sdk:5.0-nanoserver-20H2      |
| cakebuild/cake:sdk-5.0-nanoserver-ltsc2022        | mcr.microsoft.com/dotnet/sdk:5.0-nanoserver-ltsc2022  |
| cakebuild/cake:sdk-6.0-nanoserver-1809            | mcr.microsoft.com/dotnet/sdk:6.0-nanoserver-1809      |
| cakebuild/cake:sdk-6.0-nanoserver-2004            | mcr.microsoft.com/dotnet/sdk:6.0-nanoserver-2004      |
| cakebuild/cake:sdk-6.0-nanoserver-20H2            | mcr.microsoft.com/dotnet/sdk:6.0-nanoserver-20H2      |
| cakebuild/cake:sdk-6.0-nanoserver-ltsc2022        | mcr.microsoft.com/dotnet/sdk:6.0-nanoserver-ltsc2022  |
| cakebuild/cake:sdk-6.0-windowsservercore-ltsc2022 | mcr.microsoft.com/dotnet/sdk:60-windowsservercore-ltsc2022 |

## Example usage

### Interactive container

```bash
docker run --rm -it cakebuild/cake:sdk-6.0 cake --version
```

### Use as builder image

```Dockerfile
FROM cakebuild/cake:sdk-6.0-v1.3.0 AS builder

ADD .  /src

RUN Cake /src/build.cake --Target=Publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR app

COPY --from=builder /src/output .

CMD ["dotnet","MyApp.dll"]
```

### Environment variables

| Name                  | Linux                     | Windows                       |
|-----------------------|---------------------------|-------------------------------|
| CAKE_PATHS_TOOLS      | /cake_build/tools         | C:/cake_build/tools           |
| CAKE_PATHS_ADDINS     | /cake_build/tools/Addins  | C:/cake_build/tools/Addins    |
| CAKE_PATHS_MODULES    | /cake_build/tools/Modules | C:/cake_build/tools/Modules   |

These are set to be able to keep foreign binaries inside your container and also enable caching of tools, addins and modules between docker layers.

## Build Infrastructure

Containers are currently build using GitHub Actions.

### Operating systems

Windows containers require OS support for specific versions of Windows to be built, so a matrix of Windows versions is used in orchestration using the same build script.

The build script will automatically detect if Docker is configured for building Windows or Linux containers.

But you can also pass parameters to filter to specific base images to build or exclude individual to not build.

### Build script parameters

| Name                      | Description                                                                                           |
|---------------------------|-------------------------------------------------------------------------------------------------------|
| remove-base-image         | Remove mcr base image after built, useful to conserve space used when building                        |
| base-image-include-filter | Base image must start with this, can be specified multiple times to include multiple base images      |
| base-image-exclude-filter | Base image can't start with this, can be specified multiple times, to exclude multiple base images    |