[![Build Status](https://dev.azure.com/cake-build/Cake/_apis/build/status/Build%20Cake%20Docker%20Containters)](https://dev.azure.com/cake-build/Cake/_build/latest?definitionId=9) [![Docker Pulls](https://img.shields.io/docker/pulls/cakebuild/cake.svg)](https://hub.docker.com/r/cakebuild/cake/tags/) [![Docker Stars](https://img.shields.io/docker/stars/cakebuild/cake.svg)](https://hub.docker.com/r/cakebuild/cake/tags/)

# Cake docker images 🍰🐳

Cake official Docker files with Cake pre-installed

| Image                                 | Description                                                                                       |   |
|---------------------------------------|---------------------------------------------------------------------------------------------------|---|
|cakebuild/cake:2.1-sdk                 | Image with latest released Cake, based on microsoft/dotnet:2.1-sdk                                | [![2.1-sdk Size](https://img.shields.io/microbadger/image-size/cakebuild/cake/2.1-sdk.svg)](https://hub.docker.com/r/cakebuild/cake/tags/)                            |
|cakebuild/cake:2.1-sdk-bitrise         | Image with latest released Cake and Bitrise CLI,  based on cakebuild/cake:2.1-sdk                 | [![2.1-sdk-bitrise Size](https://img.shields.io/microbadger/image-size/cakebuild/cake/2.1-sdk-bitrise.svg)](https://hub.docker.com/r/cakebuild/cake/tags/)            |
|cakebuild/cake:2.1-sdk-mono            | Image with latest released Cake and Mono,  based on cakebuild/cake:2.1-sdk                        | [![2.1-sdk-mono Size](https://img.shields.io/microbadger/image-size/cakebuild/cake/2.1-sdk-mono.svg)](https://hub.docker.com/r/cakebuild/cake/tags/)                  |
|cakebuild/cake:2.1-sdk-bitrise-mono    | Image with latest released Cake, Bitrise CLI and Mono,  based on cakebuild/cake:2.1-sdk-bitrise   | [![2.1-sdk-bitrise-mono size](https://img.shields.io/microbadger/image-size/cakebuild/cake/2.1-sdk-bitrise-mono.svg)](https://hub.docker.com/r/cakebuild/cake/tags/)  |

## Example usage

### Interactive container

```bash
docker run -it --rm cakebuild/cake:2.1-sdk cake --version
```

### Use as builder image

```Dockerfile
FROM cakebuild/cake:v0.30.0-2.1-sdk AS builder

RUN apt-get update -qq \
    && curl -sL https://deb.nodesource.com/setup_9.x | bash - \
    && apt-get install -y nodejs

ADD .  /src

RUN Cake /src/build.cake --Target=Publish

FROM microsoft/dotnet:2.1.3-aspnetcore-runtime

WORKDIR app

COPY --from=builder /src/output .

CMD ["dotnet","CoreWiki.dll"]
```

## Version Tags

CI servers will at least on each release of Cake or when this repository updated build and publish new Docker images. Each build will also be tagged with version number, so you can pin to a specific version of Cake.

I.e. if `v0.30.0` is latest release Cake version then it'll be tagged `2.1-sdk` and `v0.30.0-2.1-sdk`, when `v0.31.0` is released then it'll be tagged `2.1-sdk` and `v0.31.0-2.1-sdk`, and `v0.30.0-2.1-sdk` will be untouched.

Currently the images come pre-installed with a self-contained version of Cake.CoreCLR, this means modules, addins and tools need to be .NET Core or .NET Standard 2.0 compatible.