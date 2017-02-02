
FROM microsoft/aspnetcore
MAINTAINER Sifiso
LABEL Name=nancymusicstore Version=0.0.1 
ARG source=.
WORKDIR /app
EXPOSE 5001
COPY $source .
ENTRYPOINT dotnet nancymusicstore.dll
