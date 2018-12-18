FROM cakebuild/cake:2.1-sdk

RUN apt-get update -qq \
    && apt-get install -y apt-transport-https \
    && apt-key adv --no-tty --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF \
    && echo "deb https://download.mono-project.com/repo/debian stable-stretch main" | tee /etc/apt/sources.list.d/mono-official-stable.list \
    && apt-get update -qq \
    && apt-get install -y --no-install-recommends mono-devel \
	&& rm -rf /var/lib/apt/lists/* \
    && apt-get clean