FROM microsoft/dotnet:2.1-sdk-nanoserver-1803

ADD cake.cmd c:/cake/cake.cmd

# Windows doesn't currently support setting PATH with ENV
RUN setx PATH "C:\Program Files\dotnet;c:\cake;%PATH%"

RUN dotnet --info \
	&& dotnet tool install Cake.Tool --tool-path c:\cake \
    && cake --info

