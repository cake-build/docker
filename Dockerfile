FROM microsoft/dotnet:2.1-sdk AS builder

RUN apt-get update -qq \
    && apt-get install -y git zip unzip dos2unix

ADD src src

RUN dotnet --info \
    && cd src \
    && git clone https://github.com/cake-build/cake.git \
    && cd cake \
    && latesttag=$(git describe --tags `git rev-list --tags --max-count=1`) \
    && echo checking out ${latesttag} \
    && git checkout -b ${latesttag} ${latesttag} \
    && cd .. \
    && dos2unix -q ./build.sh \
    && ./build.sh

FROM microsoft/dotnet:2.1-sdk

COPY --from=builder /app /cake
RUN rm -rf /var/lib/apt/lists/* \
    && apt-get clean

ENV PATH "$PATH:/cake"