[![Build on GitHub Ubuntu Latest Agent](https://github.com/cake-build/docker/actions/workflows/BuildGitHubAgentsUbuntuLatest.yml/badge.svg)](https://github.com/cake-build/docker/actions/workflows/BuildGitHubAgentsUbuntuLatest.yml)
[![Build on GitHub Windows 2019 Agent](https://github.com/cake-build/docker/actions/workflows/BuildGitHubAgentsWin2019.yml/badge.svg)](https://github.com/cake-build/docker/actions/workflows/BuildGitHubAgentsWin2019.yml)
[![Build on GitHub Windows 2022 Agent](https://github.com/cake-build/docker/actions/workflows/BuildGitHubAgentsWin2022.yml/badge.svg)](https://github.com/cake-build/docker/actions/workflows/BuildGitHubAgentsWin2022.yml)
[![Build on Self Hosted Agent](https://github.com/cake-build/docker/actions/workflows/BuildSelfHostedAgent.yml/badge.svg)](https://github.com/cake-build/docker/actions/workflows/BuildSelfHostedAgent.yml)
[![Docker Pulls](https://img.shields.io/docker/pulls/cakebuild/cake.svg)](https://hub.docker.com/r/cakebuild/cake/tags/) [![Docker Stars](https://img.shields.io/docker/stars/cakebuild/cake.svg)](https://hub.docker.com/r/cakebuild/cake/tags/)

# Cake docker images 🍰🐳

Cake official Docker files with .NET SDK and Cake global tool pre-installed.

## Images

Images are currently continuously built for last 10 versions of Cake version 1.0 or newer.
Currently based on official Microsoft images available ( [mcr.microsoft.com/dotnet/sdk](https://github.com/microsoft/containerregistry) ).

To pin to a specific version suffix with Cake version i.e. `cakebuild/cake:sdk-6.0` becomes `cakebuild/cake:sdk-6.0-v1.3.0`.

Tags are added dynamically as new are added to Microsoft container registry, check https://hub.docker.com/r/cakebuild/cake/tags for currently available tags.

### Linux Images

| Image                                         | Based on                                              |
|-----------------------------------------------|-------------------------------------------------------|
|  cakebuild/cakesdk:6.0                        |  mcr.microsoft.com/dotnet/sdk:6.0                     |
|  cakebuild/cakesdk:6.0-alpine                 |  mcr.microsoft.com/dotnet/sdk:6.0-alpine              |
|  cakebuild/cakesdk:6.0-alpine3.14             |  mcr.microsoft.com/dotnet/sdk:6.0-alpine3.14          |
|  cakebuild/cakesdk:6.0-alpine3.16             |  mcr.microsoft.com/dotnet/sdk:6.0-alpine3.16          |
|  cakebuild/cakesdk:6.0-alpine3.17             |  mcr.microsoft.com/dotnet/sdk:6.0-alpine3.17          |
|  cakebuild/cakesdk:6.0-alpine3.18             |  mcr.microsoft.com/dotnet/sdk:6.0-alpine3.18          |
|  cakebuild/cakesdk:6.0-bookworm-slim          |  mcr.microsoft.com/dotnet/sdk:6.0-bookworm-slim       |
|  cakebuild/cakesdk:6.0-bullseye-slim          |  mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim       |
|  cakebuild/cakesdk:6.0-cbl-mariner            |  mcr.microsoft.com/dotnet/sdk:6.0-cbl-mariner         |
|  cakebuild/cakesdk:6.0-cbl-mariner1.0         |  mcr.microsoft.com/dotnet/sdk:6.0-cbl-mariner1.0      |
|  cakebuild/cakesdk:6.0-cbl-mariner2.0         |  mcr.microsoft.com/dotnet/sdk:6.0-cbl-mariner2.0      |
|  cakebuild/cakesdk:6.0-focal                  |  mcr.microsoft.com/dotnet/sdk:6.0-focal               |
|  cakebuild/cakesdk:6.0-jammy                  |  mcr.microsoft.com/dotnet/sdk:6.0-jammy               |
|  cakebuild/cakesdk:7.0                        |  mcr.microsoft.com/dotnet/sdk:7.0                     |
|  cakebuild/cakesdk:7.0-alpine                 |  mcr.microsoft.com/dotnet/sdk:7.0-alpine              |
|  cakebuild/cakesdk:7.0-alpine3.16             |  mcr.microsoft.com/dotnet/sdk:7.0-alpine3.16          |
|  cakebuild/cakesdk:7.0-alpine3.17             |  mcr.microsoft.com/dotnet/sdk:7.0-alpine3.17          |
|  cakebuild/cakesdk:7.0-alpine3.18             |  mcr.microsoft.com/dotnet/sdk:7.0-alpine3.18          |
|  cakebuild/cakesdk:7.0-bookworm-slim          |  mcr.microsoft.com/dotnet/sdk:7.0-bookworm-slim       |
|  cakebuild/cakesdk:7.0-bullseye-slim          |  mcr.microsoft.com/dotnet/sdk:7.0-bullseye-slim       |
|  cakebuild/cakesdk:7.0-cbl-mariner            |  mcr.microsoft.com/dotnet/sdk:7.0-cbl-mariner         |
|  cakebuild/cakesdk:7.0-cbl-mariner2.0         |  mcr.microsoft.com/dotnet/sdk:7.0-cbl-mariner2.0      |
|  cakebuild/cakesdk:7.0-jammy                  |  mcr.microsoft.com/dotnet/sdk:7.0-jammy               |
|  cakebuild/cakesdk:8.0                        |  mcr.microsoft.com/dotnet/sdk:8.0                     |
|  cakebuild/cakesdk:8.0-alpine                 |  mcr.microsoft.com/dotnet/sdk:8.0-alpine              |
|  cakebuild/cakesdk:8.0-alpine3.18             |  mcr.microsoft.com/dotnet/sdk:8.0-alpine3.18          |
|  cakebuild/cakesdk:8.0-bookworm-slim          |  mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim       |
|  cakebuild/cakesdk:8.0-cbl-mariner            |  mcr.microsoft.com/dotnet/sdk:8.0-cbl-mariner         |
|  cakebuild/cakesdk:8.0-cbl-mariner2.0         |  mcr.microsoft.com/dotnet/sdk:8.0-cbl-mariner2.0      |
|  cakebuild/cakesdk:8.0-jammy                  |  mcr.microsoft.com/dotnet/sdk:8.0-jammy               |

### Windows Images

| Image                                                   | Based on                                                        |
|---------------------------------------------------------|-----------------------------------------------------------------|
|  cakebuild/cakesdk:6.0-nanoserver-1809                  |  mcr.microsoft.com/dotnet/sdk:6.0-nanoserver-1809               |
|  cakebuild/cakesdk:6.0-nanoserver-20H2                  |  mcr.microsoft.com/dotnet/sdk:6.0-nanoserver-20H2               |
|  cakebuild/cakesdk:6.0-nanoserver-ltsc2022              |  mcr.microsoft.com/dotnet/sdk:6.0-nanoserver-ltsc2022           |
|  cakebuild/cakesdk:6.0-windowsservercore-ltsc2019       |  mcr.microsoft.com/dotnet/sdk:6.0-windowsservercore-ltsc2019    |
|  cakebuild/cakesdk:6.0-windowsservercore-ltsc2022       |  mcr.microsoft.com/dotnet/sdk:6.0-windowsservercore-ltsc2022    |
|  cakebuild/cakesdk:7.0-nanoserver-1809                  |  mcr.microsoft.com/dotnet/sdk:7.0-nanoserver-1809               |
|  cakebuild/cakesdk:7.0-nanoserver-ltsc2022              |  mcr.microsoft.com/dotnet/sdk:7.0-nanoserver-ltsc2022           |
|  cakebuild/cakesdk:7.0-windowsservercore-ltsc2019       |  mcr.microsoft.com/dotnet/sdk:7.0-windowsservercore-ltsc2019    |
|  cakebuild/cakesdk:7.0-windowsservercore-ltsc2022       |  mcr.microsoft.com/dotnet/sdk:7.0-windowsservercore-ltsc2022    |
|  cakebuild/cakesdk:8.0-nanoserver-1809                  |  mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-1809               |
|  cakebuild/cakesdk:8.0-nanoserver-ltsc2022              |  mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-ltsc2022           |
|  cakebuild/cakesdk:8.0-windowsservercore-ltsc2019       |  mcr.microsoft.com/dotnet/sdk:8.0-windowsservercore-ltsc2019    |
|  cakebuild/cakesdk:8.0-windowsservercore-ltsc2022       |  mcr.microsoft.com/dotnet/sdk:8.0-windowsservercore-ltsc2022    |

## Example usage

### Interactive container

```bash
docker run --rm -it cakebuild/cake:sdk-8.0 cake --version
```

### Use as builder image

```Dockerfile
FROM cakebuild/cake:sdk-8.0-alpine-v4.0.0 AS builder

ADD .  /src

RUN Cake /src/build.cake --Target=Publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0

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

Containers are currently built using GitHub Actions.

### Operating systems

Windows containers require OS support for specific versions of Windows to be built, so a matrix of Windows versions is used in orchestration using the same build script.

The build script will automatically detect if Docker is configured for building Windows or Linux containers.

But you can also pass parameters to filter to specific base images to build or exclude individual to not build.

### Build script parameters

| Name                      | Description                                                                                           |
|---------------------------|-------------------------------------------------------------------------------------------------------|
| remove-base-image         | Remove mcr base image after build, useful to conserve space used when building                        |
| base-image-include-filter | Base image must start with this, can be specified multiple times to include multiple base images      |
| base-image-exclude-filter | Base image can't start with this, can be specified multiple times, to exclude multiple base images    |
