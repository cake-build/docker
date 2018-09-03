FROM cakebuild/cake:2.1-sdk

ENV CI="true" \
    BITRISE_SOURCE_DIR="/bitrise/src" \
    BITRISE_BRIDGE_WORKDIR="/bitrise/src" \
    BITRISE_DEPLOY_DIR="/bitrise/deploy" \
    BITRISE_CACHE_DIR="/bitrise/cache" \
    BITRISE_PREP_DIR="/bitrise/prep" \
    BITRISE_TMP_DIR="/bitrise/tmp" \
    PATH="/bitrise/go/bin/:$PATH:/usr/local/go/bin" \
    GOPATH="/bitrise/go" \
    TOOL_VER_BITRISE_CLI="1.21.0" \
    TOOL_VER_GO="1.10"

COPY ./ssh/config /root/.ssh/config

WORKDIR ${BITRISE_PREP_DIR}

RUN apt-get update -qq \
    && apt-get install -y rsync sudo \
    && mkdir -p ${BITRISE_SOURCE_DIR} \
    && mkdir -p ${BITRISE_DEPLOY_DIR} \
    && mkdir -p ${BITRISE_CACHE_DIR} \
    && mkdir -p ${BITRISE_TMP_DIR} \
    && mkdir -p ${BITRISE_PREP_DIR} \
    && wget -q https://storage.googleapis.com/golang/go${TOOL_VER_GO}.linux-amd64.tar.gz -O go-bins.tar.gz \
    && tar -C /usr/local -xvzf go-bins.tar.gz \
    && rm go-bins.tar.gz \
    && mkdir -p "$GOPATH/src" "$GOPATH/bin" && chmod -R 755 "$GOPATH" \
    && curl -fL https://github.com/bitrise-io/bitrise/releases/download/${TOOL_VER_BITRISE_CLI}/bitrise-$(uname -s)-$(uname -m) > /usr/local/bin/bitrise \
    && chmod +x /usr/local/bin/bitrise \
    && bitrise setup \
    && bitrise envman -version \
    && bitrise stepman -version \
    && bitrise stepman setup -c https://github.com/bitrise-io/bitrise-steplib.git \
    && bitrise stepman update \
    && rm -rf /var/lib/apt/lists/* \
    && apt-get clean

WORKDIR ${BITRISE_SOURCE_DIR}