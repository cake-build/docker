ARG BASE_IMAGE
FROM ${BASE_IMAGE} AS builder

ARG CAKE_VERSION
ARG PATH_SEPARATOR=":"
ARG PATH_ADDITIONAL=""

ENV CAKE_PATHS_TOOLS=/cake_build/tools \
    CAKE_PATHS_ADDINS=/cake_build/tools/Addins \
    CAKE_PATHS_MODULES=/cake_build/tools/Modules \
    PATH="/cake_build/tools${PATH_SEPARATOR}/cake_build${PATH_SEPARATOR}${PATH}${PATH_ADDITIONAL}"

ADD ./cake_build /cake_build

RUN dotnet tool install --tool-path /cake_build/tools Cake.Tool --version ${CAKE_VERSION} \
    && dotnet-cake /cake_build/setup.cake --containerVersion="${CAKE_VERSION}" \
    && cake --info \
    && dotnet cake --info

WORKDIR /src