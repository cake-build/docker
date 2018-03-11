FROM microsoft/dotnet:2.1-sdk AS builder

RUN apt update  \
    && apt install git -y unzip 

ADD src src

RUN cd src \
    && git clone https://github.com/cake-build/cake.git \
    && cd cake \
    && latesttag=$(git describe --tags `git rev-list --tags --max-count=1`) \
    && echo checking out ${latesttag} \
    && git checkout -b ${latesttag} ${latesttag} \
    && cd .. \
    && ./build.sh

FROM microsoft/dotnet:2.1-sdk

COPY --from=builder /app /cake

ENV PATH "$PATH:/cake"